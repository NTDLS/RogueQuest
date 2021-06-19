using Assets;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Library.Engine
{
    public static class MapPersistence
    {
        public static void Save(EngineCoreBase core, string fileName)
        {
            var map = new PersistMap();

            foreach (var obj in core.Actors.Tiles.Where(o => o.Visible))
            {
                map.Chunks.Add(new PersistMapEntity
                {
                    TilePath = obj.TilePath,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Velocity.Angle.Degrees,
                    DrawOrder = obj.DrawOrder,
                    Meta = obj.Meta
                });
            }

            var json = JsonConvert.SerializeObject(map);

            //System.IO.File.WriteAllText(fileName, json);

            var compressed = Utility.Compress.Zip(json);
            System.IO.File.WriteAllBytes(fileName, compressed);
        }

        public static void Load(EngineCoreBase core, string fileName, bool refreshMetadata = false)
        {
            var compressed = System.IO.File.ReadAllBytes(fileName);
            var json = Utility.Compress.Unzip(compressed);

            //var json = System.IO.File.ReadAllText(fileName);

            var map = JsonConvert.DeserializeObject<PersistMap>(json);

            core.QueueAllForDelete();

            Assembly gameAssembly = null;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("Game,"))
                {
                    gameAssembly = Assembly.Load("Game");
                }
            }

            foreach (var chunk in map.Chunks)
            {
                ActorBase tile = null;

                object[] param = { core };

                if (gameAssembly != null)
                {
                    var tileType = gameAssembly.GetType($"Game.Actors.{chunk.Meta.BasicType}");
                    tile = (ActorBase)Activator.CreateInstance(tileType, param);
                }

                if (tile == null)
                {
                    tile = (ActorBase)Activator.CreateInstance(Type.GetType("Library.Engine.ActorBase"), param);
                }

                tile.SetImage(Constants.GetAssetPath($"{chunk.TilePath}.png"));
                tile.X = chunk.X;
                tile.Y = chunk.Y;
                tile.TilePath = chunk.TilePath;
                tile.Velocity.Angle.Degrees = chunk.Angle;
                tile.DrawOrder = chunk.DrawOrder;
                tile.Meta = chunk.Meta;

                if (refreshMetadata)
                {
                    tile.RefreshMetadata();
                }

                core.Actors.Add(tile);
            }
        }
    }
}
