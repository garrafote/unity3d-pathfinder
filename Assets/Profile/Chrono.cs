using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Profile
{
    public static class Chrono
    {
        public delegate void ChronoEventHandler(string msg, TimeSpan time);

        public static event ChronoEventHandler TimeRecorded;

        static Chrono()
        {
            TimeRecorded += (m, t) => UnityEngine.Debug.Log(string.Format("[Chrono] {0} - {1:0.0000}ms", m, t.TotalMilliseconds));
        }

        public static void Run(string msg, Action action)
        {
            var startTime = DateTime.Now;
            action();
            var endTime = DateTime.Now;

            if (TimeRecorded != null)
            {
                TimeRecorded(msg, endTime - startTime);
            }
        }

        public static void Run<T>(string msg, Action<T> action, T arg1)
        {
            var startTime = DateTime.Now;
            action(arg1);
            var endTime = DateTime.Now;

            if (TimeRecorded != null)
            {
                TimeRecorded(msg, endTime - startTime);
            }
        }

        public static void Run<T1, T2>(string msg, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            var startTime = DateTime.Now;
            action(arg1, arg2);
            var endTime = DateTime.Now;

            if (TimeRecorded != null)
            {
                TimeRecorded(msg, endTime - startTime);
            }
        }

        public static void Run<T1, T2, T3>(string msg, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            var startTime = DateTime.Now;
            action(arg1, arg2, arg3);
            var endTime = DateTime.Now;

            if (TimeRecorded != null)
            {
                TimeRecorded(msg, endTime - startTime);
            }
        }
        public static void Run<T1, T2, T3, T4>(string msg, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var startTime = DateTime.Now;
            action(arg1, arg2, arg3, arg4);
            var endTime = DateTime.Now;

            if (TimeRecorded != null)
            {
                TimeRecorded(msg, endTime - startTime);
            }
        }
    }
}
