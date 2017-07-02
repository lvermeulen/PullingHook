using System.Collections.Generic;

namespace NancyWebhookProducer
{
    public class Subscriptions
    {
        private static readonly Dictionary<string, HashSet<string>> s_subscriptions = new Dictionary<string, HashSet<string>>();
        private static Subscriptions s_default;

        public static Subscriptions Default => s_default ?? (s_default = new Subscriptions());

        public IEnumerable<string> FindSubscriptions(string topic)
        {
            var results = new List<string>();

            HashSet<string> subscribers;
            if (s_subscriptions.TryGetValue(topic, out subscribers))
            {
                results.AddRange(subscribers);
            }

            if (s_subscriptions.TryGetValue("*", out subscribers))
            {
                results.AddRange(subscribers);
            }

            return results;
        }

        public void AddSubscription(string topic, string subscriberUrl)
        {
            HashSet<string> topicSubscribers;
            if (!s_subscriptions.TryGetValue(topic, out topicSubscribers))
            {
                topicSubscribers = new HashSet<string>();
            }
            topicSubscribers.Add(subscriberUrl);
            s_subscriptions[topic] = topicSubscribers;
        }

        public void RemoveSubscription(string topic, string subscriberUrl)
        {
            HashSet<string> topicSubscribers;
            if (s_subscriptions.TryGetValue(topic, out topicSubscribers))
            {
                topicSubscribers.Remove(subscriberUrl);
            }
            s_subscriptions[topic] = topicSubscribers;
        }
    }
}
