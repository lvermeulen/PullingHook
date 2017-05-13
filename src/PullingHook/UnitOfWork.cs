using System;
using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public class UnitOfWork<T, TKeyProperty>
        where T : class
    {
        private readonly IEnumerable<T> _source;
        private readonly IEnumerable<HashedPair<T>> _target;
        private readonly Func<T, TKeyProperty> _keyPropertySelector;
        private readonly IEnumerable<string> _excludedPropertyNames;

        public UnitOfWork(IEnumerable<T> source, IEnumerable<HashedPair<T>> target, Func<T, TKeyProperty> keyPropertySelector, IEnumerable<string> excludedPropertyNames = null)
        {
            _source = source;
            _target = target;
            _keyPropertySelector = keyPropertySelector;
            _excludedPropertyNames = excludedPropertyNames ?? Enumerable.Empty<string>();
        }

        public UnitOfWorkResults<T> Merge()
        {
            var updates = from sourceResult in _source
                          join targetResult in _target on _keyPropertySelector(sourceResult) equals _keyPropertySelector(targetResult.Value)
                          let hashedSourceResult = new HashedPair<T>(sourceResult)
                          where hashedSourceResult.HashValue != targetResult.HashValue
                          select hashedSourceResult;

            var uniqueComparer = new KeyComparer<T, TKeyProperty>(_keyPropertySelector);
            var results = new UnitOfWorkResults<T>
            {
                Inserts = _source.Except(_target.Select(x => x.Value), uniqueComparer).ToList(),
                Updates = updates.Select(x => x.Value).ToList(),
                Deletes = _target.Select(x => x.Value).Except(_source, uniqueComparer).ToList()
            };

            return results;
        }
    }
}
