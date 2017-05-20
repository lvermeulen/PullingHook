using Xunit;

namespace PullingHook.Tests
{
    public class KeyComparerShould
    {
        [Fact]
        public void BeEqualWhenBothAreDefaultValue()
        {
            var comparer = new KeyComparer<TypedValue<int>, int>(x => x.Value);
            TypedValue<int> t1 = null;
            TypedValue<int> t2 = null;
            Assert.True(comparer.Equals(t1, t2));
        }

        [Fact]
        public void NotBeEqualWhenOneIsDefaultValue()
        {
            var comparer = new KeyComparer<TypedValue<int>, int>(x => x.Value);
            TypedValue<int> t1 = new TypedValue<int>(1);
            TypedValue<int> t2 = null;
            Assert.False(comparer.Equals(t1, t2));
            Assert.False(comparer.Equals(t2, t1));
        }

        [Fact]
        public void BeEqualWhenBothKeyPropertiesAreDefaultValue()
        {
            var comparer = new KeyComparer<TypedValue<object>, object>(x => x.Value);
            TypedValue<object> t1 = new TypedValue<object>(null);
            TypedValue<object> t2 = new TypedValue<object>(null);
            Assert.True(comparer.Equals(t1, t2));
        }

        [Fact]
        public void NotBeEqualWhenOneKeyPropertyIsDefaultValue()
        {
            var comparer = new KeyComparer<TypedValue<object>, object>(x => x.Value);
            TypedValue<object> t1 = new TypedValue<object>(new object());
            TypedValue<object> t2 = new TypedValue<object>(null);
            Assert.False(comparer.Equals(t1, t2));
            Assert.False(comparer.Equals(t2, t1));
        }
    }
}
