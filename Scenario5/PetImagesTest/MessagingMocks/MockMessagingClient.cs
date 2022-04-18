// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Coyote.Specifications;
using PetImages;
using PetImages.Messaging;
using PetImages.Persistence;
using PetImages.Worker;
using PetImagesTest.Exceptions;
using System;
using System.Threading.Tasks;

namespace PetImagesTest.MessagingMocks
{
    public class MockMessagingClient : IMessagingClient
    {
        private readonly IWorker GenerateThumbnailWorker;

        public MockMessagingClient(
            ICosmosDatabase cosmosDatabase,
            IStorageAccount storageAccount)
        {
            this.GenerateThumbnailWorker = new GenerateThumbnailWorker(cosmosDatabase, storageAccount);
        }

        public Task SubmitMessage(Message message)
        {
            // Fire-and-forget the task to model sending an asynchronous message over the network.
            _ = Task.Run(async () =>
            {
                Logger.AsyncLocalRequestId.Value =
                    Logger.RequestId +
                    "-" +
                    Guid.NewGuid().ToString().Substring(0, 6);

                Logger.WriteLine($"Running worker for message type {message.Type}");

                try
                {
                    if (message.Type == Message.GenerateThumbnailMessageType)
                    {
                        var clonedMessage = TestHelper.Clone((GenerateThumbnailMessage)message);
                        var workerResult = await this.RunThumbnailWorkerWithRetryAsync(clonedMessage);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Encountered uncaught exception in worker for message type {message.Type}; no further retries will happen");
                    Specification.Assert(false, $"Uncaught exception in worker: {ex}");
                }
            });

            return Task.CompletedTask;
        }

        private async Task<WorkerResult> RunThumbnailWorkerWithRetryAsync(GenerateThumbnailMessage message)
        {
            WorkerResult workerResult = null;
            do
            {
                try
                {
                    workerResult = await this.GenerateThumbnailWorker.ProcessMessage(message);

                    Logger.WriteLine($"Worker {message.Type} returned with result code {workerResult.ResultCode}");
                }
                catch (SimulatedRandomFaultException)
                {
                }
            }
            while (workerResult == null || workerResult.ResultCode == WorkerResultCode.Retry);

            return workerResult;
        }
    }
}
