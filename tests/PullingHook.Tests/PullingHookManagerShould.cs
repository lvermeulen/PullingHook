using System;
using System.Linq;
using PullingHook.Hasher.Sha1;
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
            var sourceValues = new UnitOfWork<TypedValue<DateTimeOffset>, DateTimeOffset>.Results();

            var pullingSource = PullingSourceFactory.Create("Source", "Source description", 
                () => Enumerable.Repeat(new TypedValue<DateTimeOffset>(DateTimeOffset.UtcNow), 1));
            var pullingSink = PullingSinkFactory.Create<TypedValue<DateTimeOffset>, DateTimeOffset> ("Sink", "Sink description",
                (name, description, values) =>
                {
                    sourceName = name;
                    sourceDescription = description;
                    sourceValues = values;
                });

            var manager = new PullingHookManager<TypedValue<DateTimeOffset>, DateTimeOffset>(x => x.Value)
            {
                Storage = new MemoryStorage<TypedValue<DateTimeOffset>>(new Sha1Hasher())
            };
            manager.Add(new PullingConfiguration<TypedValue<DateTimeOffset>, DateTimeOffset> (TimeSpan.FromSeconds(3), pullingSource, pullingSink));

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
            var sourceResults = new UnitOfWork<TypedValue<int>, int>.Results
            {
                Inserts = sourceValues
            };
            var newValues = new UnitOfWork<TypedValue<int>, int>.Results();

            bool isSinkNotifying = false;
            // ReSharper disable once AccessToModifiedClosure
            var pullingSource = PullingSourceFactory.Create("", "", () => sourceValues);
            var pullingSink = PullingSinkFactory.Create<TypedValue<int>, int>("", "", (name, description, values) =>
            {
                isSinkNotifying = true;
                newValues = values;
            });

            var manager = new PullingHookManager<TypedValue<int>, int>(x => x.Value)
            {
                Storage = new MemoryStorage<TypedValue<int>>(new Sha1Hasher())
            };
            manager.Add(new PullingConfiguration<TypedValue<int>, int>(TimeSpan.FromSeconds(3), pullingSource, pullingSink));

            // should have inserts
            manager.ScheduledAction(manager.Configurations.First());
            Assert.True(isSinkNotifying);
            Assert.Equal(sourceResults.Inserts, newValues.Inserts);
            Assert.Equal(sourceResults.Updates, newValues.Updates);
            Assert.Equal(sourceResults.Deletes, newValues.Deletes);

            // should not have changes
            isSinkNotifying = false;
            manager.ScheduledAction(manager.Configurations.First());
            Assert.False(isSinkNotifying);

            // should have 1 insert, 2 deletes
            int numInserts = 0;
            int numUpdates = 0;
            int numDeletes = 0;
            pullingSink.OnAdded = (source, description, item) => numInserts++;
            pullingSink.OnUpdated = (source, description, item) => numUpdates++;
            pullingSink.OnRemoved = (source, description, item) => numDeletes++;
            sourceValues = new[] { new TypedValue<int>(1), new TypedValue<int>(4) };
            isSinkNotifying = false;
            manager.ScheduledAction(manager.Configurations.First());
            Assert.True(isSinkNotifying);
            Assert.Equal(1, numInserts);
            Assert.Equal(0, numUpdates);
            Assert.Equal(2, numDeletes);
        }
    }
}
