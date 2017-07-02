using System;
using Dispenser;

namespace PullingHook
{
    public static class PullingSinkFactory
    {
        public static IPullingSink<T, TKeyProperty> Create<T, TKeyProperty>(string name, string description,
            Action<string, string, Dispenser<T, TKeyProperty>.Results> notify = null) => new PullingSink<T, TKeyProperty>
        {
            Name = name,
            Description = description,
            Notify = notify
        };
    }

}
