namespace PullingHook.Tests
{
    public class TypedValue<T>
    {
        public T Value { get; }

        public TypedValue(T t)
        {
            Value = t;
        }
    }
}
