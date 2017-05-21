using System;
using System.Linq;
using System.Reflection;

namespace PullingHook
{
    public static class TypeExtensions
    {
        public static string GetAllTypeNames(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return string.Join(",", Enumerable.Repeat(type.Name, 1).Union(type.GetTypeInfo().GenericTypeArguments.Select(GetAllTypeNames)));
            }

            return type.Name;
        }
    }
}
