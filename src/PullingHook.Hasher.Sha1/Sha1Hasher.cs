using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PullingHook.Hasher.Sha1
{
    public class Sha1Hasher : IHasher
    {
        private readonly Encoding _encoding;
        private readonly IEnumerable<string> _excludePropertyNames;
        private readonly HashAlgorithm _hashAlgorithm;

        public Sha1Hasher(Encoding encoding = null, IEnumerable<string> excludePropertyNames = null)
        {
            _encoding = encoding ?? Encoding.ASCII;
            _excludePropertyNames = excludePropertyNames;
            _hashAlgorithm = new SHA1CryptoServiceProvider();
        }

        public string Hash(object obj)
        {
            // get properties with values to hash
            var props = obj.GetType()
                .GetProperties()
                .Where(p => _excludePropertyNames == null || !_excludePropertyNames.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase));

            // get property values to hash
            string hashSource = string.Join("þ", props.Select(x => x.GetValue(obj)?.ToString() ?? ""));

            // hash values
            var hashBytes = _hashAlgorithm.ComputeHash(_encoding.GetBytes(hashSource));

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
