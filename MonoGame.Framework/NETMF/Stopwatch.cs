using System.Threading;

namespace System.Diagnostics
{
    public class Stopwatch
    {
        private static readonly TimeSpan period = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * 100);
        private static readonly TimeSpan due = TimeSpan.FromTicks(0);
        private Timer timer;
        private DateTime? startDate;
        private DateTime? currentDate;

        public Stopwatch()
        {
            timer = null;
            startDate = null;
            currentDate = null;
        }

        private void Callback(object state)
        {
            currentDate = DateTime.UtcNow;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return currentDate - startDate;
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return (long)Math.Round((double)Elapsed.Ticks / (double)TimeSpan.TicksPerMillisecond);
            }
        }

        public long ElapsedTicks
        {
            get
            {
                return Elapsed.Ticks;
            }
        }

        public bool IsRunning
        {
            get
            {
                return timer != null;
            }
        }

        public void Start()
        {
            if (startDate == null)
            {
                startDate = DateTime.UtcNow;
                currentDate = startDate;
            }
            timer = new Timer(Callback, null, due, period);
        }

        public void Stop()
        {
            timer.Dispose();
            timer = null;
        }

        public void Reset()
        {
            Stop();
            startDate = null;
            currentDate = null;
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        public static Stopwatch StartNew()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }
    }
}