// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetImages.Messaging;
using PetImages.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetImages.Worker
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly ILogger<BackgroundJobService> _logger;

        private readonly IWorker GenerateThumbnailWorker;

        private readonly IMessageReceiver MessageReceiver;

        private readonly IMessagingClient MessagingClient;

        private readonly int WaitingDelayInMs = 10000;

        public BackgroundJobService(ILogger<BackgroundJobService> logger, ICosmosDatabase cosmosDatabase, IStorageAccount storageAccount, IMessageReceiver messageReceiver, IMessagingClient messagingClient)
        {
            _logger = logger;
            this.GenerateThumbnailWorker = new GenerateThumbnailWorker(cosmosDatabase, storageAccount);
            this.MessageReceiver = messageReceiver;
            this.MessagingClient = messagingClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextMessage = await this.MessageReceiver.ReadMessage();
                if (nextMessage != null)
                {
                    if (nextMessage.Type.Equals(Message.GenerateThumbnailMessageType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var thumbnailImageMessage = (GenerateThumbnailMessage)nextMessage;
                            _logger.LogInformation($"Processing Generate Thumbnail Message for {thumbnailImageMessage.AccountName} account's {thumbnailImageMessage.ImageName} image.");
                            var workerResult = await this.GenerateThumbnailWorker.ProcessMessage(thumbnailImageMessage);

                            switch (workerResult.ResultCode)
                            {
                                case WorkerResultCode.Retry:
                                    // Requeue the message for retry
                                    _logger.LogInformation($"Requeued Worker Job. Worker Message: {workerResult.Message}");
                                    await this.MessagingClient.SubmitMessage(nextMessage);
                                    await Task.Delay(WaitingDelayInMs);
                                    break;
                                case WorkerResultCode.Completed:
                                    _logger.LogInformation($"Generated thumbnail successfully. Worker Message: {workerResult.Message}");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error occured while processing thumbnail, reason: {ex}");
                        }
                    }
                    else
                    {
                        // throw for an invalid message type
                        _logger.LogError($"Invalid message type: {nextMessage.Type}");
                    }
                }
                else
                {
                    _logger.LogInformation("Worker running at: {time}. No messages processed.", DateTimeOffset.Now);
                    await Task.Delay(WaitingDelayInMs, stoppingToken);
                }
            }
        }
    }
}
