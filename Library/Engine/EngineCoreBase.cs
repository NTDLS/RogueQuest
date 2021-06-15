﻿using Assets;
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

        public bool IsRendering { get; private set; }
        public bool IsRunning { get; private set; }
        public EngineDisplay Display { get; set; }
        public object CollectionSemaphore { get; private set; } = new object();
        public object DrawingSemaphore { get; set; } = new object();
        public List<ActorBase> Actors { get; private set; }
        public List<TerrainBase> TerrainTiles { get; private set; }
        public List<MapBase> Maps { get; private set; }
        public Color BackgroundColor { get; private set; } = Color.FromArgb(46, 32, 60);

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
                Actors = new List<ActorBase>();
                TerrainTiles = new List<TerrainBase>();
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

        public List<TerrainBase> TerrainIntersections(Point<double> location, Point<double> size)
        {
            var list = new List<TerrainBase>();

            foreach (var obj in TerrainTiles.Where(o => o.Visible == true))
            {
                if (obj.Intersects(location, size))
                {
                    list.Add(obj);
                }
            }
            return list;
        }

        public T AddNewTerrain<T>(double x, double y, string tileTypeKey) where T : TerrainBase
        {
            lock (CollectionSemaphore)
            {
                var bitmap = SpriteCache.GetBitmapCached($"{tileTypeKey}.png");
                object[] param = { this };
                var obj = (TerrainBase)Activator.CreateInstance(typeof(T), param);

                obj.TileTypeKey = tileTypeKey;
                obj.SetImage(bitmap);

                obj.X = x;
                obj.Y = y;
                TerrainTiles.Add(obj);
                return (T)obj;
            }
        }

        public T AddNewTerrainTile<T>(double x, double y) where T : TerrainBase
        {
            lock (CollectionSemaphore)
            {
                object[] param = { this };
                var obj = (TerrainBase)Activator.CreateInstance(typeof(T), param);
                obj.X = x;
                obj.Y = y;
                TerrainTiles.Add(obj);
                return (T)obj;
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

        public T AddNewActor<T>() where T : ActorBase
        {
            lock (CollectionSemaphore)
            {
                object[] param = { this };
                var obj = (ActorBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = Display.RandomOffScreenLocation(100, 100);
                Actors.Add(obj);
                return (T)obj;
            }
        }

        public Bitmap GetBitmapCached(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_bitmapCache)
            {
                if (_bitmapCache.ContainsKey(path))
                {
                    result = _bitmapCache[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = newbitmap.Clone() as Bitmap;
                        _bitmapCache.Add(path, result);
                    }
                }
            }

            return result;
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

            var timeout = TimeSpan.FromMilliseconds(1);
            bool lockTaken = false;

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

            try
            {
                Monitor.TryEnter(DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (CollectionSemaphore)
                    {
                        _ScreenDC.Clear(BackgroundColor);

                        foreach (var actor in TerrainTiles.Where(o => o.Visible == true))
                        {
                            if (Display.VisibleBounds.IntersectsWith(actor.Bounds))
                            {
                                Utility.Types.DynamicCast(actor, actor.GetType()).Render(_ScreenDC);
                            }
                        }
                        foreach (var actor in Actors.Where(o => o.Visible == true))
                        {
                            if (Display.VisibleBounds.IntersectsWith(actor.Bounds))
                            {
                                Utility.Types.DynamicCast(actor, actor.GetType()).Render(_ScreenDC);
                            }
                        }

                        //Player?.Render(_ScreenDC);
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