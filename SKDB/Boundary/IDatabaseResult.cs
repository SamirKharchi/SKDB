namespace SKDB.Boundary
{
    public interface IDatabaseResult
    {
        bool Success { get; set; }

        int ItemsUpdated { get; set; }
    }
}