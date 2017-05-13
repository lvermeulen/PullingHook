using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PullingHook
{
    public static class ObjectExtensions
    {
        public static T With<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }

        public static string Hash(this object obj, IEnumerable<string> excludePropertyNames = null, HashAlgorithm hashAlgorithm = null, Encoding encoding = null)
        {
            // get properties with values to hash
            var props = obj.GetType()
                .GetProperties()
                .Where(p => excludePropertyNames == null || !excludePropertyNames.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase));

            // get property values to hash
            var hashSourceBuilder = new StringBuilder("");
            foreach (var prop in props)
            {
                var value = prop.GetValue(obj) ?? "";
                hashSourceBuilder.Append(value);
            }
            string hashSource = hashSourceBuilder.ToString();

            // hash values
            var algorithm = hashAlgorithm ?? new SHA1CryptoServiceProvider();
            encoding = encoding ?? Encoding.ASCII;
            var hashBytes = algorithm.ComputeHash(encoding.GetBytes(hashSource));

            // convert bytes to string
            var sb = new StringBuilder();
            foreach (byte hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return "0x" + sb;
        }
    }
}
