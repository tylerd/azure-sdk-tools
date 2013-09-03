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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.IaaS
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Properties;
    using Utilities.Common;
    using WindowsAzure.ServiceManagement;

    [Cmdlet(VerbsCommon.Remove, "AzureVM"), OutputType(typeof(ManagementOperationContext))]
    public class RemoveAzureVMCommand : IaaSDeploymentManagementCmdletBase
    {
        public RemoveAzureVMCommand()
        {
        }

        public RemoveAzureVMCommand(IServiceManagement channel)
        {
            Channel = channel;
        }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the role to remove.")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get;
            set;
        }

        internal override void ExecuteCommand()
        {
            base.ExecuteCommand();
            if (CurrentDeployment == null)
            {
                return;
            }

            Deployment deployment = null;
            InvokeInOperationContext(() => deployment = RetryCall(s => Channel.GetDeploymentBySlot(s, ServiceName, DeploymentSlotType.Production)));

            if (deployment.RoleList.FirstOrDefault(r => r.RoleName.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
                throw new ArgumentOutOfRangeException(String.Format(Resources.RoleInstanceCanNotBeFoundWithName, Name));
            }

            if (deployment.RoleInstanceList.Count > 1)
            {
                ExecuteClientActionInOCS(null, CommandRuntime.ToString(), s => Channel.DeleteRole(s, ServiceName, CurrentDeployment.Name, Name));
            }
            else
            {
                ExecuteClientActionInOCS(null, CommandRuntime.ToString(), s => Channel.DeleteDeploymentBySlot(s, ServiceName, DeploymentSlotType.Production));
            }
        }
    }
}