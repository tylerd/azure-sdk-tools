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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.FunctionalTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Test.Utilities;

    /// <summary>
    /// Functional tests for Database CRUD operations
    /// </summary>
    [TestClass]
    public class DatabaseTest
    {
        #region Test Script Locations
        private string serverLocation;
        private string containerName;
        private string storageName;
        private string accessKey;
        private string ieServerLocation;
        private string ieSubscriptionId;

        /// <summary>
        /// Scripts for doing context creation tests
        /// </summary>
        private const string CreateContextScript = @"Database\CreateContext.ps1";

        /// <summary>
        /// Script for doing Create and Get database tests with SQL authentication
        /// </summary>
        private const string CreateScript = @"Database\CreateAndGetDatabase.ps1";

        /// <summary>
        /// Scripts for doing database update tests
        /// </summary>
        private const string UpdateScript = @"Database\UpdateDatabase.ps1";

        /// <summary>
        /// Scripts for doing delete database tests
        /// </summary>
        private const string DeleteScript = @"Database\DeleteDatabase.ps1";

        /// <summary>
        /// Tests for doing format validation tests 
        /// </summary>
        private const string FormatValidationScript = @"Database\FormatValidation.ps1";
        private const string ImportExportScript = @"Database\ImportExportDatabase.ps1";

        #endregion

        /// <summary>
        /// The end point to use for the tests
        /// </summary>
        private const string LocalRdfeEndpoint = @"https://management.dev.mscds.com:12346/MockRDFE/";

        #region Private Fields

        /// <summary>
        /// Username to use for running the tests
        /// </summary>
        private string userName;

        /// <summary>
        /// Password to use for running the tests
        /// </summary>
        private string password;

        /// <summary>
        /// ManageUrl to use when running the tests
        /// </summary>
        private string manageUrl;

        /// <summary>
        /// Subscription Id to use when running the tests
        /// </summary>
        private string subscriptionId;

        /// <summary>
        /// Serialized Certificate to use when running the tests
        /// </summary>
        private string serializedCert;

        /// <summary>
        /// The server name to use when running the tests
        /// </summary>
        private string serverName;

        #endregion
        
        /// <summary>
        /// Get the necessary variables from settings file for the tests
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            XElement root = XElement.Load("SqlDatabaseSettings.xml");
            this.serverLocation = root.Element("ServerLocation").Value;
            this.userName = root.Element("SqlAuthUserName").Value;
            this.password = root.Element("SqlAuthPassword").Value;
            this.manageUrl = root.Element("ManageUrl").Value;
            this.serializedCert = root.Element("SerializedCert").Value;
            this.subscriptionId = root.Element("SubscriptionId").Value;
            this.serverName = new Uri(manageUrl).Host.Split('.').First();

            // Import Export parameters
            XElement importExport = root.Element("ImportExport"); 
            this.ieServerLocation = importExport.Element("ServerLocation").Value;
            this.ieSubscriptionId = importExport.Element("SubscriptionId").Value;
            this.containerName = importExport.Element("ContainerName").Value;
            this.storageName = importExport.Element("StorageName").Value;
            this.accessKey = importExport.Element("AccessKey").Value;

        }

        /// <summary>
        /// Tests context creation
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void CreateContext()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-ManageUrl \"{0}\" -UserName \"{1}\" -Password \"{2}\" "
                + "-SubscriptionId \"{3}\" -SerializedCert \"{4}\" ",
                this.manageUrl,
                this.userName,
                this.password,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(
                DatabaseTest.CreateContextScript,
                arguments);
            Assert.IsTrue(testResult);
        }

        /// <summary>
        /// Tests creating a database using SQL authentication
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void CreateAndGetDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" "
                +  "-ServerName \"{4}\" -SubscriptionID \"{5}\" -SerializedCert \"{6}\" "
                + "-Endpoint \"{7}\"",
                "testcreatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                this.serverName,
                this.subscriptionId,
                this.serializedCert,
                LocalRdfeEndpoint);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.CreateScript, arguments);
            Assert.IsTrue(testResult);
        }

        /// <summary>
        /// Tests updating a database using SQL authentication
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void UpdateDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" "
                + "-ServerName \"{4}\" -SubscriptionID \"{5}\" -SerializedCert \"{6}\" "
                + "-Endpoint \"{7}\"",
                "testupdatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                this.serverName,
                this.subscriptionId,
                this.serializedCert,
                LocalRdfeEndpoint);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.UpdateScript, arguments);
            Assert.IsTrue(testResult);
        }

        /// <summary>
        /// Tests removing a database using SQL authentication
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void DeleteDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" "
                + "-ServerName \"{4}\" -SubscriptionID \"{5}\" -SerializedCert \"{6}\" "
                + "-Endpoint \"{7}\"",
                "testDeletedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                this.serverName,
                this.subscriptionId,
                this.serializedCert,
                LocalRdfeEndpoint);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.DeleteScript, arguments);
            Assert.IsTrue(testResult);
        }

        /// <summary>
        /// Validates the object output format
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void OutputObjectFormatValidation()
        {
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid() + ".txt");
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" -OutputFile \"{4}\"",
                "testFormatdbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                outputFile);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.FormatValidationScript, arguments);
            Assert.IsTrue(testResult);

            OutputFormatValidator.ValidateOutputFormat(outputFile, @"Database\ExpectedFormat.txt");
        }

        /// <summary>
        /// Runs the script to test the import and export functionality
        /// </summary>
        [TestMethod]
        [TestCategory("Functional")]
        public void ImportExportDatabase()
        {
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid() + ".txt");

            string cmdlineArgs =
                "-UserName \"{0}\" -Password \"{1}\" -SubscriptionId \"{2}\" -SerializedCert \"{3}\" "
                + "-ContainerName \"{4}\" -StorageName \"{5}\" -StorageAccessKey \"{6}\" "
                + "-ServerLocation \"{7}\"";

            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                cmdlineArgs,
                this.userName,
                this.password,
                this.ieSubscriptionId,
                this.serializedCert,
                this.containerName,
                this.storageName,
                this.accessKey,
                this.ieServerLocation);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.ImportExportScript, arguments);
            Assert.IsTrue(testResult);
        }
    }
}
