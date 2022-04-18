// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using PetImages.Contracts;
using PetImages.CosmosContracts;
using PetImages.Exceptions;
using PetImages.Persistence;
using System.Threading.Tasks;

namespace PetImages.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ICosmosDatabase CosmosDatabase;
        private readonly IStorageAccount StorageAccount;

        public ImageController(
            ICosmosDatabase cosmosDatabase,
            IStorageAccount storageAccount)
        {
            this.CosmosDatabase = cosmosDatabase;
            this.StorageAccount = storageAccount;
        }

        /// <summary>
        /// ...
        /// </summary>
        [HttpPut]
        [Route(Routes.Images)]
        public async Task<ActionResult<Image>> CreateOrUpdateImageAsync(string accountName, Image image)
        {
            var maybeError = await ValidateImageAsync(accountName, image);
            if (maybeError != null)
            {
                return this.BadRequest(maybeError);
            }

            var imageItem = image.ToImageItem(accountName);

            // We upload the image to Azure Storage, before adding an entry to Cosmos DB
            // so that it is guaranteed to be there when user does a GET request.
            // Note: we're calling CreateOrUpdateBlobAsync because Azure Storage doesn't
            // have a create-only API.
            await StorageHelper.CreateContainerIfNotExistsAsync(this.StorageAccount, accountName);
            await this.StorageAccount.CreateOrUpdateBlockBlobAsync(accountName, image.Name, image.ContentType, image.Content);

            var maybeExistingImageItem = await CosmosHelper.GetItemIfExistsAsync<ImageItem>(
                this.CosmosDatabase,
                Constants.ImageContainerName,
                imageItem.PartitionKey,
                imageItem.Id);

            if (maybeExistingImageItem == null)
            {
                try
                {
                    imageItem = await this.CosmosDatabase.CreateItemAsync(Constants.ImageContainerName, imageItem);
                }
                catch (DatabaseItemAlreadyExistsException)
                {
                    return this.Conflict();
                }
            }
            else
            {
                if (imageItem.LastModifiedTimestamp < maybeExistingImageItem.LastModifiedTimestamp)
                {
                    return this.BadRequest(ErrorFactory.StaleLastModifiedTime(
                        imageItem.LastModifiedTimestamp,
                        maybeExistingImageItem.LastModifiedTimestamp));
                }

                try
                {
                    await this.CosmosDatabase.UpsertItemAsync(
                        Constants.ImageContainerName,
                        imageItem,
                        maybeExistingImageItem.ETag);
                }
                catch (DatabasePreconditionFailedException)
                {
                    return this.Conflict();
                }
            }

            return this.Ok(imageItem.ToImage());
        }

        [HttpGet]
        [Route(Routes.ImageInstance)]
        public async Task<ActionResult<Image>> GetImageAsync(
            [FromRoute] string accountName,
            [FromRoute] string imageName)
        {
            var maybeError = await ValidateAccountAsync(accountName);
            if (maybeError != null)
            {
                return this.BadRequest(maybeError);
            }

            try
            {
                var imageItem = await this.CosmosDatabase.GetItemAsync<ImageItem>(
                    Constants.ImageContainerName,
                    partitionKey: accountName,
                    id: imageName);
                return this.Ok(imageItem.ToImage());
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NotFound();
            }
        }

        [HttpDelete]
        [Route(Routes.ImageInstance)]
        public async Task<ActionResult> DeleteImageAsync(string accountName, string imageName)
        {
            var maybeError = await ValidateAccountAsync(accountName);
            if (maybeError != null)
            {
                return this.BadRequest(maybeError);
            }

            try
            {
                var imageItem = await this.CosmosDatabase.GetItemAsync<ImageItem>(
                    Constants.ImageContainerName,
                    partitionKey: accountName,
                    id: imageName);

                await this.CosmosDatabase.DeleteItemAsync(
                    Constants.ImageContainerName,
                    partitionKey: accountName,
                    id: imageName);

                await StorageHelper.DeleteBlobIfExistsAsync(this.StorageAccount, accountName, imageName);

                return this.Ok();
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NoContent();
            }
        }

        [HttpGet]
        [Route(Routes.ImageContentInstance)]
        public async Task<ActionResult<byte[]>> GetImageContentsAsync(
            [FromRoute] string accountName,
            [FromRoute] string imageName)
        {
            var maybeError = await ValidateAccountAsync(accountName);
            if (maybeError != null)
            {
                return this.BadRequest(maybeError);
            }

            try
            {
                var imageItem = await this.CosmosDatabase.GetItemAsync<ImageItem>(
                    Constants.ImageContainerName,
                    partitionKey: accountName,
                    id: imageName);

                var maybeBytes = await StorageHelper.GetBlobIfExistsAsync(this.StorageAccount, accountName, imageName);

                if (maybeBytes == null)
                {
                    return this.NotFound();
                }

                return this.File(maybeBytes, imageItem.ContentType);
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NotFound();
            }
        }

        private async Task<Error> ValidateImageAsync(string accountName, Image image)
        {
            var maybeError = await ValidateAccountAsync(accountName);
            if (maybeError != null)
            {
                return maybeError;
            }

            if (image == null)
            {
                return ErrorFactory.ParsingError(nameof(Image));
            }

            if (string.IsNullOrWhiteSpace(image.Name))
            {
                return ErrorFactory.InvalidParameterValueError(nameof(Image.Name), image.Name);
            }

            if (string.IsNullOrWhiteSpace(image.ContentType))
            {
                return ErrorFactory.InvalidParameterValueError(nameof(Image.ContentType), image.ContentType);
            }

            if (image.Content == null)
            {
                return ErrorFactory.InvalidParameterValueError(nameof(Image.Content), image.Content);
            }

            return null;
        }

        private async Task<Error> ValidateAccountAsync(string accountName)
        {
            if (!await CosmosHelper.DoesItemExistAsync<AccountItem>(
                CosmosDatabase,
                Constants.AccountContainerName,
                partitionKey: accountName,
                id: accountName))
            {
                return ErrorFactory.AccountDoesNotExistError(accountName);
            }

            return null;
        }
    }
}
