using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SKDB.Boundary.Helpers;

namespace SKDB.Boundary
{
    /// <summary>
    /// This class is used to define LINQ-based search criteria in order to later find one or several documents in a MongoDB collection and modify their content.<br/>
    /// Furthermore used to define one or several update operations that are evaluated per document.<br/><br/>
    ///
    /// <b>Step 1</b><br/>
    /// Finding a document in a MongoDB collection is done by defining one or several search filters, described by elements (fields) inside these documents.<br/><br/>
    ///
    /// <i>Available search filters are described by </i><see cref="DatabaseComparison"/><br/><br/>
    ///
    /// <b>Step 2</b><br/>Once this is done, you select a certain operation that is evaluated for any found (i.e. filterd) documents.<br/>
    /// Effectively this means modifying (updating) one or several fields inside these documents.<br/><br/>
    ///
    /// <i>Available update operations are described by </i><see cref="DatabaseOperator"/>
    /// </summary>
    /// <typeparam name="T">The document type</typeparam>
    public interface IDatabaseAccessorLinq<T> : IDatabaseAccessor
    {
        #region filters

        /// <summary>
        /// <para>Filters for a specific value in the given field of a document.</para>
        ///
        /// <para>These DatabaseComparison operators are <b>NOT</b> supported:</para>
        /// <code>DatabaseComparison.<b>Contains</b><br/>
        /// DatabaseComparison.<b>ContainsNot</b><br/>
        /// DatabaseComparison.<b>Exists</b><br/>
        /// DatabaseComparison.<b>ExistsNot</b>
        /// </code>
        /// <remarks>
        /// If you want to use the unsupported operators Exists or ExistsNot, use the non-value overload:<br/>
        /// <see cref="AddSearchFilter"/><br/><br/>
        /// If you want to use the unsupported operators Contains or ContainsNot, use the multi-value overload:<br/>
        /// <see cref="AddSearchFilter{TV}(Expression{Func{T, TV}}, IEnumerable{TV}, DatabaseComparison)"/></remarks>
        /// </summary>
        /// <typeparam name="TV">The value type</typeparam>
        /// <param name="field">The field name given as a LINQ expression, i.e. using dot notation</param>
        /// <param name="value">The value used for the comparison</param>
        /// <param name="op">The comparison operator to use</param>
        /// <returns>true if filter was added, false otherwise</returns>
        bool AddSearchFilter<TV>(Expression<Func<T, TV>> field,
                                 TV value,
                                 DatabaseComparison op);

        /// <summary>
        /// <para>Filters for a specific set of values in a certain field of a document</para>
        /// <para>The following DatabaseComparison operators are supported for all passed values:</para>
        /// <code>DatabaseComparison.<b>Contains</b><br/>
        /// DatabaseComparison.<b>ContainsNot</b>
        /// </code>
        /// <remarks>The other DatabaseComparison operators (except Exists and ExistsNot) are supported only for the first item in the passed values.<br/>
        /// In that case it's better to use the single value overload for clarity:<br/>
        /// <see cref="AddSearchFilter{TV}(Expression{Func{T, TV}}, TV, DatabaseComparison)"/></remarks>
        /// </summary>
        /// <typeparam name="TV">The value type</typeparam>
        /// <param name="field">The field name given as a LINQ expression, i.e. using dot notation</param>
        /// <param name="values">The values to filter for</param>
        /// <param name="op">The operator to use</param>
        /// <returns>true if filter was added. false otherwise</returns>
        bool AddSearchFilter<TV>(Expression<Func<T, TV>> field,
                                 IEnumerable<TV> values,
                                 DatabaseComparison op);

        /// <summary>
        /// <para>Filters for a specific value in a given field of a document.
        /// Use it when the field is a document itself</para>
        ///
        /// <para>These DatabaseComparison operators are supported:</para>
        /// <code>DatabaseComparison.<b>Exists</b><br/>
        /// DatabaseComparison.<b>ExistsNot</b></code>
        ///
        /// <remarks>If you want to use the unsupported operators, use the value overloads:<br/><br/>
        /// <see cref="AddSearchFilter{TV}(Expression{Func{T, TV}}, TV, DatabaseComparison)"/><br/>
        /// <see cref="AddSearchFilter{TV}(Expression{Func{T, TV}}, IEnumerable{TV}, DatabaseComparison)"/></remarks>
        /// </summary>
        /// <param name="field">The field name given as a LINQ expression, i.e. using dot notation</param>
        /// <param name="op">The comparison operator to use</param>
        /// <returns>true if filter was added, false otherwise</returns>
        bool AddSearchFilter(Expression<Func<T, object>> field, DatabaseComparison op);

