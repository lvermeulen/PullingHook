using System;
using Fluent = FluentScheduler;

namespace PullingHook.Scheduler.FluentScheduler
{
    public static class FluentSchedulerExtensions
    {
        private static int TimeSpanToSeconds(TimeSpan timespan) => 
            (int)Math.Ceiling(Math.Abs(timespan.TotalSeconds));

        public static Fluent.SecondUnit ToRunEvery(this Fluent.Schedule schedule, TimeSpan timespan) => 
            schedule.ToRunEvery(TimeSpanToSeconds(timespan)).Seconds();

        public static Fluent.SecondUnit AndEvery(this Fluent.SpecificTimeUnit specificTimeUnit, TimeSpan timespan) => 
            specificTimeUnit.AndEvery(TimeSpanToSeconds(timespan)).Seconds();
    }
}
