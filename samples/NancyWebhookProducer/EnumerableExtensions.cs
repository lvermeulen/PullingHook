using System;
using System.Collections.Generic;
using System.Linq;

namespace NancyWebhookProducer
{
    public static class EnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> items, Random rand)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var list = items.ToList();
            return list.ElementAt(rand.Next(list.Count));
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random)
        {
            var list = items.ToList();

            int count = list.Count;
            while (count > 1)
            {
                int i = random.Next(count--);
                var temp = list[count];
                list[count] = list[i];
                list[i] = temp;
            }

            return list;
        }
    }
}
