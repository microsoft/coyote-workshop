// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace PetImages.CosmosContracts
{
    public abstract class DbItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public abstract string PartitionKey { get; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
    }
}
