using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<KeyValuePair<string, T>> Hash<T>(this IEnumerable<T> items, IHasher hasher) => 
            items.Select(x => new KeyValuePair<string, T>(hasher?.Hash(x), x));
    }
}
