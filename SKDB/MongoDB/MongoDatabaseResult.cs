using System;
using MongoDB.Driver;
using SKDB.Boundary;

namespace SKDB.MongoDB
{
    internal class MongoDatabaseResult : IDatabaseResult
    {
        internal MongoDatabaseResult(UpdateResult result)
        {
            Success      = result.IsAcknowledged;
            ItemsUpdated = Convert.ToInt32(result.ModifiedCount);
        }

        internal MongoDatabaseResult(ReplaceOneResult result)
        {
            Success      = result.IsAcknowledged;
            ItemsUpdated = Convert.ToInt32(result.ModifiedCount);
        }

        internal MongoDatabaseResult(DeleteResult result)
        {
            Success      = result.IsAcknowledged;
            ItemsUpdated = Convert.ToInt32(result.DeletedCount);
        }

        internal MongoDatabaseResult(BulkWriteResult result)
        {
            Success      = result.IsAcknowledged;
            ItemsUpdated = Convert.ToInt32(result.ModifiedCount);
        }

        public bool Success      { get; set; }
        public int  ItemsUpdated { get; set; }
    }
}