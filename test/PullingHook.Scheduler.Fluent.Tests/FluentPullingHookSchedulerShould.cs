using NSubstitute;
using Xunit;

namespace PullingHook.Scheduler.Fluent.Tests
{
    public class FluentPullingHookSchedulerShould
    {
        [Fact]
        public void Start()
        {
            var manager = Substitute.For<IPullingHookManager<int, int>>();
            var scheduler = new FluentPullingHookScheduler<int, int>();
            scheduler.Start(manager);

            manager.ScheduledAction.ReceivedWithAnyArgs();
        }
    }
}
