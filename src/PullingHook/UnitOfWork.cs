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

        private readonly IEnumerable<HashedPair<T>> _source;
        private readonly IEnumerable<HashedPair<T>> _target;
        private readonly Func<T, TKeyProperty> _keyPropertySelector;

        public UnitOfWork(IEnumerable<HashedPair<T>> source, IEnumerable<HashedPair<T>> target, Func<T, TKeyProperty> keyPropertySelector)
        {
            _source = source;
            _target = target;
            _keyPropertySelector = keyPropertySelector;
        }

        public Results Merge()
        {
            var updates = from sourceResult in _source
                          join targetResult in _target on _keyPropertySelector(sourceResult.Value) equals _keyPropertySelector(targetResult.Value)
                          where sourceResult.HashValue != targetResult.HashValue
                          select sourceResult;

            var uniqueComparer = new KeyComparer<T, TKeyProperty>(_keyPropertySelector);

            return new Results
            {
                Inserts = _source.Select(x => x.Value).Except(_target.Select(x => x.Value), uniqueComparer).ToList(),
                Updates = updates.Select(x => x.Value).ToList(),
                Deletes = _target.Select(x => x.Value).Except(_source.Select(x => x.Value), uniqueComparer).ToList()
            };
        }
    }
}
