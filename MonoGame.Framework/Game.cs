using System;
using System.Diagnostics;
using System.Resources;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    ///     Provides basic graphics device initialization, game logic, and rendering code.
    /// </summary>
    public class Game
    {
        private readonly TimeSpan maxElapsedTime = TimeSpan.FromTicks(500 * TimeSpan.TicksPerMillisecond);

        private Timer timer;
        private TimeSpan targetElapsedTime;
        private TimeSpan accumulatedElapsedTime;
        private readonly GameTime gameTime;
        private Stopwatch gameTimer;
        private bool suppressDraw;

        internal Bitmap Display
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the current <see cref="Microsoft.Xna.Framework.ContentManager"/>.
        /// </summary>
        /// <value>
        ///     The current <see cref="Microsoft.Xna.Framework.ContentManager"/>.
        /// </value>
        /// <seealso cref="Microsoft.Xna.Framework.Game"/>
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

        /// <summary>
        ///     Gets or sets the target time between calls to <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> when <see cref="Microsoft.Xna.Framework.Game.IsFixedTimeStep"/> is <c>true</c>. 
        /// </summary>
        /// <value>
        ///     The target time period for the game loop.
        /// </value>
        /// <remarks>
        ///     When the game frame rate is less than <c>TargetElapsedTime</c>, <see cref="Microsoft.Xna.Framework.Game.IsRunningSlowly"/> will return <c>true</c>. 
        ///     
        ///     The default value for <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/> is 1/10th of a second.
        ///     
        ///     A fixed-step <see cref="Microsoft.Xna.Framework.Game"/> tries to call its <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> method on the fixed interval specified in <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/>. Setting <see cref="Microsoft.Xna.Framework.Game.IsFixedTimeStep"/> to <c>true</c> causes a <see cref="Microsoft.Xna.Framework.Game"/> to use a fixed-step game loop. A new MonoGame project uses a fixed-step game loop with a default <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/> of 1/10th of a second.
        ///     
        ///     In a fixed-step game loop, <see cref="Microsoft.Xna.Framework.Game"/> calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> once the <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/> has elapsed. After <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> is called, if it is not time to call <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> again, <see cref="Microsoft.Xna.Framework.Game"/> calls <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/>.
        ///     
        ///     If <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> takes too long to process, <see cref="Microsoft.Xna.Framework.Game"/> sets <see cref="Microsoft.Xna.Framework.Game.IsRunningSlowly"/> to <c>true</c> and calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> again, without calling <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/> in between. When an update runs longer than the <see cref="Microsoft.Xna.Framework.GameTime.TargetElapsedTime"/>, <see cref="Microsoft.Xna.Framework.Game"/> responds by calling <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> extra times and dropping the frames associated with those updates to catch up. This ensures that <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> will have been called the expected number of times when the game loop catches up from a slowdown. You can check the value of <see cref="Microsoft.Xna.Framework.GameTime.IsRunningSlowly"/> in your <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> if you want to detect dropped frames and shorten your <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> processing to compensate. You can reset the elapsed times by calling <see cref="Microsoft.Xna.Framework.Game.ResetElapsedTime()"/>.
        /// </remarks>
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

        /// <summary>
        ///     Gets or sets a value indicating whether to use fixed time steps. 
        /// </summary>
        /// <value>
        ///     <c>true</c> if using fixed time steps; <c>false</c> otherwise. 
        /// </value>
        /// <remarks>
        ///     The default value for IsFixedTimeStep is <c>true</c>.
        ///     
        ///     A fixed-step <see cref="Microsoft.Xna.Framework.Game"/> tries to call its <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> method on the fixed interval specified in <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/>. Setting <see cref="Microsoft.Xna.Framework.Game.IsFixedTimeStep"/> to <c>true</c> causes a <see cref="Microsoft.Xna.Framework.Game"/> to use a fixed-step game loop. A new MonoGame project uses a fixed-step game loop with a default <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/> of 1/10th of a second.
        ///     
        ///     In a fixed-step game loop, <see cref="Microsoft.Xna.Framework.Game"/> calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> once the <see cref="Microsoft.Xna.Framework.Game.TargetElapsedTime"/> has elapsed. After <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> is called, if it is not time to call <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> again, <see cref="Microsoft.Xna.Framework.Game"/> calls <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/>.
        ///     
        ///     If <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> takes too long to process, <see cref="Microsoft.Xna.Framework.Game"/> sets <see cref="Microsoft.Xna.Framework.Game.IsRunningSlowly"/> to <c>true</c> and calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> again, without calling <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/> in between. When an update runs longer than the <see cref="Microsoft.Xna.Framework.GameTime.TargetElapsedTime"/>, <see cref="Microsoft.Xna.Framework.Game"/> responds by calling <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> extra times and dropping the frames associated with those updates to catch up. This ensures that <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> will have been called the expected number of times when the game loop catches up from a slowdown. You can check the value of <see cref="Microsoft.Xna.Framework.GameTime.IsRunningSlowly"/> in your <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> if you want to detect dropped frames and shorten your <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> processing to compensate. You can reset the elapsed times by calling <see cref="Microsoft.Xna.Framework.Game.ResetElapsedTime()"/>.
        /// </remarks>
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
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        /// <summary>
        ///     Call this method to initialize the game, begin running the game loop, and start processing events for the game. 
        /// </summary>
        /// <param name="display">
        ///     The device's display.
        /// </param>
        /// <param name="manager">
        ///     The <see cref="System.Resources.ResourceManager"/> for the project.
        /// </param>
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

        /// <summary>
        ///     Resets the elapsed time counter. 
        /// </summary>
        /// <remarks>
        ///     Use this method if your game is recovering from a slow-running state, and <see cref="Microsoft.Xna.Framework.GameTime.ElapsedGameTime"/> is too large to be useful.
        /// </remarks>
        public void ResetElapsedTime()
        {
            gameTimer.Restart();
            accumulatedElapsedTime = TimeSpan.Zero;
            gameTime.ElapsedGameTime = TimeSpan.Zero;
        }

        /// <summary>
        ///     Prevents calls to <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/> until the next <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/>. 
        /// </summary>
        /// <remarks>
        ///     Call this method during <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> to prevent any calls to <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/> until after the next call to <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/>. This method can be used on small devices to conserve battery life if the display does not change as a result of <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/>. For example, if the screen is static with no background animations, the player input can be examined during <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> to determine whether the player is performing any action. If no input is detected, this method allows the game to skip drawing until the next update. 
        /// </remarks>
        public void SuppressDraw()
        {
            suppressDraw = true;
        }

        private void Tick(object state)
        {
            Tick();
        }

        /// <summary>
        ///     Updates the game's clock and calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> and <see cref="Microsoft.Xna.Framework.Game.Draw(Microsoft.Xna.Framework.GameTime)"/>. 
        /// </summary>
        /// <remarks>
        ///     In a fixed-step game, <see cref="Microsoft.Xna.Framework.Game.Tick()"/> calls <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> only after a target time interval has elapsed.
        ///     
        ///     In a variable-step game, <see cref="Microsoft.Xna.Framework.Game.Update(Microsoft.Xna.Framework.GameTime)"/> is called every time <see cref="Microsoft.Xna.Framework.Game.Tick()"/> is called.
        /// </remarks>
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
            if (accumulatedElapsedTime > maxElapsedTime)
                accumulatedElapsedTime = maxElapsedTime;

            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.gametime.isrunningslowly.aspx
            // Calculate IsRunningSlowly for the fixed time step, but only when the accumulated time
            // exceeds the target time.

            if (IsFixedTimeStep)
            {
                gameTime.ElapsedGameTime = TargetElapsedTime;
                int stepCount = 0;

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
            {
                suppressDraw = false;
            }
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
