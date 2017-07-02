using System;
using System.Collections.Generic;
using Dispenser;

namespace PullingHook.Fluent
{
    public static class PullingHook<T, TKeyProperty>
    {
        public static PullingHookWithKeyPropertySelector<T, TKeyProperty> WithKeyProperty(Func<T, TKeyProperty> keyPropertySelector) =>
            new PullingHookWithKeyPropertySelector<T, TKeyProperty>
            {
                KeyPropertySelector = keyPropertySelector
            };
    }

    public abstract class PullingHookBuilder<T, TKeyProperty>
    {
        internal Func<T, TKeyProperty> KeyPropertySelector { get; set; }
        internal IHashedPairStorage<T> Storage { get; set; }
        internal IPullingScheduler<T, TKeyProperty> Scheduler { get; set; }
        internal TimeSpan Interval { get; set; }
        internal IPullingSource<T> PullingSource { get; set; }
        internal IPullingSink<T, TKeyProperty> PullingSink { get; set; }
    }

    public class PullingHookWithKeyPropertySelector<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        public PullingHookWithStorage<T, TKeyProperty> WithStorage(IHashedPairStorage<T> storage) => 
            new PullingHookWithStorage<T, TKeyProperty>
            {
                KeyPropertySelector = KeyPropertySelector,
                Storage = storage
            };
    }

    public class PullingHookWithStorage<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        public PullingHookWithScheduler<T, TKeyProperty> WithScheduler(IPullingScheduler<T, TKeyProperty> scheduler) => 
            new PullingHookWithScheduler<T, TKeyProperty>
            {
                KeyPropertySelector = KeyPropertySelector,
                Storage = Storage,
                Scheduler = scheduler
            };
    }

    public class PullingHookWithScheduler<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        public PullingHookWithSource<T, TKeyProperty> When(TimeSpan interval, Func<IEnumerable<T>> puller, string sourceName = null, string sourceDescription = null) => 
            new PullingHookWithSource<T, TKeyProperty>
            {
                KeyPropertySelector = KeyPropertySelector,
                Storage = Storage,
                Scheduler = Scheduler,
                Interval = interval,
                PullingSource = PullingSourceFactory.Create(sourceName, sourceDescription, puller)
            };
    }

    public class PullingHookWithSource<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        private IPullingHookManager<T, TKeyProperty> _manager;

        public PullingHookWithSink<T, TKeyProperty> Then(Action<string, string, Dispenser<T, TKeyProperty>.Results> pusher, string sinkName = null, string sinkDescription = null)
        {
            PullingSink = PullingSinkFactory.Create(sinkName, sinkDescription, pusher);

            _manager = new PullingHookManager<T, TKeyProperty>(KeyPropertySelector)
            {
                Storage = Storage
            };
            _manager.Add(new PullingConfiguration<T, TKeyProperty>(Interval, PullingSource, PullingSink));

            return new PullingHookWithSink<T, TKeyProperty>(Scheduler, PullingSink, _manager);
        }
    }

    public class PullingHookWithSink<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        private readonly IPullingScheduler<T, TKeyProperty> _scheduler;
        private readonly IPullingSink<T, TKeyProperty> _sink;
        private readonly IPullingHookManager<T, TKeyProperty> _manager;

        public PullingHookWithSink(IPullingScheduler<T, TKeyProperty> scheduler, IPullingSink<T, TKeyProperty> sink, IPullingHookManager<T, TKeyProperty> manager)
        {
            _scheduler = scheduler;
            _sink = sink;
            _manager = manager;
        }

        public StartablePullingHook<T, TKeyProperty> Build() => 
            new StartablePullingHook<T, TKeyProperty>(_scheduler, _manager);

        public PullingHookWithSink<T, TKeyProperty> OnAdded(Action<string, string, T> adder)
        {
            _sink.OnAdded = adder;

            return this;
        }

        public PullingHookWithSink<T, TKeyProperty> OnChanged(Action<string, string, T> updater)
        {
            _sink.OnUpdated = updater;

            return this;
        }

        public PullingHookWithSink<T, TKeyProperty> OnRemoved(Action<string, string, T> remover)
        {
            _sink.OnRemoved = remover;

            return this;
        }
    }

    public class StartablePullingHook<T, TKeyProperty>
    {
        private readonly IPullingScheduler<T, TKeyProperty> _scheduler;
        private readonly IPullingHookManager<T, TKeyProperty> _manager;

        public StartablePullingHook(IPullingScheduler<T, TKeyProperty> scheduler, IPullingHookManager<T, TKeyProperty> manager)
        {
            _scheduler = scheduler;
            _manager = manager;
        }

        public void Pull()
        {
            foreach (var configuration in _manager.Configurations)
            {
                _manager.ScheduledAction(configuration);
            }
        }

        public StoppablePullingHook<T, TKeyProperty> Start()
        {
            _scheduler.Start(_manager);
            return new StoppablePullingHook<T, TKeyProperty>(_scheduler);
        }
    }

    public class StoppablePullingHook<T, TKeyProperty>
    {
        private readonly IPullingScheduler<T, TKeyProperty> _scheduler;

        public StoppablePullingHook(IPullingScheduler<T, TKeyProperty> scheduler)
        {
            _scheduler = scheduler;
        }

        public void Stop()
        {
            _scheduler.Stop();
        }
    }
}
