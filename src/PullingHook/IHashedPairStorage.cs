using System.Collections.Generic;
using Dispenser;

namespace PullingHook
{
    public interface IHashedPairStorage<T>
    {
        IEnumerable<HashedPair<T>> Retrieve(string key);
        IEnumerable<HashedPair<T>> Store(string key, IEnumerable<T> values);
    }
}
