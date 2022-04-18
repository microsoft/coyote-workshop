// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Text;
using System.Threading;

namespace PetImages
{
    public class Logger
    {
        public static AsyncLocal<string> AsyncLocalRequestId = new AsyncLocal<string>();

        public static string RequestId => AsyncLocalRequestId.Value;

        private static StringBuilder logs = new StringBuilder();

        public static void WriteLine(string msg)
        {
            string prefix = string.IsNullOrEmpty(RequestId) ?
                string.Empty :
                $"-- {RequestId} -- ";

            logs.AppendLine($"{prefix}{msg}");
        }

        public static void WriteToFile(string filePath)
        {
            using (var sw = File.CreateText(filePath))
            {
                sw.Write(logs.ToString());
            }
        }

        public static void Clear()
        {
            logs = new StringBuilder();
        }
    }
}
