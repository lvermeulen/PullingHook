using System;
using System.Collections.Generic;

namespace PullingHook
{
    public static class PullingSourceFactory
    {
        public static IPullingSource<T> Create<T>(string name, string description, Func<IEnumerable<T>> pull) => new PullingSource<T>
        {
            Name = name,
            Description = description,
            Pull = pull
        };
    }
}
