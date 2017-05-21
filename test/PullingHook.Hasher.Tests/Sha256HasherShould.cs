using PullingHook.Hasher.Sha256;
using PullingHook.Tests;
using Xunit;

namespace PullingHook.Hasher.Tests
{
    public class Sha256HasherShould
    {
        private readonly IHasher _hasher = new Sha256Hasher();

        [Fact]
        public void ComputeHash()
        {
            TypedValue<int> nullValue = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal("", _hasher.Hash(nullValue));

            var typedInt0 = new TypedValue<int>(0);
            Assert.Equal("0x5FECEB66FFC86F38D952786C6D696C79C2DBC239DD4E91B46729D73A27FB57E9", _hasher.Hash(typedInt0));

            var typedInt3 = new TypedValue<int>(3);
            Assert.Equal("0x4E07408562BEDB8B60CE05C1DECFE3AD16B72230967DE01F640B7E4729B49FCE", _hasher.Hash(typedInt3));
        }
    }
}
