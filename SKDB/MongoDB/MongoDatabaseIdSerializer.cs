using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SKDB.Boundary;
using SKDB.Boundary.Factories;

namespace SKDB.MongoDB
{
    internal class MongoDatabaseIdSerializer : SerializerBase<IDatabaseId>
    {
        public override IDatabaseId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var id       = context.Reader.ReadObjectId();
            var entityId = DatabaseIdFactory.CreateEmpty(DatabaseIdType.MongoDb);
            entityId.Id = id.ToString();
            return entityId;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IDatabaseId value)
        {
            var objId = ObjectId.Parse(value.Id);
            context.Writer.WriteObjectId(objId);
        }
    }
}