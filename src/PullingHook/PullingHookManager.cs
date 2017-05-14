using System;
using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public class PullingHookManager<T, TKeyProperty> : IPullingHookManager<T>
        where T : class
    {
        private readonly Func<T, TKeyProperty> _keyPropertySelector;
        private readonly List<IPullingConfiguration<T>> _configurations = new List<IPullingConfiguration<T>>();

        public IPullingSourceStorage<T> Storage { get; set; }

        private void PerformScheduledAction(IPullingConfiguration<T> pullingConfiguration)
        {
            // pull new values
            var newValues = pullingConfiguration.Source.Pull()
                .ToList(); //TODO: avoid enumerable resolution

            // get previous values
            string key = typeof(T).GetAllTypeNames();
            var previousValues = Storage.Retrieve(key);

            // analyze differences
            var unitOfWork = new UnitOfWork<T, TKeyProperty>(newValues, previousValues, _keyPropertySelector);
            var results = unitOfWork.Merge();

            // store new values
            Storage.Store(key, newValues);

            // notify all
            var source = pullingConfiguration.Source;
            var sink = pullingConfiguration.Sink;
            //TODO: only notify when any inserts/updates/deletes
            sink.Notify(source.Name, source.Description, results);

            // notify each insert
            if (sink.OnAdded != null)
            {
                foreach (var added in results.Inserts)
                {
                    sink.OnAdded(source.Name, source.Description, added);
                }
            }

            // notify each update
            if (sink.OnUpdated != null)
            {
                foreach (var updated in results.Updates)
                {
                    sink.OnUpdated(source.Name, source.Description, updated);
                }
            }

            // notify each delete
            if (sink.OnRemoved != null)
            {
                foreach (var removed in results.Deletes)
                {
                    sink.OnRemoved(source.Name, source.Description, removed);
                }
            }
        }

        public Action<IPullingConfiguration<T>> ScheduledAction => PerformScheduledAction;

        public IEnumerable<IPullingConfiguration<T>> Configurations => _configurations.AsEnumerable();

        public PullingHookManager(Func<T, TKeyProperty> keyPropertySelector)
        {
            _keyPropertySelector = keyPropertySelector;
        }

        public IPullingConfiguration<T> Add(IPullingConfiguration<T> configuration)
        {
            _configurations.Add(configuration);

            return configuration;
        }

        public void Remove(IPullingConfiguration<T> configuration)
        {
            _configurations.Remove(configuration);
        }
    }
}
