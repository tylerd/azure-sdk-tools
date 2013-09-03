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


namespace Microsoft.WindowsAzure.Management.ServiceManagement.Helpers
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Xml.Serialization;
    using Model;
    using WindowsAzure.ServiceManagement;
    using Properties;
    using System.Text.RegularExpressions;
    using System.Management.Automation;

    public static class PersistentVMHelper
    {
        public static void SaveStateToFile(PersistentVM role, string filePath)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role", Resources.MissingPersistentVMRole);
            }
            
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes ignoreAttrib = new XmlAttributes();
            ignoreAttrib.XmlIgnore = true;
            overrides.Add(typeof(DataVirtualHardDisk), "MediaLink", ignoreAttrib);
            overrides.Add(typeof(DataVirtualHardDisk), "SourceMediaLink", ignoreAttrib);
            overrides.Add(typeof(OSVirtualHardDisk), "MediaLink", ignoreAttrib);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PersistentVM), overrides, new Type[] { typeof(NetworkConfigurationSet) }, null, null);
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, role);
            }
        }

        public static PersistentVM LoadStateFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException(Resources.MissingPersistentVMFile, "filePath");
            }

            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes ignoreAttrib = new XmlAttributes();
            ignoreAttrib.XmlIgnore = true;
            overrides.Add(typeof(DataVirtualHardDisk), "MediaLink", ignoreAttrib);
            overrides.Add(typeof(DataVirtualHardDisk), "SourceMediaLink", ignoreAttrib);
            overrides.Add(typeof(OSVirtualHardDisk), "MediaLink", ignoreAttrib);
            overrides.Add(typeof(OSVirtualHardDisk), "SourceImageName", ignoreAttrib);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PersistentVM), overrides, new Type[] { typeof(NetworkConfigurationSet) }, null, null);

            PersistentVM role = null;
            
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                role = serializer.Deserialize(stream) as PersistentVM;
            }

            return role;
        }

        // Returns a RoleNamesCollection based on instances in the roleInstanceList
        // whose RoleInstance.RoleName matches the roleName passed in.  Wildcards
        // are handled for the roleName passed in.
        // This function also verifies that the RoleInstance exists before adding the
        // RoleName to the RoleNamesCollection.
        public static RoleNamesCollection GetRoleNames(RoleInstanceList roleInstanceList, string roleName)
        {
            var roleNamesCollection = new RoleNamesCollection();
            if (!string.IsNullOrEmpty(roleName))
            {
                if (WildcardPattern.ContainsWildcardCharacters(roleName))
                {
                    WildcardOptions wildcardOptions = WildcardOptions.IgnoreCase | WildcardOptions.Compiled;
                    WildcardPattern wildcardPattern = new WildcardPattern(roleName, wildcardOptions);

                    foreach (RoleInstance role in roleInstanceList)
                        if (!string.IsNullOrEmpty(role.RoleName) && wildcardPattern.IsMatch(role.RoleName))
                        {
                            roleNamesCollection.Add(role.RoleName);
                        }
                }
                else
                {
                    var roleInstance = roleInstanceList.Where(r => r.RoleName != null).
                        FirstOrDefault(r => r.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
                    if (roleInstance != null)
                    {
                        roleNamesCollection.Add(roleName);
                    }
                }
            }
            return roleNamesCollection;
        }
    }
}
