using System;
using System.Collections.Generic;

namespace PullingHook
{
    public static class PullingSink
    {
        public static IPullingSink<T> Create<T>(string name, string description, Action<string, string, T> notify, Comparer<T> comparer = null) => new PullingSink<T>
        {
            Name = name,
            Description = description,
            Notify = notify,
            Comparer = comparer
        };
    }

    public class PullingSink<T> : IPullingSink<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Action<string, string, T> Notify { get; set; }
        public Comparer<T> Comparer { get; set; }
    }
}
