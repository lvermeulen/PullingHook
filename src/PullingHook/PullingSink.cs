using System;

namespace PullingHook
{
    public static class PullingSink
    {
        public static IPullingSink<T, TKeyProperty> Create<T, TKeyProperty>(string name, string description, 
            Action<string, string, UnitOfWork<T, TKeyProperty>.Results> notify = null) => new PullingSink<T, TKeyProperty>
        {
            Name = name,
            Description = description,
            Notify = notify
        };
    }

    public class PullingSink<T, TKeyProperty> : IPullingSink<T, TKeyProperty>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Action<string, string, UnitOfWork<T, TKeyProperty>.Results> Notify { get; set; }
        public Action<string, string, T> OnAdded { get; set; }
        public Action<string, string, T> OnUpdated { get; set; }
        public Action<string, string, T> OnRemoved { get; set; }
    }
}
