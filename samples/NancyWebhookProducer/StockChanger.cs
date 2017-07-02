using System;
using System.Collections.Generic;
using System.Linq;
using Dispenser;
using Dispenser.Hasher.Sha1;

namespace NancyWebhookProducer
{
    public class StockChanger
    {
        private const int MINIMUM_STOCKITEMS = 5;

        private static readonly Random s_random = new Random();
        private static readonly Dispenser<StockItem, string> s_dispenser = new Dispenser<StockItem, string>();
        private static readonly IHasher s_hasher = new Sha1Hasher();

        private static readonly List<string> s_allSkus = new List<string>
        {
            "Bolt #6",
            "Bolt #8",
            "Bolt #10",
            "Bolt #12",
            "Bolt 1/4\"",
            "Bolt 5/16\"",
            "Bolt 3/8\"",
            "Bolt 7/16\"",
            "Bolt 1/2\"",
            "Bolt 9/16\"",
            "Bolt 5/8\"",
            "Bolt 3/4\"",
            "Bolt 7/8\"",
            "Bolt 1\"",
            "Bolt 1-1/8\"",
            "Bolt 1-1/4\""
        };

        public List<StockItem> CurrentStock { get; private set; }

        public StockChanger()
        {
            CurrentStock = s_allSkus
                .Shuffle(s_random)
                .Take(MINIMUM_STOCKITEMS)
                .Select(x => new StockItem(x, s_random.Next(10) + 1))
                .ToList();
        }

        public IEnumerable<StockItem> Next()
        {
            var newStock = CurrentStock
                .Select(x => new StockItem(x.Sku, x.Quantity))
                .ToList();

            var absentItems = s_allSkus
                .Except(newStock.Select(x => x.Sku))
                .ToList();

            // decrease stock levels
            foreach (var stockItem in newStock)
            {
                stockItem.Quantity--;
            }

            var stockItemsToRemove = newStock
                .Where(x => x.Quantity == 0)
                .ToList();

            // replace empty items with previously absent items
            foreach (var stockItem in stockItemsToRemove)
            {
                newStock.Remove(stockItem);

                string absentSku = absentItems.Random(s_random);
                absentItems.Remove(absentSku);
                newStock.Add(new StockItem(absentSku, s_random.Next(10) + 1));
            }

            // print changes
            var changes = s_dispenser.Dispense(newStock.Hash(s_hasher), CurrentStock.Hash(s_hasher), x => x.Sku);
            foreach (var inserted in changes.Inserts)
            {
                Console.WriteLine($"Stock changed - Item added: {inserted.Sku}");
            }
            foreach (var changed in changes.Updates)
            {
                Console.WriteLine($"Stock changed - Item changed: {changed.Sku}");
            }
            foreach (var deleted in changes.Deletes)
            {
                Console.WriteLine($"Stock changed - Item removed: {deleted.Sku}");
            }

            CurrentStock = newStock;
            return CurrentStock;
        }
    }
}
