// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PetImages
{
    public static class Constants
    {
        public static readonly string DatabaseName = "PetImages";
        public static readonly string AccountContainerName = "Accounts";
        public static readonly string ImageContainerName = "Images";

        // Note: Queue Name should always be in lowercase for Azure Storage Queues
        public static readonly string ThumbnailQueueName = "thumbnailmessages";
    }
}
