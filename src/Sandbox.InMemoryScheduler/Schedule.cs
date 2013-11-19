using System;
using System.Threading;

namespace Sandbox.InMemoryScheduler
{
    public class Schedule
    {
        readonly int _startTime;
        readonly int _endTime;
        readonly Func<Log.Delegate> _log;

        Schedule(
            int startTime, int endTime,
            Func<Log.Delegate> log)
        {
            _startTime = startTime;
            _endTime = endTime;
            _log = log;
        }

        public static Schedule Create(Func<Log.Delegate> log)
        {
            return new Schedule(0, 0, log);
        }

        public Schedule At(int milliseconds, Action action)
        {
            var _ = new Timer(
                o => Run(action), null,
                _startTime + milliseconds, Timeout.Infinite);

            _log.Debug(m => m("action {0} at {1}", action, _startTime + milliseconds));

            return new Schedule(
                _startTime,
                Math.Max(_startTime + milliseconds, _endTime),
                _log);
        }

        void Run(Action action)
        {
            _log.Debug(m => m("running action {0}", action));

            action();
        }

        public Schedule Then(Action action)
        {
            return new Schedule(
                _endTime, _endTime,
                _log)
                .At(0, action);
        }

        public Schedule ThenAt(int milliseconds, Action action)
        {
            return Wait().At(milliseconds, action);
        }

        public Schedule Wait()
        {
            return new Schedule(
                _endTime, _endTime,
                _log);
        }

        public Schedule Wait(int milliseconds)
        {
            return new Schedule(
                _endTime + milliseconds, _endTime + milliseconds,
                _log);
        }
    }
}