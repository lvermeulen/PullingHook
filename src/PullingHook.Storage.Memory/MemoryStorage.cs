﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PullingHook.Storage.Memory
{
    public class MemoryStorage<T> : IPullingSourceStorage<T>
    {
        private readonly IHasher _hasher;
        private readonly ConcurrentDictionary<string, IEnumerable<HashedPair<T>>> _storage = new ConcurrentDictionary<string, IEnumerable<HashedPair<T>>>();

        public MemoryStorage(IHasher hasher)
        {
            _hasher = hasher;
        }

        public IEnumerable<HashedPair<T>> Retrieve(string key)
        {
            IEnumerable<HashedPair<T>> values;
            if (!_storage.TryGetValue(key, out values))
            {
                return Enumerable.Empty<HashedPair<T>>();
            }

            return values;
        }

        public void Store(string key, IEnumerable<T> values)
        {
            _storage[key] = values.Select(x => new HashedPair<T>(x, _hasher));
        }
    }
}
