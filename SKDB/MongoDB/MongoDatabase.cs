using MongoDB.Driver;
using SKDB.Boundary;
using SKDB.MongoDB.Contracts;

namespace SKDB.MongoDB
{
    internal class MongoDatabase : IDatabase, IDatabaseInternal
    {
        private readonly IMongoDatabase mDatabase;

        internal MongoDatabase(IMongoDatabase db) => mDatabase = db;

        IMongoCollection<T> IDatabaseInternal.GetContext<T>(string name) => mDatabase.GetCollection<T>(name);

        public void DropContext(string name) => mDatabase?.DropCollection(name);
    }
}