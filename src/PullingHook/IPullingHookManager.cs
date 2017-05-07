using System;
using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingHookManager<T>
    {
        Func<T, T> ResultHandler { get; }
        Action<IPullingConfiguration<T>> ScheduledAction { get; }
        IEnumerable<IPullingConfiguration<T>> Configurations { get; }
        IPullingConfiguration<T> Add(IPullingConfiguration<T> configuration);
        void Remove(IPullingConfiguration<T> configuration);
    }
}