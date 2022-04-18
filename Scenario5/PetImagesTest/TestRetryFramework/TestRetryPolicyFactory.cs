// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Coyote.Random;
using PetImages.RetryFramework;
using PetImagesTest.Exceptions;
using Polly;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PetImages.TestRetryFramework
{
    public class TestRetryPolicyFactory
    {
        public static IAsyncPolicy GetOneTimeFailRetryAsncPolicy(Func<Exception, bool> isRetryableException = null)
        {
            return Policy.WrapAsync(
                RetryPolicyFactory.GetAsyncRetryExponential(),
                new OneTimeFailPolicy());
        }

        public static RandomPermanentFailurePolicy GetRandomPermanentFailureAsyncPolicy(Func<Exception, bool> isRetryableException = null)
        {
            return new RandomPermanentFailurePolicy();
        }
    }

    public class OneTimeFailPolicy : AsyncPolicy
    {
        private const string KeyName = "throw-exception-intentionally";

        protected override async Task<TResult> ImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            if (!context.ContainsKey(KeyName))
            {
                context.Add(KeyName, true);
                _ = await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
                throw new SocketException((int)SocketError.TimedOut);
            }
            else
            {
                context.Remove(KeyName);
                return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            }
        }
    }

    public class RandomPermanentFailurePolicy : AsyncPolicy
    {
        private const string KeyName = "throw-exception-intentionally";

        private readonly Generator randomGenerator = Generator.Create();

        public bool ShouldRandomlyFail { get; set; } = true;

        protected override async Task<TResult> ImplementationAsync<TResult>(
            Func<Context, CancellationToken, Task<TResult>> action,
            Context context,
            CancellationToken cancellationToken,
            bool continueOnCapturedContext)
        {
            if (!ShouldRandomlyFail)
            {
                return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            }

            if (!context.ContainsKey(KeyName))
            {
                var shouldFail = randomGenerator.NextBoolean();
                context.Add(KeyName, shouldFail);
            }

            _ = context.TryGetValue(KeyName, out object shouldFailValue);

            if ((bool)shouldFailValue)
            {
                throw new SimulatedRandomFaultException();
            }
            else
            {
                return await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            }
        }
    }
}
