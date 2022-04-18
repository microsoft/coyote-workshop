// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PetImages.Messaging;
using System.Threading.Tasks;

namespace PetImages.Worker
{
    public interface IMessageReceiver
    {
        Task<Message> ReadMessage();
    }
}
