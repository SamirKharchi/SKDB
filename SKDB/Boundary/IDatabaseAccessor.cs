using System;
using System.Collections.Generic;
using SKDB.Boundary.Helpers;

namespace SKDB.Boundary
{
    /// <summary>
    /// This class is used to define search criteria in order to later find one or several documents in a MongoDB collection and modify their content.<br/>
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
    public interface IDatabaseAccessor
    {
        /// <summary>
        /// Resets the mongo filter
        /// </summary>
        void Reset();

        bool HasUpdates();
        bool HasSearchFilters();
        bool HasArrayUpdates();

        #region filters

        /// <summary>
        /// <para>Filters for a specific ObjectId in the _id field of a document</para>
        /// <para>The following DatabaseComparison operators are <b>NOT</b> supported:</para>
        /// <code>DatabaseComparison.<b>Contains</b><br/>
        /// DatabaseComparison.<b>ContainsNot</b></code>
        /// </summary>
        /// <param name="id">The object id to filter for</param>
        /// <param name="op">The operation to use for filtering. Default: Equals</param>
        /// <returns>true if filter was added. false otherwise (e.g. if an unsupported op was specified)</returns>
        bool AddSearchFilterById(IDatabaseId id, DatabaseComparison op);

        /// <summary>
        /// Calls <see cref="AddSearchFilter{TV}(string, TV, DatabaseComparison)"/> with the database's own object id type (e.g. MongoDB ObjectId)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        bool AddSearchFilter(string field, IDatabaseId value, DatabaseComparison op);

        /// <summary>
        /// <para>Filters for a certain value in the given field of a document</para>
        ///
        /// <para>The following DatabaseComparison operators are <b>NOT</b> supported:</para>
        /// <code>DatabaseComparison.<b>Contains</b><br/>
        /// DatabaseComparison.<b>ContainsNot</b></code>
        /// </summary>
        /// <param name="field">The field name</param>
        /// <param name="value">The value to filter for</param>
        /// <param name="op">The operation to use for filtering. Default: Equals</param>
        /// <returns>true if filter was added. false otherwise (e.g. if an unsupported op was specified)</returns>
        bool AddSearchFilter<TV>(string field,
                                 TV value,
                                 DatabaseComparison op);

        /// <summary>
        /// Calls <see cref="AddSearchFilter{TV}(string,IEnumerable{TV},DatabaseComparison)"/> with the database's own object id type (e.g. MongoDB ObjectId)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        bool AddSearchFilter(string field, IEnumerable<IDatabaseId> values, DatabaseComparison op);

        /// <summary>
        /// <para>Filters for a specific set of values in a certain field of a document</para>
        ///
        /// <para>The following DatabaseComparison operators are supported for all passed values:</para>
        /// <code>DatabaseComparison.<b>Contains</b><br/>
        /// DatabaseComparison.<b>ContainsNot</b><br/>
        /// DatabaseComparison.<b>Exists</b><br/>
        /// DatabaseComparison.<b>ExistsNot</b></code>
        ///
        /// <remarks><para>The other DatabaseComparison operators are supported only for the first item in the passed values.<br></br>
        /// In that case it's better to use the single value overload for clarity:</para>
        /// <see cref="AddSearchFilter{TV}(string, TV ,DatabaseComparison)"/></remarks>
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <param name="field">The field name</param>
        /// <param name="values">The values to filter for</param>
        /// <param name="op">The operator to use</param>
        /// <returns>true if filter was added. false otherwise</returns>
        bool AddSearchFilter<TV>(string field,
                                 IEnumerable<TV> values,
                                 DatabaseComparison op);

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
        /// <typeparam name="TV"></typeparam>
        /// <param name="field">The name of the field to update</param>
        /// <param name="value">The value the field is updated with. Might not be required depending on op (<see cref="DatabaseOperator"/>)</param>
        /// <param name="op">The operation to evaluate</param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperation<TV>(string field,
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
        /// <param name="field">The name of the field to update</param>
        /// <param name="value">The value the field is updated with</param>
        /// <param name="op">The operation to evaluate. The following are supported:<br/><br/>
        /// DatabaseOperator.<b>IncrementNumericalValue (default)</b><br/>
        /// DatabaseOperator.<b>MultiplyNumericalValue</b>
        /// </param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperationNumerics<TV>(string field,
                                      TV value,
                                      DatabaseOperator op) where TV : IComparable;

