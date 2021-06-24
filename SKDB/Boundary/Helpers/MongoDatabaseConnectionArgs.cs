using SKDB.Boundary.Factories;

namespace SKDB.Boundary.Helpers
{
    public class MongoDatabaseConnectionArgs : DatabaseConnectionFactoryArgs
    {
        public MongoDatabaseConnectionArgs(string databaseName, string applicationName)
        {
            ServerType      = DatabaseServerType.MongoDb;
            DatabaseName    = databaseName;
            ApplicationName = applicationName;
        }
    }
}