using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PullingHook
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<KeyValuePair<string, T>> Hash<T>(this IEnumerable<T> items, IEnumerable<string> excludePropertyNames = null, HashAlgorithm hashAlgorithm = null, Encoding encoding = null)
        {
            return items
                .Select(x => new KeyValuePair<string, T>(
                    x.Hash(excludePropertyNames, hashAlgorithm, encoding), 
                    x)
                );
        }
    }
}
