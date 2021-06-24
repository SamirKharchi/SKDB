using System.Security;

namespace SKDB.Boundary
{
    /// <summary>
    /// Represents the database server.
    /// </summary>
    public interface IDatabaseServer
    {
        IDatabaseClient CreateClient(string dbName, string userName, SecureString password);

        IDatabaseClient CreateClient(string dbName);
    }
}