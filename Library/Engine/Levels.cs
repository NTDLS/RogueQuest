using Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Library.Engine
{
    public class Levels
    {
        public EngineCoreBase Core { get; set; }
        public List<byte[]> Bytes { get; set; }

        public Levels(EngineCoreBase core)
        {
            Core = core;
        }

        /// <summary>
        /// Saves an entire game file, all levels, items and character state.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            var saveFile = new BinarySaveFile()
            {
                Bytes = this.Bytes,
                State = Core.State
            };

            var json = JsonConvert.SerializeObject(saveFile);
            var compressed = Utility.Compress.Zip(json);
            System.IO.File.WriteAllBytes(fileName, compressed);
        }

        /// <summary>
        /// Saves an entire game file, all levels, items and character state. All levels remain compressed in byte arrays.
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            var compressedSaveFile = System.IO.File.ReadAllBytes(fileName);

            var json = Utility.Compress.Unzip(compressedSaveFile);

            var saveFile = JsonConvert.DeserializeObject<BinarySaveFile>(json);

            Bytes = saveFile.Bytes;

            Core.State = saveFile.State;

            if (Core.State == null)
            {
                Core.State = new GameState();
            }
            if (Core.State.Character == null)
            {
                Core.State.Character = new PlayerState();
            }

            PopLevel(saveFile.State.CurrentLevel);
        }

        /// <summary>
        /// Pushes the game tiles to the level array as compressed json.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void PushLevel(int levelNumber)
        {
            var chunks = new List<LevelChunk>();

            foreach (var obj in Core.Actors.Tiles.Where(o => o.Visible))
            {
                chunks.Add(new LevelChunk
                {
                    TilePath = obj.TilePath,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Velocity.Angle.Degrees,
                    DrawOrder = obj.DrawOrder,
                    Meta = obj.Meta
                });
            }

            var json = JsonConvert.SerializeObject(chunks);

            var compressed = Utility.Compress.Zip(json);

            this.Bytes[levelNumber] = compressed;
        }

        /// <summary>
        /// Decompresses a level from bytes and pushes it to the game tiles. This is how you change maps.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="refreshMetadata"></param>
        public void PopLevel(int levelNumber, bool refreshMetadata = false)
        {
            var bytes = Bytes[levelNumber];

            var json = Utility.Compress.Unzip(bytes);

            var chunks = JsonConvert.DeserializeObject<List<LevelChunk>>(json);

            Core.QueueAllForDelete();
            Core.PurgeAllDeletedTiles();

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

            foreach (var chunk in chunks)
            {
                ActorBase tile = null;

                object[] param = { Core };

                if (gameAssembly != null)
                {
                    var tileType = gameAssembly.GetType($"Game.Actors.{chunk.Meta.ActorClass}");
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
                    tile.RefreshMetadata(false);
                }

                Core.Actors.Add(tile);
            }
        }
    }
}
