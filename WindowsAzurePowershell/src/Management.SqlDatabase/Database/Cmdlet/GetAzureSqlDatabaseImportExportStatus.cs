// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Database.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.Common;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport;
    using Microsoft.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Exports a database from SQL Azure into blob storage.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDatabaseImportExportStatus", ConfirmImpact = ConfirmImpact.None)]
    public class GetAzureSqlDatabaseImportExportStatus : SqlDatabaseManagementCmdletBase
    {
        #region Parameter sets

        /// <summary>
        /// The name of the parameter set that uses a RequestObject
        /// </summary>
        internal const string ByRequestObjectParameterSet =
            "ByRequestObject";

        /// <summary>
        /// The name of the parameter set that gets the connection information from
        /// the parameters
        /// </summary>
        internal const string ByConnectionInfoParameterSet =
            "ByConnectionInfo";

        #endregion

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="GetAzureSqlDatabaseImportExportStatus"/> class.
        /// </summary>
        public GetAzureSqlDatabaseImportExportStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="GetAzureSqlDatabaseImportExportStatus"/> class.
        /// </summary>
        /// <param name="channel">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        public GetAzureSqlDatabaseImportExportStatus(ISqlDatabaseManagement channel)
        {
            this.Channel = channel;
        }

        #region Parameters

        /// <summary>
        /// Gets or sets the user name for connecting to the database
        /// </summary>
        [Parameter(Mandatory = true, Position = 0,
            ParameterSetName = ByConnectionInfoParameterSet,
            HelpMessage = "The user name for connecting to the database")]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for connecting to the database
        /// </summary>
        [Parameter(Mandatory = true, Position = 1,
            ParameterSetName = ByConnectionInfoParameterSet,
            HelpMessage = "The password for connecting to the database")]
        [ValidateNotNullOrEmpty]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the server the database resides in
        /// </summary>
        [Parameter(Mandatory = true, Position = 2,
            ParameterSetName = ByConnectionInfoParameterSet,
            HelpMessage = "The name of the server the database is in")]
        [ValidateNotNullOrEmpty]
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the request Id of the operation to get the status of
        /// </summary>
        [Parameter(Mandatory = true, Position = 3,
            ParameterSetName = ByConnectionInfoParameterSet,
            HelpMessage = "The request Id of the operation to get the status of")]
        [ValidateNotNullOrEmpty]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the request object 
        /// </summary>
        [Parameter(Mandatory = true, Position = 0,
            ParameterSetName = ByRequestObjectParameterSet,
            HelpMessage = "The ImportExportRequest object returned from starting the request")]
        [ValidateNotNullOrEmpty]
        public ImportExportRequest Request { get; set; }

        #endregion

        /// <summary>
        /// Performs the call to export database using the server data service context channel.
        /// </summary>
        /// <param name="serverName">The name of the server to connect to.</param>
        /// <param name="userName">The username for authentication</param>
        /// <param name="password">The password for authentication</param>
        /// <param name="requestId">The request Id of the operation to query</param>
        /// <returns>The status of the import/export operation</returns>
        internal ArrayOfStatusInfo GetAzureSqlDatabaseImportExportStatusProcess(
            string serverName, 
            string userName,
            string password,
            string requestId)
        {
            ArrayOfStatusInfo result = null;

            try
            {
                this.InvokeInOperationContext(() =>
                {
                    result = RetryCall(subscription =>
                        this.Channel.GetImportExportStatus(
                            subscription,
                            serverName, 
                            serverName + DataServiceConstants.AzureSqlDatabaseDnsSuffix,
                            userName, 
                            password, 
                            requestId));

                    Operation operation = WaitForSqlDatabaseOperation();
                });
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return result;
        }

        /// <summary>
        /// Process the export request
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Starting to process the record");
            try
            {
                base.ProcessRecord();

                string serverName = null;
                string userName = null;
                string password = null;
                string requestId = null;

                switch (this.ParameterSetName)
                {
                    case ByRequestObjectParameterSet:
                        serverName = this.Request.ServerName;
                        userName = this.Request.SqlCredentials.UserName;
                        password = this.Request.SqlCredentials.Password;
                        requestId = this.Request.RequestGuid;
                        break;
                    case ByConnectionInfoParameterSet:
                        serverName = this.ServerName;
                        userName = this.Username;
                        password = this.Password;
                        requestId = this.RequestId;
                        break;
                }

                ArrayOfStatusInfo status = 
                    this.GetAzureSqlDatabaseImportExportStatusProcess(
                        serverName, 
                        userName, 
                        password, 
                        requestId);

                if (status == null)
                {
                    this.WriteVerbose("The result is null");
                }

                this.WriteVerbose("Status: " + status[0].Status);

                this.WriteObject(status);
            }
            catch (Exception ex)
            {
                this.WriteDebug("There was an error: " + ex.Message);
                this.WriteWindowsAzureError(
                    new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
                this.WriteExceptionError(ex);
            }
        }
    }
}
