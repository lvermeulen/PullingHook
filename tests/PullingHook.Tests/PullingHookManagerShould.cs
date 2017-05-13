using System;
using System.Linq;
using PullingHook.Storage.Memory;
using Xunit;

namespace PullingHook.Tests
{
    public class PullingHookManagerShould
    {
        private class TypedValue<T>
        {
            public T Value { get; }

            public TypedValue(T t)
            {
                Value = t;
            }
        }

        [Fact]
        public void NotifyWhenSourceIsPulled()
        {
            string sourceName = "";
            string sourceDescription = "";
            var sourceValues = new UnitOfWorkResults<TypedValue<DateTimeOffset>>();

            var pullingSource = PullingSource.Create("Source", "Source description", 
                () => Enumerable.Repeat(new TypedValue<DateTimeOffset>(DateTimeOffset.UtcNow), 1));
            var pullingSink = PullingSink.Create<TypedValue<DateTimeOffset>> ("Sink", "Sink description",
                (name, description, values) =>
                {
                    sourceName = name;
                    sourceDescription = description;
                    sourceValues = values;
                });

            var manager = new PullingHookManager<TypedValue<DateTimeOffset>, DateTimeOffset>(x => x.Value)
            {
                Storage = new MemoryStorage<TypedValue<DateTimeOffset>>()
            };
            manager.Add(new PullingConfiguration<TypedValue<DateTimeOffset>> (TimeSpan.FromSeconds(3), pullingSource, pullingSink));

            manager.ScheduledAction(manager.Configurations.First());
            Assert.Equal("Source", sourceName);
            Assert.Equal("Source description", sourceDescription);
            Assert.True(sourceValues.Updates.All(x => x.Value > DateTimeOffset.MinValue)
                || sourceValues.Inserts.All(x => x.Value > DateTimeOffset.MinValue)
                || sourceValues.Deletes.All(x => x.Value > DateTimeOffset.MinValue));
        }

        [Fact]
        public void DetectChanges()
        {
            var sourceValues = new[] { new TypedValue<int>(1), new TypedValue<int>(2), new TypedValue<int>(3) };
            var sourceResults = new UnitOfWorkResults<TypedValue<int>>
            {
                Inserts = sourceValues
            };
            var newValues = new UnitOfWorkResults<TypedValue<int>>();

            bool isSinkNotifying = false;
            // ReSharper disable once AccessToModifiedClosure
            var pullingSource = PullingSource.Create("", "", () => sourceValues);
            var pullingSink = PullingSink.Create<TypedValue<int>>("", "", (name, description, values) =>
            {
                isSinkNotifying = true;
                newValues = values;
            });

            var manager = new PullingHookManager<TypedValue<int>, int>(x => x.Value)
            {
                Storage = new MemoryStorage<TypedValue<int>>()
            };
            manager.Add(new PullingConfiguration<TypedValue<int>>(TimeSpan.FromSeconds(3), pullingSource, pullingSink));

            // should have inserts
            manager.ScheduledAction(manager.Configurations.First());
            Assert.True(isSinkNotifying);
            Assert.Equal(sourceResults.Inserts, newValues.Inserts);
            Assert.Equal(sourceResults.Updates, newValues.Updates);
            Assert.Equal(sourceResults.Deletes, newValues.Deletes);

            // should not have changes
            isSinkNotifying = false;
            manager.ScheduledAction(manager.Configurations.First());
            Assert.True(isSinkNotifying);
            Assert.NotEqual(sourceResults.Inserts, newValues.Inserts);
            Assert.Equal(sourceResults.Updates, newValues.Updates);
            Assert.Equal(sourceResults.Deletes, newValues.Deletes);

            // should have 1 insert, 2 deletes
            sourceValues = new[] { new TypedValue<int>(1), new TypedValue<int>(4) };
            isSinkNotifying = false;
            manager.ScheduledAction(manager.Configurations.First());
            Assert.True(isSinkNotifying);
            Assert.Equal(1, newValues.Inserts.Count());
            Assert.Equal(0, newValues.Updates.Count());
            Assert.Equal(2, newValues.Deletes.Count());
        }
    }
}
