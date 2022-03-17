using Assets;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Library.Engine
{
    public class ActorController
    {
        private Graphics _miniMapDC;
        private Bitmap _miniMapBitmap;
        private readonly int _miniMapWidth = 350;
        private readonly int _miniMapHeight = 350;
        private Point<double> _miniMapScale;
        private Point<double> _miniMapOffset;
        private int _miniMapDistance = 10;
        public EngineCoreBase Core { get; set; }
        public List<ActorBase> Tiles { get; private set; }
        public Assembly GameAssembly { get; private set; } = null;

        public ActorController(EngineCoreBase core)
        {
            Core = core;

            lock (Core.CollectionSemaphore)
            {
                Tiles = new List<ActorBase>();
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("Game,"))
                {
                    GameAssembly = Assembly.Load("Game");
                }
            }
        }

        public IEnumerable<ActorBase> OnScreenTiles()
        {
            RectangleF window = new RectangleF(
                (int)Core.Display.BackgroundOffset.X,
                (int)Core.Display.BackgroundOffset.Y,
                Core.Display.DrawingSurface.Width,
                Core.Display.DrawingSurface.Height);

            return Tiles.Where(o => o.AlwaysRender || window.IntersectsWith(o.Bounds) || o.DrawRealitiveToBackgroundOffset == false);
        }

        public void Render(Graphics screenDc)
        {
            lock (Core.CollectionSemaphore)
            {
                if (Core.DrawMinimap)
                {
                    if (_miniMapBitmap == null && Core.Display.VisibleBounds.Width > 0 && Core.Display.VisibleBounds.Height > 0)
                    {
                        double miniMapVisionWidth = Core.Display.VisibleBounds.Width * _miniMapDistance;
                        double miniMapVisionHeight = Core.Display.VisibleBounds.Height * _miniMapDistance;

                        _miniMapBitmap = new Bitmap((int)_miniMapWidth, (int)_miniMapHeight);

                        _miniMapScale = new Point<double>((double)_miniMapBitmap.Width / miniMapVisionWidth, (double)_miniMapBitmap.Height / miniMapVisionHeight);
                        _miniMapOffset = new Point<double>(_miniMapWidth / 2.0, _miniMapHeight / 2.0);

                        _miniMapDC = Graphics.FromImage(_miniMapBitmap);
                        _miniMapDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        _miniMapDC.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                        _miniMapDC.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        _miniMapDC.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        _miniMapDC.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        Rectangle rect = new Rectangle(0, 0, _miniMapBitmap.Width, _miniMapBitmap.Height);
                    }

                    if (_miniMapBitmap != null)
                    {
                        _miniMapDC.Clear(Color.Black);
                    }
                }

                var onScreenTiles = OnScreenTiles();

                List<ActorBase> renderTiles = new List<ActorBase>();

                //Add first layer of tiles.
                renderTiles.AddRange(onScreenTiles.Where(o => o.Visible == true && o.DoNotDraw == false
                        && o.Meta.ActorClass != Types.ActorClassName.ActorFriendyBeing
                        && o.Meta.ActorClass != Types.ActorClassName.ActorHostileBeing
                        && o.Meta.ActorClass != Types.ActorClassName.ActorPlayer
                        && o.Meta.ActorClass != Types.ActorClassName.ActorPlayer
                        && o.Meta.ActorClass != Types.ActorClassName.ActorAnimation
                        && o.Meta.ActorClass != Types.ActorClassName.ActorDialog)
                    .OrderBy(o => o.DrawOrder ?? 0).ToList());

                //Add top layer of tiles.
                renderTiles.AddRange(onScreenTiles.Where(o => o.Visible == true && o.DoNotDraw == false
                        && (o.Meta.ActorClass == Types.ActorClassName.ActorFriendyBeing
                        || o.Meta.ActorClass == Types.ActorClassName.ActorHostileBeing
                        || o.Meta.ActorClass == Types.ActorClassName.ActorAnimation
                        || o.Meta.ActorClass == Types.ActorClassName.ActorPlayer)
                        && o.Meta.ActorClass != Types.ActorClassName.ActorDialog)
                    .OrderBy(o => o.DrawOrder ?? 0).ToList());

                //Add dialogs.
                renderTiles.AddRange(onScreenTiles.Where(o => o.Visible == true && o.DoNotDraw == false
                    && o.Meta.ActorClass == Types.ActorClassName.ActorDialog)
                    .OrderBy(o => o.DrawOrder ?? 0).ToList());

                var player = renderTiles.Where(o => o.Meta.ActorClass == Types.ActorClassName.ActorPlayer).FirstOrDefault();

                foreach (var obj in renderTiles)
                {
                    if (obj.Meta.HasBeenViewed == false
                        && Core.BlindPlay == true && player != null && obj.DistanceTo(player) < Core.BlindPlayDistance)
                    {
                        obj.Invalidate();
                        obj.Meta.HasBeenViewed = true;
                    }

                    if (Core.BlindPlay == false || obj.Meta.HasBeenViewed == true
                        || obj.Meta.ActorClass == Types.ActorClassName.ActorDialog
                        || obj.Meta.ActorClass == Types.ActorClassName.ActorAnimation)
                    {
                        Native.Types.DynamicCast(obj, obj.GetType()).Render(screenDc);
                    }
                }

                if (Core.DrawMinimap)
                {
                    if (_miniMapBitmap != null)
                    {
                        RectangleF miniMapVision = new RectangleF(
                            (int)(Core.Display.BackgroundOffset.X / _miniMapDistance),
                            (int)(Core.Display.BackgroundOffset.Y / _miniMapDistance),
                            (Core.Display.DrawingSurface.Width * _miniMapDistance),
                            (Core.Display.DrawingSurface.Height * _miniMapDistance)
                            );

                        var miniMapTiles = Tiles.Where(o => o.Visible == true && o.DoNotDraw == false && o.ReadyForDeletion == false
                            && miniMapVision.IntersectsWith(o.Bounds) || o.DrawRealitiveToBackgroundOffset == false
                        
                        );

                        foreach (var obj in miniMapTiles)
                        {
                            Native.Types.DynamicCast(obj, obj.GetType()).RenderMiniMap(_miniMapDC, _miniMapScale, _miniMapOffset);
                        }

                        screenDc.DrawImage(_miniMapBitmap, new Rectangle(0, 0, _miniMapBitmap.Width, _miniMapBitmap.Height));
                    }
                }
            }
        }

        public void ResetAllTilesMetadata()
        {
            foreach (var obj in this.Tiles)
            {
                obj.RefreshMetadata(true);
            }
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            return (from o in Tiles
                    where o is T
                    && o.Visible == true
                    select o as T).ToList();
        }

        public List<T> OfType<T>() where T : class
        {
            return (from o in Tiles
                    where o is T
                    select o as T).ToList();
        }

        public void QueueAllForDelete()
        {
            foreach (var obj in this.Tiles)
            {
                obj.QueueForDelete();
            }
        }

        public List<ActorBase> Intersections(ActorBase with, int distance)
        {
            var objs = new List<ActorBase>();

            Point<double> withLocation = new Point<double>(with.Location.X - distance, with.Location.Y - distance);

            foreach (var obj in Tiles.Where(o => o.Visible == true))
            {
                if (obj != with)
                {
                    if (obj.Intersects(withLocation, new Point<double>(with.Size.Width + (distance * 2), with.Size.Height + (distance * 2))))
                    {
                        objs.Add(obj);
                    }
                }
            }
            return objs;
        }

        public List<ActorBase> Intersections(ActorBase with)
        {
            var objs = new List<ActorBase>();

            foreach (var obj in Tiles.Where(o => o.Visible == true))
            {
                if (obj != with)
                {
                    if (obj.Intersects(with.BoundLocation, new Point<double>(with.Size.Width, with.Size.Height)))
                    {
                        objs.Add(obj);
                    }
                }
            }
            return objs;
        }

        public List<ActorBase> Intersections(double x, double y, double width, double height)
        {
            return Intersections(new Point<double>(x, y), new Point<double>(width, height));
        }

        public List<ActorBase> Intersections(Point<double> location, Point<double> size)
        {
            lock (Core.CollectionSemaphore)
            {
                var objs = new List<ActorBase>();

                foreach (var obj in Tiles.Where(o => o.Visible == true))
                {
                    if (obj.Intersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            }
        }

        public void Add(ActorBase obj)
        {
            lock (Core.CollectionSemaphore)
            {
                Tiles.Add(obj);
            }
        }

        public ActorBase AddDynamic(TileIdentifier tile, double x, double y)
        {
            object[] param = { this.Core };

            var tileType = GameAssembly.GetType($"Game.Actors.{tile.Meta.ActorClass.ToString()}");
            var obj = (ActorBase)Activator.CreateInstance(tileType, param);

            obj.Meta = tile.Meta;
            obj.TilePath = tile.TilePath;
            obj.SetImage(SpriteCache.GetBitmapCached(obj.ImagePath));
            obj.X = x;
            obj.Y = y;

            Tiles.Add(obj);

            obj.Invalidate();

            return obj;
        }

        public ActorBase AddDynamic(string actorClass, double x, double y, string TilePath, TileMetadata meta = null)
        {
            object[] param = { this.Core };

            var tileType = GameAssembly.GetType($"Game.Actors.{actorClass}");
            var obj = (ActorBase)Activator.CreateInstance(tileType, param);

            if (meta != null)
            {
                obj.Meta = meta;
            }

            obj.TilePath = TilePath;
            obj.SetImage(SpriteCache.GetBitmapCached(obj.ImagePath));
            obj.X = x;
            obj.Y = y;

            Tiles.Add(obj);

            return obj;
        }

        public T AddNew<T>(double x, double y, string TilePath) where T : ActorBase
        {
            lock (Core.CollectionSemaphore)
            {
                object[] param = { Core };
                var obj = (ActorBase)Activator.CreateInstance(typeof(T), param);

                obj.TilePath = TilePath;
                obj.SetImage(SpriteCache.GetBitmapCached(obj.ImagePath));
                obj.X = x;
                obj.Y = y;

                Tiles.Add(obj);

                return (T)obj;
            }
        }

        public T AddNewAnimation<T>(double x, double y, string imageFrames, Size? frameSize, int frameDelayMilliseconds = 10, PlayMode playMode = null) where T : ActorBase
        {
            lock (Core.CollectionSemaphore)
            {
                object[] param = { Core, Constants.GetAssetPath($"{imageFrames}.png"), frameSize, frameDelayMilliseconds, playMode };
                var obj = (ActorBase)Activator.CreateInstance(typeof(T), param);

                obj.TilePath = imageFrames;
                obj.X = x;
                obj.Y = y;
                obj.Meta = new TileMetadata()
                {
                    ActorClass = Types.ActorClassName.ActorAnimation
                };

                Tiles.Add(obj);

                return (T)obj;
            }
        }

        public T AddNew<T>() where T : ActorBase
        {
            lock (Core.CollectionSemaphore)
            {
                object[] param = { Core };
                var obj = (ActorBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = Core.Display.RandomOffScreenLocation(100, 100);
                Tiles.Add(obj);
                return (T)obj;
            }
        }
    }
}
