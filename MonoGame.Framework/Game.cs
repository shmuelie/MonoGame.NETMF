using System;
using System.Resources;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public class Game
    {
        private DateTime gameStartTime;
        private DateTime lastCallToDraw;
        private DateTime lastCallToUpdate;
        private Timer timer;
        private TimeSpan targetElapsedTime;

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
                timer.Change(TimeSpan.FromTicks(0), value);
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
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 100);
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public void Run(Bitmap display, ResourceManager manager)
        {
            IsFixedTimeStep = true;
            gameStartTime = DateTime.UtcNow;
            lastCallToDraw = gameStartTime;
            lastCallToUpdate = gameStartTime;
            Display = display;
            Content = new ContentManager(manager);
            Graphics = new GraphicsDeviceManager(this);
            SpriteBatch = new SpriteBatch(Graphics.GraphicsDevice);
            Graphics.GraphicsDevice.SetRenderTarget(null);
            Initialize();
            LoadContent();
            timer = new Timer(Tick, null, TimeSpan.FromTicks(0), TargetElapsedTime);
        }

        public void ResetElapsedTime()
        {
            gameStartTime = DateTime.UtcNow;
        }

        private void Tick(object state)
        {
            Tick();
        }

        public void Tick()
        {
            DateTime drawNow = DateTime.UtcNow;
            GameTime drawTime = new GameTime(drawNow - gameStartTime, drawNow - lastCallToDraw);
            Draw(drawTime);
            lastCallToDraw = DateTime.UtcNow;
            DateTime updateNow = DateTime.UtcNow;
            GameTime updateTime = new GameTime(updateNow - gameStartTime, updateNow - lastCallToUpdate);
            Update(updateTime);
            lastCallToUpdate = DateTime.UtcNow;
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
