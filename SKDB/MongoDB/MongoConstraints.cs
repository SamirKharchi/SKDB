using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using MongoDB.Driver;
using SKDB.Boundary;

namespace SKDB.MongoDB
{
    /// <summary>
    /// Represents read operation result constraints such as sorting, skipping, limiting and projecting
    /// </summary>
    /// <typeparam name="T">The document type</typeparam>
    internal class MongoConstraints<T> : IDatabaseConstraintsLinq<T>
    {
        #region api_internals

        private readonly List<SortDefinition<T>> mSorters     = new List<SortDefinition<T>>();
        private readonly FindOptions<T>          mFinderAsync = new FindOptions<T>();

        private bool mIsDirtySorter;

        private static void Reset(FindOptions<T> obj)
        {
            obj.Limit      = null;
            obj.Skip       = null;
            obj.Projection = null;
            obj.Sort       = null;
        }

        internal SortDefinition<T> Sorter => Builders<T>.Sort.Combine(mSorters);
        internal FindOptions<T>    Finder { get; private set; } = new FindOptions<T>();

        internal FindOptions<T> FinderAsync
        {
            get
            {
                if (mFinderAsync.Sort is null || !mIsDirtySorter)
                {
                    return mFinderAsync;
                }

                mFinderAsync.Sort = Builders<T>.Sort.Combine(mSorters);
                mIsDirtySorter    = false;
                return mFinderAsync;
            }
        }

        #endregion

        public void Reset()
        {
            mSorters.Clear();

            MongoConstraints<T>.Reset(Finder);
            MongoConstraints<T>.Reset(mFinderAsync);
        }

        public void Sort(string field, SortOrder direction)
        {
            switch (direction)
            {
                case SortOrder.Ascending:
                    mSorters.Add(Builders<T>.Sort.Ascending(field));
                    break;
                case SortOrder.Descending:
                    mSorters.Add(Builders<T>.Sort.Descending(field));
                    break;
            }

            mIsDirtySorter = true;
        }

        public void Sort(Expression<Func<T, object>> field, SortOrder direction)
        {
            Sort(field.ToString().ToLower(), direction);
        }

        public void Skip(int skip)
        {
            Finder.Skip      = skip;
            FinderAsync.Skip = skip;
        }

        public void Limit(int limit)
        {
            Finder.Limit      = limit;
            FinderAsync.Limit = limit;
        }

        private void SetProjection(ProjectionDefinition<T> projection)
        {
            Finder.Projection      = projection;
            FinderAsync.Projection = projection;
        }

        public void Include(string field)
        {
            var projection = Builders<T>.Projection.Include(field).Exclude("_id");
            SetProjection(projection);
        }

        public void Include(Expression<Func<T, object>> field)
        {
            var projection = Builders<T>.Projection.Include(field).Exclude("_id");
            SetProjection(projection);
        }
    }
}