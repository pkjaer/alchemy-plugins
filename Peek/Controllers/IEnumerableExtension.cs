using System;
using System.Collections.Generic;
using System.Linq;

namespace Alchemy.Plugins.Peek.Controllers
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}