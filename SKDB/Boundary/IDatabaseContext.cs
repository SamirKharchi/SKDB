using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SKDB.MongoDB;

namespace SKDB.Boundary
{
    /// <summary>
    /// Represents a database context, which is e.g.  a MongoDB collection (or an SQL table) and allows several CRUD (Create-Read-Update-Delete) operations.
    /// Retrieve it via <see cref="IDatabaseClient.ConnectToContext{TEntity}(string)"/>.
    /// </summary>
    /// <typeparam name="T">The document type of the collection</typeparam>
    public interface IDatabaseContext<T>
    {
        /// <summary>
        /// Removes the collection (and also all indexes!) from the database.
        /// </summary>
        void Remove();

        /// <summary>
        /// Reads the amount of documents available in the collection
        /// </summary>
        /// <returns>The amount of documents in the collection</returns>
        long GetCount();

        /// <summary>
        /// Reads a single document from the database collection. The accessor must make sure to define a search filter that
        /// applies to a single document only<br/>
        /// If it doesn't and applies to several documents an <exception cref="InvalidOperationException">InvalidOperationException</exception> is thrown.
        /// </summary>
        /// <param name="accessor">The accessor must make sure to define a search filter that applies to a single document only.</param>
        /// <returns>The single document found or null if none is found for the search filter</returns>
        T Get(IDatabaseAccessor accessor);

        /// <summary>
        /// Reads all documents from the database collection.
        /// </summary>
        /// <param name="accessor">An optional accessor defining a search filter. If no accessor is defined all documents in the collection are returned.</param>
        /// <param name="constraints">Optional constraints for the resulting list of documents.</param>
        /// <returns>A list of all found documents.</returns>
        List<T> GetAll(IDatabaseAccessor accessor = null, IDatabaseConstraints constraints = null);

        /// <summary>
        /// Adds a document to the collection.
        /// </summary>
        /// <param name="document">The document to add.</param>
        void Add(T document);

        /// <summary>
        /// Replaces a document with another one.
        /// </summary>
        /// <param name="document">The document that replaces the available one in the collection.</param>
        /// <param name="accessor">An accessor that provides the search filter in order to find the document(s) to replace</param>
        /// <returns>The result of the replacement operation</returns>
        IDatabaseResult Replace(T document, IDatabaseAccessor accessor);

        /// <summary>
        /// Updates elements of a document according to update operations defined in a given accessor.
        /// </summary>
        /// <param name="accessor">The accessor defining a document search filter and update operations.</param>
        /// <returns>The result of the update operations.</returns>
        IDatabaseResult Update(IDatabaseAccessor accessor);

        /// <summary>
        /// Updates elements of a document according to update operations defined in a given accessor.
        /// </summary>
        /// <param name="accessor">The accessor defining a document search filter and update operations.</param>
        /// <returns>The document before it was modified</returns>
        T UpdateAndReturn(IDatabaseAccessor accessor);

        /// <summary>
        /// Deletes the first document that is found by the search filter defined in the accessor.
        /// </summary>
        /// <param name="accessor">The accessor providing the document search filter.</param>
        /// <returns>The result of the delete operation.</returns>
        IDatabaseResult Delete(IDatabaseAccessor accessor);

        /// <summary>
        /// Simultaneously performs several different operations defined by the <see cref="MongoMultiWriter{T}"/>
        /// </summary>
        /// <param name="filters">The accessor providing the document search filter.</param>
        /// <returns>The result of the multi writing operation.</returns>
        IDatabaseResult MultiWrite(IDatabaseMultiWriter<T> filters);

        /// <summary>
        /// Adds an (ascending) index for a specific document's field
        /// </summary>
        /// <param name="field">The field defined by a LINQ expression</param>
        /// <returns>The name of the created index</returns>
        string AddIndex(string field);

        /// <summary>
        /// Adds an (ascending) index for a specific document's field
        /// </summary>
        /// <param name="field">The field defined by a LINQ expression</param>
        /// <returns>The name of the created index</returns>
        string AddIndex(Expression<Func<T, object>> field);
    }
}