using System;

namespace PullingHook
{
    public class PullingConfiguration<T, TKeyProperty> : IPullingConfiguration<T, TKeyProperty>
    {
        public IPullingSchedule Schedule { get; }
        public IPullingSource<T> Source { get; }
        public IPullingSink<T, TKeyProperty> Sink { get; }

        public PullingConfiguration(TimeSpan interval, IPullingSource<T> pullingSource, IPullingSink<T, TKeyProperty> pullingSink)
        {
            Schedule = new PullingSchedule { Interval = interval };
            Source = pullingSource;
            Sink = pullingSink;
        }
    }
}