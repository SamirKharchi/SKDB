using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace SKDB.MongoDB
{
    /// <summary>
    /// A MongoDB convention that forces all keys in a document to be serialized
    /// and stored in lower case letters.
    /// </summary>
    internal class MongoLowerCaseConvention : IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap) => memberMap.SetElementName(memberMap.MemberName.ToLower());

        public string Name => this.GetType().ToString();
    }
}