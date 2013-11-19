using System;
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
            // Func<Log.Delegate> log = null; // can pass a null log

            var start = DateTime.UtcNow;
            var stop = DateTime.MinValue;

            var reset = new AutoResetEvent(false);

            log.Write(m => m("create shedule"));

            Schedule.Create(log)
                    .At(200, () => stop = DateTime.UtcNow, "set stop time")
                    .At(300, () => { throw new Exception("Boo!"); }, "throw exception")
                    .ThenAt(100, () => reset.Set(), "stop waiting");

            log.Write(m => m("waiting"));
            reset.WaitOne();

            log.Write(m => m("start {0}", start.Millisecond));
            log.Write(m => m("stop {0}", stop.Millisecond));

            Assert.True(stop > start);
        }
    }
}