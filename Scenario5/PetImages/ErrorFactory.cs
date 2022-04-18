// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Contracts;
using System;

namespace PetImages
{
    public class ErrorFactory
    {
        public static readonly string ParsingErrorCode = "ParsingError";
        public static readonly string ValidationErrorCode = "ValidationError";
        public static readonly string AccountDoesNotExistErrorCode = "AccountDoesNotExist";

        public static Error AccountDoesNotExistError(string accountName)
        {
            return new Error()
            {
                Code = AccountDoesNotExistErrorCode,
                Message = $"Account {accountName} does not exist"
            };
        }

        public static Error ParsingError(string modelName)
        {
            return new Error()
            {
                Code = ParsingErrorCode,
                Message = $"Could not parse {modelName}"
            };
        }

        public static Error InvalidParameterValueError(string parameterName, object parameterValue)
        {
            return new Error()
            {
                Code = ValidationErrorCode,
                Message = $"{parameterName} does not have a valid value: {parameterValue}"
            };
        }


        public static Error StaleLastModifiedTime(DateTime givenLastModifiedTime, DateTime existingLastModifiedTime)
        {
            return new Error()
            {
                Code = ValidationErrorCode,
                Message = $"Given last modified time {givenLastModifiedTime} was older than existing last modified time {existingLastModifiedTime}"
            };
        }
    }
}
