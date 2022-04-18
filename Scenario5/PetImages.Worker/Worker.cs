// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Messaging;
using System.Threading.Tasks;

namespace PetImages.Worker
{
    public interface IWorker
    {
        Task<WorkerResult> ProcessMessage(Message message);
    }
}
