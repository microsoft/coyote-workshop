// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using System.Net;

namespace PetImagesTest.Clients
{
    public class ServiceResponse<T>
    {
        public HttpStatusCode? StatusCode { get; set; }

        public T Resource { get; set; }

        public Error Error { get; set; }
    }
}