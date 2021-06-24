using System.Collections.Generic;
using System.Linq;

namespace SKDB
{
    internal static class Utils
    {
        internal static bool None<TSource>(this IEnumerable<TSource> source) => !source.Any();
        internal static bool IsNotNull(this object source) => source is not null;
    }
}