        #endregion

        #region updates

        /// <summary>
        /// <para>Adds a <see cref="DatabaseOperator"/> applied to a certain field.</para>
        /// <para>The operator is evaluated for any defined filters when passed to either one of the following collection calls:</para>
        /// <see cref="IDatabaseContext{T}.Update"/><br/>
        /// <see cref="IDatabaseContext{T}.UpdateAndReturn"/><br/>
        /// <see cref="IDatabaseContextAsync{T}.UpdateAsync"/><br/>
        /// <see cref="IDatabaseContextAsync{T}.UpdateAndReturnAsync"/><br/>
        /// </summary>
        /// <typeparam name="TV">The value type of the field to update</typeparam>
        /// <param name="field">The name of the field to update defined by a LINQ expression, i.e. using dot notation</param>
        /// <param name="value">The value the field is updated with. Might not be required depending on op (<see cref="DatabaseOperator"/>)</param>
        /// <param name="op">The operation to evaluate. The following are supported:<br/><br/>
        /// DatabaseOperator.<b>AddOrOverwrite</b><br/>
        /// DatabaseOperator.<b>OverwriteIfGreater</b><br/>
        /// DatabaseOperator.<b>OverwriteIfLower</b>
        /// </param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperation<TV>(Expression<Func<T, TV>> field,
                              TV value,
                              DatabaseOperator op);

        /// <summary>
        /// <para>Adds a <see cref="DatabaseOperator"/> applied to a certain field holding a numeric value (must be either <b>Int32, Int64 or Double</b>).</para>
        /// <para>The operator is evaluated for any defined filters when passed to either one of the following collection calls:</para>
        /// <see cref="IDatabaseContext{T}.Update"/><br/>
        /// <see cref="IDatabaseContext{T}.UpdateAndReturn"/><br/>
        /// <see cref="IDatabaseContextAsync{T}.UpdateAsync"/><br/>
        /// <see cref="IDatabaseContextAsync{T}.UpdateAndReturnAsync"/><br/>
        /// </summary>
        /// <typeparam name="TV">The value type of the field to update</typeparam>
        /// <param name="field">The name of the field to update defined by a LINQ expression, i.e. using dot notation</param>
        /// <param name="value">The value the field is updated with</param>
        /// <param name="op">The operation to evaluate. The following are supported:<br/><br/>
        /// DatabaseOperator.<b>IncrementNumericalValue (default)</b><br/>
        /// DatabaseOperator.<b>MultiplyNumericalValue</b>
        /// </param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperationNumerics<TV>(Expression<Func<T, TV>> field,
                                      TV value,
                                      DatabaseOperator op)
            where TV : IComparable;

        #endregion

        #region array_specific

        /// <summary>
        /// <para>Filters for a specific field of an array element using expressions.</para>
        /// </summary>
        /// <typeparam name="TV">The array element type</typeparam>
        /// <param name="arrayField">An expression defining the array field, e.g. <code>x => x.arrayName</code></param>
        /// <param name="element">An unary expression on the array element</param>
        /// <returns>true if filter was added, false otherwise</returns>
        bool AddSearchInArrayFilter<TV>(Expression<Func<T, IEnumerable<TV>>> arrayField, Expression<Func<TV, bool>> element);

        /// <summary>
        /// <para>Removes and element from an array by using a custom expression using dot notation.</para>
        /// <remarks>
        /// <b>Requires a search filter on the document via an AddSearchFilter call.</b><br/><br/>
        /// </remarks>
        /// <example>
        /// For example, to remove an element from a document's items array by its id (ObjectId)<br/><br/>
        /// <code>
        /// mongofilter.AddSearchFilterById(id_of_document);<br/>
        /// mongofilter.AddOperationRemoveFromArrayByExpression(document => document.items, docItem => (docItem.Id == id_of_item_to_remove));
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TItem">The array element type</typeparam>
        /// <param name="field">The field LINQ expression, i.e. using dot notation, that describes the array containing the element to remove</param>
        /// <param name="filter">The filter expression that describes the element to remove</param>
        void AddOperationRemoveFromArrayByExpression<TItem>(Expression<Func<T, IEnumerable<TItem>>> field,
                                                            Expression<Func<TItem, bool>> filter);

        #endregion
    }
}