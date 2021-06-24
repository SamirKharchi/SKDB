using MongoDB.Driver;

namespace SKDB.MongoDB.Contracts
{
    /// <summary>
    /// Extension contract in order to avoid cycles in the dependency diagram
    /// </summary>
    internal interface IDatabaseInternal
    {
        IMongoCollection<T> GetContext<T>(string name);
    }
}