using System;

namespace PullingHook
{
    public static class PullingSink
    {
        public static IPullingSink<T> Create<T>(string name, string description, Action<string, string, UnitOfWorkResults<T>> notify = null) => new PullingSink<T>
        {
            Name = name,
            Description = description,
            Notify = notify
        };
    }

    public class PullingSink<T> : IPullingSink<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Action<string, string, UnitOfWorkResults<T>> Notify { get; set; }
        public Action<string, string, T> OnAdded { get; set; }
        public Action<string, string, T> OnUpdated { get; set; }
        public Action<string, string, T> OnRemoved { get; set; }
    }
}
