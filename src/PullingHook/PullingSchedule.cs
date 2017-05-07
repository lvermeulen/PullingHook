using System;

namespace PullingHook
{
    public class PullingSchedule : IPullingSchedule
    {
        public TimeSpan Interval { get; set; }
    }
}