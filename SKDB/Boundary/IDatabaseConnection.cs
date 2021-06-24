using System;
using System.Security;

namespace SKDB.Boundary
{
    public interface IDatabaseConnection
    {
        bool IsConnected { get; }

        bool Connect();

        bool Connect(Action serializers);

        bool Connect(string host, int port, string userName, SecureString password);

        bool Connect(string host, int port, string userName, SecureString password, Action serializers);

        IDatabaseClient Client { get; }
    }
}