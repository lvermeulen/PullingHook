using System;
using Dispenser;

namespace PullingHook
{
    public interface IPullingSink<T, TKeyProperty>
    {
        string Name { get; }
        string Description { get; }
        Action<string, string, Dispenser<T, TKeyProperty>.Results> Notify { get; }
        Action<string, string, T> OnAdded { get; set; }
        Action<string, string, T> OnUpdated { get; set; }
        Action<string, string, T> OnRemoved { get; set; }
    }
}