        /// <summary>
        /// Helper function which effectively calls <see cref="AddOperation{TV}(string,TV,DatabaseOperator)"/> and
        /// Updates an element defined by a specific element id in an array with a specific name.
        /// </summary>
        /// <typeparam name="TV">The value type of the element to update</typeparam>
        /// <param name="arrayAndId">Must contain the name of the array and the id of the element to update</param>
        /// <param name="value">The value to update the element with. Might not be required depending on op (<see cref="DatabaseOperator"/>)</param>
        /// <param name="op">The operation to evaluate.</param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperation<TV>(ArrayNameElementId arrayAndId,
                              TV value,
                              DatabaseOperator op);

        /// <summary>
        /// Helper function which effectively calls <see cref="AddOperation{TV}(string,TV,DatabaseOperator)"/> and
        /// Updates an element in an array which itself is embedded as an element in an array.
        /// </summary>
        /// <typeparam name="TV">The value type of the element to update</typeparam>
        /// <param name="arrayAndId">Must contain the name of the first-level array and the id of the element that contains the second-level array</param>
        /// <param name="arrayInArrayAndId">Must contain the name of the second-level array and the id of the contained element to update</param>
        /// <param name="value">The value to update the element with. Might not be required depending on op (<see cref="DatabaseOperator"/>)</param>
        /// <param name="op">The operation to evaluate.</param>
        /// <returns>true if operation was successfully added, false otherwise</returns>
        bool AddOperation<TV>(ArrayNameElementId arrayAndId,
                              ArrayNameElementId arrayInArrayAndId,
                              TV value,
                              DatabaseOperator op);

        #endregion

        #region array_specific

        /// <summary>
        /// <para>Filters for a specific field of an array element by its id.</para>
        /// </summary>
        /// <param name="arrayField">The name of the array field</param>
        /// <param name="id">The id of the element in question</param>
        /// <returns>true if filter was added, false otherwise</returns>
        bool AddSearchInArrayByIdFilter(string arrayField, IDatabaseId id);

        /// <summary>
        /// <para>Removes and element from an array by using the array name and the elements object id.</para>
        /// <remarks>
        /// <b>Requires a search filter on the document via an AddSearchFilter call.</b><br/><br/>
        /// </remarks>
        /// <example>
        /// For example, to remove an element from a document's items array by its id (ObjectId)<br/><br/>
        /// <c>mongofilter.AddOperationRemoveFromArrayById("items", IdOfItemToRemove)</c>
        /// </example>
        /// </summary>
        /// <param name="array">The name of the array containing the element to remove</param>
        /// <param name="id">The id of the element to remove contained in the array</param>
        void AddOperationRemoveFromArrayById(string array, IDatabaseId id);

        /// <summary>
        /// Removes an document that is inside an array (arrayInArray) and that array is embedded itself as a document in an array (array):
        /// <code>
        /// document<br/>
        /// {<br/>
        /// .   _id: id_of_document<br/>
        /// .   items:<br/>
        /// .   {<br/>
        /// ...      items[0]<br/>
        /// ...      {<br/>
        /// .....         _id: id_of_array_item<br/>
        /// .....         elements:<br/>
        /// .....         {<br/>
        /// .......            elements[0]<br/>
        /// .......            {<br/>
        /// .........               _id: id_of_element_to_remove<br/>
        /// .......            }<br/>
        /// .......            [...]<br/>
        /// .....         }<br/>
        /// ...       }<br/>
        /// ...      items[1]<br/>
        /// ...      [...]<br/>
        /// .    }<br/>
        /// }<br/>
        /// </code>
        /// <example>
        /// <b>Important: Requires search filters on the root document and on the array item id via AddSearchFilter calls:</b>
        /// <code>
        /// mongofilter.AddSearchFilterById(id_of_document);<br/>
        /// mongofilter.AddSearchFilter("items._id", id_of_array_item);<br/>
        /// mongofilter.AddOperationRemoveFromNestedArrayById("items", "elements", id_of_element_to_remove);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="array">The name of the array that contains the nested array</param>
        /// <param name="arrayInArray">The name of the nested array to remove an object element from</param>
        /// <param name="id">The id of the object element contained in the nested array</param>
        void AddOperationRemoveFromNestedArrayById(string array,
                                                   string arrayInArray,
                                                   IDatabaseId id);

        #endregion
    }
}