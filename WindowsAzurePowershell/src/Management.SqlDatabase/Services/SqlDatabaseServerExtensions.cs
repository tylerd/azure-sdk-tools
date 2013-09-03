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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.ImportExport;

    /// <summary>
    /// Method implementations for Sql Azure managment
    /// </summary>
    public static partial class SqlDatabaseManagementExtensionMethods
    {
        /// <summary>
        /// Gets a list of all servers for a subscription.
        /// </summary>
        /// <param name="proxy">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        /// <param name="subscriptionId">
        /// The subscription id from which to retrieve the list of servers.
        /// </param>
        /// <returns>The list of servers under a subscription.</returns>
        public static SqlDatabaseServerList GetServers(this ISqlDatabaseManagement proxy, string subscriptionId)
        {
            return proxy.EndGetServers(proxy.BeginGetServers(subscriptionId, null, null));
        }

        /// <summary>
        /// Creates a new server in the current subscription.
        /// </summary>
        /// <param name="proxy">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        /// <param name="subscriptionId">
        /// The subscription id in which to create the new server.
        /// </param>
        /// <param name="administratorLogin">
        /// The administrator login name for the new server.
        /// </param>
        /// <param name="administratorLoginPassword">
        /// The administrator login password for the new server.
        /// </param>
        /// <param name="location">
        /// The location in which to create the new server.
        /// </param>
        /// <returns>The XmlElement with the new server information.</returns>
        public static XmlElement NewServer(
            this ISqlDatabaseManagement proxy, 
            string subscriptionId, 
            string administratorLogin, 
            string administratorLoginPassword, 
            string location)
        {
            var input = new NewSqlDatabaseServerInput()
            {
                AdministratorLogin = administratorLogin,
                AdministratorLoginPassword = administratorLoginPassword,
                Location = location
            };

            var inputproxy = proxy.BeginNewServer(subscriptionId, input, null, null);
            var result = proxy.EndNewServer(inputproxy);
            return result;
        }

        /// <summary>
        /// Removes an existing server in the current subscription.
        /// </summary>
        /// <param name="proxy">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        /// <param name="subscriptionId">
        /// The subscription id which contains the server.
        /// </param>
        /// <param name="serverName">
        /// The name of the server to remove.
        /// </param>
        public static void RemoveServer(this ISqlDatabaseManagement proxy, string subscriptionId, string serverName)
        {
            proxy.EndRemoveServer(proxy.BeginRemoveServer(subscriptionId, serverName, null, null));
        }

        /// <summary>
        /// Resets the administrator password for an existing server in the
        /// current subscription.
        /// </summary>
        /// <param name="proxy">
        /// Channel used for communication with Azure's service management APIs.
        /// </param>
        /// <param name="subscriptionId">
        /// The subscription id which contains the server.
        /// </param>
        /// <param name="serverName">
        /// The name of the server for which to reset the password.
        /// </param>
        /// <param name="password">
        /// The new password for the server.
        /// </param>
        public static void SetPassword(this ISqlDatabaseManagement proxy, string subscriptionId, string serverName, string password)
        {
            // create an xml element for the request body
            var xml = string.Empty;

            using (var tx = new StringWriter())
            {
                var tw = new XmlTextWriter(tx);
                tw.WriteStartDocument();
                tw.WriteStartElement("AdministratorLoginPassword", Constants.SqlDatabaseManagementNamespace);
                tw.WriteString(password);
                tw.WriteEndElement();

                xml = tx.ToString();
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var el = (XmlElement)doc.FirstChild.NextSibling;

            proxy.EndSetPassword(proxy.BeginSetPassword(subscriptionId, serverName, el, null, null));
        }

        /// <summary>
        /// Exports a database into blob storage
        /// </summary>
        /// <param name="proxy">Channel used for communication with Azure's service management APIs.</param>
        /// <param name="subscriptionId">The subscription ID in which the server resides</param>
        /// <param name="serverName">The name of the server that contains the database</param>
        /// <param name="input">The input data for the export operation</param>
        /// <returns>An <see cref="XmlElement"/> containing the request ID (GUID).</returns>
        public static XmlElement ExportDatabase(
            this ISqlDatabaseManagement proxy, 
            string subscriptionId, 
            string serverName, 
            ExportInput input)
        {
            return proxy.EndExportDatabase(proxy.BeginExportDatabase(
                subscriptionId, 
                serverName, 
                input, 
                null, 
                null));
        }

        /// <summary>
        /// Imports a database from blob storage
        /// </summary>
        /// <param name="proxy">Channel used for communication with Azure's service management APIs</param>
        /// <param name="subscriptionId">The subscription ID in which the server resides</param>
        /// <param name="serverName">The name of the server to put the database in</param>
        /// <param name="input">The input data for the import operation</param>
        /// <returns>An <see cref="XmlElement"/> containing the request ID (GUID).</returns>
        public static XmlElement ImportDatabase(
            this ISqlDatabaseManagement proxy,
            string subscriptionId,
            string serverName,
            ImportInput input)
        {
            return proxy.EndImportDatabase(proxy.BeginImportDatabase(
                subscriptionId,
                serverName,
                input,
                null,
                null));
        }

        /// <summary>
        /// Gets the status of an import/export operation
        /// </summary>
        /// <param name="proxy">Channel used for communication with Azure's service management APIs.</param>
        /// <param name="subscriptionId">The subscription ID in which the server resides</param>
        /// <param name="serverName">The name of the server that contains the database</param>
        /// <param name="userName">The username to connect to the server</param>
        /// <param name="password">The password to connect to the server</param>
        /// <param name="requestId">The request Id of the import/export operation</param>
        /// <returns>A <see cref="ArrayOfStatusInfo"/> object containting the import/export status</returns>
        public static ArrayOfStatusInfo GetImportExportStatus(
            this ISqlDatabaseManagement proxy,
            string subscriptionId,
            string serverName,
            string fullyQualifiedServerName,
            string userName,
            string password,
            string requestId)
        {
            return proxy.EndGetImportExportStatus(
                proxy.BeginGetImportExportStatus(
                    subscriptionId,
                    serverName,
                    fullyQualifiedServerName,
                    userName,
                    password,
                    requestId,
                    null,
                    null));
        }
    }
}