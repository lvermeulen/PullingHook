﻿using System;
using PullingHook.Hasher.Sha1;
using PullingHook.Scheduler.Fluent;
using PullingHook.Storage.Memory;
using Xunit;

namespace PullingHook.Fluent.Tests
{
    public class PullingHookShould
    {
        [Fact]
        public void BeFluent()
        {
            var pullingHook = PullingHook<decimal, decimal>
                .WithKeyProperty(x => x)
                .WithStorage(new MemoryStorage<decimal>(new Sha1Hasher()))
                .WithScheduler(new FluentPullingHookScheduler<decimal, decimal>())
                .When(TimeSpan.FromSeconds(500), () => new[] {1M, 2M, 3M})
                .Then((name, description, changes) => { })
                .OnAdded((name, description, item) => { })
                .OnUpdated((name, description, item) => { })
                .OnRemoved((name, description, item) => { });

            Assert.IsType<StartablePullingHook<decimal, decimal>>(pullingHook);

            var stoppablePullingHook = pullingHook.Start();
            Assert.IsType<StartablePullingHook<decimal, decimal>>(pullingHook);

            stoppablePullingHook.Stop();
        }
    }
}