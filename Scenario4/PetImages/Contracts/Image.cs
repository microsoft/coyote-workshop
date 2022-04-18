// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.CosmosContracts;
using System;

namespace PetImages.Contracts
{
    public enum ImageState { Creating, Created };

    public class Image
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public string[] Tags { get; set; }

        public byte[] Content { get; set; }

        public string State { get; set; }

        public DateTime LastModifiedTimestamp { get; set; }

        public ImageItem ToImageItem(string accountName, string blobName = null, string thumbnailBlobName = null)
        {
            return new ImageItem()
            {
                Id = Name,
                AccountName = accountName,
                ContentType = ContentType,
                BlobName = blobName,
                ThumbnailBlobName = thumbnailBlobName,
                Tags = Tags,
                State = State,
                LastModifiedTimestamp = LastModifiedTimestamp
            };
        }
    }
}
