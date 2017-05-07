using System;

namespace PullingHook
{
    public interface IPullingSource<out T>
    {
        string Name { get; }
        string Description { get; }
        Func<T> Pull { get; }
    }
}
