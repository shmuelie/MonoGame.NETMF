using System;
using System.Diagnostics;
using System.Resources;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public class Game
    {
        private readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(500 * TimeSpan.TicksPerMillisecond);

        private Timer timer;
        private TimeSpan targetElapsedTime;
        private TimeSpan accumulatedElapsedTime;
        private readonly GameTime gameTime;
        private Stopwatch gameTimer;
        private bool suppressDraw;
        private bool skippedTick;

        internal Bitmap Display
        {
            get;
            set;
        }

        public ContentManager Content
        {
            get;
            private set;
        }

        public GraphicsDeviceManager Graphics
        {
            get;
            private set;
        }

        public SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        public TimeSpan TargetElapsedTime
        {
            get
            {
                return targetElapsedTime;
            }
            set
            {
                if (timer != null)
                {
                    timer.Change(TimeSpan.FromTicks(0), value);
                }
                targetElapsedTime = value;
            }
        }

        public bool IsFixedTimeStep
        {
            get;
            set;
        }

        public Game()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 100);
            gameTime = new GameTime();
            gameTimer = new Stopwatch();
            skippedTick = false;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public void Run(Bitmap display, ResourceManager manager)
        {
            Display = display;
            Content = new ContentManager(manager);
            Graphics = new GraphicsDeviceManager(this);
            SpriteBatch = new SpriteBatch(Graphics.GraphicsDevice);
            Graphics.GraphicsDevice.SetRenderTarget(null);
            Initialize();
            LoadContent();
            ResetElapsedTime();
            timer = new Timer(Tick, null, TimeSpan.FromTicks(0), TargetElapsedTime);
        }

        public void ResetElapsedTime()
        {
            gameTimer.Restart();
            accumulatedElapsedTime = TimeSpan.Zero;
            gameTime.ElapsedGameTime = TimeSpan.Zero;
        }

        public void SuppressDraw()
        {
            suppressDraw = true;
        }

        private void Tick(object state)
        {
            Tick();
        }

        public void Tick()
        {
            // NOTE: This code is very sensitive and can break very badly
            // with even what looks like a safe change.  Be sure to test 
            // any change fully in both the fixed and variable timestep 
            // modes across multiple devices and platforms.

            // Advance the accumulated elapsed time.
            accumulatedElapsedTime += gameTimer.Elapsed;
            gameTimer.Restart();

            // If we're in the fixed timestep mode and not enough time has elapsed
            // to perform an update we sleep off the the remaining time to save battery
            // life and/or release CPU time to other threads and processes.
            if (IsFixedTimeStep && accumulatedElapsedTime < TargetElapsedTime)
            {
                int sleepTime = (TargetElapsedTime - accumulatedElapsedTime).Milliseconds;

                // NOTE: While sleep can be inaccurate in general it is 
                // accurate enough for frame limiting purposes if some
                // fluctuation is an acceptable result.
                System.Threading.Thread.Sleep(sleepTime);

                accumulatedElapsedTime += TimeSpan.FromTicks(sleepTime * TimeSpan.TicksPerMillisecond);
            }

            // Do not allow any update to take longer than our maximum.
            if (accumulatedElapsedTime > _maxElapsedTime)
                accumulatedElapsedTime = _maxElapsedTime;

            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.gametime.isrunningslowly.aspx
            // Calculate IsRunningSlowly for the fixed time step, but only when the accumulated time
            // exceeds the target time.

            if (IsFixedTimeStep)
            {
                gameTime.ElapsedGameTime = TargetElapsedTime;
                var stepCount = 0;

                gameTime.IsRunningSlowly = (accumulatedElapsedTime > TargetElapsedTime);

                // Perform as many full fixed length time steps as we can.
                while (accumulatedElapsedTime >= TargetElapsedTime)
                {
                    gameTime.TotalGameTime += TargetElapsedTime;
                    accumulatedElapsedTime -= TargetElapsedTime;
                    ++stepCount;

                    Update(gameTime);
                }

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                gameTime.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                gameTime.ElapsedGameTime = accumulatedElapsedTime;
                gameTime.TotalGameTime += accumulatedElapsedTime;
                accumulatedElapsedTime = TimeSpan.Zero;
                // Always set the RunningSlowly flag to false here when we are in fast-as-possible mode.
                gameTime.IsRunningSlowly = false;

                Update(gameTime);
            }

            // Draw unless the update suppressed it.
            if (suppressDraw)
                suppressDraw = false;
            else
            {
                Draw(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
