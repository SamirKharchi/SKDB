using System;
using MongoDB.Driver;
using SKDB.Boundary;

namespace SKDB.MongoDB
{
    internal sealed class MongoClient : IDatabaseClient
    {
        #region api_internals

        private readonly global::MongoDB.Driver.MongoClient mClient;
        private readonly string                             mDbName;

        internal MongoClient(global::MongoDB.Driver.MongoClient client, string dbName)
        {
            mClient = client ?? throw new NullReferenceException("Invalid Mongo client passed to NavieMongoClient");
            mDbName = string.IsNullOrWhiteSpace(dbName)
                ? throw new ArgumentNullException(nameof(dbName), "Invalid database name passed.")
                : dbName;
        }

        #endregion

        public IDatabase Database => new MongoDatabase(mClient.GetDatabase(mDbName));

        IDatabaseContextAsync<TEntity> IDatabaseClient.ConnectToContext<TEntity>(string name) =>
            new MongoCollection<TEntity>(this, name.ToLower());
    }
}