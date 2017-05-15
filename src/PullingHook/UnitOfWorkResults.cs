using System.Collections.Generic;
using System.Linq;

namespace PullingHook
{
    public class UnitOfWorkResults<T>
    {
        public IEnumerable<T> Inserts { get; set; } = Enumerable.Empty<T>(); 
        public IEnumerable<T> Updates { get; set; } = Enumerable.Empty<T>();
        public IEnumerable<T> Deletes { get; set; } = Enumerable.Empty<T>();

        public bool HasChanges => Inserts.Any() || Updates.Any() || Deletes.Any();
    }
}
