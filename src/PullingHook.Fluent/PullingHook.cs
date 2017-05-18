using System;
using System.Collections.Generic;

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
        internal IPullingSourceStorage<T> Storage { get; set; }
        internal IPullingScheduler<T, TKeyProperty> Scheduler { get; set; }
        internal TimeSpan Interval { get; set; }
        internal IPullingSource<T> PullingSource { get; set; }
        internal IPullingSink<T, TKeyProperty> PullingSink { get; set; }
    }

    public class PullingHookWithKeyPropertySelector<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        public PullingHookWithStorage<T, TKeyProperty> WithStorage(IPullingSourceStorage<T> storage) => 
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

        private StartablePullingHook<T, TKeyProperty> Build(Action<string, string, UnitOfWork<T, TKeyProperty>.Results> pusher = null, string sinkName = null, string sinkDescription = null)
        {
            PullingSink = PullingSinkFactory.Create(sinkName, sinkDescription, pusher ?? ((name, description, results) => { }));

            _manager = new PullingHookManager<T, TKeyProperty>(KeyPropertySelector) { Storage = Storage };
            _manager.Add(new PullingConfiguration<T, TKeyProperty>(Interval, PullingSource, PullingSink));

            return new StartablePullingHook<T, TKeyProperty>(_manager, Scheduler);
        }

        public StartablePullingHook<T, TKeyProperty> Then(Action<string, string, UnitOfWork<T, TKeyProperty>.Results> pusher, string sinkName = null, string sinkDescription = null) => 
            Build(pusher, sinkName, sinkDescription);

        public StartablePullingHook<T, TKeyProperty> OnAdded(Action<string, string, T> adder)
        {
            var result = Build();
            PullingSink.OnAdded = adder;

            return result;
        }

        public StartablePullingHook<T, TKeyProperty> OnUpdated(Action<string, string, T> updater)
        {
            var result = Build();
            PullingSink.OnUpdated = updater;

            return result;
        }

        public StartablePullingHook<T, TKeyProperty> OnRemoved(Action<string, string, T> remover)
        {
            var result = Build();
            PullingSink.OnRemoved = remover;

            return result;
        }
    }

    public class StartablePullingHook<T, TKeyProperty> : PullingHookWithSource<T, TKeyProperty>
    {
        private readonly IPullingHookManager<T, TKeyProperty> _manager;

        public StartablePullingHook(IPullingHookManager<T, TKeyProperty> manager, IPullingScheduler<T, TKeyProperty> scheduler)
        {
            _manager = manager;
            Scheduler = scheduler;
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
            Scheduler.Start(_manager);
            return new StoppablePullingHook<T, TKeyProperty>(Scheduler);
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
