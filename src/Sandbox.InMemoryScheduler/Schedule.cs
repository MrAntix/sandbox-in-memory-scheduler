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

        public Schedule At(int milliseconds, Action action, string label)
        {
            var _ = new Timer(
                o => Run(action, label), null,
                _startTime + milliseconds, Timeout.Infinite);

            _log.Write(m => m("action '{0}' at {1}", label, _startTime + milliseconds));

            return new Schedule(
                _startTime,
                Math.Max(_startTime + milliseconds, _endTime),
                _log);
        }

        void Run(Action action, string label)
        {
            _log.Write(m => m("running action '{0}'", label));

            try
            {
                action();

                _log.Write(m => m("completed action '{0}'", label));
            }
            catch (Exception ex)
            {
                _log.Write(m => m("error running action '{0}'\n{1}", label, ex));
            }
        }

        public Schedule Then(Action action, string label)
        {
            return new Schedule(
                _endTime, _endTime,
                _log)
                .At(0, action, label);
        }

        public Schedule ThenAt(int milliseconds, Action action, string label)
        {
            return Wait().At(milliseconds, action, label);
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