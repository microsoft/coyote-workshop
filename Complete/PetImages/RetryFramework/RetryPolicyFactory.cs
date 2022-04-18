// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Polly;
using System;
using System.Net;
using System.Net.Sockets;

namespace PetImages.RetryFramework
{
    public class RetryPolicyFactory
    {
        public static IAsyncPolicy GetAsyncRetryExponential(int numOfAttempts = 3, Func<Exception, bool> isRetryableException = null)
        {
            var shouldRetry = isRetryableException ?? DefaultRetryableNetworkExceptions;

            AsyncPolicy asyncPolicy = Policy
                .Handle<Exception>(ex => shouldRetry(ex))
                .WaitAndRetryAsync(numOfAttempts, retryAttempt =>
                                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            return asyncPolicy;
        }

        public static bool DefaultRetryableNetworkExceptions(Exception ex)
        {
            if (ex is WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ConnectFailure ||
                    webEx.Status == WebExceptionStatus.ConnectionClosed ||
                    webEx.Status == WebExceptionStatus.KeepAliveFailure ||
                    webEx.Status == WebExceptionStatus.NameResolutionFailure ||
                    webEx.Status == WebExceptionStatus.ReceiveFailure ||
                    webEx.Status == WebExceptionStatus.SendFailure ||
                    webEx.Status == WebExceptionStatus.Timeout)
                {
                    return true;
                }
            }
            else if (ex is SocketException sockEx)
            {
                if (sockEx.SocketErrorCode == SocketError.Interrupted ||
                    sockEx.SocketErrorCode == SocketError.NetworkDown ||
                    sockEx.SocketErrorCode == SocketError.NetworkUnreachable ||
                    sockEx.SocketErrorCode == SocketError.NetworkReset ||
                    sockEx.SocketErrorCode == SocketError.ConnectionAborted ||
                    sockEx.SocketErrorCode == SocketError.ConnectionReset ||
                    sockEx.SocketErrorCode == SocketError.TimedOut ||
                    sockEx.SocketErrorCode == SocketError.HostDown ||
                    sockEx.SocketErrorCode == SocketError.HostUnreachable ||
                    sockEx.SocketErrorCode == SocketError.TryAgain)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
