namespace SKDB.Boundary
{
    /// <summary>
    /// Support to add several operations that are bulk written into the DB, i.e.
    /// the operations are passed simultaneously (not sequentially) to the database.
    /// Very useful and efficient when deleting, updating or creating several items from\in a collection.
    /// </summary>
    /// <typeparam name="T">The document type</typeparam>
    public interface IDatabaseMultiWriter<in T>
    {
        void Delete(IDatabaseAccessor accessor);

        void Replace(IDatabaseAccessor accessor, T document);

        void Update(IDatabaseAccessor accessor);

        void Create(T document);

        void Clear();
    }
}