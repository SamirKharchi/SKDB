using SKDB.MongoDB;

namespace SKDB.Boundary.Factories
{
    public static class DatabaseConnectionFactory
    {
        public static IDatabaseConnection CreateMongoConnection(DatabaseConnectionFactoryArgs args) =>
            DatabaseConnectionFactory.Create(DatabaseServerType.MongoDb, args.DatabaseName, args.ApplicationName);
        public static IDatabaseConnection Create(DatabaseConnectionFactoryArgs args) =>
            DatabaseConnectionFactory.Create(args.ServerType, args.DatabaseName, args.ApplicationName);

        public static IDatabaseConnection Create(DatabaseServerType serverType,
                                                 string databaseName,
                                                 string applicationName)
        {
            switch (serverType)
            {
                case DatabaseServerType.MongoDb:
                    return new MongoDatabaseConnection(serverType, databaseName, applicationName);
                default:
                    return null;
            }
        }
    }
}