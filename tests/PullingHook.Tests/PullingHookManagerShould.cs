using System;
using System.Linq;
using Xunit;

namespace PullingHook.Tests
{
    public class PullingHookManagerShould
    {
        [Fact]
        public void NotifyWhenSourceIsPulled()
        {
            string sourceName = "";
            string sourceDescription = "";
            var sourceValue = DateTimeOffset.MinValue;

            var pullingSource = PullingSource.Create("Source", "Source description", 
                () => DateTimeOffset.UtcNow);
            var pullingSink = PullingSink.Create<DateTimeOffset>("Sink", "Sink description",
                (name, description, value) =>
                {
                    sourceName = name;
                    sourceDescription = description;
                    sourceValue = value;
                });

            var manager = new PullingHookManager<DateTimeOffset>();
            manager.Add(new PullingConfiguration<DateTimeOffset>(TimeSpan.FromSeconds(3), pullingSource, pullingSink));

            manager.ScheduledAction(manager.Configurations.First());
            Assert.Equal("Source", sourceName);
            Assert.Equal("Source description", sourceDescription);
            Assert.True(sourceValue > DateTimeOffset.MinValue);
        }

        [Fact]
        public void OnlyNotifyWhenSourceValueIsDifferent()
        {
            var sourceValues = new[] { 1, 2, 3 };
            int[] newValues = { };

            var pullingSource = PullingSource.Create("", "", () => sourceValues);
            var pullingSink = PullingSink.Create<int[]>("", "", (name, description, value) => newValues = value);

            var manager = new PullingHookManager<int[]>();
            manager.Add(new PullingConfiguration<int[]>(TimeSpan.FromSeconds(3), pullingSource, pullingSink));

            manager.ScheduledAction(manager.Configurations.First());
            Assert.Equal(sourceValues, newValues);

            manager.ScheduledAction(manager.Configurations.First());
            Assert.NotEqual(sourceValues, newValues);
        }
    }
}
