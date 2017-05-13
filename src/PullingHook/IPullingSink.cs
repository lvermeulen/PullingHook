using System;

namespace PullingHook
{
    public interface IPullingSink<T>
    {
        string Name { get; }
        string Description { get; }
        Action<string, string, UnitOfWorkResults<T>> Notify { get; }
        Action<string, string, T> OnAdded { get; }
        Action<string, string, T> OnUpdated { get; }
        Action<string, string, T> OnRemoved { get; }
    }
}
