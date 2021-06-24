using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using SKDB.Boundary;
using SKDB.MongoDB.Contracts;
using SKDB.Properties;

namespace SKDB.MongoDB
{
    internal class MongoCollection<T> : IDatabaseContextAsync<T>
    {
        #region api_internals

        private readonly IDatabase      mDatabase;
        private readonly string         mCollection;
        private readonly FindOptions<T> mSingleLimiter;

        private IMongoCollection<T> Collection => ((IDatabaseInternal) mDatabase).GetContext<T>(mCollection);

        internal MongoCollection(IDatabase database, string collectionName)
        {
            mDatabase   = database;
            mCollection = collectionName;
            mSingleLimiter = new FindOptions<T>
            {
                Limit           = 1,
                NoCursorTimeout = false,
                MaxAwaitTime    = new TimeSpan(10)
            };
        }

        internal MongoCollection(IDatabaseClient client, string collectionName)
            : this(client.Database, collectionName) { }

        private MongoCollection() { }

        #endregion

        public void Remove() => mDatabase.DropContext(mCollection);

        public T Get([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            try
            {
                return Collection.Find(mongoAccessor.Filter).SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new
                    InvalidOperationException("NavieMongoAccessor filter is returning more than a single document.");
            }
        }

        public async Task<T> GetAsync([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;

            var cursor = await Collection.FindAsync(mongoAccessor.Filter, mSingleLimiter);
            try
            {
                return await cursor.SingleOrDefaultAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                throw new
                    InvalidOperationException("NavieMongoAccessor filter is returning more than a single document.");
            }
        }

        public List<T> GetAll(IDatabaseAccessor accessor = null, IDatabaseConstraints constraints = null)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            var filter        = mongoAccessor.IsNotNull() ? mongoAccessor?.Filter : Builders<T>.Filter.Empty;

            if (constraints is null)
            {
                return Collection.Find(filter).ToList();
            }

            var mongoConstraints = (MongoConstraints<T>) constraints;
            return Collection.Find(filter)
                             .Sort(mongoConstraints.Sorter)
                             .Skip(mongoConstraints.Finder.Skip)
                             .Limit(mongoConstraints.Finder.Limit)
                             .Project(mongoConstraints.Finder.Projection)
                             .ToList();
        }

        public async Task<List<T>> GetAllAsync(IDatabaseAccessor accessor = null,
                                               IDatabaseConstraints constraints = null)
        {
            var mongoAccessor    = (IDatabaseAccessorInternal<T>) accessor;
            var mongoConstraints = (MongoConstraints<T>) constraints;
            try
            {
                var filter = mongoAccessor is not null ? mongoAccessor.Filter : Builders<T>.Filter.Empty;
                var cursor = await Collection.FindAsync(filter, mongoConstraints?.FinderAsync);
                return await cursor.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public long GetCount() => Collection.CountDocuments(Builders<T>.Filter.Empty);

        public async Task<long> GetCountAsync() =>
            await Collection.CountDocumentsAsync(Builders<T>.Filter.Empty).ConfigureAwait(false);

        public void Add([NotNull] T document) => Collection.InsertOne(document);

        public async Task AddAsync([NotNull] T document) =>
            await Collection.InsertOneAsync(document).ConfigureAwait(false);

        public IDatabaseResult Replace([NotNull] T document, [NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            return new MongoDatabaseResult(Collection.ReplaceOne(mongoAccessor.Filter, document));
        }

        public async Task<IDatabaseResult> ReplaceAsync([NotNull] T document, [NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            return new MongoDatabaseResult(await Collection
                                                      .ReplaceOneAsync(mongoAccessor.Filter, document)
                                                      .ConfigureAwait(false));
        }

        public IDatabaseResult Update([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            if (mongoAccessor.Updates == null)
            {
                throw new ArgumentNullException(nameof(mongoAccessor.Updates),
                                                "The passed accessor does not define required update operations.");
            }
            return new MongoDatabaseResult(Collection.UpdateOne(mongoAccessor.Filter, mongoAccessor.Updates,
                                                                     mongoAccessor.UpdateOptions));
        }

        public async Task<IDatabaseResult> UpdateAsync([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            if (mongoAccessor.Updates == null)
            {
                throw new ArgumentNullException(nameof(mongoAccessor.Updates),
                                                "The passed accessor does not define required update operations.");
            }
            return new MongoDatabaseResult(await Collection
                                                      .UpdateOneAsync(mongoAccessor.Filter, mongoAccessor.Updates,
                                                                      mongoAccessor.UpdateOptions)
                                                      .ConfigureAwait(false));
        }

        public T UpdateAndReturn([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            if (mongoAccessor.Updates == null)
            {
                throw new ArgumentNullException(nameof(mongoAccessor.Updates),
                                                "The passed accessor does not define required update operations.");
            }
            return Collection.FindOneAndUpdate(
                                               mongoAccessor.Filter,
                                               mongoAccessor.Updates,
                                               new FindOneAndUpdateOptions<T, T>()
                                               {
                                                   ReturnDocument = ReturnDocument.Before,
                                                   IsUpsert       = true
                                               });
        }

        public async Task<T> UpdateAndReturnAsync([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            if (mongoAccessor.Updates == null)
            {
                throw new ArgumentNullException(nameof(mongoAccessor.Updates),
                                                "The passed accessor does not define required update operations.");
            }
            return await Collection.FindOneAndUpdateAsync(
                                                          mongoAccessor.Filter,
                                                          mongoAccessor.Updates,
                                                          new FindOneAndUpdateOptions<T, T>()
                                                          {
                                                              ReturnDocument = ReturnDocument.Before,
                                                              IsUpsert       = true
                                                          }).ConfigureAwait(false);
        }

        public IDatabaseResult Delete([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            return new MongoDatabaseResult(Collection.DeleteOne(mongoAccessor.Filter));
        }

        public async Task<IDatabaseResult> DeleteAsync([NotNull] IDatabaseAccessor accessor)
        {
            var mongoAccessor = (IDatabaseAccessorInternal<T>) accessor;
            return new MongoDatabaseResult(await Collection
                                                      .DeleteOneAsync(mongoAccessor.Filter).ConfigureAwait(false));
        }

        public IDatabaseResult MultiWrite([NotNull] IDatabaseMultiWriter<T> filters)
        {
            var mongoFilters = (MongoMultiWriter<T>) filters;
            return new MongoDatabaseResult(Collection.BulkWrite(mongoFilters.Filter));
        }

        public async Task<IDatabaseResult> MultiWriteAsync([NotNull] IDatabaseMultiWriter<T> filters)
        {
            var mongoFilters = (MongoMultiWriter<T>) filters;
            return new MongoDatabaseResult(await Collection
                                                      .BulkWriteAsync(mongoFilters.Filter).ConfigureAwait(false));
        }

        public string AddIndex([NotNull] string field)
        {
            var keys = Builders<T>.IndexKeys.Ascending(field);
            return Collection.Indexes.CreateOne(new CreateIndexModel<T>(keys));
        }

        public string AddIndex([NotNull] Expression<Func<T, object>> field)
        {
            var keys = Builders<T>.IndexKeys.Ascending(field);
            return Collection.Indexes.CreateOne(new CreateIndexModel<T>(keys));
        }

        public async Task<string> AddIndexAsync([NotNull] string field)
        {
            var keys = Builders<T>.IndexKeys.Ascending(field);
            return await Collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(keys)).ConfigureAwait(false);
        }

        public async Task<string> AddIndexAsync([NotNull] Expression<Func<T, object>> field)
        {
            var keys = Builders<T>.IndexKeys.Ascending(field);
            return await Collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(keys)).ConfigureAwait(false);
        }
    }
}