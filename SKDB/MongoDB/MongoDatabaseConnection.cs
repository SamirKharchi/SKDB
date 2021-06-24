using System;
using System.Security;
using SKDB.Boundary;
using SKDB.Boundary.Factories;

namespace SKDB.MongoDB
{
    internal class MongoDatabaseConnection : IDatabaseConnection
    {
        #region PRIVATE

        private          IDatabaseServer    mServer;
        private readonly DatabaseServerType mServerType;
        private readonly string             mDatabaseName;
        private readonly string             mApplicationName;

        private bool ConnectClient(string userName, SecureString password)
        {
            try
            {
                Client = (userName.IsNotNull() && password.IsNotNull())
                    ? mServer.CreateClient(mDatabaseName, userName, password)
                    : mServer.CreateClient(mDatabaseName);
            }
            catch (NullReferenceException)
            {
                return false;
            }

            return true;
        }

        private bool InitIfConnected()
        {
            if (IsConnected)
            {
                return false;
            }
            Client = null;
            return true;
        }

        #endregion

        internal MongoDatabaseConnection(DatabaseServerType serverType,
                                             string databaseName,
                                             string applicationName)
        {
            mServerType      = serverType;
            mDatabaseName    = databaseName;
            mApplicationName = applicationName;
        }

        public bool IsConnected => (mServer.IsNotNull() && Client.IsNotNull());

        public bool Connect(Action serializers)
        {
            if (!InitIfConnected())
            {
                return true;
            }

            mServer = DatabaseServerFactory.Create(mServerType, mApplicationName, serializers);

            return ConnectClient(null, null);
        }

        public bool Connect() => Connect(null);

        public bool Connect(string host,
                            int port,
                            string userName,
                            SecureString password) =>
            Connect(host, port, userName, password, null);

        public bool Connect(string host,
                            int port,
                            string userName,
                            SecureString password,
                            Action serializers)
        {
            if (!InitIfConnected())
            {
                return true;
            }

            mServer = DatabaseServerFactory.Create(mServerType, mApplicationName, host, port, serializers);

            return ConnectClient(userName, password);
        }

        public IDatabaseClient Client { get; private set; }
    }
}