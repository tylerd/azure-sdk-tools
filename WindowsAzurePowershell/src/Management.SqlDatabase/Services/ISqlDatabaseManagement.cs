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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Services
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport;

    /// <summary>
    /// The Windows Azure SQL Database related part of the external API
    /// </summary>
    public partial interface ISqlDatabaseManagement
    {
        /// <summary>
        /// Enumerates SQL Database servers that are provisioned for a subscription.  
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/services/sqlservers/servers")]
        IAsyncResult BeginGetServers(string subscriptionId, AsyncCallback callback, object state);

        SqlDatabaseServerList EndGetServers(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new SQL Database server to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/sqlservers/servers")]
        IAsyncResult BeginNewServer(string subscriptionId, NewSqlDatabaseServerInput input, AsyncCallback callback, object state);

        XmlElement EndNewServer(IAsyncResult asyncResult);

        /// <summary>
        /// Drops a SQL Database server from a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}")]
        IAsyncResult BeginRemoveServer(string subscriptionId, string serverName, AsyncCallback callback, object state);

        void EndRemoveServer(IAsyncResult asyncResult);

        /// <summary>
        /// Sets the administrative password of a SQL Database server for a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}?op=ResetPassword", BodyStyle = WebMessageBodyStyle.Bare)]
        IAsyncResult BeginSetPassword(string subscriptionId, string serverName, XmlElement password, AsyncCallback callback, object state);

        void EndSetPassword(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves a list of all the firewall rules for a SQL Database server that belongs to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/firewallrules")]
        IAsyncResult BeginGetServerFirewallRules(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlDatabaseFirewallRulesList EndGetServerFirewallRules(IAsyncResult asyncResult);

        /// <summary>
        /// Creates a new firewall rule for a SQL Database server that belongs to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/firewallrules")]
        IAsyncResult BeginNewServerFirewallRule(string subscriptionId, string serverName, SqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state);

        void EndNewServerFirewallRule(IAsyncResult asyncResult);

        /// <summary>
        /// Updates an existing firewall rule for a SQL Database server that belongs to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginUpdateServerFirewallRule(string subscriptionId, string serverName, string ruleName, SqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state);

        void EndUpdateServerFirewallRule(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes a firewall rule from a SQL Database server that belongs to a subscription
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginRemoveServerFirewallRule(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state);

        void EndRemoveServerFirewallRule(IAsyncResult asyncResult);

        /// <summary>
        /// Enumerates SQL Databases on a server that is provisioned on a subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription ID that the server belongs to</param>
        /// <param name="serverName">The server name that the databases belongs to</param>
        /// <param name="callback">The callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> from the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", 
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/databases?contentview=generic")]
        IAsyncResult BeginGetDatabases(
            string subscriptionId, 
            string serverName, 
            AsyncCallback callback, 
            object state);
        
        /// <summary>
        /// Given the <see cref="IAsyncResult"/> from the BeginGetDatabases web request
        /// this finishes the operation and returns a <see cref="SqlDatabaseList"/> containing
        /// all the databases
        /// </summary>
        /// <param name="asyncResult">The web request result</param>
        /// <returns>A list of SqlDatabase objects.</returns>
        SqlDatabaseList EndGetDatabases(IAsyncResult asyncResult);

        /// <summary>
        /// Get a SQL Database from the server that is provisioned on a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription Id that the server belongs to</param>
        /// <param name="serverName">The name of the server the database belongs to</param>
        /// <param name="databaseName">The name of the database to retrieve</param>
        /// <param name="callback">The callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> from the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", 
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/databases/{databaseName}")]
        IAsyncResult BeginGetDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            AsyncCallback callback, 
            object state);
        
        /// <summary>
        /// Finishes the Async web request and gets the database
        /// </summary>
        /// <param name="asyncResult">The web request result</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/> representing the database</returns>
        SqlDatabaseResponse EndGetDatabase(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new SQL Database to a server  for a given subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription Id that the server belongs to</param>
        /// <param name="serverName">The name of the server to place the new database in</param>
        /// <param name="input">The parameters for creating a new database</param>
        /// <param name="callback">The callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> from the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", 
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/databases")]
        IAsyncResult BeginNewDatabase(
            string subscriptionId, 
            string serverName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state);
        
        /// <summary>
        /// Finishes the Async web request and gets the result of creating a new database
        /// </summary>
        /// <param name="asyncResult">The web request result</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/> representing the new database</returns>
        SqlDatabaseResponse EndNewDatabase(IAsyncResult asyncResult);

        /// <summary>
        /// Updates an existing SQL database on a server for the given subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription Id that the server belongs to</param>
        /// <param name="serverName">The name of the server containing the database to change</param>
        /// <param name="databaseName">The name of the database to change</param>
        /// <param name="input">The parameters for changing the database</param>
        /// <param name="callback">The callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> from the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", 
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/databases/{databaseName}")]
        IAsyncResult BeginUpdateDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state);

        /// <summary>
        /// Finishes the Async web request and gets the result of updating a database
        /// </summary>
        /// <param name="asyncResult">The web request result</param>
        /// <returns>A <see cref="SqlDatabaseResponse"/> representing the new database</returns>
        SqlDatabaseResponse EndUpdateDatabase(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes and Existing SQL Database on a server for a given subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription Id that the server belongs to</param>
        /// <param name="serverName">The name of the server containing the database to change</param>
        /// <param name="databaseName">The name of the database to remove</param>
        /// <param name="input">The parameters for deleting the database</param>
        /// <param name="callback">The callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> from the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", 
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/databases/{databaseName}")]
        IAsyncResult BeginRemoveDatabase(
            string subscriptionId, 
            string serverName, 
            string databaseName, 
            SqlDatabaseInput input, 
            AsyncCallback callback, 
            object state);

        /// <summary>
        /// Finishes the Async web request for removing a database
        /// </summary>
        /// <param name="asyncResult">The web request result</param>
        void EndRemoveDatabase(IAsyncResult asyncResult);


        /// <summary>
        /// Exports a database to blob storage
        /// </summary>
        /// <param name="subscriptionId">The subscription id that the server belongs to</param>
        /// <param name="serverName">The name of the server the database resides in</param>
        /// <param name="input">An <see cref="ExportInput"/> object containing connection info</param>
        /// <param name="callback">The async callback object</param>
        /// <param name="state">the state object</param>
        /// <returns>An <see cref="IAsyncResult"/> for the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST",
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/DacOperations/Export")]
        IAsyncResult BeginExportDatabase(
            string subscriptionId,
            string serverName,
            ExportInput input,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Finishes the web request to export database to blob storage
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> of the call to: 
        /// BeginExportDatabase</param>
        /// <returns>An <see cref="XmlElement"/> that contains the request ID of the operation</returns>
        XmlElement EndExportDatabase(IAsyncResult asyncResult);

        /// <summary>
        /// Initiates importing a database from blob storage
        /// </summary>
        /// <param name="subscriptionId">The subscription id that the server belongs to</param>
        /// <param name="serverName">The name of the server to import the database into</param>
        /// <param name="input">An <see cref="ExportInput"/> object containing connection info</param>
        /// <param name="callback">The async callback object</param>
        /// <param name="state">the state object</param>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST",
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/DacOperations/Import")]
        IAsyncResult BeginImportDatabase(
            string subscriptionId,
            string serverName,
            ImportInput input,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Ends the call to BeginImportDatabase.  
        /// </summary>
        /// <param name="asyncResult">The result of calling BeginImportDatabase</param>
        /// <returns>An <see cref="XmlElement"/> that contains the GUID for the import operation</returns>
        XmlElement EndImportDatabase(IAsyncResult asyncResult);

        /// <summary>
        /// Gets the status of an import/export operation
        /// </summary>
        /// <param name="subscriptionId">The subscription id that the server belongs to</param>
        /// <param name="serverName">The name of the server the database resides in</param>
        /// <param name="fullyQualifiedServerName">The fully qualified server name</param>
        /// <param name="userName">The username to connect to the database</param>
        /// <param name="password">The password to connect to the database</param>
        /// <param name="requestId">The request ID for the operation to query</param>
        /// <param name="callback">The async callback object</param>
        /// <param name="state">The state object</param>
        /// <returns>An <see cref="IAsyncResult"/> for the web request</returns>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET",
            UriTemplate = @"{subscriptionId}/services/sqlservers/servers/{serverName}/DacOperations"
            + "/Status?servername={fullyQualifiedServerName}&username={userName}&password={password}" +
            "&reqId={requestId}")]
        IAsyncResult BeginGetImportExportStatus(
            string subscriptionId,
            string serverName,
            string fullyQualifiedServerName,
            string userName,
            string password, 
            string requestId,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Finishes the web request to get the import/export operation status
        /// </summary>
        /// <param name="asyncResult">The result of calling <see cref="BeginGetImportExportStatus"/>
        /// </param>
        /// <returns>An <see cref="ArrayOfStatusInfo"/> object</returns>
        ArrayOfStatusInfo EndGetImportExportStatus(IAsyncResult asyncResult);
    }
}
