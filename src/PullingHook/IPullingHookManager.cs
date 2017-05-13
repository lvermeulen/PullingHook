using System;
using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingHookManager<T>
    {
        IPullingSourceStorage<T> Storage { get; }
        Action<IPullingConfiguration<T>> ScheduledAction { get; }
        IEnumerable<IPullingConfiguration<T>> Configurations { get; }
        IPullingConfiguration<T> Add(IPullingConfiguration<T> configuration);
        void Remove(IPullingConfiguration<T> configuration);
    }
}