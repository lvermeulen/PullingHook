using Fluent = FluentScheduler;

namespace PullingHook.Scheduler.FluentScheduler
{
    public class FluentPullingHookScheduler<T> : IPullingScheduler<T>
    {
        private IPullingHookManager<T> _pullingHookManager;

        public void Start(IPullingHookManager<T> pullingHookManager)
        {
            _pullingHookManager = pullingHookManager;

            var registry = new Fluent.Registry();
            foreach (var pullingConfiguration in _pullingHookManager.Configurations)
            {
                registry
                    .Schedule(() => _pullingHookManager.ScheduledAction(pullingConfiguration))
                    .ToRunNow()
                    .AndEvery(pullingConfiguration.Schedule.Interval);
            }

            Fluent.JobManager.Start();
        }

        public void Stop()
        {
            Fluent.JobManager.StopAndBlock();
        }
    }
}
