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


namespace Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Extensions;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Model;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Test.FunctionalTests.ConfigDataInfo;
    using Microsoft.WindowsAzure.Management.ServiceManagement.Test.Properties;
    using Microsoft.WindowsAzure.ServiceManagement;
    

    [TestClass]
    public class FunctionalTest : ServiceManagementTest
    {
        private string serviceName;
        private string vmName;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            //SetTestSettings();

            if (defaultAzureSubscription.Equals(null))
            {
                Assert.Inconclusive("No Subscription is selected!");
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            serviceName = Utilities.GetUniqueShortName(serviceNamePrefix);
            vmName = Utilities.GetUniqueShortName(vmNamePrefix);
            pass = false;
            testStartTime = DateTime.Now;         
        }
              
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Get-AzureStorageAccount)")]
        [Ignore]
        public void ScriptTestSample()
        {
            var result = vmPowershellCmdlets.RunPSScript("Get-Help Save-AzureVhd -full");
        }  

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((New,Get,Set,Remove)-AzureAffinityGroup)")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Resources\\affinityGroupData.csv", "affinityGroupData#csv", DataAccessMethod.Sequential)]
        public void AzureAffinityGroupTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            string affinityName1 = Convert.ToString(TestContext.DataRow["affinityName1"]);
            string affinityLabel1 = Convert.ToString(TestContext.DataRow["affinityLabel1"]);
            string location1 = CheckLocation(Convert.ToString(TestContext.DataRow["location1"]));
            string description1 = Convert.ToString(TestContext.DataRow["description1"]);

            string affinityName2 = Convert.ToString(TestContext.DataRow["affinityName2"]);
            string affinityLabel2 = Convert.ToString(TestContext.DataRow["affinityLabel2"]);
            string location2 = CheckLocation(Convert.ToString(TestContext.DataRow["location2"]));
            string description2 = Convert.ToString(TestContext.DataRow["description2"]);
           
            try
            {
                ServiceManagementCmdletTestHelper vmPowershellCmdlets = new ServiceManagementCmdletTestHelper();

                // Remove previously created affinity groups
                foreach (var aff in vmPowershellCmdlets.GetAzureAffinityGroup(null))
                {
                    if (aff.Name == affinityName1 || aff.Name == affinityName2)
                    {
                        vmPowershellCmdlets.RemoveAzureAffinityGroup(aff.Name);
                    }
                }
               
                // New-AzureAffinityGroup
                vmPowershellCmdlets.NewAzureAffinityGroup(affinityName1, location1, affinityLabel1, description1);
                vmPowershellCmdlets.NewAzureAffinityGroup(affinityName2, location2, affinityLabel2, description2);
                Console.WriteLine("Affinity groups created: {0}, {1}", affinityName1, affinityName2);

                // Get-AzureAffinityGroup

                pass = AffinityGroupVerify(vmPowershellCmdlets.GetAzureAffinityGroup(affinityName1)[0], affinityName1, affinityLabel1, location1, description1);
                pass &= AffinityGroupVerify(vmPowershellCmdlets.GetAzureAffinityGroup(affinityName2)[0], affinityName2, affinityLabel2, location2, description2);
                

                // Set-AzureAffinityGroup
                vmPowershellCmdlets.SetAzureAffinityGroup(affinityName2, affinityLabel1, description1);
                Console.WriteLine("update affinity group: {0}", affinityName2);

                pass &= AffinityGroupVerify(vmPowershellCmdlets.GetAzureAffinityGroup(affinityName2)[0], affinityName2, affinityLabel1, location2, description1);
               

                // Remove-AzureAffinityGroup
                vmPowershellCmdlets.RemoveAzureAffinityGroup(affinityName2);
                pass &= Utilities.CheckRemove(vmPowershellCmdlets.GetAzureAffinityGroup, affinityName2);
                vmPowershellCmdlets.RemoveAzureAffinityGroup(affinityName1);
                pass &= Utilities.CheckRemove(vmPowershellCmdlets.GetAzureAffinityGroup, affinityName1);
                
            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail(e.ToString());
            }
        }

        private bool AffinityGroupVerify(AffinityGroupContext affContext, string name, string label, string location, string description)
        {
            bool result = true;

            Console.WriteLine("AffinityGroup: Name - {0}, Location - {1}, Label - {2}, Description - {3}", affContext.Name, affContext.Location, affContext.Label, affContext.Description);
            try
            {
                Assert.AreEqual(affContext.Name, name, "Error: Affinity Name is not equal!");
                Assert.AreEqual(affContext.Label, label, "Error: Affinity Label is not equal!");
                Assert.AreEqual(affContext.Location, location, "Error: Affinity Location is not equal!");
                Assert.AreEqual(affContext.Description, description, "Error: Affinity Description is not equal!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                result = false;
            }
            return result;
        }

        private string CheckLocation(string loc)
        {
            string checkLoc = vmPowershellCmdlets.GetAzureLocationName(new string[] { loc });
            if (string.IsNullOrEmpty(checkLoc))
            {
                foreach (LocationsContext l in vmPowershellCmdlets.GetAzureLocation())
                {
                    if (l.AvailableServices.Contains("Storage"))
                    {
                        return l.Name;
                    }
                }
                return null;
            }
            else
            {
                return checkLoc;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (New-AzureCertificateSetting)")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Resources\\certificateData.csv", "certificateData#csv", DataAccessMethod.Sequential)]
        public void AzureCertificateSettingTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            // Create a certificate
            string cerFileName = Convert.ToString(TestContext.DataRow["cerFileName"]);
            X509Certificate2 certCreated = Utilities.CreateCertificate(password);
            byte[] certData2 = certCreated.Export(X509ContentType.Cert);
            File.WriteAllBytes(cerFileName, certData2);

            // Install the .cer file to local machine.
            StoreLocation certStoreLocation = StoreLocation.CurrentUser;
            StoreName certStoreName = StoreName.My;
            X509Certificate2 installedCert = Utilities.InstallCert(cerFileName, certStoreLocation, certStoreName);

            PSObject certToUpload = vmPowershellCmdlets.RunPSScript(
                String.Format("Get-Item cert:\\{0}\\{1}\\{2}", certStoreLocation.ToString(), certStoreName.ToString(), installedCert.Thumbprint))[0];

            try
            {
                vmPowershellCmdlets.NewAzureService(serviceName, locationName);
                vmPowershellCmdlets.AddAzureCertificate(serviceName, certToUpload);

                CertificateSettingList certList = new CertificateSettingList();
                certList.Add(vmPowershellCmdlets.NewAzureCertificateSetting(certStoreName.ToString(), installedCert.Thumbprint));

                AzureVMConfigInfo azureVMConfigInfo = new AzureVMConfigInfo(vmName, InstanceSize.Small, imageName);
                AzureProvisioningConfigInfo azureProvisioningConfig = new AzureProvisioningConfigInfo(OS.Windows, certList, username, password);

                PersistentVMConfigInfo persistentVMConfigInfo = new PersistentVMConfigInfo(azureVMConfigInfo, azureProvisioningConfig, null, null);

                PersistentVM vm = vmPowershellCmdlets.GetPersistentVM(persistentVMConfigInfo);

                vmPowershellCmdlets.NewAzureVM(serviceName, new[] { vm });

                PersistentVMRoleContext result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);

                Console.WriteLine("{0} is created", result.Name);

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;                
                Assert.Fail(e.ToString());
            }
            finally
            {
                Utilities.UninstallCert(installedCert, certStoreLocation, certStoreName);
            }

        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "PAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((New,Get,Set,Remove,Move)-AzureDeployment)")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Resources\\package.csv", "package#csv", DataAccessMethod.Sequential)]
        public void AzureDeploymentTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);
            
            // Choose the package and config files from local machine
            string packageName = Convert.ToString(TestContext.DataRow["packageName"]);
            string configName = Convert.ToString(TestContext.DataRow["configName"]);
            string upgradePackageName = Convert.ToString(TestContext.DataRow["upgradePackage"]);
            string upgradeConfigName = Convert.ToString(TestContext.DataRow["upgradeConfig"]);
            string upgradeConfigName2 = Convert.ToString(TestContext.DataRow["upgradeConfig2"]);            

            var packagePath1 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + packageName);
            var configPath1 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + configName);
            var packagePath2 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + upgradePackageName);
            var configPath2 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + upgradeConfigName);
            var configPath3 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + upgradeConfigName2);

            Assert.IsTrue(File.Exists(packagePath1.FullName), "file not exist={0}", packagePath1);
            Assert.IsTrue(File.Exists(packagePath2.FullName), "file not exist={0}", packagePath2);
            Assert.IsTrue(File.Exists(configPath1.FullName), "file not exist={0}", configPath1);
            Assert.IsTrue(File.Exists(configPath2.FullName), "file not exist={0}", configPath2);
            Assert.IsTrue(File.Exists(configPath3.FullName), "file not exist={0}", configPath3);
            
            string deploymentName = "deployment1";
            string deploymentLabel = "label1";
            DeploymentInfoContext result;

            try
            {
                vmPowershellCmdlets.NewAzureService(serviceName, serviceName, locationName);
                Console.WriteLine("service, {0}, is created.", serviceName);

                vmPowershellCmdlets.NewAzureDeployment(serviceName, packagePath1.FullName, configPath1.FullName, DeploymentSlotType.Staging, deploymentLabel, deploymentName, false, false);
                result = vmPowershellCmdlets.GetAzureDeployment(serviceName, DeploymentSlotType.Staging);
                pass = Utilities.PrintAndCompareDeployment(result, serviceName, deploymentName, deploymentLabel, DeploymentSlotType.Staging, null, 1);
                Console.WriteLine("successfully deployed the package");


                // Move the deployment from 'Staging' to 'Production'
                vmPowershellCmdlets.MoveAzureDeployment(serviceName);
                result = vmPowershellCmdlets.GetAzureDeployment(serviceName, DeploymentSlotType.Production);
                pass &= Utilities.PrintAndCompareDeployment(result, serviceName, deploymentName, deploymentLabel, DeploymentSlotType.Production, null, 1);                
                Console.WriteLine("successfully moved");

                // Set the deployment status to 'Suspended'
                vmPowershellCmdlets.SetAzureDeploymentStatus(serviceName, DeploymentSlotType.Production, DeploymentStatus.Suspended);
                result = vmPowershellCmdlets.GetAzureDeployment(serviceName, DeploymentSlotType.Production);
                pass &= Utilities.PrintAndCompareDeployment(result, serviceName, deploymentName, deploymentLabel, DeploymentSlotType.Production, DeploymentStatus.Suspended, 1);
                Console.WriteLine("successfully changed the status");

                // Update the deployment
                vmPowershellCmdlets.SetAzureDeploymentConfig(serviceName, DeploymentSlotType.Production, configPath2.FullName);
                result = vmPowershellCmdlets.GetAzureDeployment(serviceName, DeploymentSlotType.Production);
                pass &= Utilities.PrintAndCompareDeployment(result, serviceName, deploymentName, deploymentLabel, DeploymentSlotType.Production, null, 2);
                Console.WriteLine("successfully updated the deployment");

                // Upgrade the deployment
                DateTime start = DateTime.Now;
                vmPowershellCmdlets.SetAzureDeploymentUpgrade(serviceName, DeploymentSlotType.Production, UpgradeType.Simultaneous, packagePath2.FullName, configPath3.FullName);
                TimeSpan duration = DateTime.Now - start;
                Console.WriteLine("Auto upgrade took {0}.", duration);

                result = vmPowershellCmdlets.GetAzureDeployment(serviceName, DeploymentSlotType.Production);
                pass &= Utilities.PrintAndCompareDeployment(result, serviceName, deploymentName, serviceName, DeploymentSlotType.Production, null, 4);
                Console.WriteLine("successfully updated the deployment");
                               
                vmPowershellCmdlets.RemoveAzureDeployment(serviceName, DeploymentSlotType.Production, true);

                pass &= Utilities.CheckRemove(vmPowershellCmdlets.GetAzureDeployment, serviceName, DeploymentSlotType.Production);                
            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((New,Get)-AzureDns)")]
        public void AzureDnsTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            string dnsName = "OpenDns1";
            string ipAddress = "208.67.222.222";

            try
            {
                vmPowershellCmdlets.NewAzureService(serviceName, locationName);

                DnsServer dns = vmPowershellCmdlets.NewAzureDns(dnsName, ipAddress);

                AzureVMConfigInfo azureVMConfigInfo = new AzureVMConfigInfo(vmName, InstanceSize.ExtraSmall, imageName);
                AzureProvisioningConfigInfo azureProvisioningConfig = new AzureProvisioningConfigInfo(OS.Windows, username, password);     
           
                PersistentVMConfigInfo persistentVMConfigInfo = new PersistentVMConfigInfo(azureVMConfigInfo, azureProvisioningConfig, null, null);           
                
                PersistentVM vm = vmPowershellCmdlets.GetPersistentVM(persistentVMConfigInfo);  
           
                vmPowershellCmdlets.NewAzureVM(serviceName, new []{vm}, null, new[]{dns}, null, null, null, null);

                Assert.IsTrue(Verify.AzureDns(vmPowershellCmdlets.GetAzureDeployment(serviceName).DnsSettings, dns));
                pass = true;

            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Get-AzureLocation)")]
        public void AzureLocationTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            try
            {
                foreach (LocationsContext loc in vmPowershellCmdlets.GetAzureLocation())
                {
                    Console.WriteLine("Location: Name - {0}, DisplayName - {1}", loc.Name, loc.DisplayName);
                }

                pass = true;

            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }            
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Get-AzureOSVersion)")]
        public void AzureOSVersionTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);       

            try
            {
                foreach (OSVersionsContext osVersions in vmPowershellCmdlets.GetAzureOSVersion())
                {
                    Console.WriteLine("OS Version: Family - {0}, FamilyLabel - {1}, Version - {2}", osVersions.Family, osVersions.FamilyLabel, osVersions.Version);
                }

                pass = true;

            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }
        }


        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "PAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((Get,Set)-AzureRole)")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Resources\\package.csv", "package#csv", DataAccessMethod.Sequential)]
        public void AzureRoleTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            // Choose the package and config files from local machine
            string packageName = Convert.ToString(TestContext.DataRow["packageName"]);
            string configName = Convert.ToString(TestContext.DataRow["configName"]);
            string upgradePackageName = Convert.ToString(TestContext.DataRow["upgradePackage"]);
            string upgradeConfigName = Convert.ToString(TestContext.DataRow["upgradeConfig"]);

            var packagePath1 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + packageName);
            var configPath1 = new FileInfo(Directory.GetCurrentDirectory() + "\\" + configName);

            Assert.IsTrue(File.Exists(packagePath1.FullName), "VHD file not exist={0}", packagePath1);
            Assert.IsTrue(File.Exists(configPath1.FullName), "VHD file not exist={0}", configPath1);

            string deploymentName = "deployment1";
            string deploymentLabel = "label1";
            string slot = DeploymentSlotType.Production;

            //DeploymentInfoContext result;
            string roleName = "";

            try
            {
                vmPowershellCmdlets.NewAzureService(serviceName, serviceName, locationName);

                vmPowershellCmdlets.NewAzureDeployment(serviceName, packagePath1.FullName, configPath1.FullName, slot, deploymentLabel, deploymentName, false, false);

            
                foreach (RoleContext role in vmPowershellCmdlets.GetAzureRole(serviceName, slot, null, false))
                {
                    Console.WriteLine("Role: Name - {0}, ServiceName - {1}, DeploymenntID - {2}, InstanceCount - {3}", role.RoleName, role.ServiceName, role.DeploymentID, role.InstanceCount);
                    Assert.AreEqual(serviceName, role.ServiceName);
                    roleName = role.RoleName;
                }
                
                vmPowershellCmdlets.SetAzureRole(serviceName, slot, roleName, 2);

                foreach (RoleContext role in vmPowershellCmdlets.GetAzureRole(serviceName, slot, null, false))
                {
                    Console.WriteLine("Role: Name - {0}, ServiceName - {1}, DeploymenntID - {2}, InstanceCount - {3}", role.RoleName, role.ServiceName, role.DeploymentID, role.InstanceCount);
                    Assert.AreEqual(serviceName, role.ServiceName);
                    Assert.AreEqual(2, role.InstanceCount);                   
                }

                pass = true;                

            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "PAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (New-AzureServiceRemoteDesktopConfig)")]
        public void AzureServiceDiagnosticsExtensionConfigTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            List<string> defaultRoles = new List<string>(new string[] { "AllRoles" });
            string[] roles = new string[] { "WebRole1", "WorkerRole2" };
            string thumb = "abc";
            string alg = "sha1";

            // Create a certificate
            X509Certificate2 cert = Utilities.CreateCertificate(password);

            string storage = defaultAzureSubscription.CurrentStorageAccount;
            XmlDocument daConfig = new XmlDocument();
            daConfig.Load(@".\da.xml");

            try
            {
                //// Case 1: No thumbprint, no Certificate
                ExtensionConfigurationInput resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, daConfig);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, daConfig));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, null, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles)));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, daConfig, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), daConfig));

                // Case 2: Thumbprint, no algorithm
                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, null);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, null, thumb));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, null, daConfig);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, daConfig, thumb));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, null, null, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), null, thumb));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, null, daConfig, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), daConfig, thumb));

                // Case 3: Thumbprint and algorithm
                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, alg);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, null, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, alg, daConfig);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, daConfig, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, alg, null, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), null, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, thumb, alg, daConfig, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), daConfig, thumb, alg));

                // Case 4: Certificate
                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, cert);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, null, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, cert, daConfig);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, defaultRoles, daConfig, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, cert, null, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), null, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceDiagnosticsExtensionConfig(storage, cert, daConfig, roles);
                Assert.IsTrue(VerifyExtensionConfigDiag(resultConfig, storage, new List<string>(roles), daConfig, cert.Thumbprint, null, cert));

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }
        }

        private bool VerifyExtensionConfigDiag(ExtensionConfigurationInput resultConfig, string storage, List<string> roles, XmlDocument wadconfig = null, string thumbprint = null, string algorithm = null, X509Certificate2 cert = null)
        {
            try
            {
                string resultStorageAccount = GetInnerText(resultConfig.PublicConfiguration, "Name");
                string resultWadCfg = Utilities.GetInnerXml(resultConfig.PublicConfiguration, "WadCfg");
                if (string.IsNullOrWhiteSpace(resultWadCfg))
                {
                    resultWadCfg = null;
                }
                string resultStorageKey = GetInnerText(resultConfig.PrivateConfiguration, "StorageKey");

                Console.WriteLine("Type: {0}, StorageAccountName:{1}, StorageKey: {2}, WadCfg: {3}, CertificateThumbprint: {4}, ThumbprintAlgorithm: {5}, X509Certificate: {6}",
                    resultConfig.Type, resultStorageAccount, resultStorageKey, resultWadCfg, resultConfig.CertificateThumbprint, resultConfig.ThumbprintAlgorithm, resultConfig.X509Certificate);

                Assert.AreEqual(resultConfig.Type, "Diagnostics", "Type is not equal!");
                Assert.AreEqual(resultStorageAccount, storage);
                Assert.IsTrue(Utilities.CompareWadCfg(resultWadCfg, wadconfig));

                if (string.IsNullOrWhiteSpace(thumbprint))
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(resultConfig.CertificateThumbprint));
                }
                else
                {
                    Assert.AreEqual(resultConfig.CertificateThumbprint, thumbprint, "Certificate thumbprint is not equal!");
                }
                if (string.IsNullOrWhiteSpace(algorithm))
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(resultConfig.ThumbprintAlgorithm));
                }
                else
                {
                    Assert.AreEqual(resultConfig.ThumbprintAlgorithm, algorithm, "Thumbprint algorithm is not equal!");
                }
                Assert.AreEqual(resultConfig.X509Certificate, cert, "X509Certificate is not equal!");
                if (resultConfig.Roles.Count == 1 && string.IsNullOrEmpty(resultConfig.Roles[0].RoleName))
                {
                    Assert.IsTrue(roles.Contains(resultConfig.Roles[0].RoleType.ToString()));
                }
                else
                {
                    foreach (ExtensionRole role in resultConfig.Roles)
                    {
                        Assert.IsTrue(roles.Contains(role.RoleName));
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "PAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (New-AzureServiceRemoteDesktopConfig)")]
        public void AzureServiceRemoteDesktopExtensionConfigTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            PSCredential cred = new PSCredential(username, Utilities.convertToSecureString(password));
            DateTime exp = DateTime.Now.AddMonths(18);
            DateTime defaultExp = DateTime.Now.AddMonths(12);
            List<string> defaultRoles = new List<string>(new string[] { "AllRoles" });
            string[] roles = new string[]{"WebRole1", "WorkerRole2"};
            string thumb = "abc";
            string alg = "sha1";

            // Create a certificate
            X509Certificate2 cert = Utilities.CreateCertificate(password);

            try
            {
                // Case 1: No thumbprint, no Certificate
                ExtensionConfigurationInput resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, defaultExp));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, exp);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, exp));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, null, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), defaultExp));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, exp, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), exp));

                // Case 2: Thumbprint, no algorithm
                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, null);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, defaultExp, thumb));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, null, exp);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, exp, thumb));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, null, null, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), defaultExp, thumb));                

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, null, exp, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), exp, thumb));

                // Case 3: Thumbprint and algorithm
                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, alg);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, defaultExp, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, alg, exp);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, exp, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, alg, null, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), defaultExp, thumb, alg));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, thumb, alg, exp, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), exp, thumb, alg));

                // Case 4: Certificate
                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, cert);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, defaultExp, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, cert, null, exp);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, defaultRoles, exp, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, cert, null, null, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), defaultExp, cert.Thumbprint, null, cert));

                resultConfig = vmPowershellCmdlets.NewAzureServiceRemoteDesktopExtensionConfig(cred, cert, null, exp, roles);
                Assert.IsTrue(VerifyExtensionConfigRDP(resultConfig, username, password, new List<string>(roles), exp, cert.Thumbprint, null, cert));

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }
        }

        private bool VerifyExtensionConfigRDP(ExtensionConfigurationInput resultConfig, string user, string pass, List<string> roles, DateTime exp, string thumbprint = null, string algorithm = null, X509Certificate2 cert = null)
        {
            try
            {
                string resultUserName = GetInnerText(resultConfig.PublicConfiguration, "UserName");
                string resultPassword = GetInnerText(resultConfig.PrivateConfiguration, "Password");
                string resultExpDate = GetInnerText(resultConfig.PublicConfiguration, "Expiration");

                Console.WriteLine("Type: {0}, UserName:{1}, Password: {2}, ExpirationDate: {3}, CertificateThumbprint: {4}, ThumbprintAlgorithm: {5}, X509Certificate: {6}",
                    resultConfig.Type, resultUserName, resultPassword, resultExpDate, resultConfig.CertificateThumbprint, resultConfig.ThumbprintAlgorithm, resultConfig.X509Certificate);

                Assert.AreEqual(resultConfig.Type, "RDP", "Type is not equal!");
                Assert.AreEqual(resultUserName, user);
                Assert.AreEqual(resultPassword, pass);
                Assert.IsTrue(Utilities.CompareDateTime(exp, resultExpDate));

                if (string.IsNullOrWhiteSpace(thumbprint))
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(resultConfig.CertificateThumbprint));
                }
                else
                {
                    Assert.AreEqual(resultConfig.CertificateThumbprint, thumbprint, "Certificate thumbprint is not equal!");
                }

                if (string.IsNullOrWhiteSpace(algorithm))
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(resultConfig.ThumbprintAlgorithm));
                }
                else
                {
                    Assert.AreEqual(resultConfig.ThumbprintAlgorithm, algorithm, "Thumbprint algorithm is not equal!");
                }
                Assert.AreEqual(resultConfig.X509Certificate, cert, "X509Certificate is not equal!");
                if (resultConfig.Roles.Count == 1 && string.IsNullOrEmpty(resultConfig.Roles[0].RoleName))
                {
                    Assert.IsTrue(roles.Contains(resultConfig.Roles[0].RoleType.ToString()));
                }
                else
                {
                    foreach (ExtensionRole role in resultConfig.Roles)
                    {
                        Assert.IsTrue(roles.Contains(role.RoleName));
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetInnerText(string xmlString, string tag)
        {
            string removedHeader = xmlString.Substring(xmlString.IndexOf('<', 2));

            byte[] encodedString = Encoding.UTF8.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(encodedString);
            stream.Flush();
            stream.Position = 0;

            XmlDocument xml = new XmlDocument();
            xml.Load(stream);
            return xml.GetElementsByTagName(tag)[0].InnerText;
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((Get,Set)-AzureSubnet)")]
        public void AzureSubnetTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            try
            {
                vmPowershellCmdlets.NewAzureService(serviceName, serviceName, locationName);
                
                PersistentVM vm = vmPowershellCmdlets.NewAzureVMConfig(new AzureVMConfigInfo(vmName, InstanceSize.Small, imageName));
                AzureProvisioningConfigInfo azureProvisioningConfig = new AzureProvisioningConfigInfo(OS.Windows, username, password);
                azureProvisioningConfig.Vm = vm;

                string [] subs = new []  {"subnet1", "subnet2", "subnet3"};
                vm = vmPowershellCmdlets.SetAzureSubnet(vmPowershellCmdlets.AddAzureProvisioningConfig(azureProvisioningConfig), subs);
                
                SubnetNamesCollection subnets = vmPowershellCmdlets.GetAzureSubnet(vm);
                foreach (string subnet in subnets)
                {
                    Console.WriteLine("Subnet: {0}", subnet);
                }                
                CollectionAssert.AreEqual(subnets, subs);
                
                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((New,Get)-AzureStorageKey)")]
        public void AzureStorageKeyTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);
            
            try
            {
                StorageServiceKeyOperationContext key1 = vmPowershellCmdlets.GetAzureStorageAccountKey(defaultAzureSubscription.CurrentStorageAccount); // Get-AzureStorageAccountKey
                Console.WriteLine("Primary - {0}", key1.Primary);
                Console.WriteLine("Secondary - {0}", key1.Secondary);

                StorageServiceKeyOperationContext key2 = vmPowershellCmdlets.NewAzureStorageAccountKey(defaultAzureSubscription.CurrentStorageAccount, KeyType.Secondary);
                Console.WriteLine("Primary - {0}", key2.Primary);
                Console.WriteLine("Secondary - {0}", key2.Secondary);

                Assert.AreEqual(key1.Primary, key2.Primary);
                Assert.AreNotEqual(key1.Secondary, key2.Secondary);

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }            
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((New,Get,Set,Remove)-AzureStorageAccount)")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Resources\\storageAccountTestData.csv", "storageAccountTestData#csv", DataAccessMethod.Sequential)]
        public void AzureStorageAccountTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            string storageAccountPrefix = Convert.ToString(TestContext.DataRow["NamePrefix"]);
            string locationName1 = CheckLocation(Convert.ToString(TestContext.DataRow["Location1"]));
            string locationName2 = CheckLocation(Convert.ToString(TestContext.DataRow["Location2"]));
            string affinityGroupName = Convert.ToString(TestContext.DataRow["AffinityGroupName"]);

            string[] label = new string[3] {
                Convert.ToString(TestContext.DataRow["Label1"]),
                Convert.ToString(TestContext.DataRow["Label2"]),
                Convert.ToString(TestContext.DataRow["Label3"])};
            string[] description = new string[3] {
                Convert.ToString(TestContext.DataRow["Description1"]),
                Convert.ToString(TestContext.DataRow["Description2"]),
                Convert.ToString(TestContext.DataRow["Description3"])};
            bool?[] geoReplicationSettings = new bool?[3] { true, false, null };

            bool geoReplicationEnabled = true;
           
            string[] storageName = new string[2] {
                Utilities.GetUniqueShortName(storageAccountPrefix),
                Utilities.GetUniqueShortName(storageAccountPrefix)};

            string[][] storageStaticProperties =  new string[2][] {
                new string[3] {storageName[0], locationName1, null},
                new string [3] {storageName[1], null, affinityGroupName}};

            try
            {
                // New-AzureStorageAccount test
                vmPowershellCmdlets.NewAzureStorageAccount(storageName[0], locationName1, null, null, null);
                Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[0])[0],
                    storageStaticProperties[0], storageName[0], null, true));
                Console.WriteLine("{0} is created", storageName[0]);

                if (Utilities.CheckRemove(vmPowershellCmdlets.GetAzureAffinityGroup, affinityGroupName))
                {
                    vmPowershellCmdlets.NewAzureAffinityGroup(affinityGroupName, locationName2, label[0], description[0]);
                }

                vmPowershellCmdlets.NewAzureStorageAccount(storageName[1], null, affinityGroupName, null, null);
                Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[1])[0],
                    storageStaticProperties[1], storageName[1], null, true));
                Console.WriteLine("{0} is created", storageName[1]);

                // Set-AzureStorageAccount & Remove-AzureStorageAccount test
                for (int i = 0; i < 2; i++)
                {

                    for (int j = 0; j < 3; j++)
                    {
                        vmPowershellCmdlets.SetAzureStorageAccount(storageName[i], label[j], null, geoReplicationSettings[j]);                        
                        if (geoReplicationSettings[j] != null)
                        {
                            geoReplicationEnabled = geoReplicationSettings[j].Value;
                        }                       
                        Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[i])[0],
                            storageStaticProperties[i], label[j], null, geoReplicationEnabled));
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        vmPowershellCmdlets.SetAzureStorageAccount(storageName[i], null, description[j], geoReplicationSettings[j]);
                        if (geoReplicationSettings[j] != null)
                        {
                            geoReplicationEnabled = geoReplicationSettings[j].Value;
                        }
                        Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[i])[0],
                            storageStaticProperties[i], label[2], description[j], geoReplicationEnabled));
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        vmPowershellCmdlets.SetAzureStorageAccount(storageName[i], null, null, geoReplicationSettings[j]);
                        if (geoReplicationSettings[j] != null)
                        {
                            geoReplicationEnabled = geoReplicationSettings[j].Value;
                        }
                        Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[i])[0],
                            storageStaticProperties[i], label[2], description[2], geoReplicationEnabled));
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        vmPowershellCmdlets.SetAzureStorageAccount(storageName[i], label[j], description[j], geoReplicationSettings[j]);
                        if (geoReplicationSettings[j] != null)
                        {
                            geoReplicationEnabled = geoReplicationSettings[j].Value;
                        }
                        Assert.IsTrue(StorageAccountVerify(vmPowershellCmdlets.GetAzureStorageAccount(storageName[i])[0],
                            storageStaticProperties[i], label[j], description[j], geoReplicationEnabled));
                    }

                    vmPowershellCmdlets.RemoveAzureStorageAccount(storageName[i]);
                    Assert.IsTrue(Utilities.CheckRemove(vmPowershellCmdlets.GetAzureStorageAccount, storageName[i]), "The storage account was not removed");
                }

                vmPowershellCmdlets.RemoveAzureAffinityGroup(affinityGroupName);

                pass = true;

            }
            catch (Exception e)
            {
                pass = false;

                // Clean-up storage if it is not removed.
                foreach (string storage in storageName)
                {

                    if (!Utilities.CheckRemove(vmPowershellCmdlets.GetAzureStorageAccount, storage))
                    {
                        vmPowershellCmdlets.RemoveAzureStorageAccount(storage);
                    }
                }

                // Clean-up affinity group created.
                if (!Utilities.CheckRemove(vmPowershellCmdlets.GetAzureAffinityGroup, affinityGroupName))
                {
                    vmPowershellCmdlets.RemoveAzureAffinityGroup(affinityGroupName);
                }

                Assert.Fail("Exception occurred: {0}", e.ToString());
            }            
        }

        private bool StorageAccountVerify(StorageServicePropertiesOperationContext storageContext,
            string [] staticParameters, string label, string description, bool geo)
        {
            string name = staticParameters[0];
            string location = staticParameters[1];
            string affinity = staticParameters[2];

            Console.WriteLine("Name: {0}, Label: {1}, Description: {2}, AffinityGroup: {3}, Location: {4}, GeoReplicationEnabled: {5}",
                storageContext.StorageAccountName,
                storageContext.Label,
                storageContext.StorageAccountDescription,
                storageContext.AffinityGroup,
                storageContext.Location,
                storageContext.GeoReplicationEnabled);

            try
            {
                Assert.AreEqual(storageContext.StorageAccountName, name, "Error: Storage Account Name is not equal!");
                Assert.AreEqual(storageContext.Label, label, "Error: Storage Account Label is not equal!");
                Assert.AreEqual(storageContext.StorageAccountDescription, description, "Error: Storage Account Description is not equal!");
                Assert.AreEqual(storageContext.AffinityGroup, affinity, "Error: Affinity Group is not equal!");
                Assert.AreEqual(storageContext.Location, location, "Error: Location is not equal!");
                Assert.AreEqual(storageContext.GeoReplicationEnabled, geo, "Error: GeoReplicationEnabled is not equal!");
                Console.WriteLine("All contexts are matched!!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((Get,Set,Remove)-AzureVNetConfig)")]
        public void AzureVNetConfigTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);
            
            string affinityGroup = "WestUsAffinityGroup";

            try
            {
                if (Utilities.CheckRemove(vmPowershellCmdlets.GetAzureAffinityGroup, affinityGroup))
                {
                    vmPowershellCmdlets.NewAzureAffinityGroup(affinityGroup, Resource.Location, null, null);
                }

                vmPowershellCmdlets.SetAzureVNetConfig(vnetConfigFilePath);

                var result = vmPowershellCmdlets.GetAzureVNetConfig(vnetConfigFilePath);

                vmPowershellCmdlets.SetAzureVNetConfig(vnetConfigFilePath);

                Collection<VirtualNetworkSiteContext> vnetSites = vmPowershellCmdlets.GetAzureVNetSite(null);
                foreach (var re in vnetSites)
                {
                    Console.WriteLine("VNet: {0}", re.Name);
                }

                vmPowershellCmdlets.RemoveAzureVNetConfig();

                Collection<VirtualNetworkSiteContext> vnetSitesAfter = vmPowershellCmdlets.GetAzureVNetSite(null);

                Assert.AreNotEqual(vnetSites.Count, vnetSitesAfter.Count, "No Vnet is removed");
                
                foreach (var re in vnetSitesAfter)
                {
                    Console.WriteLine("VNet: {0}", re.Name);
                }

                pass = true;

            }
            catch (Exception e)
            {
                if (e.ToString().Contains("while in use"))
                {
                    Console.WriteLine(e.InnerException.ToString());
                }
                else
                {
                    pass = false;
                    Assert.Fail("Exception occurred: {0}", e.ToString());
                }
            }           
        }

        [TestCleanup]
        public virtual void CleanUp()
        {

            Console.WriteLine("Test {0}", pass ? "passed" : "failed");
            
            // Cleanup            
            if ((cleanupIfPassed && pass) || (cleanupIfFailed && !pass))
            {
                Console.WriteLine("Starting to clean up created VM and service.");

                //try
                //{

                //    vmPowershellCmdlets.RemoveAzureVM(vmName, serviceName);
                //    Console.WriteLine("VM, {0}, is deleted", vmName);
                 
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("Error during removing VM: {0}", e.ToString());
                //}

                try
                {
                    vmPowershellCmdlets.RemoveAzureService(serviceName);
                    Console.WriteLine("Service, {0}, is deleted", serviceName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during removing VM: {0}", e.ToString());
                }                
            }            
        }
    }
}
