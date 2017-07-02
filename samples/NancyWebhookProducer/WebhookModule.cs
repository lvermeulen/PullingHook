using System;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;

namespace NancyWebhookProducer
{
    public class WebhookModule : NancyModule
    {
        public WebhookModule()
        {
            Post("/subscribe/{topic}", parameters =>
            {
                string topic = parameters.topic;
                string subscriberUrl = RequestStream.FromStream(Request.Body).AsString();
                Console.WriteLine($"Adding subscription to topic {topic}: {subscriberUrl}");

                Subscriptions.Default.AddSubscription(topic, subscriberUrl);

                return Negotiate.WithStatusCode(HttpStatusCode.OK);
            });

            Post("/unsubscribe/{topic}", parameters =>
            {
                string topic = parameters.topic;
                string subscriberUrl = (Request.Body as RequestStream).AsString();
                Console.WriteLine($"Removing subscription to topic {topic}: {subscriberUrl}");

                Subscriptions.Default.RemoveSubscription(topic, subscriberUrl);

                return Negotiate.WithStatusCode(HttpStatusCode.OK);
            });
        }
    }
}
