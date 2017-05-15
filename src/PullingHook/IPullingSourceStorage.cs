using System.Collections.Generic;

namespace PullingHook
{
    public interface IPullingSourceStorage<T>
    {
        IEnumerable<HashedPair<T>> Retrieve(string key);
        IEnumerable<HashedPair<T>> Store(string key, IEnumerable<T> values);
    }
}
