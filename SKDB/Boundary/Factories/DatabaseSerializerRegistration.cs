using System;
using MongoDB.Bson.Serialization;

namespace SKDB.Boundary.Factories
{
    public static class DatabaseSerializerRegistration
    {
        public static bool Create<T, TSerializer>() where TSerializer : IBsonSerializer, new()
        {
            try
            {
                BsonSerializer.RegisterSerializer(typeof(T), new TSerializer());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}