﻿using System;
using System.Linq;
using Dispenser;
using Dispenser.Hasher.Sha1;
using NSubstitute;
using PullingHook.Storage.Memory;
using Xunit;

namespace PullingHook.Tests
{
    public class PullingHookManagerShould
    {
        [Fact]
        public void NotifyWhenSourceIsPulled()
        {
            const string SOURCE_NAME = "Source";
            const string SOURCE_DESCRIPTION = "Source description";
            const string SINK_NAME = "Sink";
            const string SINK_DESCRIPTION = "Sink description";

            string sourceName = "";
            string sourceDescription = "";
            var sourceValues = new Dispenser<TypedValue<DateTimeOffset>, DateTimeOffset>.Results();

            var pullingSource = PullingSourceFactory.Create(SOURCE_NAME, SOURCE_DESCRIPTION, 
                () => Enumerable.Repeat(new TypedValue<DateTimeOffset>(DateTimeOffset.UtcNow), 1));
            var pullingSink = PullingSinkFactory.Create<TypedValue<DateTimeOffset>, DateTimeOffset> (SINK_NAME, SINK_DESCRIPTION,
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

            var firstConfiguration = manager.Configurations.First();
            Assert.Equal(SINK_NAME, firstConfiguration.Sink.Name);
            Assert.Equal(SINK_DESCRIPTION, firstConfiguration.Sink.Description);

            manager.ScheduledAction(firstConfiguration);
            Assert.Equal(SOURCE_NAME, sourceName);
            Assert.Equal(SOURCE_DESCRIPTION, sourceDescription);
            Assert.True(sourceValues.Updates.All(x => x.Value > DateTimeOffset.MinValue)
                || sourceValues.Inserts.All(x => x.Value > DateTimeOffset.MinValue)
                || sourceValues.Deletes.All(x => x.Value > DateTimeOffset.MinValue));
        }

        [Fact]
        public void DetectChanges()
        {
            var sourceValues = new[] { new TypedValue<int>(1), new TypedValue<int>(2), new TypedValue<int>(3) };
            var sourceResults = new Dispenser<TypedValue<int>, int>.Results
            {
                Inserts = sourceValues
            };
            var newValues = new Dispenser<TypedValue<int>, int>.Results();

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

            // should have interval
            Assert.Equal(TimeSpan.FromSeconds(3), manager.Configurations.FirstOrDefault()?.Schedule.Interval);

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

        [Fact]
        public void AddAndRemoveConfigurations()
        {
            var configuration = Substitute.For<IPullingConfiguration<TypedValue<int>, int>>();
            var manager = new PullingHookManager<TypedValue<int>, int>(x => x.Value);

            manager.Add(configuration);
            Assert.Equal(1, manager.Configurations.Count());

            manager.Remove(configuration);
            Assert.Equal(0, manager.Configurations.Count());
        }
    }
}
