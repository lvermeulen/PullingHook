using System;
using FluentScheduler;

namespace PullingHook.Scheduler.Fluent
{
    public static class FluentSchedulerExtensions
    {
        private static int TimeSpanToSeconds(TimeSpan timespan) => 
            (int)Math.Ceiling(Math.Abs(timespan.TotalSeconds));

        public static SecondUnit ToRunEvery(this Schedule schedule, TimeSpan timespan) => 
            schedule.ToRunEvery(TimeSpanToSeconds(timespan)).Seconds();

        public static SecondUnit AndEvery(this SpecificTimeUnit specificTimeUnit, TimeSpan timespan) => 
            specificTimeUnit.AndEvery(TimeSpanToSeconds(timespan)).Seconds();
    }
}
