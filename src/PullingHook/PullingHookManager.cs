﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dispenser;

namespace PullingHook
{
    public class PullingHookManager<T, TKeyProperty> : IPullingHookManager<T, TKeyProperty>
    {
        private readonly Func<T, TKeyProperty> _keyPropertySelector;
        private readonly List<IPullingConfiguration<T, TKeyProperty>> _configurations = new List<IPullingConfiguration<T, TKeyProperty>>();
        private readonly Dispenser<T, TKeyProperty> _dispenser = new Dispenser<T, TKeyProperty>();

        public IHashedPairStorage<T> Storage { get; set; }

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

        private void PerformScheduledAction(IPullingConfiguration<T, TKeyProperty> pullingConfiguration)
        {
            // pull new values
            var newValues = pullingConfiguration.Source.Pull();

            // get previous values
            string key = typeof(T).GetAllTypeNames();
            var previousValues = Storage?.Retrieve(key);

            // store new values
            var hashedNewValues = Storage?.Store(key, newValues);

            // analyze differences
            var results = _dispenser.Dispense(hashedNewValues, previousValues, _keyPropertySelector);

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

        public Action<IPullingConfiguration<T, TKeyProperty>> ScheduledAction => 
            PerformScheduledAction;

        public IEnumerable<IPullingConfiguration<T, TKeyProperty>> Configurations => 
            _configurations.AsEnumerable();

        public PullingHookManager(Func<T, TKeyProperty> keyPropertySelector)
        {
            _keyPropertySelector = keyPropertySelector;
        }

        public IPullingConfiguration<T, TKeyProperty> Add(IPullingConfiguration<T, TKeyProperty> configuration)
        {
            _configurations.Add(configuration);

            return configuration;
        }

        public void Remove(IPullingConfiguration<T, TKeyProperty> configuration)
        {
            _configurations.Remove(configuration);
        }
    }
}
