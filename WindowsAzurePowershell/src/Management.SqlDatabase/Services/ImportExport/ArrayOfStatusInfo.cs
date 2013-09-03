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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Services
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport;

    /// <summary>
    /// Represents an array of status info objects.  The result of querying the IE service
    /// for the status of an IE operation.
    /// </summary>
    [CollectionDataContract(Name = "ArrayOfStatusInfo", ItemName = "StatusInfo",
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
    public class ArrayOfStatusInfo : List<StatusInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfStatusInfo"/> class.
        /// </summary>
        public ArrayOfStatusInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayOfStatusInfo"/> class.  Populates
        /// the list with a set of elements.
        /// </summary>
        /// <param name="statusList">The list of <see cref="StatusInfo"/> to populate
        /// the list with.</param>
        public ArrayOfStatusInfo(IEnumerable<StatusInfo> statusList)
            : base(statusList)
        {
        }
    }
}
