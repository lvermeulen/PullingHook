using FluentScheduler;

namespace PullingHook.Scheduler.Fluent
{
    public class FluentPullingHookScheduler<T, TKeyProperty> : IPullingScheduler<T, TKeyProperty>
    {
        private IPullingHookManager<T, TKeyProperty> _pullingHookManager;

        public void Start(IPullingHookManager<T, TKeyProperty> pullingHookManager)
        {
            _pullingHookManager = pullingHookManager;

            var registry = new Registry();
            foreach (var pullingConfiguration in _pullingHookManager.Configurations)
            {
                registry
                    .Schedule(() => _pullingHookManager.ScheduledAction(pullingConfiguration))
                    .ToRunNow()
                    .AndEvery(pullingConfiguration.Schedule.Interval);
            }

            JobManager.Start();
        }

        public void Stop()
        {
            JobManager.StopAndBlock();
        }
    }
}
