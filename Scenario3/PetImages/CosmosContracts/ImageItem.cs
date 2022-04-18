// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using System;

namespace PetImages.CosmosContracts
{
    public class ImageItem : DbItem
    {
        public override string PartitionKey => AccountName;

        public string AccountName { get; set; }

        public string ContentType { get; set; }

        public string[] Tags { get; set; }

        public DateTime LastModifiedTimestamp { get; set; }

        public Image ToImage()
        {
            return new Image()
            {
                Name = Id,
                ContentType = ContentType,
                Tags = Tags,
                LastModifiedTimestamp = LastModifiedTimestamp
            };
        }
    }
}
