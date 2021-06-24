namespace SKDB.Boundary.Helpers
{
    /// <summary>
    /// Holds an array name and the id of an element inside the array.
    /// </summary>
    public readonly struct ArrayNameElementId
    {
        /// <summary>
        /// The name of the array to access
        /// </summary>
        public readonly string ArrayName;

        /// <summary>
        /// The id of the element to access inside the array
        /// </summary>
        public readonly IDatabaseId ArrayElementId;

        /// <summary>
        /// Constructs an ArrayNameElementId out of a name and an element id.
        /// </summary>
        /// <param name="arrayName">The name of the array to access</param>
        /// <param name="arrayElementId">The id of the element inside the array to access</param>
        ArrayNameElementId(string arrayName, IDatabaseId arrayElementId)
        {
            ArrayName      = arrayName;
            ArrayElementId = arrayElementId;
        }
    }
}