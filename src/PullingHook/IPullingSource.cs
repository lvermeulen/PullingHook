using System;
using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingSource<out T>
    {
        string Name { get; }
        string Description { get; }
        Func<IEnumerable<T>> Pull { get; }
    }
}
