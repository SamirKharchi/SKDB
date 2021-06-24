using System;
using MongoDB.Bson;
using SKDB.Generic;
using SKDB.MongoDB;

namespace SKDB.Boundary.Factories
{
    public enum DatabaseIdType
    {
        MongoDb,
        Long,
        String
    }

    public static class DatabaseIdFactory
    {
        public static IDatabaseId Create(DatabaseIdType type)
        {
            switch (type)
            {
                case DatabaseIdType.MongoDb:
                {
                    return new MongoDatabaseId();
                }
                case DatabaseIdType.Long:
                {
                    return new LongDatabaseId();
                }
                default:
                {
                    return new DefaultDatabaseId();
                }
            }
        }
        public static IDatabaseId CreateEmpty(DatabaseIdType type)
        {
            switch (type)
            {
                case DatabaseIdType.MongoDb:
                {
                    return new MongoDatabaseId(ObjectId.Empty);
                }
                case DatabaseIdType.Long:
                {
                    return new LongDatabaseId(0);
                }
                default:
                {
                    return new DefaultDatabaseId(Guid.Empty);
                }
            }
        }
    }
}