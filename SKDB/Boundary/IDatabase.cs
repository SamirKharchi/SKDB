namespace SKDB.Boundary
{
    public interface IDatabase
    {
        void DropContext(string name);
    }
}