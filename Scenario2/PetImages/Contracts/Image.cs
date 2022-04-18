// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using System;

namespace PetImages.Contracts
{
    public class Image
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public string[] Tags { get; set; }

        public byte[] Content { get; set; }

        public DateTime LastModifiedTimestamp { get; set; }

        public ImageItem ToImageItem(string accountName)
        {
            return new ImageItem()
            {
                Id = Name,
                AccountName = accountName,
                ContentType = ContentType,
                Tags = Tags,
                LastModifiedTimestamp = LastModifiedTimestamp
            };
        }
    }
}
