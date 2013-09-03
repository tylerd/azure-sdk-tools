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
using System.Net;
using Microsoft.WindowsAzure.Management.Utilities.MediaService;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities;
using Microsoft.WindowsAzure.Management.Utilities.Properties;

namespace Microsoft.WindowsAzure.Management.MediaService
{
    public enum KeyType
    {
        Primary,
        Secondary
    }

    /// <summary>
    ///     Gets an azure website.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureMediaServicesKey", SupportsShouldProcess = true), OutputType(typeof(string))]
    public class NewAzureMediaServiceKeyCommand : AzureMediaServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The media services account name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The media services key type <Primary|Secondary>.")]
        [ValidateNotNullOrEmpty]
        public KeyType KeyType { get; set; }

        [Parameter(Position = 2, HelpMessage = "Do not confirm regeneration of the key.")]
        public SwitchParameter Force { get; set; }

        public IMediaServicesClient MediaServicesClient { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format(Resources.RegenerateKeyWarning),
                          Resources.RegenerateKeyWhatIfMessage,
                          string.Empty,
                          () =>
                              {
                                  MediaServicesClient = MediaServicesClient ?? new MediaServicesClient(CurrentSubscription, WriteDebug);

                                  bool result;
                                  CatchAggregatedExceptionFlattenAndRethrow(() => { result = MediaServicesClient.RegenerateMediaServicesAccountAsync(Name, KeyType.ToString()).Result; });

                                  MediaServiceAccountDetails account = null;
                                  CatchAggregatedExceptionFlattenAndRethrow(() => { account = MediaServicesClient.GetMediaServiceAsync(Name).Result; });
                                  string newKey = KeyType == KeyType.Primary ? account.AccountKeys.Primary : account.AccountKeys.Secondary;

                                  WriteObject(newKey);
                              });
        }
    }
}