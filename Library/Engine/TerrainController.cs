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
                foreach (var obj in Tiles.Where(o => o.Visible == true))
                {
                    if (Core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
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

        public List<TerrainBase> Intersections(Point<double> location, Point<double> size)
        {
            lock (Core.CollectionSemaphore)
            {
                var list = new List<TerrainBase>();

                foreach (var obj in Tiles.Where(o => o.Visible == true))
                {
                    if (obj.Intersects(location, size))
                    {
                        list.Add(obj);
                    }
                }
                return list;
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
                var bitmap = SpriteCache.GetBitmapCached($"{tileTypeKey}.png");
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
