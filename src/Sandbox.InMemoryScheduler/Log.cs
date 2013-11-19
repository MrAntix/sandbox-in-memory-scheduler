using System;

namespace Sandbox.InMemoryScheduler
{
    public static class Log
    {
        public static readonly Func<Delegate> ToDebug 
            = () => (f, a) => System.Diagnostics.Debug.WriteLine(string.Concat(DateTime.UtcNow.Millisecond, ": ", f), a);

        public static void Write(
            this Func<Delegate> log, Action<Delegate> getMessage)
        {
            if (log == null) return;

            getMessage(log());
        }

        public delegate void Delegate(string format, params object[] args);
    }
}