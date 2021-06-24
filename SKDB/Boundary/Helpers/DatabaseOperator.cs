namespace SKDB.Boundary.Helpers
{
    /// <summary>
    /// Defines what update operation should be applied to fields (value and array) in a document
    /// </summary>
    public enum DatabaseOperator
    {
        AddOrOverwrite,
        Remove,
        AddIfNotExist,
        OverwriteIfGreater,
        OverwriteIfLower,
        IncrementNumericalValue,
        MultiplyNumericalValue,
        RemoveFirstFromArray,
        RemoveLastFromArray,
        RemoveFromArray,
        AddToArray
    }

    /// <summary>
    /// Defines what comparison operator are used for the document search filters.
    /// </summary>
    public enum DatabaseComparison
    {
        Equals,
        Differs,
        Greater,
        Less,
        GreaterOrEquals,
        LessOrEquals,
        Contains,
        ContainsNot,
        Exists,
        ExistsNot,
        ArrayElementField
    }
}