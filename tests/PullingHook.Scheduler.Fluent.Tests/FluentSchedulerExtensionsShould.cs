using System;
using FluentScheduler;
using NSubstitute;
using Xunit;

namespace PullingHook.Scheduler.Fluent.Tests
{
    public class FluentSchedulerExtensionsShould
    {
        [Theory]
        [InlineData(3)]
        [InlineData(300)]
        [InlineData(3000)]
        public void ToRunEvery(int seconds)
        {
            var schedule = Substitute.For<Schedule>((Action)(() => { }));
            var interval = TimeSpan.FromSeconds(seconds);
            schedule.ToRunEvery(interval);

            schedule.Received().ToRunEvery((int)Math.Ceiling(Math.Abs(interval.TotalSeconds))).Seconds();
        }
    }
}
