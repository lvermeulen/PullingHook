using System;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;

namespace NancyWebhookConsumer
{
    public class NotificationModule : NancyModule
    {
        public NotificationModule()
        {
            Post("/notify/{topic}", parameters =>
            {
                string topic = parameters.topic;
                string data = RequestStream.FromStream(Request.Body).AsString();

                Console.WriteLine($"Incoming webhook: topic {topic}, data: {data}");
                return Negotiate.WithStatusCode(HttpStatusCode.OK);
            });
        }
    }
}
