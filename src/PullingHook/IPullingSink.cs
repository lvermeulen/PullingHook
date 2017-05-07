using System;
using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingSink<T>
    {
        string Name { get; }
        string Description { get; }
        Action<string, string, T> Notify { get; }
        Comparer<T> Comparer { get; }
    }
}
