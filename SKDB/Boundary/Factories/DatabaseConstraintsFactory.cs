using SKDB.MongoDB;

namespace SKDB.Boundary.Factories
{
    public class DatabaseConstraintsFactory
    {
        public static IDatabaseConstraints Create<T>(DatabaseServerType type)
        {
            switch (type)
            {
                case DatabaseServerType.MongoDb:
                {
                    return new MongoConstraints<T>();
                }
                default:
                    return null;
            }
        }
    }
}