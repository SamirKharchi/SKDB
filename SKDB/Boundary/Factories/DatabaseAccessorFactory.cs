using SKDB.MongoDB;

namespace SKDB.Boundary.Factories
{
    /// <summary>
    /// Factory to build document accessors, i.e. <see cref="IDatabaseAccessor"/>.
    /// </summary>
    public static class DatabaseAccessorFactory
    {
        public static IDatabaseAccessor Create<T>(DatabaseServerType type)
        {
            switch (type)
            {
                case DatabaseServerType.MongoDb:
                {
                    return new MongoAccessor<T>();
                }
                default:
                    return null;
            }
        }
    }
}