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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// The information needed to connect to a SQL Azure database for the export operation
    /// </summary>
    [SerializableAttribute]
    [DataContractAttribute(Name = "ConnectionInfo", 
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
    public class ConnectionInfo : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the name of the database to connect to
        /// </summary>
        [DataMemberAttribute]
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the password for connecting to the database
        /// </summary>
        [DataMemberAttribute]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the server the database is in
        /// </summary>
        [DataMemberAttribute]
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the username for connecting to the database
        /// </summary>
        [DataMemberAttribute]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the extension data for the connection info
        /// </summary>
        [BrowsableAttribute(false)]
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
