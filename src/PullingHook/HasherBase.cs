using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable once RedundantUsingDirective
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace PullingHook
{
    public abstract class HasherBase : IHasher
    {
        private readonly Encoding _encoding;
        private readonly IEnumerable<string> _excludePropertyNames;

        protected abstract HashAlgorithm HashAlgorithm { get; }

        public HasherBase(Encoding encoding = null, IEnumerable<string> excludePropertyNames = null)
        {
            _encoding = encoding ?? Encoding.ASCII;
            _excludePropertyNames = excludePropertyNames;
        }

        public string Hash(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            // get properties with values to hash
            var props = obj.GetType()
                .GetProperties()
                .Where(p => _excludePropertyNames == null || !_excludePropertyNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

            // get property values to hash
            string hashSource = string.Join("þ", props.Select(x => x.GetValue(obj)?.ToString() ?? ""));

            // hash values
            var hashBytes = HashAlgorithm.ComputeHash(_encoding.GetBytes(hashSource));

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
