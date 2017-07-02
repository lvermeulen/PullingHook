using System;
using Dispenser.Hasher.Sha1;
using Flurl;
using Flurl.Http;
using Nancy.Hosting.Self;
using PullingHook.Fluent;
using PullingHook.Scheduler.Fluent;
using PullingHook.Storage.Memory;

namespace NancyWebhookProducer
{
    public static class Program
    {
        private static void SendWebhook(string topic, StockItem stockItem)
        {
            // send webhook to each subscriber
            foreach (string subscriber in Subscriptions.Default.FindSubscriptions(topic))
            {
                Console.WriteLine($"PullingHook sending webhook: topic {topic}, item {stockItem.Sku} with quantity {stockItem.Quantity}");

                subscriber
                    .AppendPathSegment(topic)
                    .WithHeader("Accept", "application/json")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(stockItem)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public static void Main()
        {
            const int PORT = 8898;

            var uri = new Uri($"http://localhost:{PORT}/");
            var hostConfiguration = new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };
            using (var nancyHost = new NancyHost(hostConfiguration, uri))
            {
                nancyHost.Start();

                var stockChanger = new StockChanger();

                var pullingHook = PullingHook<StockItem, string>
                    .WithKeyProperty(x => x.Sku)
                    .WithStorage(new MemoryStorage<StockItem>(new Sha1Hasher()))
                    .WithScheduler(new FluentPullingHookScheduler<StockItem, string>())
                    .When(TimeSpan.FromSeconds(3), () => stockChanger.Next(), "Stock")
                    .Then((name, description, changes) => Console.WriteLine("PullingHook: found some changes"))
                    .OnAdded((name, description, item) => SendWebhook("added", item))
                    .OnChanged((name, description, item) => SendWebhook("changed", item))
                    .OnRemoved((name, description, item) => SendWebhook("removed", item))
                    .Build()
                    .Start();

                Console.WriteLine($"Now listening on {uri}. Press any key to stop");
                Console.ReadKey();

                pullingHook.Stop();

                //var dispenser = new Dispenser.Dispenser<StockItem, string>();
                //var hasher = new Sha1Hasher();
                //var stock = stockChanger.CurrentStock;
                //for (int i = 0; i < 10; i++)
                //{
                //    var newStock = System.Linq.Enumerable.ToList(stockChanger.Next());
                //    var diff = dispenser.Dispense(Dispenser.EnumerableExtensions.Hash(newStock, hasher), Dispenser.EnumerableExtensions.Hash(stock, hasher), x => x.Sku);
                //    stock = newStock;
                //}
            }
        }
    }
}
