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
    using System.Runtime.Serialization;

    /// <summary>
    /// The necessary information to submit an export request to IE
    /// </summary>
    [SerializableAttribute]
    [DataContractAttribute(Name = "ExportInput", 
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
    public partial class ExportInput : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the blob credentials for the export destination
        /// </summary>
        [DataMemberAttribute(IsRequired = true, EmitDefaultValue = false)]
        public BlobCredentials BlobCredentials { get; set; }

        /// <summary>
        /// Gets or sets the connection info used for connecting to the database
        /// </summary>
        [DataMemberAttribute(IsRequired = true, EmitDefaultValue = false)]
        public ConnectionInfo ConnectionInfo { get; set; }

        /// <summary>
        /// Gets or sets the extension data for the export input object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
