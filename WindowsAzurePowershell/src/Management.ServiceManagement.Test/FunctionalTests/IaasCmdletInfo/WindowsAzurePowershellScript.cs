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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.IaasCmdletInfo
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.PowershellCore;

    public class WindowsAzurePowershellScript : PowershellCmdletScript
    {
        private static readonly string[] modules = new[]
        {
            Path.Combine(Utilities.windowsAzurePowershellPath, Utilities.windowsAzurePowershellModuleStorage),
            Path.Combine(Utilities.windowsAzurePowershellPath, Utilities.windowsAzurePowershellModuleManagement),
            Path.Combine(Utilities.windowsAzurePowershellPath, Utilities.windowsAzurePowershellModuleService),
            Path.Combine(Utilities.windowsAzurePowershellPath, Utilities.windowsAzurePowershellModuleServiceManagement),
            Path.Combine(Utilities.windowsAzurePowershellPath, Utilities.windowsAzurePowershellModuleServiceManagementPlatformImageRepository)
        };

        public WindowsAzurePowershellScript(List<string> cmdlets) : base(cmdlets, ConstructModules())
        {
        }
        public WindowsAzurePowershellScript() : base(ConstructModules())
        {
        }

        public static string[] Modules
        {
            get
            {
                return modules;
            }
        }

        private static PowershellModule[] ConstructModules()
        {
            return new[]
            {
                new PowershellModule(Utilities.windowsAzurePowershellModuleManagement, Utilities.windowsAzurePowershellPath),
                new PowershellModule(Utilities.windowsAzurePowershellModuleStorage, Utilities.windowsAzurePowershellPath),
                new PowershellModule(Utilities.windowsAzurePowershellModuleService, Utilities.windowsAzurePowershellPath),
                new PowershellModule(Utilities.windowsAzurePowershellModuleServiceManagement, Utilities.windowsAzurePowershellPath),
                new PowershellModule(Utilities.windowsAzurePowershellModuleServiceManagementPlatformImageRepository, Utilities.windowsAzurePowershellPath)
            };
        }
    }
}