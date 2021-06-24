using System.Data.SqlClient;

namespace SKDB.Boundary
{
    /// <summary>
    /// Represents read operation result constraints such as sorting, skipping, limiting and projecting
    /// </summary>
    public interface IDatabaseConstraints
    {
        void Reset();

        void Sort(string field, SortOrder direction);

        void Skip(int skip);

        void Limit(int limit);

        void Include(string field);
    }
}