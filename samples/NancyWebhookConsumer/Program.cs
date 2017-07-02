using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Nancy.Hosting.Self;

namespace NancyWebhookConsumer
{
    public static class Program
    {
        private static void HandleSubscription(Uri baseUri, bool subscribe)
        {
            string notificationUrl = new Uri(baseUri, "notify").ToString();
            string serverUrl = subscribe
                ? "http://localhost:8898/subscribe/*"
                : "http://localhost:8898/unsubscribe/*";

            try
            {
                serverUrl
                    .WithHeader("Accept", "application/json")
                    .PostAsync(new StringContent(notificationUrl))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(1);
            }
        }

        private static void Subscribe(Uri baseUri)
        {
            HandleSubscription(baseUri, true);
        }

        private static void Unsubscribe(Uri baseUri)
        {
            HandleSubscription(baseUri, false);
        }

        public static void Main()
        {
            const int PORT = 8899;

            // wait for producer to come up
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            var uri = new Uri($"http://localhost:{PORT}/");
            var hostConfiguration = new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };
            using (var nancyHost = new NancyHost(hostConfiguration, uri))
            {
                nancyHost.Start();

                Console.WriteLine("Subscribing to all topics");
                Subscribe(uri);

                Console.WriteLine($"Now listening on {uri}/price. Press any key to stop");
                Console.ReadKey();

                Console.WriteLine("Unsubscribing from all topics");
                Unsubscribe(uri);
            }
        }
    }
}
