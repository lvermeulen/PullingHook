using System;
using System.Linq;

namespace PullingHook
{
    public static class TypeExtensions
    {
        public static string GetAllTypeNames(this Type type)
        {
            if (type.IsGenericType)
            {
                return string.Join(",", Enumerable.Repeat(type.Name, 1).Union(type.GetGenericArguments().Select(GetAllTypeNames)));
            }

            return type.Name;
        }
    }
}
