using System;
using Dispenser;

namespace PullingHook
{
    public class PullingSink<T, TKeyProperty> : IPullingSink<T, TKeyProperty>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Action<string, string, Dispenser<T, TKeyProperty>.Results> Notify { get; set; }
        public Action<string, string, T> OnAdded { get; set; }
        public Action<string, string, T> OnUpdated { get; set; }
        public Action<string, string, T> OnRemoved { get; set; }
    }
}
