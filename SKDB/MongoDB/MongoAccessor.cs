using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SKDB.Boundary;
using SKDB.Boundary.Helpers;
using SKDB.MongoDB.Contracts;
using SKDB.Properties;

namespace SKDB.MongoDB
{
    internal class MongoAccessor<T> : IDatabaseAccessorLinq<T>, IDatabaseAccessorInternal<T>
    {
        #region api_internals

        private readonly List<FilterDefinition<T>> mFilters = new List<FilterDefinition<T>>();

        private readonly List<UpdateDefinition<T>> mUpdates = new List<UpdateDefinition<T>>();

        private readonly List<ArrayFilterDefinition> mArrayFilters = new List<ArrayFilterDefinition>();

        FilterDefinition<T> IDatabaseAccessorInternal<T>.Filter => Builders<T>.Filter.And(mFilters);

        UpdateDefinition<T> IDatabaseAccessorInternal<T>.Updates => Builders<T>.Update.Combine(mUpdates);

        UpdateOptions IDatabaseAccessorInternal<T>.UpdateOptions =>
            new UpdateOptions() {ArrayFilters = mArrayFilters.None() ? null : mArrayFilters, IsUpsert = true};

        private static ObjectId ToObjectId(IDatabaseId id)
        {
            if (id is MongoDatabaseId databaseId)
            {
                return databaseId.InternalId;
            }
            return ObjectId.Parse(id.Id);
        }

        #endregion

        public void Reset()
        {
            mFilters.Clear();
            mUpdates.Clear();
            mArrayFilters.Clear();
        }

        public bool HasUpdates() => mUpdates.Any();

        public bool HasSearchFilters() => mFilters.Any();

        public bool HasArrayUpdates() => mArrayFilters.Any();

        #region filters

        public bool AddSearchFilterById([NotNull] IDatabaseId id, DatabaseComparison op = DatabaseComparison.Equals)
        {
            return AddSearchFilter("_id", MongoAccessor<T>.ToObjectId(id), op);
        }

        public bool AddSearchFilter(string field, IDatabaseId value, DatabaseComparison op)
        {
            return AddSearchFilter(field, MongoAccessor<T>.ToObjectId(value), op);
        }

        public bool AddSearchFilter(string field, IEnumerable<IDatabaseId> values, DatabaseComparison op)
        {
            return AddSearchFilter(field, values.Select(MongoAccessor<T>.ToObjectId), op);
        }

