// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using PetImages.CosmosContracts;
using PetImages.Exceptions;
using PetImages.Messaging;
using PetImages.Persistence;
using System;
using System.Threading.Tasks;

namespace PetImages.Worker
{
    public class GenerateThumbnailWorker : IWorker
    {
        private readonly ICosmosDatabase CosmosDatabase;
        private readonly IStorageAccount StorageAccount;

        public GenerateThumbnailWorker(
            ICosmosDatabase cosmosDatabase,
            IStorageAccount storageAccount)
        {
            this.CosmosDatabase = cosmosDatabase;
            this.StorageAccount = storageAccount;
        }

        public async Task<WorkerResult> ProcessMessage(Message message)
        {
            var thumbnailMessage = (GenerateThumbnailMessage)message;

            var accountName = thumbnailMessage.AccountName;
            var imageName = thumbnailMessage.ImageName;
            var requestId = thumbnailMessage.RequestId;

            var maybeImageItem = await CosmosHelper.GetItemIfExistsAsync<ImageItem>(
                this.CosmosDatabase,
                Constants.ImageContainerName,
                partitionKey: accountName,
                id: imageName);

            if (maybeImageItem == null || maybeImageItem.LastTouchedByRequestId != requestId)
            {
                return new WorkerResult
                {
                    ResultCode = WorkerResultCode.Retry,
                    Message = "Needs Retry.",
                };
            }

            var maybeImageBytes = await StorageHelper.GetBlobIfExistsAsync(this.StorageAccount, accountName, maybeImageItem.BlobName);
            if (maybeImageBytes == null)
            {
                return new WorkerResult
                {
                    ResultCode = WorkerResultCode.Completed,
                    Message = "Blob not found",
                };
            }

            var thumbnailBytes = GenerateThumbnail(maybeImageBytes);
            var thumbnailBlobName = Guid.NewGuid().ToString();
            await this.StorageAccount.CreateOrUpdateBlockBlobAsync(accountName, thumbnailBlobName, "image/jpeg", thumbnailBytes);

            maybeImageItem.ThumbnailBlobName = thumbnailBlobName;
            maybeImageItem.State = ImageState.Created.ToString();

            try
            {
                await this.CosmosDatabase.ReplaceItemAsync(
                    Constants.ImageContainerName,
                    maybeImageItem,
                    ifMatchEtag: maybeImageItem.ETag);
            }
            catch (DatabasePreconditionFailedException)
            {
                return new WorkerResult
                {
                    ResultCode = WorkerResultCode.Retry,
                    Message = "Needs Retry.",
                };
            }

            return new WorkerResult
            {
                ResultCode = WorkerResultCode.Completed,
                Message = "Thumbnail Generated.",
            };
        }

        /// <summary>
        /// Dummy implementation of GenerateThumbnail that returns the same bytes as the image.
        /// </summary>
        private static byte[] GenerateThumbnail(byte[] imageContents) => imageContents;
    }
}
