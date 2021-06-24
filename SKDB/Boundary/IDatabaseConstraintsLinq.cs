using System;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace SKDB.Boundary
{
    public interface IDatabaseConstraintsLinq<T> : IDatabaseConstraints
    {
        void Sort(Expression<Func<T, object>> field, SortOrder direction);

        void Include(Expression<Func<T, object>> field);
    }
}