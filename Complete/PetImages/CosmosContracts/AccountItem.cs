// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;

namespace PetImages.CosmosContracts
{
    public class AccountItem : DbItem
    {
        public override string PartitionKey => Id;

        public string ContactEmailAddress { get; set; }

        public Account ToAccount()
        {
            return new Account()
            {
                Name = Id,
                ContactEmailAddress = ContactEmailAddress
            };
        }
    }
}
