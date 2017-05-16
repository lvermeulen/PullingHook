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
        public Func<T, TKeyProperty> KeyPropertySelector { get; set; }
        public IPullingSourceStorage<T> Storage { get; set; }
        public IPullingScheduler<T, TKeyProperty> Scheduler { get; set; }
        public TimeSpan Interval { get; set; }
        public IPullingSource<T> PullingSource { get; set; }
        public IPullingSink<T, TKeyProperty> PullingSink { get; set; }
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
        public PullingHookWithSource<T, TKeyProperty> When(TimeSpan interval, Func<IEnumerable<T>> puller) => 
            new PullingHookWithSource<T, TKeyProperty>
            {
                KeyPropertySelector = KeyPropertySelector,
                Storage = Storage,
                Scheduler = Scheduler,
                Interval = interval,
                PullingSource = PullingSourceFactory.Create("", "", puller) //TODO: source name & description
            };
    }

    public class PullingHookWithSource<T, TKeyProperty> : PullingHookBuilder<T, TKeyProperty>
    {
        public StartablePullingHook<T, TKeyProperty> Then(Action<string, string, UnitOfWork<T, TKeyProperty>.Results> pusher)
        {
            PullingSink = PullingSinkFactory.Create("", "", pusher); //TODO: sink name & description

            var manager = new PullingHookManager<T, TKeyProperty>(KeyPropertySelector) { Storage = Storage };
            manager.Add(new PullingConfiguration<T, TKeyProperty>(Interval, PullingSource, PullingSink));

            return new StartablePullingHook<T, TKeyProperty>(manager, KeyPropertySelector, Scheduler);
        }

        //TODO: OnAdded
        //TODO: OnUpdated
        //TODO: OnRemoved
    }

    public class StartablePullingHook<T, TKeyProperty>
    {
        private readonly IPullingHookManager<T, TKeyProperty> _manager;
        private readonly Func<T, TKeyProperty> _keyProperySelector;
        protected readonly IPullingScheduler<T, TKeyProperty> Scheduler;

        public StartablePullingHook(IPullingHookManager<T, TKeyProperty> manager, Func<T, TKeyProperty> keyProperySelector, IPullingScheduler<T, TKeyProperty> scheduler)
        {
            _manager = manager;
            _keyProperySelector = keyProperySelector;
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
            return new StoppablePullingHook<T, TKeyProperty>(_manager, _keyProperySelector, Scheduler);
        }
    }

    public class StoppablePullingHook<T, TKeyProperty> : StartablePullingHook<T, TKeyProperty>
    {
        public StoppablePullingHook(IPullingHookManager<T, TKeyProperty> manager, Func<T, TKeyProperty> keyProperySelector, IPullingScheduler<T, TKeyProperty> scheduler)
            : base(manager, keyProperySelector, scheduler)
        { }

        public void Stop()
        {
            Scheduler.Stop();
        }
    }
}
