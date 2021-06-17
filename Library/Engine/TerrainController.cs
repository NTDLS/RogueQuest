using Assets;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Library.Engine
{
    public class TerrainController
    {
        public EngineCoreBase Core { get; set; }
        public List<TerrainBase> Tiles { get; private set; }

        public TerrainController(EngineCoreBase core)
        {
            Core = core;

            lock (Core.CollectionSemaphore)
            {
                Tiles = new List<TerrainBase>();
            }
        }

        public void Render(Graphics dc)
        {
            lock (Core.CollectionSemaphore)
            {
                foreach (var obj in Tiles.Where(o => o.Visible == true).OrderBy(o => o.DrawOrder))
                {
                    RectangleF window = new RectangleF(
                        (int)Core.Display.BackgroundOffset.X,
                        (int)Core.Display.BackgroundOffset.Y,
                        Core.Display.DrawingSurface.Width,
                        Core.Display.DrawingSurface.Height);

                    //if (Core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                    if (window.IntersectsWith(obj.Bounds))
                    {
                        Utility.Types.DynamicCast(obj, obj.GetType()).Render(dc);
                    }
                }
            }
        }

        public void QueueAllForDelete()
        {
            foreach (var obj in this.Tiles)
            {
                obj.QueueForDelete();
            }
        }

        public List<TerrainBase> Intersections(TerrainBase with)
        {
            var objs = new List<TerrainBase>();

            foreach (var obj in Tiles.Where(o => o.Visible == true))
            {
                if (obj.Intersects(with.Location, new Point<double>(with.Size.Width, with.Size.Height)))
                {
                    objs.Add(obj);
                }
            }
            return objs;
        }

        public List<TerrainBase> Intersections(double x, double y, double width, double height)
        {
            return Intersections(new Point<double>(x, y), new Point<double>(width, height));
        }

        public List<TerrainBase> Intersections(Point<double> location, Point<double> size)
        {
            lock (Core.CollectionSemaphore)
            {
                var objs = new List<TerrainBase>();

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
        public void Add(TerrainBase obj)
        {
            lock (Core.CollectionSemaphore)
            {
                Tiles.Add(obj);
            }
        }

        public T AddNew<T>(double x, double y, string tileTypeKey) where T : TerrainBase
        {
            lock (Core.CollectionSemaphore)
            {
                var bitmap = SpriteCache.GetBitmapCached(Constants.GetAssetPath($"{tileTypeKey}.png"));
                object[] param = { Core };
                var obj = (TerrainBase)Activator.CreateInstance(typeof(T), param);

                obj.TileTypeKey = tileTypeKey;
                obj.SetImage(bitmap);

                obj.X = x;
                obj.Y = y;
                Tiles.Add(obj);
                return (T)obj;
            }
        }

        public T AddNew<T>(double x, double y) where T : TerrainBase
        {
            lock (Core.CollectionSemaphore)
            {
                object[] param = { Core };
                var obj = (TerrainBase)Activator.CreateInstance(typeof(T), param);
                obj.X = x;
                obj.Y = y;
                Tiles.Add(obj);
                return (T)obj;
            }
        }
    }
}
