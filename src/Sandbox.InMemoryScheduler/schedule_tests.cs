using System;
using System.Diagnostics;
using System.Threading;

using Xunit;

namespace Sandbox.InMemoryScheduler
{
    public class schedule_tests
    {
        [Fact]
        public void run_action_in()
        {
            var log = Log.ToDebug;

            var start = DateTime.UtcNow;
            var stop = DateTime.MinValue;

            var reset = new AutoResetEvent(false);

            log.Debug(m=>m("create shedule"));

            Schedule.Create(Log.ToDebug)
                    .At(200, () => stop = DateTime.UtcNow)
                    .ThenAt(100,() => reset.Set());

            log.Debug(m => m("waiting"));
            reset.WaitOne();

            log.Debug(m => m("start {0}", start.Millisecond));
            log.Debug(m => m("stop {0}", stop.Millisecond));

            Assert.True(stop > start);
        }
    }
}