namespace SKDB.Boundary.Factories
{
    public class DatabaseConnectionFactoryArgs
    {
        public DatabaseServerType ServerType      { get; set; }
        public string             DatabaseName    { get; set; }
        public string             ApplicationName { get; set; }
    }
}