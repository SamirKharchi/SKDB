using MongoDB.Driver;

namespace SKDB.MongoDB.Contracts
{
    /// <summary>
    /// Extension contract in order to avoid cycles in the dependency diagram
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IDatabaseAccessorInternal<T>
    {
        FilterDefinition<T> Filter { get; }

        UpdateDefinition<T> Updates { get; }

        UpdateOptions UpdateOptions { get; }
    }
}