﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.IaasCmdletInfo
{
    using Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.ConfigDataInfo;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.PowershellCore;

    public class AddAzureEndpointCmdletInfo : CmdletsInfo
    {
        public AddAzureEndpointCmdletInfo(AzureEndPointConfigInfo endPointConfig)
        {
            this.cmdletName = Utilities.AddAzureEndpointCmdletName;
              
            this.cmdletParams.Add(new CmdletParam("Name", endPointConfig.EndpointName));
            this.cmdletParams.Add(new CmdletParam("LocalPort", endPointConfig.EndpointLocalPort));
            if (endPointConfig.EndpointPublicPort.HasValue)
            {
                this.cmdletParams.Add(new CmdletParam("PublicPort", endPointConfig.EndpointPublicPort));
            }
            if (endPointConfig.Acl != null)
            {
                this.cmdletParams.Add(new CmdletParam("ACL", endPointConfig.Acl));
            }
            this.cmdletParams.Add(new CmdletParam("Protocol", endPointConfig.EndpointProtocol.ToString()));
            this.cmdletParams.Add(new CmdletParam("VM", endPointConfig.Vm));

            if (endPointConfig.ParamSet == AzureEndPointConfigInfo.ParameterSet.DefaultProbe)
            {
                this.cmdletParams.Add(new CmdletParam("LBSetName", endPointConfig.LBSetName));
                this.cmdletParams.Add(new CmdletParam("DefaultProbe"));
            }
            if (endPointConfig.ParamSet == AzureEndPointConfigInfo.ParameterSet.LoadBalancedNoProbe)
            {
                this.cmdletParams.Add(new CmdletParam("LBSetName", endPointConfig.LBSetName));
                this.cmdletParams.Add(new CmdletParam("NoProbe"));
            }
            else if (endPointConfig.ParamSet == AzureEndPointConfigInfo.ParameterSet.CustomProbe)
            {
                this.cmdletParams.Add(new CmdletParam("LBSetName", endPointConfig.LBSetName));
                this.cmdletParams.Add(new CmdletParam("ProbePort", endPointConfig.ProbePort));
                this.cmdletParams.Add(new CmdletParam("ProbeProtocol", endPointConfig.ProbeProtocol.ToString()));
                if ("http" == endPointConfig.ProbeProtocol.ToString())
                {
                    this.cmdletParams.Add(new CmdletParam("ProbePath", endPointConfig.ProbePath));
                }

                if (endPointConfig.ProbeInterval.HasValue)
                {
                    this.cmdletParams.Add(new CmdletParam("ProbeIntervalInSeconds", endPointConfig.ProbeInterval));
                }

                if (endPointConfig.ProbeTimeout.HasValue)
                {
                    this.cmdletParams.Add(new CmdletParam("ProbeTimeoutInSeconds", endPointConfig.ProbeTimeout));
                }
            }

            if (endPointConfig.DirectServerReturn)
            {
                this.cmdletParams.Add(new CmdletParam("DirectServerReturn", endPointConfig.DirectServerReturn));
            }
        }

        public AddAzureEndpointCmdletInfo()
        {
            this.cmdletName = Utilities.AddAzureEndpointCmdletName;
        }
    }
}
