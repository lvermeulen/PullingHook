using System;
using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public class PullingHookManager<T> : IPullingHookManager<T>
    {
        private readonly List<IPullingConfiguration<T>> _configurations = new List<IPullingConfiguration<T>>();

        public Func<T, T> ResultHandler { get; set; }

        public Action<IPullingConfiguration<T>> ScheduledAction => pullingConfiguration =>
        {
            var result = pullingConfiguration.Source.Pull();

            var resultHandler = ResultHandler ?? (t => t);
            var handledResult = resultHandler(result);

            var source = pullingConfiguration.Source;
            var sink = pullingConfiguration.Sink;
            var comparer = sink.Comparer;
            if (comparer == null || sink.Comparer.Compare(result, handledResult) != 0)
            {
                sink.Notify(source.Name, source.Description, result);
            }
        };

        public IEnumerable<IPullingConfiguration<T>> Configurations => _configurations.AsEnumerable();

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
