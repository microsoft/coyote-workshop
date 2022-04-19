// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Coyote;
using Microsoft.Coyote.Specifications;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetImages;
using PetImages.Contracts;
using PetImagesTest.PersistenceMocks;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PetImagesTest.Clients
{
    [TestClass]
    public class Tests
    {
        private static readonly bool useInMemoryClient = true;

        public async Task TestFirstScenarioAsync()
        {
            var serviceClient = await InitializeSystemAsync();

            var accountName = "MyAccount";

            var account1 = new Account()
            {
                Name = accountName,
                ContactEmailAddress = "john@acme.com"
            };

            var account2 = new Account()
            {
                Name = accountName,
                ContactEmailAddress = "sally@contoso.com"
            };

            // Call CreateAccount twice without awaiting, which makes both methods run
            // asynchronously with each other.
            var task1 = serviceClient.CreateAccountAsync(account1);
            var task2 = serviceClient.CreateAccountAsync(account2);

            // Then wait both requests to complete.
            await Task.WhenAll(task1, task2);

            var statusCode1 = task1.Result.StatusCode;
            var statusCode2 = task2.Result.StatusCode;

            // Finally, assert that only one of the two requests succeeded and the other
            // failed. Note that we do not know which one of the two succeeded as the
            // requests ran concurrently (this is why we use an exclusive OR).
            Assert.IsTrue(
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.Conflict) ||
                (statusCode1 == HttpStatusCode.Conflict && statusCode2 == HttpStatusCode.OK));
        }

        public async Task TestSecondScenarioUpdateAsync()
        {
            var serviceClient = await InitializeSystemAsync();

            string accountName = "MyAccount";
            string imageName = "pet.jpg";
            string contentType = "image/jpeg";

            // Create an account request payload
            var account = new Account()
            {
                Name = accountName,
                ContactEmailAddress = "john@acme.com"
            };

            var accountResult = await serviceClient.CreateAccountAsync(account);
            Assert.IsTrue(accountResult.StatusCode == HttpStatusCode.OK);

            var createResult = await serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image()
                {
                    Name = imageName,
                    ContentType = contentType,
                    Content = GetDogImageBytes()
                });
            Assert.IsTrue(createResult.StatusCode == HttpStatusCode.OK);

            var utcNow = DateTime.UtcNow;

            var updateResult1 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image()
                {
                    Name = imageName,
                    ContentType = contentType,
                    Content = GetCatImageBytes(),
                    LastModifiedTimestamp = utcNow
                });
            var updateResult2 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image()
                {
                    Name = imageName,
                    ContentType = contentType,
                    Content = GetParrotImageBytes(),
                    LastModifiedTimestamp = utcNow.AddDays(1)
                });

            await Task.WhenAll(updateResult1, updateResult2);

            var statusCode1 = updateResult1.Result.StatusCode;
            var statusCode2 = updateResult2.Result.StatusCode;

            Assert.IsTrue(
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.Conflict) ||
                (statusCode1 == HttpStatusCode.Conflict && statusCode2 == HttpStatusCode.OK) ||
                (statusCode1 == HttpStatusCode.BadRequest && statusCode2 == HttpStatusCode.OK) ||
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.OK));
        }

        public async Task TestSecondScenarioCreateAsync()
        {
            var serviceClient = await InitializeSystemAsync();

            string accountName = "MyAccount";
            string imageName = "pet.jpg";
            string contentType = "image/jpeg";

            // Create an account request payload
            var account = new Account()
            {
                Name = accountName,
                ContactEmailAddress = "john@acme.com"
            };

            var accountResult = await serviceClient.CreateAccountAsync(account);
            Assert.IsTrue(accountResult.StatusCode == HttpStatusCode.OK);

            var utcNow = DateTime.UtcNow;

            var createResult1 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image()
                {
                    Name = imageName,
                    ContentType = contentType,
                    Content = GetCatImageBytes(),
                    LastModifiedTimestamp = utcNow
                });
            var createResult2 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image()
                {
                    Name = imageName,
                    ContentType = contentType,
                    Content = GetParrotImageBytes(),
                    LastModifiedTimestamp = utcNow.AddDays(1)
                });

            await Task.WhenAll(createResult1, createResult2);

            var statusCode1 = createResult1.Result.StatusCode;
            var statusCode2 = createResult2.Result.StatusCode;

            Assert.IsTrue(
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.Conflict) ||
                (statusCode1 == HttpStatusCode.Conflict && statusCode2 == HttpStatusCode.OK) ||
                (statusCode1 == HttpStatusCode.BadRequest && statusCode2 == HttpStatusCode.OK) ||
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.OK));
        }

        public async Task TestThirdScenarioAsync()
        {
            var serviceClient = await InitializeSystemAsync();

            string accountName = "MyAccount";
            string imageName = "pet.jpg";
            string contentType = "image/jpeg";

            // Create an account request payload
            var account = new Account()
            {
                Name = accountName,
                ContactEmailAddress = "john@acme.com"
            };

            var accountResult = await serviceClient.CreateAccountAsync(account);
            Assert.IsTrue(accountResult.StatusCode == HttpStatusCode.OK);

            var task1 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image() { Name = imageName, ContentType = contentType, Content = GetDogImageBytes() });
            var task2 = serviceClient.CreateOrUpdateImageAsync(
                accountName,
                new Image() { Name = imageName, ContentType = contentType, Content = GetCatImageBytes() });

            await Task.WhenAll(task1, task2);

            var statusCode1 = task1.Result.StatusCode;
            var statusCode2 = task2.Result.StatusCode;

            Assert.IsTrue(
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.Conflict) ||
                (statusCode1 == HttpStatusCode.Conflict && statusCode2 == HttpStatusCode.OK) ||
                (statusCode1 == HttpStatusCode.OK && statusCode2 == HttpStatusCode.OK));

            var imageContentResult = await serviceClient.GetImageContentAsync(accountName, imageName);
            Assert.IsTrue(imageContentResult.StatusCode == HttpStatusCode.OK);

            if (statusCode1 == HttpStatusCode.OK && statusCode2 != HttpStatusCode.OK)
            {
                Assert.IsTrue(IsDogImage(imageContentResult.Resource));
            }
            else if (statusCode1 != HttpStatusCode.OK && statusCode2 == HttpStatusCode.OK)
            {
                Assert.IsTrue(IsCatImage(imageContentResult.Resource));
            }
            else
            {
                Assert.IsTrue(
                    IsDogImage(imageContentResult.Resource) ||
                    IsCatImage(imageContentResult.Resource));
            }
        }

        [TestMethod]
        public void SystematicTestFirstScenario()
        {
            RunSystematicTest(TestFirstScenarioAsync);
        }

        [TestMethod]
        public void SystematicTestSecondScenarioUpdate()
        {
            RunSystematicTest(TestSecondScenarioUpdateAsync);
        }

        [TestMethod]
        public void SystematicTestSecondScenarioCreate()
        {
            RunSystematicTest(TestSecondScenarioCreateAsync);
        }

        [TestMethod]
        public void SystematicTestThirdScenario()
        {
            RunSystematicTest(TestThirdScenarioAsync);
        }

        private static async Task<IServiceClient> InitializeSystemAsync()
        {
            Logger.WriteLine("\r\nBeginning test iteration\r\n");

            if (useInMemoryClient)
            {
                var cosmosState = new MockCosmosState();

                var cosmosDatabase = new MockCosmosDatabase(cosmosState);

                await cosmosDatabase.CreateContainerIfNotExistsAsync(Constants.AccountContainerName);
                await cosmosDatabase.CreateContainerIfNotExistsAsync(Constants.ImageContainerName);

                var storageAccount = new MockStorageAccount();

                var serviceClient = new InMemoryTestServiceClient(
                    cosmosDatabase,
                    storageAccount);

                return serviceClient;
            }
            else
            {
                var factory = new ServiceFactory();
                await factory.InitializeCosmosDatabaseAsync();

                return new TestServiceClient(factory);
            }
        }

        /// <summary>
        /// Invoke the Coyote systematic testing engine to run the specified test multiple iterations,
        /// each iteration exploring potentially different interleavings using some underlying program
        /// exploration strategy (by default a uniform probabilistic strategy).
        /// </summary>
        /// <remarks>
        /// Learn more in our documentation: https://microsoft.github.io/coyote/how-to/unit-testing
        /// </remarks>
        private static void RunSystematicTest(Func<Task> test, string reproducibleScheduleFilePath = null)
        {
            // Configuration for how to run a concurrency unit test with Coyote.
            // This configuration will run the test 1000 times exploring different paths each time.
            var config = Configuration
                .Create()
                .WithMaxSchedulingSteps(5000)
                .WithTestingIterations(useInMemoryClient ? (uint)1000 : 100);

            if (reproducibleScheduleFilePath != null)
            {
                var trace = File.ReadAllText(reproducibleScheduleFilePath);
                config = config.WithReplayStrategy(trace);
            }

            async Task TestActionAsync()
            {
                Specification.RegisterMonitor<TestLivenessSpec>();
                await test();
                Specification.Monitor<TestLivenessSpec>(new TestTerminalEvent());
            };

            var testingEngine = TestingEngine.Create(config, TestActionAsync);

            try
            {
                testingEngine.Run();

                string assertionText = testingEngine.TestReport.GetText(config);
                assertionText +=
                    $"{Environment.NewLine} Random Generator Seed: " +
                    $"{testingEngine.TestReport.Configuration.RandomGeneratorSeed}{Environment.NewLine}";
                foreach (var bugReport in testingEngine.TestReport.BugReports)
                {
                    assertionText +=
                    $"{Environment.NewLine}" +
                    "Bug Report: " + bugReport.ToString(CultureInfo.InvariantCulture);
                }

                if (testingEngine.TestReport.NumOfFoundBugs > 0)
                {
                    var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture);
                    var reproducibleTraceFileName = $"buggy-{timeStamp}.schedule";
                    assertionText += Environment.NewLine + "Reproducible trace which leads to the bug can be found at " +
                        $"{Path.Combine(Directory.GetCurrentDirectory(), reproducibleTraceFileName)}";

                    File.WriteAllText(reproducibleTraceFileName, testingEngine.ReproducibleTrace);
                }

                Assert.IsTrue(testingEngine.TestReport.NumOfFoundBugs == 0, assertionText);

                Console.WriteLine(testingEngine.TestReport.GetText(config));
            }
            finally
            {
                testingEngine.Stop();

                Logger.WriteToFile("PetImages.log");
                Logger.Clear();
            }
        }

        private static byte[] GetDogImageBytes() => new byte[] { 1, 2, 3 };
        private static byte[] GetCatImageBytes() => new byte[] { 4, 5, 6 };
        private static byte[] GetParrotImageBytes() => new byte[] { 7, 8, 9 };

        private static bool IsDogImage(byte[] bytes) => bytes.SequenceEqual(GetDogImageBytes());
        private static bool IsDogThumbnail(byte[] bytes) => bytes.SequenceEqual(GetDogImageBytes());
        private static bool IsCatImage(byte[] bytes) => bytes.SequenceEqual(GetCatImageBytes());
        private static bool IsCatThumbnail(byte[] bytes) => bytes.SequenceEqual(GetCatImageBytes());
        private static bool IsParrotImage(byte[] bytes) => bytes.SequenceEqual(GetParrotImageBytes());
        private static bool IsParrotThumbnail(byte[] bytes) => bytes.SequenceEqual(GetParrotImageBytes());
    }

    public class TestLivenessSpec : Monitor
    {
        [Hot]
        [Start]
        [OnEventGotoState(typeof(TestTerminalEvent), typeof(Terminal))]
        private class Init : State
        {
        }

        [OnEventGotoState(typeof(TestTerminalEvent), typeof(Terminal))]
        private class Terminal : State
        {
        }
    }

    public class TestTerminalEvent : Event
    {
    }
}
