// ----------------------------------------------------------------------------------
//
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
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.Utilities.MediaService;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities;
using Microsoft.WindowsAzure.ServiceManagement;

namespace Microsoft.WindowsAzure.Management.MediaService
{
    [Cmdlet(VerbsCommon.New, "AzureMediaServicesAccount"), OutputType(typeof(AccountCreationResult))]
    public class NewAzureMediaServiceCommand : AzureMediaServicesHttpClientCommandBase
    {
        public IMediaServicesClient MediaServicesClient { get; set; }


        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The media service account name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The media service location.")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(Position = 3, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Storage account name")]
        [ValidateNotNullOrEmpty]
        public string StorageAccountName { get; set; }




        public override void ExecuteCmdlet()
        {
            MediaServicesClient = MediaServicesClient ?? new MediaServicesClient(CurrentSubscription, WriteDebug);


            StorageService storage = null; 
            Uri storageEndPoint = null;
            string storageAccountKey = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { storage = MediaServicesClient.GetStorageServiceKeys(StorageAccountName).Result; });
            storageAccountKey = storage.StorageServiceKeys.Primary;
           

            CatchAggregatedExceptionFlattenAndRethrow(() => { storage = MediaServicesClient.GetStorageServiceProperties(StorageAccountName).Result; });

            if (storage.StorageServiceProperties != null && storage.StorageServiceProperties.Endpoints.Count > 0)
            {
                storageEndPoint = new Uri(storage.StorageServiceProperties.Endpoints[0]);
            }
            else
            {
                throw new Exception(string.Format(Utilities.Properties.Resources.EndPointNotFoundForBlobStorage, Name));
            }

            AccountCreationResult result = null;
            var request = new AccountCreationRequest
            {
                AccountName = Name,
                BlobStorageEndpointUri = storageEndPoint.ToString(),
                Region = Location,
                StorageAccountKey = storageAccountKey,
                StorageAccountName = StorageAccountName
            };
            CatchAggregatedExceptionFlattenAndRethrow(() => { result = MediaServicesClient.CreateNewAzureMediaServiceAsync(request).Result; });
            WriteObject(result, false);
        }
    }
}