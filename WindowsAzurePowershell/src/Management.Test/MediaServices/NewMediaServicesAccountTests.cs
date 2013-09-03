// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Management.MediaService;
using Microsoft.WindowsAzure.Management.Test.Utilities.Common;
using Microsoft.WindowsAzure.Management.Utilities.MediaService;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities;
using Microsoft.WindowsAzure.ServiceManagement;
using Moq;
namespace Microsoft.WindowsAzure.Management.Test.MediaServices
{
    [TestClass]
    public class NewMediaServicesAccountTests : TestBase
    {
        [TestMethod]
        public void NewMediaServiceAccountShouldPassWithValidParameters()
        {
            // Setup
            var clientMock = new Mock<IMediaServicesClient>();

            const string storageAccountName = "teststorage";
            const string storageAccountKey = "key";
            const string accountName = "testaccount";
            const string region = "West US";
            const string blobStorageEndpointUri = "http://awesome.blob.core.windows.net/";

            AccountCreationRequest request = new AccountCreationRequest()
            {
                AccountName = accountName,
                BlobStorageEndpointUri = blobStorageEndpointUri,
                Region = region,
                StorageAccountKey = storageAccountKey,
                StorageAccountName = storageAccountName

            };

            clientMock.Setup(f => f.CreateNewAzureMediaServiceAsync(It.Is<AccountCreationRequest>(creationRequest => request.AccountName == accountName))).Returns(Task.Factory.StartNew(() =>
            {
                return new AccountCreationResult()
                {
                    AccountId = Guid.NewGuid().ToString(),
                    Name = request.AccountName,
                    Subscription = Guid.NewGuid().ToString()
                };
            }));


            clientMock.Setup(f => f.GetStorageServiceKeys(storageAccountName)).Returns(Task.Factory.StartNew(() =>
            {
                return new StorageService()
                {
                    StorageServiceKeys = new StorageServiceKeys()
                    {
                        Primary = storageAccountKey,
                        Secondary = storageAccountKey

                    }
                };
            }));


            clientMock.Setup(f => f.GetStorageServiceProperties(storageAccountName)).Returns(Task.Factory.StartNew(() =>
            {
                return new StorageService()
                {
                    StorageServiceProperties = new StorageServiceProperties()
                    {
                        Endpoints = new EndpointList()
					{
						blobStorageEndpointUri
					}
                    }
                };
            }));

            // Test
            var command = new NewAzureMediaServiceCommand()
            {
                CommandRuntime = new MockCommandRuntime(),
                Name = accountName,
                Location = region,
                StorageAccountName = storageAccountName,
                MediaServicesClient = clientMock.Object,
            };

            command.ExecuteCmdlet();
            Assert.AreEqual(1, ((MockCommandRuntime)command.CommandRuntime).OutputPipeline.Count);
            var accountCreationResult = (AccountCreationResult)((MockCommandRuntime)command.CommandRuntime).OutputPipeline.FirstOrDefault();
            Assert.IsNotNull(accountCreationResult);
            Assert.AreEqual(accountName, accountCreationResult.Name);
        }
    }
}