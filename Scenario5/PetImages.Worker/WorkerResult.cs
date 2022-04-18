// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace PetImages.Worker
{
    public class WorkerResult
    {
        public WorkerResultCode ResultCode { get; set; }

        public string Message { get; set; }
    }

    public enum WorkerResultCode
    {
        Completed,
        Retry
    }
}
