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
    public class FunctionalTestCommonVhd : ServiceManagementTest
    {
        private const string vhdNamePrefix = "os.vhd";
        private string vhdName;
        protected static string vhdBlobLocation;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            //SetTestSettings();

            if (defaultAzureSubscription.Equals(null))
            {
                Assert.Inconclusive("No Subscription is selected!");
            }

            vhdBlobLocation = string.Format("{0}{1}/{2}", blobUrlRoot, vhdContainerName, vhdNamePrefix);
            if (string.IsNullOrEmpty(localFile))
            {
                try
                {
                    CredentialHelper.CopyTestData(testDataContainer, osVhdName, vhdContainerName, vhdNamePrefix);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Assert.Inconclusive("Upload vhd is not set!");
                }
            }
            else
            {
                try
                {
                    vmPowershellCmdlets.AddAzureVhd(new FileInfo(localFile), vhdBlobLocation);
                }
                catch (Exception e)
                {
                    if (e.ToString().Contains("already exists"))
                    {
                        // Use the already uploaded vhd.
                        Console.WriteLine("Using already uploaded blob..");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            vhdName = Utilities.GetUniqueShortName(vhdNamePrefix);
            CopyCommonVhd(vhdContainerName, vhdNamePrefix, vhdName);
            pass = false;
            testStartTime = DateTime.Now;
        }

        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((Add,Get,Update,Remove)-AzureDisk)")]
        public void AzureDiskTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            string mediaLocation = String.Format("{0}{1}/{2}", blobUrlRoot, vhdContainerName, vhdName);

            try
            {
                vmPowershellCmdlets.AddAzureDisk(vhdName, mediaLocation, vhdName, null);

                bool found = false;
                foreach (DiskContext disk in vmPowershellCmdlets.GetAzureDisk(vhdName))
                {
                    Console.WriteLine("Disk: Name - {0}, Label - {1}, Size - {2},", disk.DiskName, disk.Label, disk.DiskSizeInGB);
                    if (disk.DiskName == vhdName && disk.Label == vhdName)
                    {
                        found = true;
                        Console.WriteLine("{0} is found", disk.DiskName);
                    }

                }
                Assert.IsTrue(found, "Error: Disk is not added");

                string newLabel = "NewLabel";
                vmPowershellCmdlets.UpdateAzureDisk(vhdName, newLabel);

                DiskContext disk2 = vmPowershellCmdlets.GetAzureDisk(vhdName)[0];

                Console.WriteLine("Disk: Name - {0}, Label - {1}, Size - {2},", disk2.DiskName, disk2.Label, disk2.DiskSizeInGB);
                Assert.AreEqual(newLabel, disk2.Label);
                Console.WriteLine("Disk Label is successfully updated");

                vmPowershellCmdlets.RemoveAzureDisk(vhdName, false);
                Assert.IsTrue(Utilities.CheckRemove(vmPowershellCmdlets.GetAzureDisk, vhdName), "The disk was not removed");
                pass = true;
            }
            catch (Exception e)
            {
                pass = false;

                if (e.ToString().Contains("ResourceNotFound"))
                {
                    Console.WriteLine("Please upload {0} file to \\vhdtest\\ blob directory before running this test", vhdName);
                }

                Assert.Fail("Exception occurs: {0}", e.ToString());
            }
        }

        private void CopyCommonVhd(string vhdContainerName, string vhdName, string myVhdName)
        {
            vmPowershellCmdlets.RunPSScript(string.Format("Start-AzureStorageBlobCopy -SrcContainer {0} -SrcBlob {1} -DestContainer {2} -DestBlob {3}", vhdContainerName, vhdName, vhdContainerName, myVhdName));
        }

        [TestMethod(), TestCategory("Functional"), TestCategory("BVT"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet ((Add,Get,Save,Update,Remove)-AzureVMImage)")]
        public void AzureVMImageTest()
        {
            StartTest(MethodBase.GetCurrentMethod().Name, testStartTime);

            string newImageName = Utilities.GetUniqueShortName("vmimage");
            string mediaLocation = string.Format("{0}{1}/{2}", blobUrlRoot, vhdContainerName, vhdName);

            string oldLabel = "old label";
            string newLabel = "new label";

            try
            {
                OSImageContext result = vmPowershellCmdlets.AddAzureVMImage(newImageName, mediaLocation, OS.Windows, oldLabel);

                OSImageContext resultReturned = vmPowershellCmdlets.GetAzureVMImage(newImageName)[0];
                Assert.IsTrue(CompareContext<OSImageContext>(result, resultReturned));

                result = vmPowershellCmdlets.UpdateAzureVMImage(newImageName, newLabel);

                resultReturned = vmPowershellCmdlets.GetAzureVMImage(newImageName)[0];
                Assert.IsTrue(CompareContext<OSImageContext>(result, resultReturned));

                vmPowershellCmdlets.RemoveAzureVMImage(newImageName, true);
                Assert.IsTrue(Utilities.CheckRemove(vmPowershellCmdlets.GetAzureVMImage, newImageName));

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Assert.Fail("Exception occurred: {0}", e.ToString());
            }
            finally
            {
                if (!Utilities.CheckRemove(vmPowershellCmdlets.GetAzureVMImage, newImageName))
                {
                    vmPowershellCmdlets.RemoveAzureVMImage(newImageName, true);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Set-AzureVMSize)")]
        public void AzureVMImageSizeTest()
        {

            string newImageName = Utilities.GetUniqueShortName("vmimage");
            string mediaLocation = string.Format("{0}{1}/{2}", blobUrlRoot, vhdContainerName, vhdName);

            try
            {
                Array instanceSizes = Enum.GetValues(typeof(InstanceSize));
                int arrayLength = instanceSizes.GetLength(0);

                for (int i = 1; i < arrayLength; i++)
                {
                    // Add-AzureVMImage test for VM size
                    OSImageContext result = vmPowershellCmdlets.AddAzureVMImage(newImageName, mediaLocation, OS.Windows, (InstanceSize)instanceSizes.GetValue(i));
                    OSImageContext resultReturned = vmPowershellCmdlets.GetAzureVMImage(newImageName)[0];
                    Assert.IsTrue(CompareContext<OSImageContext>(result, resultReturned));

                    // Update-AzureVMImage test for VM size
                    result = vmPowershellCmdlets.UpdateAzureVMImage(newImageName, (InstanceSize)instanceSizes.GetValue(Math.Max((i + 1) % arrayLength, 1)));
                    resultReturned = vmPowershellCmdlets.GetAzureVMImage(newImageName)[0];
                    Assert.IsTrue(CompareContext<OSImageContext>(result, resultReturned));

                    vmPowershellCmdlets.RemoveAzureVMImage(newImageName);
                }
            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
        }

        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Set-AzureVMSize)")]
        public void HiMemVMSizeTest()
        {
            string serviceName = Utilities.GetUniqueShortName(serviceNamePrefix);
            string vmName = Utilities.GetUniqueShortName(vmNamePrefix);

            try
            {
                PersistentVMRoleContext result;

                // New-AzureQuickVM test for VM size
                vmPowershellCmdlets.NewAzureQuickVM(OS.Windows, vmName, serviceName, imageName, username, password, locationName, InstanceSize.A6);
                result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                Assert.AreEqual(InstanceSize.A6.ToString(), result.InstanceSize);
                Console.WriteLine("VM size, {0}, is verified for New-AzureQuickVM", InstanceSize.A6.ToString());
                vmPowershellCmdlets.RemoveAzureService(serviceName);

                // New-AzureVMConfig test for VM size
                AzureVMConfigInfo azureVMConfigInfo = new AzureVMConfigInfo(vmName, InstanceSize.A7, imageName);
                AzureProvisioningConfigInfo azureProvisioningConfig = new AzureProvisioningConfigInfo(OS.Windows, username, password);
                PersistentVMConfigInfo persistentVMConfigInfo = new PersistentVMConfigInfo(azureVMConfigInfo, azureProvisioningConfig, null, null);
                PersistentVM vm = vmPowershellCmdlets.GetPersistentVM(persistentVMConfigInfo);
                vmPowershellCmdlets.NewAzureVM(serviceName, new[] { vm }, locationName);
                result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                Assert.AreEqual(InstanceSize.A7.ToString(), result.InstanceSize);
                Console.WriteLine("VM size, {0}, is verified for New-AzureVMConfig", InstanceSize.A7.ToString());

                // Set-AzureVMSize test for Hi-MEM VM size
                vmPowershellCmdlets.SetVMSize(vmName, serviceName, new SetAzureVMSizeConfig(InstanceSize.A6));
                result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                Assert.AreEqual(InstanceSize.A6.ToString(), result.InstanceSize);
                Console.WriteLine("SetVMSize is verified from A7 to A6");

                vmPowershellCmdlets.SetVMSize(vmName, serviceName, new SetAzureVMSizeConfig(InstanceSize.A7));
                result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                Assert.AreEqual(InstanceSize.A7.ToString(), result.InstanceSize);
                Console.WriteLine("SetVMSize is verified from A6 to A7");
            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
            finally
            {
                if (!Utilities.CheckRemove(vmPowershellCmdlets.GetAzureService, serviceName))
                {
                    if ((cleanupIfFailed && !pass) || (cleanupIfPassed && pass))
                    {
                        vmPowershellCmdlets.RemoveAzureService(serviceName);
                    }
                }
            }
        }

        [TestMethod(), TestCategory("Functional"), TestProperty("Feature", "IAAS"), Priority(1), Owner("hylee"), Description("Test the cmdlet (Set-AzureVMSize)")]
        public void RegularVMSizeTest()
        {
            string serviceName = Utilities.GetUniqueShortName(serviceNamePrefix);
            string vmName = Utilities.GetUniqueShortName(vmNamePrefix);

            try
            {
                Array instanceSizes = Enum.GetValues(typeof(InstanceSize));
                vmPowershellCmdlets.NewAzureQuickVM(OS.Windows, vmName, serviceName, imageName, username, password, locationName, InstanceSize.A6);
                PersistentVMRoleContext result;

                foreach (InstanceSize size in instanceSizes)
                {
                    if (!size.Equals(InstanceSize.A6) && !size.Equals(InstanceSize.A7))
                    {
                        // Set-AzureVMSize test for regular VM size
                        vmPowershellCmdlets.SetVMSize(vmName, serviceName, new SetAzureVMSizeConfig(size));
                        result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                        Assert.AreEqual(size.ToString(), result.InstanceSize);
                        Console.WriteLine("VM size, {0}, is verified for Set-AzureVMSize", size.ToString());
                    }

                    if (size.Equals(InstanceSize.ExtraLarge))
                    {
                        vmPowershellCmdlets.SetVMSize(vmName, serviceName, new SetAzureVMSizeConfig(InstanceSize.Small));
                        result = vmPowershellCmdlets.GetAzureVM(vmName, serviceName);
                        Assert.AreEqual(InstanceSize.Small.ToString(), result.InstanceSize);
                    }

                }

                pass = true;
            }
            catch (Exception e)
            {
                pass = false;
                Console.WriteLine("Exception occurred: {0}", e.ToString());
                throw;
            }
            finally
            {
                if (!Utilities.CheckRemove(vmPowershellCmdlets.GetAzureService, serviceName))
                {
                    if ((cleanupIfFailed && !pass) || (cleanupIfPassed && pass))
                    {
                        vmPowershellCmdlets.RemoveAzureService(serviceName);
                    }
                }
            }
        }

        private bool CompareContext<T>(T obj1, T obj2)
        {
            bool result = true;
            Type type = typeof(T);

            foreach(PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                string typeName = property.PropertyType.FullName;
                if (typeName.Equals("System.String") || typeName.Equals("System.Int32") || typeName.Equals("System.Uri") || typeName.Contains("Nullable"))
                {

                    var obj1Value = property.GetValue(obj1, null);
                    var obj2Value = property.GetValue(obj2, null);

                    if (obj1Value == null)
                    {
                        result &= (obj2Value == null);
                    }
                    else
                    {
                        result &= (obj1Value.Equals(obj2Value));
                    }
                }
                else
                {
                    Console.WriteLine("This type is not compared: {0}", typeName);
                }
            }

            return result;
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            Console.WriteLine("Test {0}", pass ? "passed" : "failed");
        }
    }
}
