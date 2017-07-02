using FluentScheduler;

namespace PullingHook.Scheduler.Fluent
{
    public class FluentPullingHookScheduler<T, TKeyProperty> : IPullingScheduler<T, TKeyProperty>
    {
        public void Start(IPullingHookManager<T, TKeyProperty> pullingHookManager)
        {
            var registry = new Registry();
            foreach (var pullingConfiguration in pullingHookManager.Configurations)
            {
                registry
                    .Schedule(() => pullingHookManager.ScheduledAction(pullingConfiguration))
                    .ToRunNow()
                    .AndEvery(pullingConfiguration.Schedule.Interval);
            }

            JobManager.Initialize(registry);
            JobManager.Start();
        }

        public void Stop()
        {
            JobManager.StopAndBlock();
        }
    }
}
