using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SKDB.Boundary;

namespace SKDB.MongoDB
{
    internal class MongoDatabaseId : IDatabaseId
    {
        internal MongoDatabaseId() => InternalId = ObjectId.GenerateNewId();

        internal MongoDatabaseId(ObjectId id) => InternalId = id;

        [BsonId] internal ObjectId InternalId { get; set; }

        [BsonIgnore]
        public string Id
        {
            get => InternalId.ToString();
            set => InternalId = ObjectId.Parse(value);
        }

        public override string ToString() => Id;

        public override bool Equals(object obj) => InternalId == (obj as MongoDatabaseId)?.InternalId;

        protected bool Equals(MongoDatabaseId other) => InternalId.Equals(other.InternalId);

        public override int GetHashCode() => InternalId.GetHashCode();
    }
}