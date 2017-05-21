using PullingHook.Hasher.Sha1;
using PullingHook.Tests;
using Xunit;

namespace PullingHook.Hasher.Tests
{
    public class Sha1HasherShould
    {
        private readonly IHasher _hasher = new Sha1Hasher();

        [Theory]
        [InlineData()]
        public void ComputeHash()
        {
            TypedValue<int> nullValue = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal("", _hasher.Hash(nullValue));

            var typedInt0 = new TypedValue<int>(0);
            Assert.Equal("0xB6589FC6AB0DC82CF12099D1C2D40AB994E8410C", _hasher.Hash(typedInt0));

            var typedInt3 = new TypedValue<int>(3);
            Assert.Equal("0x77DE68DAECD823BABBB58EDB1C8E14D7106E83BB", _hasher.Hash(typedInt3));
        }
    }
}
