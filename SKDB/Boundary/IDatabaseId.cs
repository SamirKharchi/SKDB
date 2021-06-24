namespace SKDB.Boundary
{
    public interface IDatabaseIdentity<T>
    {
        T Id { get; set; }
    }

    public interface IDatabaseId : IDatabaseIdentity<string>
    {
    }
}