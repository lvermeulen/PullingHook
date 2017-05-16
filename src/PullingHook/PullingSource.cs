using System;
using System.Collections.Generic;

namespace PullingHook
{
    public class PullingSource<T> : IPullingSource<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<IEnumerable<T>> Pull { get; set; }
    }
}
