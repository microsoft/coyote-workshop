// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using PetImages.Contracts;
using PetImages.CosmosContracts;
using PetImages.Exceptions;
using PetImages.Persistence;
using System;
using System.Threading.Tasks;

namespace PetImages.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ICosmosDatabase CosmosDatabase;

        public ImageController(
            ICosmosDatabase cosmosDatabase)
        {
            this.CosmosDatabase = cosmosDatabase;
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

            throw new NotImplementedException();
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

                return this.Ok();
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NoContent();
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
