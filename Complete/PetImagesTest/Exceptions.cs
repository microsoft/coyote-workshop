// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;

namespace PetImagesTest.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        private readonly HttpResponseMessage responseMessage;

        public InternalServerErrorException(HttpResponseMessage responseMessage)
        {
            this.responseMessage = responseMessage;
        }
    }

    public class SimulatedRandomFaultException : Exception
    {
    }
}
