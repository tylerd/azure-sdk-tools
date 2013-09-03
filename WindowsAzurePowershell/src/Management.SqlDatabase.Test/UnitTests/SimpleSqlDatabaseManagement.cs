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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTests
{
    using System;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport;
    using Microsoft.WindowsAzure.Management.Test.Utilities.Common;
    using Services;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Simple implementation of the <see cref="ISqlDatabaseManagement"/> interface that can be
    /// used for mocking basic interactions without involving Azure directly.
    /// </summary>
    public class SimpleSqlDatabaseManagement : ISqlDatabaseManagement
    {
        /// <summary>
        /// Gets or sets a value indicating whether the thunk wrappers will
        /// throw an exception if the thunk is not implemented.  This is useful
        /// when debugging a test.
        /// </summary>
        public bool ThrowsIfNotImplemented { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSqlDatabaseManagement"/> class.
        /// </summary>
        public SimpleSqlDatabaseManagement()
        {
            ThrowsIfNotImplemented = true;
        }

        #region Implementation Thunks

        #region GetServers

        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseServerList> GetServersThunk { get; set; }
        public IAsyncResult BeginGetServers(string subscriptionId, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public SqlDatabaseServerList EndGetServers(IAsyncResult asyncResult)
        {
            if (GetServersThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return GetServersThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("GetServersThunk is not implemented!");
            }

            return default(SqlDatabaseServerList);
        }

        #endregion

        #region NewServer

        public Func<SimpleServiceManagementAsyncResult, XmlElement> NewServerThunk { get; set; }
        public IAsyncResult BeginNewServer(string subscriptionId, NewSqlDatabaseServerInput input, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public XmlElement EndNewServer(IAsyncResult asyncResult)
        {
            if (NewServerThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return NewServerThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("NewServerThunk is not implemented!");
            }

            return default(XmlElement);
        }

        #endregion

        #region RemoveServer

        public Action<SimpleServiceManagementAsyncResult> RemoveServerThunk { get; set; }
        public IAsyncResult BeginRemoveServer(string subscriptionId, string serverName, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public void EndRemoveServer(IAsyncResult asyncResult)
        {
            if (RemoveServerThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                RemoveServerThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("RemoveServerThunk is not implemented!");
            }
        }

        #endregion

        #region SetPassword

        public Action<SimpleServiceManagementAsyncResult> SetPasswordThunk { get; set; }
        public IAsyncResult BeginSetPassword(string subscriptionId, string serverName, XmlElement password, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["password"] = password;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public void EndSetPassword(IAsyncResult asyncResult)
        {
            if (SetPasswordThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                SetPasswordThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("SetPasswordThunk is not implemented!");
            }
        }

        #endregion

        #region GetServerFirewallRules

        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseFirewallRulesList> GetServerFirewallRulesThunk { get; set; }
        public IAsyncResult BeginGetServerFirewallRules(string subscriptionId, string serverName, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public SqlDatabaseFirewallRulesList EndGetServerFirewallRules(IAsyncResult asyncResult)
        {
            if (GetServerFirewallRulesThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return GetServerFirewallRulesThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("GetServerFirewallRulesThunk is not implemented!");
            }

            return default(SqlDatabaseFirewallRulesList);
        }

        #endregion

        #region NewServerFirewallRule

        public Action<SimpleServiceManagementAsyncResult> NewServerFirewallRuleThunk { get; set; }
        public IAsyncResult BeginNewServerFirewallRule(string subscriptionId, string serverName, SqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public void EndNewServerFirewallRule(IAsyncResult asyncResult)
        {
            if (NewServerFirewallRuleThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                NewServerFirewallRuleThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("NewServerFirewallRuleThunk is not implemented!");
            }
        }

        #endregion

        #region UpdateServerFirewallRule

        public Action<SimpleServiceManagementAsyncResult> UpdateServerFirewallRuleThunk { get; set; }
        public IAsyncResult BeginUpdateServerFirewallRule(string subscriptionId, string serverName, string ruleName, SqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["ruleName"] = ruleName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public void EndUpdateServerFirewallRule(IAsyncResult asyncResult)
        {
            if (UpdateServerFirewallRuleThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                UpdateServerFirewallRuleThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("UpdateServerFirewallRuleThunk is not implemented!");
            }
        }

        #endregion

        #region RemoveServerFirewallRule

        public Action<SimpleServiceManagementAsyncResult> RemoveServerFirewallRuleThunk { get; set; }
        public IAsyncResult BeginRemoveServerFirewallRule(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["ruleName"] = ruleName;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        public void EndRemoveServerFirewallRule(IAsyncResult asyncResult)
        {
            if (RemoveServerFirewallRuleThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                RemoveServerFirewallRuleThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("RemoveServerFirewallRuleThunk is not implemented!");
            }
        }

        #endregion

        #region GetDatabases

        /// <summary>
        /// Gets or sets the Thunk for the GetDatabases opertaion
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseList> GetDatabasesThunk { get; set; }

        /// <summary>
        /// A mock call to BeginGetDatabases
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="callback">the callback to pass through</param>
        /// <param name="state">the state to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginGetDatabases(
            string subscriptionId, 
            string serverName, 
            AsyncCallback callback, 
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["callback"] = callback;
            result.Values["state"] = state;

            return result;
        }
        
        /// <summary>
        /// A mock call to EndGetDatabases
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginGetDatabases call</param>
        /// <returns>A <see cref="SqlDatabaseList"/>: the result of calling the thunk on the input</returns>
        public SqlDatabaseList EndGetDatabases(IAsyncResult asyncResult)
        {
            if (this.GetDatabasesThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return this.GetDatabasesThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("GetDatabasesThunk is not implemented!");
            }

            return default(SqlDatabaseList);
        }

        #endregion

        #region GetDatabase

        /// <summary>
        /// Gets or sets the Thunk for the GetDatabase opertaion
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseResponse> GetDatabaseThunk { get; set; }

        /// <summary>
        /// A mock call to BeginGetDatabase
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="databaseName">The name of the database to pass through</param>
        /// <param name="callback">the callback object to pass through</param>
        /// <param name="state">the state object to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginGetDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            AsyncCallback callback, 
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["databaseName"] = databaseName;
            result.Values["callback"] = callback;
            result.Values["state"] = state;

            return result;
        }

        /// <summary>
        /// A mock call to EndGetDatabase
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginGetDatabase call</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/>: the result of calling the thunk on the input</returns>
        public SqlDatabaseResponse EndGetDatabase(IAsyncResult asyncResult)
        {
            if (this.GetDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return this.GetDatabaseThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("GetDatabaseThunk is not implemented!");
            }

            return default(SqlDatabaseResponse);
        }

        #endregion

        #region NewDatabase

        /// <summary>
        /// Gets or sets the Thunk for the NewDatabase opertaion
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseResponse> NewDatabaseThunk { get; set; }

        /// <summary>
        /// A mock call to BeginNewDatabase
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="input">The input object to pass through</param>
        /// <param name="callback">The callback object to pass through</param>
        /// <param name="state">The state object to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginNewDatabase(
            string subscriptionId, 
            string serverName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;

            return result;
        }

        /// <summary>
        /// A mock call to EndNewDatabase
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginNewDatabase call</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/>: the result of calling the thunk on the input</returns>
        public SqlDatabaseResponse EndNewDatabase(IAsyncResult asyncResult)
        {
            if (this.NewDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return this.NewDatabaseThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("NewDatabaseThunk is not implemented!");
            }

            return default(SqlDatabaseResponse);
        }

        #endregion

        #region UpdateDatabase

        /// <summary>
        /// Gets or sets the Thunk for the NewDatabase opertaion
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, SqlDatabaseResponse> UpdateDatabaseThunk { get; set; }

        /// <summary>
        /// A mock call to BeginUpdateDatabase
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="databaseName">The name of the database to pass through</param>
        /// <param name="input">the input object to pass through</param>
        /// <param name="callback">the callback object to pass through</param>
        /// <param name="state">the state object to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginUpdateDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["databaseName"] = databaseName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;

            return result;
        }

        /// <summary>
        /// A mock call to EndUpdateDatabase
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginUpdateDatabase call</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/>: the result of calling the thunk on the input</returns>
        public SqlDatabaseResponse EndUpdateDatabase(IAsyncResult asyncResult)
        {
            if (this.UpdateDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return this.UpdateDatabaseThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("UpdateDatabaseThunk is not implemented!");
            }

            return default(SqlDatabaseResponse);
        }

        #endregion

        #region RemoveDatabase

        /// <summary>
        /// Gets or sets the Thunk for the RemoveDatabase opertaion
        /// </summary>
        public Action<SimpleServiceManagementAsyncResult> RemoveDatabaseThunk { get; set; }

        /// <summary>
        /// A mock call to BeginRemoveDatabase
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="databaseName">The name of the database to pass through</param>
        /// <param name="input">the input object to pass through</param>
        /// <param name="callback">the callback object to pass through</param>
        /// <param name="state">the state object to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginRemoveDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["databaseName"] = databaseName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;

            return result;
        }

        /// <summary>
        /// A mock call to EndRemoveDatabase
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginRemoveDatabase call</param>
        public void EndRemoveDatabase(IAsyncResult asyncResult)
        {
            if (this.RemoveDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result = asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                this.RemoveDatabaseThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("RemoveDatabaseThunk is not implemented!");
            }
        }

        #endregion

        #region Import/Export Database Status

        /// <summary>
        /// Gets or sets the thunk for ImportExportStatus 
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, ArrayOfStatusInfo> GetImportExporStatusThunk { get; set; }

        /// <summary>
        /// Begins a mock call to GetImportExportStatus
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="fullyQualifiedServerName">The fully qualified server name to pass through</param>
        /// <param name="userName">The userName to pass through</param>
        /// <param name="password">The password to pass through</param>
        /// <param name="requestId">The requestId to pass through</param>
        /// <param name="callback">The callback to pass through</param>
        /// <param name="state">The state to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginGetImportExportStatus(
            string subscriptionId,
            string serverName,
            string fullyQualifiedServerName,
            string userName,
            string password,
            string requestId,
            AsyncCallback callback,
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["fullyQualifiedServerName"] = fullyQualifiedServerName;
            result.Values["userName"] = userName;
            result.Values["password"] = password;
            result.Values["requestId"] = requestId;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        /// <summary>
        /// A mock call to EndGetImportExportStatus
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginGetImportExportStatus call</param>
        /// <returns>A <see cref="ArrayOfStatusInfo"/>: the result of calling the thunk on the input</returns>
        public ArrayOfStatusInfo EndGetImportExportStatus(IAsyncResult asyncResult)
        {
            if (GetImportExporStatusThunk != null)
            {
                SimpleServiceManagementAsyncResult result =
                    asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return GetImportExporStatusThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("ExportDatabaseThunk is not implemented!");
            }

            return default(ArrayOfStatusInfo);
        }

        #endregion

        #region Import Database

        /// <summary>
        /// Gets or sets the Thunk for the ImportDatabase opertaion
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, XmlElement> ImportDatabaseThunk { get; set; }

        /// <summary>
        /// A mock call to BeginImportDatabase
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="input">The input object to pass through</param>
        /// <param name="callback">The callback object to pass through</param>
        /// <param name="state">The state object to pass through</param>
        /// <returns>An <see cref="IAsyncResult"/> of the mock request</returns>
        public IAsyncResult BeginImportDatabase(
            string subscriptionId,
            string serverName,
            ImportInput input,
            AsyncCallback callback,
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        /// <summary>
        /// A mock call to EndImportDatabase
        /// </summary>
        /// <param name="asyncResult">The result of the mock BeginImportDatabase call</param>
        /// <returns>A <see cref="XmlElement"/>: the result of calling the thunk on the input</returns>
        public XmlElement EndImportDatabase(IAsyncResult asyncResult)
        {
            if (ImportDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result =
                    asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return ImportDatabaseThunk(result);
            }
            else if (ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("ImportDatabaseThunk is not implemented!");
            }

            return default(XmlElement);
        }

        #endregion

        #region Export Database

        /// <summary>
        /// Gets or sets the Thunk used for testing the Export Database functionality
        /// </summary>
        public Func<SimpleServiceManagementAsyncResult, XmlElement> ExportDatabaseThunk { get; set; }

        /// <summary>
        /// Starts a mock call to Begin Export Database.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id to pass through</param>
        /// <param name="serverName">The server name to pass through</param>
        /// <param name="input">The input object to pass through</param>
        /// <param name="callback">The callback object to pass through</param>
        /// <param name="state">The state object to pass through</param>
        /// <returns>A <see cref="SimpleServiceManagementAsyncResult"/> object</returns>
        public IAsyncResult BeginExportDatabase(
            string subscriptionId,
            string serverName,
            ExportInput input,
            AsyncCallback callback,
            object state)
        {
            SimpleServiceManagementAsyncResult result = new SimpleServiceManagementAsyncResult();
            result.Values["subscriptionId"] = subscriptionId;
            result.Values["serverName"] = serverName;
            result.Values["input"] = input;
            result.Values["callback"] = callback;
            result.Values["state"] = state;
            return result;
        }

        /// <summary>
        /// Ends the mock call to Export Database
        /// </summary>
        /// <param name="asyncResult">The result of calling BeginExportDatabase</param>
        /// <returns>An XmlElement with the request GUID</returns>
        public XmlElement EndExportDatabase(IAsyncResult asyncResult)
        {
            if (this.ExportDatabaseThunk != null)
            {
                SimpleServiceManagementAsyncResult result =
                    asyncResult as SimpleServiceManagementAsyncResult;
                Assert.IsNotNull(result, "asyncResult was not SimpleServiceManagementAsyncResult!");

                return this.ExportDatabaseThunk(result);
            }
            else if (this.ThrowsIfNotImplemented)
            {
                throw new NotImplementedException("ExportDatabaseThunk is not implemented!");
            }

            return default(XmlElement);
        }
        #endregion

        #endregion
    }
}