        public bool AddSearchFilter<TV>([NotNullNotEmpty] string field,
                                        TV value,
                                        DatabaseComparison op = DatabaseComparison.Equals)
        {
            switch (op)
            {
                case DatabaseComparison.Equals:
                    mFilters.Add(Builders<T>.Filter.Eq(field, value));
                    break;
                case DatabaseComparison.Differs:
                    mFilters.Add(Builders<T>.Filter.Ne(field, value));
                    break;
                case DatabaseComparison.Exists:
                    mFilters.Add(Builders<T>.Filter.Exists(field));
                    break;
                case DatabaseComparison.ExistsNot:
                    mFilters.Add(Builders<T>.Filter.Exists(field, false));
                    break;
                case DatabaseComparison.Greater:
                    mFilters.Add(Builders<T>.Filter.Gt(field, value));
                    break;
                case DatabaseComparison.Less:
                    mFilters.Add(Builders<T>.Filter.Lt(field, value));
                    break;
                case DatabaseComparison.GreaterOrEquals:
                    mFilters.Add(Builders<T>.Filter.Gte(field, value));
                    break;
                case DatabaseComparison.LessOrEquals:
                    mFilters.Add(Builders<T>.Filter.Lte(field, value));
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool AddSearchFilter<TV>([NotNullNotEmpty] string field,
                                        IEnumerable<TV> values,
                                        DatabaseComparison op = DatabaseComparison.Contains)
        {
            if ((values is null || values.None()) &&
                (op != DatabaseComparison.Exists && op != DatabaseComparison.ExistsNot))
            {
                return false;
            }

            switch (op)
            {
                case DatabaseComparison.Exists:
                    mFilters.Add(Builders<T>.Filter.Exists(field));
                    break;
                case DatabaseComparison.ExistsNot:
                    mFilters.Add(Builders<T>.Filter.Exists(field, false));
                    break;
                case DatabaseComparison.Contains:
                    mFilters.Add(Builders<T>.Filter.In(field, values));
                    break;
                case DatabaseComparison.ContainsNot:
                    mFilters.Add(Builders<T>.Filter.Nin(field, values));
                    break;
                default:
                    return AddSearchFilter(field, values.First(), op);
            }

            return true;
        }

        public bool AddSearchFilter<TV>([NotNull] Expression<Func<T, TV>> field,
                                        [NotNull] TV value,
                                        DatabaseComparison op = DatabaseComparison.Equals)
        {
            switch (op)
            {
                case DatabaseComparison.Equals:
                    mFilters.Add(Builders<T>.Filter.Eq(field, value));
                    break;
                case DatabaseComparison.Differs:
                    mFilters.Add(Builders<T>.Filter.Ne(field, value));
                    break;
                case DatabaseComparison.Greater:
                    mFilters.Add(Builders<T>.Filter.Gt(field, value));
                    break;
                case DatabaseComparison.Less:
                    mFilters.Add(Builders<T>.Filter.Lt(field, value));
                    break;
                case DatabaseComparison.GreaterOrEquals:
                    mFilters.Add(Builders<T>.Filter.Gte(field, value));
                    break;
                case DatabaseComparison.LessOrEquals:
                    mFilters.Add(Builders<T>.Filter.Lte(field, value));
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool AddSearchFilter<TV>([NotNull] Expression<Func<T, TV>> field,
                                        [NotNull] IEnumerable<TV> values,
                                        DatabaseComparison op = DatabaseComparison.Contains)
        {
            if (values.None())
            {
                return false;
            }

            switch (op)
            {
                case DatabaseComparison.Contains:
                    mFilters.Add(Builders<T>.Filter.In(field, values));
                    break;
                case DatabaseComparison.ContainsNot:
                    mFilters.Add(Builders<T>.Filter.Nin(field, values));
                    break;
                default:
                    return AddSearchFilter(field, values.First(), op);
            }

            return true;
        }

        public bool AddSearchFilter([NotNull] Expression<Func<T, object>> field,
                                    DatabaseComparison op = DatabaseComparison.Exists)
        {
            switch (op)
            {
                case DatabaseComparison.Exists:
                    mFilters.Add(Builders<T>.Filter.Exists(field));
                    break;
                case DatabaseComparison.ExistsNot:
                    mFilters.Add(Builders<T>.Filter.Exists(field, false));
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a native MongoDB filter definition. Use if you know what you are doing.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        internal void AddSearchFilter(FilterDefinition<T> filter)
        {
            mFilters.Add(filter);
        }

        #endregion

        #region updates

        public bool AddOperation<TV>([NotNullNotEmpty] string field,
                                     TV value,
                                     DatabaseOperator op = DatabaseOperator.AddOrOverwrite)
        {
            switch (op)
            {
                case DatabaseOperator.AddOrOverwrite:
                    mUpdates.Add(Builders<T>.Update.Set(field, value));
                    break;
                case DatabaseOperator.Remove:
                    mUpdates.Add(Builders<T>.Update.Unset(field));
                    break;
                case DatabaseOperator.AddIfNotExist:
                    mUpdates.Add(Builders<T>.Update.AddToSet(field, value));
                    break;
                case DatabaseOperator.OverwriteIfGreater:
                    mUpdates.Add(Builders<T>.Update.Max(field, value));
                    break;
                case DatabaseOperator.OverwriteIfLower:
                    mUpdates.Add(Builders<T>.Update.Min(field, value));
                    break;
                case DatabaseOperator.RemoveFirstFromArray:
                    mUpdates.Add(Builders<T>.Update.PopFirst(field));
                    break;
                case DatabaseOperator.RemoveLastFromArray:
                    mUpdates.Add(Builders<T>.Update.PopLast(field));
                    break;
                case DatabaseOperator.RemoveFromArray:
                    mUpdates.Add(Builders<T>.Update.Pull(field, value));
                    break;
                case DatabaseOperator.AddToArray:
                    mUpdates.Add(Builders<T>.Update.Push(field, value));
                    break;
                case DatabaseOperator.IncrementNumericalValue:
                case DatabaseOperator.MultiplyNumericalValue:
                    return AddOperationNumerics(field, value as IComparable, op);
                default:
                    return false;
            }

            return true;
        }

        public bool AddOperation<TV>([NotNull] Expression<Func<T, TV>> field,
                                     [NotNull] TV value,
                                     DatabaseOperator op = DatabaseOperator.AddOrOverwrite)
        {
            switch (op)
            {
                case DatabaseOperator.AddOrOverwrite:
                    mUpdates.Add(Builders<T>.Update.Set(field, value));
                    break;
                case DatabaseOperator.OverwriteIfGreater:
                    mUpdates.Add(Builders<T>.Update.Max(field, value));
                    break;
                case DatabaseOperator.OverwriteIfLower:
                    mUpdates.Add(Builders<T>.Update.Min(field, value));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public bool AddOperationNumerics<TV>([NotNullNotEmpty] string field,
                                             [NotNull] TV value,
                                             DatabaseOperator op = DatabaseOperator.IncrementNumericalValue)
            where TV : IComparable
        {
            if (value == null || (!(value is Int32) && !(value is Int64) && !(value is Double)))
            {
                return false;
            }

            switch (op)
            {
                case DatabaseOperator.IncrementNumericalValue:
                    mUpdates.Add(Builders<T>.Update.Inc(field, value));
                    break;
                case DatabaseOperator.MultiplyNumericalValue:
                    mUpdates.Add(Builders<T>.Update.Mul(field, value));
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool AddOperationNumerics<TV>([NotNull] Expression<Func<T, TV>> field,
                                             [NotNull] TV value,
                                             DatabaseOperator op = DatabaseOperator.IncrementNumericalValue)
            where TV : IComparable
        {
            if (value == null || (!(value is Int32) && !(value is Int64) && !(value is Double)))
            {
                return false;
            }

            switch (op)
            {
                case DatabaseOperator.IncrementNumericalValue:
                    mUpdates.Add(Builders<T>.Update.Inc(field, value));
                    break;
                case DatabaseOperator.MultiplyNumericalValue:
                    mUpdates.Add(Builders<T>.Update.Mul(field, value));
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool AddOperation<TV>([NotNull] ArrayNameElementId arrayAndId,
                                     TV value,
                                     DatabaseOperator op = DatabaseOperator.AddOrOverwrite)
        {
            ArrayFilterDefinition<BsonDocument> arrayFilter =
                new BsonDocument($"elem._id",
                                 new BsonDocument("$eq", MongoAccessor<T>.ToObjectId(arrayAndId.ArrayElementId)));
            mArrayFilters.Add(arrayFilter);

            return AddOperation($"{arrayAndId.ArrayName}.$[elem]", value, op);
        }

        public bool AddOperation<TV>([NotNull] ArrayNameElementId arrayAndId,
                                     [NotNull] ArrayNameElementId arrayInArrayAndId,
                                     TV value,
                                     DatabaseOperator op = DatabaseOperator.AddOrOverwrite)
        {
            ArrayFilterDefinition<BsonDocument> arrayFilter =
                new BsonDocument($"elema._id", MongoAccessor<T>.ToObjectId(arrayAndId.ArrayElementId));
            ArrayFilterDefinition<BsonDocument> arrayInArrayFilter =
                new BsonDocument($"elemb._id", MongoAccessor<T>.ToObjectId(arrayInArrayAndId.ArrayElementId));
            mArrayFilters.Add(arrayFilter);
            mArrayFilters.Add(arrayInArrayFilter);

            return AddOperation($"{arrayInArrayAndId.ArrayName}.$[elema].{arrayInArrayAndId.ArrayName}.$[elemb]", value,
                                op);
        }

        /// <summary>
        /// Adds a native MongoDB update definition. Use if you know what you are doing.
        /// </summary>
        /// <param name="operation">The update definition.</param>
        internal void AddOperation([NotNull] UpdateDefinition<T> operation)
        {
            mUpdates.Add(operation);
        }

        #endregion

        #region array_specific

        public bool AddSearchInArrayByIdFilter(string arrayField, IDatabaseId id)
        {
            var idFilter = Builders<T>.Filter.Eq("_id", MongoAccessor<T>.ToObjectId(id));
            mFilters.Add(Builders<T>.Filter.ElemMatch(arrayField, idFilter));
            return true;
        }

        public bool AddSearchInArrayFilter<TV>([NotNull] Expression<Func<T, IEnumerable<TV>>> field,
                                               [NotNull] Expression<Func<TV, bool>> element)
        {
            mFilters.Add(Builders<T>.Filter.ElemMatch(field, element));
            return true;
        }

        public void AddOperationRemoveFromArrayById([NotNullNotEmpty] string array, [NotNull] IDatabaseId id)
        {
            mUpdates.Add(Builders<T>.Update.PullFilter(array,
                                                       Builders<T>.Filter.Eq("_id",
                                                                             MongoAccessor<T>.ToObjectId(id))));
        }

        public void AddOperationRemoveFromArrayByExpression<TItem>(
            [NotNull] Expression<Func<T, IEnumerable<TItem>>> field,
            [NotNull] Expression<Func<TItem, bool>> filter)
        {
            mUpdates.Add(Builders<T>.Update.PullFilter(field, filter));
        }

        /// <summary>
        /// <para>Removes and element from an array by using a custom expression and a native mongodb filter definition. Use if you know what you are doing.</para>
        /// <remarks>
        /// <b>Requires a search filter on the document via an AddSearchFilter call.</b><br/><br/>
        /// </remarks>
        /// <example>
        /// For example, to remove an element from a document's items array by its id (ObjectId)<br/><br/>
        /// <code>
        /// mongofilter.AddSearchFilterById(id_of_document);<br/>
        /// mongofilter.AddOperationRemoveFromArrayByFilter(document => document.items, docTypeBuilder.Filter.Eq("_id", id_of_item_to_remove))
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TItem">The array element type</typeparam>
        /// <param name="field">The field LINQ expression, i.e. using dot notation, that describes the array containing the element to remove</param>
        /// <param name="filter">The filter definition that describes the element to remove</param>
        internal void AddOperationRemoveFromArrayByFilter<TItem>([NotNull] Expression<Func<T, IEnumerable<TItem>>> field,
                                                               [NotNull] FilterDefinition<TItem> filter)
        {
            mUpdates.Add(Builders<T>.Update.PullFilter(field, filter));
        }

        public void AddOperationRemoveFromNestedArrayById([NotNullNotEmpty] string array,
                                                          [NotNullNotEmpty] string arrayInArray,
                                                          [NotNull] IDatabaseId id)
        {
            mUpdates.Add(Builders<T>.Update.PullFilter($"{array}.$.{arrayInArray}",
                                                       Builders<T>.Filter.Eq("_id",
                                                                             MongoAccessor<T>.ToObjectId(id))));
        }

        #endregion
    }
}