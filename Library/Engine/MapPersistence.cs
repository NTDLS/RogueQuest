using Assets;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Library.Engine
{
    public static class MapPersistence
    {

        /// <summary>
        /// Saves a map and optionally a game state.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fileName"></param>
        /// <param name="state"></param>
        public static void Save(EngineCoreBase core, string fileName, GameState state = null)
        {
            var map = new PersistMap()
            {
                State = state
            };

            foreach (var obj in core.Actors.Tiles.Where(o => o.Visible))
            {
                map.Chunks.Add(new PersistMapChunk
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

            System.IO.File.WriteAllText(fileName, json);

            //var compressed = Utility.Compress.Zip(json);
            //System.IO.File.WriteAllBytes(fileName, compressed);
        }

        public static void Load(EngineCoreBase core, string fileName, bool refreshMetadata = false)
        {
            //var compressed = System.IO.File.ReadAllBytes(fileName);
            //var json = Utility.Compress.Unzip(compressed);

            var json = System.IO.File.ReadAllText(fileName);

            var map = JsonConvert.DeserializeObject<PersistMap>(json);

            core.State = map.State;

            if (core.State == null)
            {
                core.State = new GameState();
            }
            if (core.State.Character == null)
            {
                core.State.Character = new PlayerState();
            }

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
