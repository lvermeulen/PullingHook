using System;
using System.Collections.Generic;
using Xunit;

namespace PullingHook.Tests
{
    public class TypeExtensionsShould
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(double))]
        public void GetSimpleTypeNames(Type type)
        {
            string result = type.GetAllTypeNames();
            Assert.Equal(type.Name, result);
        }

        [Theory]
        [InlineData(typeof(IEnumerable<string>), "IEnumerable`1,String")]
        [InlineData(typeof(KeyValuePair<string, IEnumerable<int>>), "KeyValuePair`2,String,IEnumerable`1,Int32")]
        public void GetComplexTypeNames(Type type, string expected)
        {
            string result = type.GetAllTypeNames();
            Assert.Equal(expected, result);
        }
    }
}
