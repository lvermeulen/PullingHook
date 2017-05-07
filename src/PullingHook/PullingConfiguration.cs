using System;

namespace PullingHook
{
    public class PullingConfiguration<T> : IPullingConfiguration<T>
    {
        public IPullingSchedule Schedule { get; }
        public IPullingSource<T> Source { get; }
        public IPullingSink<T> Sink { get; }

        public PullingConfiguration(TimeSpan interval, IPullingSource<T> pullingSource, IPullingSink<T> pullingSink)
        {
            Schedule = new PullingSchedule { Interval = interval };
            Source = pullingSource;
            Sink = pullingSink;
        }
    }
}