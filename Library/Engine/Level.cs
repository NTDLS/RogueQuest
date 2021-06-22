using Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Library.Engine
{
    public class Level
    {
        public string Name { get; set; }
        public List<LevelChunk> Chunks { get; set; }
        public GameState State { get; set; }
        public List<Container> Containers { get; set; }

        public Level()
        {
            Chunks = new List<LevelChunk>();
            Containers = new List<Container>();
        }

        /// <summary>
        /// Saves a level and optionally a game state.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="fileName"></param>
        /// <param name="state"></param>
        public static void Save(EngineCoreBase core, string fileName, GameState state = null)
        {
            var level = new Level()
            {
                State = state,
                Containers = core.Actors.Containers.Collection
            };

            foreach (var obj in core.Actors.Tiles.Where(o => o.Visible))
            {
                level.Chunks.Add(new LevelChunk
                {
                    TilePath = obj.TilePath,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Velocity.Angle.Degrees,
                    DrawOrder = obj.DrawOrder,
                    Meta = obj.Meta
                });
            }

            var json = JsonConvert.SerializeObject(level);

            System.IO.File.WriteAllText(fileName, json);

            //var compressed = Utility.Compress.Zip(json);
            //System.IO.File.WriteAllBytes(fileName, compressed);
        }

        public static void Load(EngineCoreBase core, string fileName, bool refreshMetadata = false)
        {
            //var compressed = System.IO.File.ReadAllBytes(fileName);
            //var json = Utility.Compress.Unzip(compressed);

            var json = System.IO.File.ReadAllText(fileName);

            var level = JsonConvert.DeserializeObject<Level>(json);

            core.State = level.State;

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

            core.Actors.Containers.Set(level.Containers);

            foreach (var chunk in level.Chunks)
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
