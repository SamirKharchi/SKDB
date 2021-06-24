﻿namespace SKDB.Boundary
{
    /// <summary>
    /// Represents a client. It can only be generated by the according
    /// factory functions <see cref="IDatabaseServer.CreateClient(string)"/> / <see cref="IDatabaseServer.CreateClient(string, string, System.Security.SecureString)"/>
    /// and is used to connect to a <see cref="IDatabaseContext{T}"/>/<see cref="IDatabaseContextAsync{T}"/>.
    ///
    /// <para>The collection does not need to exist. If it doesn't, it is automatically generated in a lazy fashion.
    /// This means only when you insert documents into the collection it will be generated.</para>
    /// </summary>
    public interface IDatabaseClient
    {
        IDatabase Database { get; }

        IDatabaseContextAsync<TEntity> ConnectToContext<TEntity>(string name);
    }
}