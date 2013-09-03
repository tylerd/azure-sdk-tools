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
    /// Represents the input info necessary to send an Import request
    /// </summary>
    [SerializableAttribute]
    [DataContractAttribute(Name = "ImportInput",
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
    public class ImportInput : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the azure edition
        /// </summary>
        [DataMemberAttribute]
        public string AzureEdition { get; set; }

        /// <summary>
        /// Gets or sets the blob credentials
        /// </summary>
        [DataMemberAttribute(IsRequired = true, EmitDefaultValue = false)]
        public BlobCredentials BlobCredentials { get; set; }

        /// <summary>
        /// Gets or sets the connection information for SQL Azure
        /// </summary>
        [DataMemberAttribute(IsRequired = true, EmitDefaultValue = false)]
        public ConnectionInfo ConnectionInfo { get; set; }

        /// <summary>
        /// Gets or sets the size of the database in GB
        /// </summary>
        [DataMemberAttribute]
        public int DatabaseSizeInGB { get; set; }

        /// <summary>
        /// Gets or sets the import input object extension data
        /// </summary>
        [BrowsableAttribute(false)]
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
