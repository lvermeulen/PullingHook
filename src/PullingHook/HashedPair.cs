namespace PullingHook
{
    public class HashedPair<T>
    {
        public HashedPair(T t, IHasher hasher)
        {
            Value = t;
            HashValue = hasher?.Hash(Value);
        }

        public string HashValue { get; }
        public T Value { get; }
    }
}
