using System;
using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingHookManager<T, TKeyProperty>
    {
        IPullingSourceStorage<T> Storage { get; }
        IHasher Hasher { get; }
        Action<IPullingConfiguration<T, TKeyProperty>> ScheduledAction { get; }
        IEnumerable<IPullingConfiguration<T, TKeyProperty>> Configurations { get; }
        IPullingConfiguration<T, TKeyProperty> Add(IPullingConfiguration<T, TKeyProperty> configuration);
        void Remove(IPullingConfiguration<T, TKeyProperty> configuration);
    }
}