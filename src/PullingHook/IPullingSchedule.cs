using System;

namespace PullingHook
{
    public interface IPullingSchedule
    {
        TimeSpan Interval { get; }
    }
}
