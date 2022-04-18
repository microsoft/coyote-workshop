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
    public class AccountController : ControllerBase
    {
        private readonly ICosmosDatabase CosmosDatabase;

        public AccountController(ICosmosDatabase cosmosDatabase)
        {
            this.CosmosDatabase = cosmosDatabase;
        }

        /// <summary>
        /// CreateAccountAsync fixed.
        /// </summary>
        [HttpPost]
        [Route(Routes.Accounts)]
        public async Task<ActionResult<Account>> CreateAccountAsync(Account account)
        {
            var maybeError = ValidateAccount(account);
            if (maybeError != null)
            {
                return this.BadRequest(maybeError);
            }

            var accountItem = account.ToItem();

            try
            {
                accountItem = await this.CosmosDatabase.CreateItemAsync(Constants.AccountContainerName, accountItem);
            }
            catch (DatabaseItemAlreadyExistsException)
            {
                return this.Conflict();
            }

            return this.Ok(accountItem.ToAccount());
        }

        [HttpGet]
        [Route(Routes.AccountInstance)]
        public async Task<ActionResult<Account>> GetAccountAsync([FromRoute] string accountName)
        {
            try
            {
                var accountItem = await this.CosmosDatabase.GetItemAsync<AccountItem>(
                    Constants.AccountContainerName,
                    partitionKey: accountName,
                    id: accountName);

                return this.Ok(accountItem.ToAccount());
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NotFound();
            }
        }

        [HttpDelete]
        [Route(Routes.AccountInstance)]
        public async Task<ActionResult<Account>> DeleteAccountAsync([FromRoute] string accountName)
        {
            try
            {
                await this.CosmosDatabase.DeleteItemAsync(
                    Constants.AccountContainerName,
                    partitionKey: accountName,
                    id: accountName);

                return this.Ok();
            }
            catch (DatabaseItemDoesNotExistException)
            {
                return this.NoContent();
            }
        }

        private static Error ValidateAccount(Account account)
        {
            if (account == null)
            {
                return ErrorFactory.ParsingError(nameof(Account));
            }

            if (string.IsNullOrWhiteSpace(account.Name))
            {
                return ErrorFactory.InvalidParameterValueError(nameof(Account.Name), account.Name);
            }

            if (string.IsNullOrWhiteSpace(account.ContactEmailAddress))
            {
                return ErrorFactory.InvalidParameterValueError(nameof(Account.Name), account.Name);
            }

            return null;
        }
    }
}
