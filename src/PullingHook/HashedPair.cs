namespace PullingHook
{
    public class HashedPair<T>
    {
        public HashedPair(T t)
        {
            Value = t;
            HashValue = Value.Hash();
        }

        public string HashValue { get; }
        public T Value { get; }
    }
}
