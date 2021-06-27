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

        public Levels Levels { get; set; }
        public GameState State { get; set; }
        public bool IsRendering { get; private set; }
        public bool IsRunning { get; private set; }
        public EngineDisplay Display { get; private set; }
        public object CollectionSemaphore { get; private set; } = new object();
        public object DrawingSemaphore { get; private set; } = new object();
        public ActorController Actors { get; private set; }
        public Color BackgroundColor { get; private set; } = Color.FromArgb(46, 32, 60);

        #endregion

        private Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();

        #region Events.

        public delegate void StartEvent(EngineCoreBase sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(EngineCoreBase sender);
        public event StopEvent OnStop;

        #endregion

        public void Save(string fileName)
        {
            PushCurrentLevel();
            Levels.Save(fileName);
        }

        public void Load(string fileName)
        {
            Levels.Load(fileName);

        }
        public void PushCurrentLevel()
        {
            Levels.PushLevel(State.CurrentLevel);
        }

        public void PopCurrentLevel()
        {
            Levels.PopLevel(State.CurrentLevel);
        }

        public void SelectLevel(int levelNumber)
        {
            Levels.PushLevel(State.CurrentLevel);
            Levels.PopLevel(levelNumber);
            State.CurrentLevel = levelNumber;
        }

        public void DeleteLevel(int levelNumber)
        {
            if (Levels.Collection.Count <= 1)
            {
                return;//Can't delete the last level.
            }

            if (levelNumber == State.CurrentLevel)
            {
                Levels.PopLevel(0);
                State.CurrentLevel = 0;
            }

            if (levelNumber == State.DefaultLevel)
            {
                State.DefaultLevel = 0;
            }

            Levels.Collection.RemoveAt(levelNumber);
        }

        /// <summary>
        /// Set the level number for the start of a new game.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void SetDefaultLevel(int levelNumber)
        {
            State.DefaultLevel = levelNumber;
        }

        public EngineCoreBase(Control drawingSurface, Size visibleSize)
        {
            Display = new EngineDisplay(drawingSurface, visibleSize);
            Levels = new Levels(this);
            State = new GameState();

            lock (CollectionSemaphore)
            {
                Actors = new ActorController(this);
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

        public void ResetAllTilesMetadata()
        {
            Actors.ResetAllTilesMetadata();

            foreach (var obj in this.State.Items)
            {
                Guid? uid = obj.Tile.Meta.UID;

                var freshMeta = TileMetadata.GetFreshMetadata(obj.Tile.TilePath);
                obj.Tile.Meta.OverrideWith(freshMeta);

                if (uid != null)
                {
                    obj.Tile.Meta.UID = (Guid)uid; //Never change the UID once it is set.
                }

                //All items need UIDs.
                if (obj.Tile.Meta.UID == null)
                {
                    obj.Tile.Meta.UID = Guid.NewGuid();
                }
            }
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
