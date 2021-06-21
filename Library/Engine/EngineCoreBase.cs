using Assets;
using Library.Engine;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Library.Engine
{
    public class EngineCoreBase
    {
        #region Public properties.

        public GameState State { get; set; }
        public bool IsRendering { get; private set; }
        public bool IsRunning { get; private set; }
        public EngineDisplay Display { get; set; }
        public object CollectionSemaphore { get; private set; } = new object();
        public object DrawingSemaphore { get; set; } = new object();
        public ActorController Actors { get; private set; }
        public InputController Input { get; set; }
        public List<MapBase> Maps { get; private set; }
        public Color BackgroundColor { get; private set; } = Color.FromArgb(46, 32, 60);

        public List<Rectangle> debugRects = new List<Rectangle>();

        #endregion

        private Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();

        #region Events.

        public delegate void StartEvent(EngineCoreBase sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(EngineCoreBase sender);
        public event StopEvent OnStop;

        #endregion

        public EngineCoreBase(Control drawingSurface, Size visibleSize)
        {
            Display = new EngineDisplay(drawingSurface, visibleSize);

            lock (CollectionSemaphore)
            {
                Actors = new ActorController(this);
                Input = new InputController(this);
                Maps = new List<MapBase>();
            }
        }

        public void Start()
        {
            OnStart?.Invoke(this);
            IsRunning = true;
        }

        public void Stop()
        {
            if (IsRunning == false)
            {
                return;
            }

            IsRunning = false;

            OnStop?.Invoke(this);
        }

        public virtual void HandleSingleKeyPress(Keys key)
        {
        }

        public void ResizeDrawingSurface(Size visibleSize)
        {
            Display.ResizeDrawingSurface(visibleSize);

            lock (CollectionSemaphore)
            {
                _ScreenBitmap = null;
            }
        }

        public void QueueAllForDelete()
        {
            lock (this.CollectionSemaphore)
            {
                Actors.QueueAllForDelete();
            }
        }

        public void PurgeAllDeletedTiles()
        {
            lock (this.CollectionSemaphore)
            {
                Actors.Tiles.RemoveAll(o => o.ReadyForDeletion);
            }
        }

        public T AddNewMap<T>() where T : MapBase
        {
            lock (CollectionSemaphore)
            {
                object[] param = { this };
                var obj = (MapBase)Activator.CreateInstance(typeof(T), param);
                Maps.Add(obj);
                return (T)obj;
            }
        }

        private Bitmap _ScreenBitmap = null;
        private Graphics _ScreenDC = null;

        private Object _LatestFrameLock = new Object();

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public Bitmap Render()
        {
            IsRendering = true;

            bool lockTaken = false;
            var timeout = TimeSpan.FromMilliseconds(1);

            try
            {
                Monitor.TryEnter(DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (CollectionSemaphore)
                    {
                        if (_ScreenBitmap == null)
                        {
                            _ScreenBitmap = new Bitmap(Display.DrawingSurface.Width, Display.DrawingSurface.Height);

                            _ScreenDC = Graphics.FromImage(_ScreenBitmap);
                            _ScreenDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            _ScreenDC.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                            _ScreenDC.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            _ScreenDC.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            _ScreenDC.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        }

                        _ScreenDC.Clear(BackgroundColor);

                        Actors.Render(_ScreenDC);

                        foreach (var rc in debugRects)
                        {
                            Pen pen = new Pen(Color.Orange, 2);
                            _ScreenDC.DrawRectangle(pen, rc);
                        }
                    }
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(DrawingSemaphore);
                }
            }

            IsRendering = false;

            return _ScreenBitmap;
        }
    }
}
