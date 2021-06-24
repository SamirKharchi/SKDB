using SKDB.MongoDB;

namespace SKDB.Boundary.Factories
{
    public class DatabaseMultiWriterFactory
    {
        public static IDatabaseMultiWriter<T> Create<T>(DatabaseServerType type)
        {
            switch (type)
            {
                case DatabaseServerType.MongoDb:
                {
                    return new MongoMultiWriter<T>();
                }
                default:
                    return null;
            }
        }

    }
}