// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PetImages
{
    public class Routes
    {
        public const string Accounts = "/accounts";
        public const string AccountInstance = Accounts + "/{accountName}";

        public const string Images = AccountInstance + "/images";
        public const string ImageInstance = Images + "/{imageName}";
        public const string ImageContentInstance = ImageInstance + "/content";
        public const string ImageThumbnailInstance = ImageInstance + "/thumbnail";
    }
}
