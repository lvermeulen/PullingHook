using System;
using System.Collections.Generic;

namespace PullingHook
{
    public class KeyComparer<T, TKeyProperty> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, TKeyProperty> _keyPropertySelector;

        public KeyComparer(Func<T, TKeyProperty> keyPropertySelector)
        {
            _keyPropertySelector = keyPropertySelector;
        }

        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xKeyValue = _keyPropertySelector(x);
            var yKeyValue = _keyPropertySelector(y);

            if (EqualityComparer<TKeyProperty>.Default.Equals(xKeyValue, default(TKeyProperty)) 
                && EqualityComparer<TKeyProperty>.Default.Equals(yKeyValue, default(TKeyProperty)))
            {
                return true;
            }

            if (EqualityComparer<TKeyProperty>.Default.Equals(xKeyValue, default(TKeyProperty)) 
                || EqualityComparer<TKeyProperty>.Default.Equals(yKeyValue, default(TKeyProperty)))
            {
                return false;
            }

            return xKeyValue.Equals(yKeyValue);
        }

        public int GetHashCode(T obj) => obj == null || EqualityComparer<TKeyProperty>.Default.Equals(_keyPropertySelector(obj), default(TKeyProperty))
            ? base.GetHashCode()
            : _keyPropertySelector(obj).GetHashCode();
    }
}
