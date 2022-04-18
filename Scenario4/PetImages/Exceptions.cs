// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Microsoft.Azure.Cosmos;
using System;

namespace PetImages.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(CosmosException cosmosException)
            : base(cosmosException?.Message, cosmosException)
        {
        }
    }

    public class DatabaseContainerAlreadyExistsException : DatabaseException
    {
        public DatabaseContainerAlreadyExistsException(CosmosException cosmosException)
            : base(cosmosException)
        {
        }
    }

    public class DatabaseContainerDoesNotExistException : DatabaseException
    {
        public DatabaseContainerDoesNotExistException(CosmosException cosmosException)
            : base(cosmosException)
        {
        }
    }

    public class DatabaseItemAlreadyExistsException : DatabaseException
    {
        public DatabaseItemAlreadyExistsException(CosmosException cosmosException)
            : base(cosmosException)
        {
        }
    }

    public class DatabaseItemDoesNotExistException : DatabaseException
    {
        public DatabaseItemDoesNotExistException(CosmosException cosmosException)
            : base(cosmosException)
        {
        }
    }

    public class DatabasePreconditionFailedException : DatabaseException
    {
        public DatabasePreconditionFailedException(CosmosException cosmosException)
            : base(cosmosException)
        {
        }
    }

    public class StorageException : Exception
    {
        public StorageException(RequestFailedException requestFailedException)
            : base(requestFailedException?.Message, requestFailedException)
        {
        }
    }

    public class StorageContainerAlreadyExistsException : StorageException
    {
        public StorageContainerAlreadyExistsException(RequestFailedException requestFailedException)
            : base(requestFailedException)
        {
        }
    }

    public class StorageContainerDoesNotExistException : StorageException
    {
        public StorageContainerDoesNotExistException(RequestFailedException requestFailedException)
            : base(requestFailedException)
        {
        }
    }

    public class BlobAlreadyExistsException : StorageException
    {
        public BlobAlreadyExistsException(RequestFailedException requestFailedException)
            : base(requestFailedException)
        {
        }
    }

    public class BlobDoesNotExistException : StorageException
    {
        public BlobDoesNotExistException(RequestFailedException requestFailedException)
            : base(requestFailedException)
        {
        }
    }
}
