using System;
using SKDB.Boundary;

namespace SKDB.Generic
{
    /// <summary>
    /// string based id
    /// </summary>
    public class DefaultDatabaseId : IDatabaseId
    {
        internal DefaultDatabaseId() => Id = Guid.NewGuid().ToString();

        internal DefaultDatabaseId(Guid id) => Id = id.ToString();

        public string Id { get; set; }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return Id == (obj as DefaultDatabaseId)?.Id;
        }

        protected bool Equals(DefaultDatabaseId other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}