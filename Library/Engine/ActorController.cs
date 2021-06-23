using Assets;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Library.Engine
{
    public class ActorController
    {
        public EngineCoreBase Core { get; set; }
        public List<ActorBase> Tiles { get; private set; }
        public ContainerController Containers { get; private set; }

        public ActorController(EngineCoreBase core)
        {
            Core = core;

            Containers = new ContainerController(Core);

            lock (Core.CollectionSemaphore)
            {
                Tiles = new List<ActorBase>();
            }
        }

        public void Render(Graphics dc)
        {

            lock (Core.CollectionSemaphore)
            {
                List<ActorBase> renderTiles = new List<ActorBase>();

                renderTiles.AddRange(Tiles.Where(o => o.Visible == true)
                    .Where(o => o.Meta.ActorClass != Types.ActorClassName.ActorFriendyBeing
                        && o.Meta.ActorClass != Types.ActorClassName.ActorHostileBeing
                        && o.Meta.ActorClass != Types.ActorClassName.ActorPlayer)
                    .OrderBy(o => o.DrawOrder).ToList());

                renderTiles.AddRange(Tiles.Where(o => o.Visible == true)
                    .Where(o => o.Meta.ActorClass == Types.ActorClassName.ActorFriendyBeing
                        || o.Meta.ActorClass == Types.ActorClassName.ActorHostileBeing
                        || o.Meta.ActorClass == Types.ActorClassName.ActorPlayer)
                    .OrderBy(o => o.DrawOrder).ToList());

                foreach (var obj in renderTiles)
                {
                    RectangleF window = new RectangleF(
                        (int)Core.Display.BackgroundOffset.X,
                        (int)Core.Display.BackgroundOffset.Y,
                        Core.Display.DrawingSurface.Width,
                        Core.Display.DrawingSurface.Height);

                    if (window.IntersectsWith(obj.Bounds))
                    {
                        Utility.Types.DynamicCast(obj, obj.GetType()).Render(dc);
                    }
                }
            }
        }

        public void ResetAllTilesMetadata()
        {
            foreach (var obj in this.Tiles)
            {
                if (obj.TilePath.Contains("Chest"))
                {
                }

                obj.RefreshMetadata(true);
            }

            foreach (var container in this.Containers.Collection)
            {
                foreach (var obj in container.Contents)
                {
                    var freshMeta = TileMetadata.GetFreshMetadata(obj.TilePath);
                    obj.Meta.OverrideWith(freshMeta);
                }
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

        public T AddNew<T>(double x, double y, string TilePath) where T : ActorBase
        {
            lock (Core.CollectionSemaphore)
            {
                var bitmap = SpriteCache.GetBitmapCached(Constants.GetAssetPath($"{TilePath}.png"));
                object[] param = { Core };
                var obj = (ActorBase)Activator.CreateInstance(typeof(T), param);

                obj.TilePath = TilePath;
                obj.SetImage(bitmap);

                obj.X = x;
                obj.Y = y;
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
