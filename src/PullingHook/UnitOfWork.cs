using System;
using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public class UnitOfWork<T, TKeyProperty>
    {
        public class Results
        {
            public IEnumerable<T> Inserts { get; set; } = Enumerable.Empty<T>();
            public IEnumerable<T> Updates { get; set; } = Enumerable.Empty<T>();
            public IEnumerable<T> Deletes { get; set; } = Enumerable.Empty<T>();

            public bool HasChanges => Inserts.Any() || Updates.Any() || Deletes.Any();
        }

        private readonly IEnumerable<T> _source;
        private readonly IEnumerable<HashedPair<T>> _target;
        private readonly IHasher _hasher;
        private readonly Func<T, TKeyProperty> _keyPropertySelector;

        public UnitOfWork(IEnumerable<T> source, IEnumerable<HashedPair<T>> target, IHasher hasher, Func<T, TKeyProperty> keyPropertySelector)
        {
            _source = source;
            _target = target;
            _hasher = hasher;
            _keyPropertySelector = keyPropertySelector;
        }

        public Results Merge()
        {
            var updates = from sourceResult in _source
                          join targetResult in _target on _keyPropertySelector(sourceResult) equals _keyPropertySelector(targetResult.Value)
                          let hashedSourceResult = new HashedPair<T>(sourceResult, _hasher)
                          where hashedSourceResult.HashValue != targetResult.HashValue
                          select hashedSourceResult;

            var uniqueComparer = new KeyComparer<T, TKeyProperty>(_keyPropertySelector);

            return new Results
            {
                Inserts = _source.Except(_target.Select(x => x.Value), uniqueComparer).ToList(),
                Updates = updates.Select(x => x.Value).ToList(),
                Deletes = _target.Select(x => x.Value).Except(_source, uniqueComparer).ToList()
            };
        }
    }
}
