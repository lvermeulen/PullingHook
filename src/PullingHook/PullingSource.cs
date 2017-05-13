using System;
using System.Collections.Generic;

namespace PullingHook
{
    public static class PullingSource
    {
        public static IPullingSource<T> Create<T>(string name, string description, Func<IEnumerable<T>> pull) => new PullingSource<T>
        {
            Name = name,
            Description = description,
            Pull = pull
        };
    }

    public class PullingSource<T> : IPullingSource<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<IEnumerable<T>> Pull { get; set; }
    }
}
