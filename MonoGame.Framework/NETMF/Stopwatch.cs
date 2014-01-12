using System.Threading;

namespace System.Diagnostics
{
    public class Stopwatch
    {
        private DateTime startDate;

        public Stopwatch()
        {
            startDate = DateTime.MinValue;
        }

        public TimeSpan Elapsed
        {
            get
            {
                if (startDate != DateTime.MinValue)
                {
                    return DateTime.UtcNow - startDate;
                }
                return TimeSpan.FromTicks(0);
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
            get;
            private set;
        }

        public void Start()
        {
            if (startDate == DateTime.MinValue)
            {
                startDate = DateTime.UtcNow;
            }
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Reset()
        {
            Stop();
            startDate = DateTime.MinValue;
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