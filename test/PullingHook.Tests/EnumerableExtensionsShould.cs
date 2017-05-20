using System.Collections.Generic;
using PullingHook.Hasher.Sha1;
using Xunit;

namespace PullingHook.Tests
{
    public class EnumerableExtensionsShould
    {
        private readonly IHasher _hasher;

        public EnumerableExtensionsShould()
        {
            _hasher = new Sha1Hasher();
        }

        [Fact]
        public void Hash()
        {
            var value1 = new TypedValue<int>(1);
            var value2 = new TypedValue<int>(2);
            var value3 = new TypedValue<int>(3);

            var hashedPair1 = new HashedPair<TypedValue<int>>(value1, _hasher);
            var hashedPair2 = new HashedPair<TypedValue<int>>(value2, _hasher);
            var hashedPair3 = new HashedPair<TypedValue<int>>(value3, _hasher);

            var items = new[] { value1, value2, value3 };
            var expectedHashedItems = new[]
            {
                new KeyValuePair<string, TypedValue<int>>(hashedPair1.HashValue, value1),
                new KeyValuePair<string, TypedValue<int>>(hashedPair2.HashValue, value2),
                new KeyValuePair<string, TypedValue<int>>(hashedPair3.HashValue, value3)
            };
            var hashedItems = items.Hash(_hasher);

            Assert.NotNull(hashedItems);
            Assert.Equal(expectedHashedItems, hashedItems);
        }
    }
}
