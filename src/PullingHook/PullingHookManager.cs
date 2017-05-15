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

        private void NotifyEach(Action<string, string, T> action, IEnumerable<T> items, string sourceName, string sourceDescription)
        {
            if (action == null)
            {
                return;
            }

            foreach (var item in items)
            {
                action(sourceName, sourceDescription, item);
            }
        }

        private void PerformScheduledAction(IPullingConfiguration<T> pullingConfiguration)
        {
            // pull new values
            var newValues = pullingConfiguration.Source.Pull()
                .ToList(); //TODO: avoid enumerable resolution

            // get previous values
            string key = typeof(T).GetAllTypeNames();
            var previousValues = Storage.Retrieve(key);

            // store new values
            Storage.Store(key, newValues);

            // analyze differences
            var unitOfWork = new UnitOfWork<T, TKeyProperty>(newValues, previousValues, _keyPropertySelector); //TODO: add excludedPropertyNames
            var results = unitOfWork.Merge();

            // notify all
            var source = pullingConfiguration.Source;
            var sink = pullingConfiguration.Sink;

            if (results.HasChanges)
            {
                sink.Notify(source.Name, source.Description, results);

                NotifyEach(sink.OnAdded, results.Inserts, source.Name, source.Description);
                NotifyEach(sink.OnUpdated, results.Updates, source.Name, source.Description);
                NotifyEach(sink.OnRemoved, results.Deletes, source.Name, source.Description);
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
