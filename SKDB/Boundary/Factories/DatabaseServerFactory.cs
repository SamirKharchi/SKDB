using System;
using SKDB.MongoDB;
using SKDB.Properties;

namespace SKDB.Boundary.Factories
{
    public enum DatabaseServerType
    {
        MongoDb
    }

    public static class DatabaseServerFactory
    {
        public static IDatabaseServer Create(DatabaseServerType type, [NotNullNotEmpty] string application, Action serializers = null)
        {
            switch (type)
            {
                case DatabaseServerType.MongoDb:
                {
                    return new MongoServer(application, serializers);
                }
                default:
                    return null;
            }
        }

        public static IDatabaseServer Create(DatabaseServerType type,
                                             [NotNullNotEmpty] string application,
                                             [NotNullNotEmpty] string host,
                                             int port,
                                             Action serializers = null)
        {
            switch (type)
            {
                case DatabaseServerType.MongoDb:
                {
                    return new MongoServer(application, host, port, serializers);
                }
                default:
                    return null;
            }
        }
    }
}