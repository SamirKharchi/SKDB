using System;
using MongoDB.Bson.Serialization.Attributes;
using SKDB.Boundary;

namespace SKDB.Generic
{
    /// <summary>
    /// A long number based database id. The ids are not guaranteed to be unique using the default constructor
    /// so callers should always make sure to explicitly assign an id here.
    /// </summary>
    public class LongDatabaseId : IDatabaseId
    {
        internal LongDatabaseId() => InternalId = new Random(DateTime.Now.Ticks.GetHashCode()).Next();

        internal LongDatabaseId(long id) => InternalId = id;

        [BsonId]
        internal long InternalId { get; set; }

        [BsonIgnore]
        public string Id
        {
            get => InternalId.ToString();
            set
            {
                try
                {
                    InternalId = long.Parse(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return InternalId == (obj as LongDatabaseId)?.InternalId;
        }

        protected bool Equals(LongDatabaseId other)
        {
            return InternalId.Equals(other.InternalId);
        }

        public override int GetHashCode()
        {
            return InternalId.GetHashCode();
        }
    }
}