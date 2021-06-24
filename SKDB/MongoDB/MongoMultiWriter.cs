using System.Collections.Generic;
using MongoDB.Driver;
using SKDB.Boundary;
using SKDB.MongoDB.Contracts;

namespace SKDB.MongoDB
{
    internal class MongoMultiWriter<T> : IDatabaseMultiWriter<T>
    {
        #region api_internals

        internal List<WriteModel<T>> Filter { get; } = new List<WriteModel<T>>();

        #endregion

        public void Delete(IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>)accessor;
            Filter.Add(new DeleteOneModel<T>(mongoAccessor.Filter));
        }

        public void Replace(IDatabaseAccessor accessor, T document)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>)accessor;
            Filter.Add(new ReplaceOneModel<T>(mongoAccessor.Filter, document));
        }

        public void Update(IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>)accessor;
            Filter.Add(new UpdateOneModel<T>(mongoAccessor.Filter, mongoAccessor.Updates));
        }

        public void Create(T document) => Filter.Add(new InsertOneModel<T>(document));

        public void Clear() => Filter.Clear();
    }
}