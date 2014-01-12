using System.Threading;

namespace System.Diagnostics
{
    public class Stopwatch
    {
        private static readonly TimeSpan period = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * 100);
        private static readonly TimeSpan due = TimeSpan.FromTicks(0);
        private Timer timer;
        private DateTime startDate;
        private DateTime currentDate;

        public Stopwatch()
        {
            timer = null;
            startDate = DateTime.MinValue;
            currentDate = DateTime.MinValue;
        }

        private void Callback(object state)
        {
            currentDate = DateTime.UtcNow;
        }

        public TimeSpan Elapsed
        {
            get
            {
                if ((currentDate != DateTime.MinValue) && (startDate != DateTime.MinValue))
                {
                    return currentDate - startDate;
                }
                return due;
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
            if (startDate == DateTime.MinValue)
            {
                startDate = DateTime.UtcNow;
                currentDate = startDate;
            }
            timer = new Timer(Callback, null, due, period);
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public void Reset()
        {
            Stop();
            startDate = DateTime.MinValue;
            currentDate = DateTime.MinValue;
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