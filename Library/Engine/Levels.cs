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
        public List<Level> Collection { get; set; } = new List<Level>();

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
            var saveFile = new SaveFile()
            {
                Collection = this.Collection,
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

            var saveFile = JsonConvert.DeserializeObject<SaveFile>(json);

            Collection = saveFile.Collection;

            Core.State = saveFile.State;

            if (Core.State == null)
            {
                Core.State = new GameState();
            }
            if (Core.State.Character == null)
            {
                Core.State.Character = new PlayerState();
            }
        }

        public bool RenameLevel(string oldName, string newName)
        {
            if (oldName == newName)
            {
                return true;
            }

            if (Collection.Where(o => o.Name == newName).Any())
            {
                return false; //Duplicate name exists
            }

            int levelIndex = GetIndex(oldName);
            Core.Levels.Collection[levelIndex].Name = newName;

            return true;
        }

        public int GetIndex(string name)
        {
            for (int i = 0; i < this.Collection.Count; i++)
            {
                if (Collection[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Adds a new level to the collection and returns its index.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int AddNew(string name)
        {
            this.Collection.Add(new Level()
            {
                Bytes = new byte[0],
                Name = name
            });

            return this.Collection.Count - 1;
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

            this.Collection[levelNumber].Bytes = compressed;
        }

        public List<LevelChunk> GetChunks(int levelNumber)
        {
            var bytes = Collection[levelNumber].Bytes;

            if (bytes.Length > 0)
            {
                var json = Utility.Compress.Unzip(bytes);
                var chunks = JsonConvert.DeserializeObject<List<LevelChunk>>(json);
                return chunks;

            }

            return new List<LevelChunk>();
        }

        public List<List<LevelChunk>> GetAllLevelsChunks()
        {
            var chunks = new List<List<LevelChunk>>();

            for (int level = 0; level < Collection.Count; level++)
            {
                if (chunks.Count < level + 1)
                {
                    chunks.Add(new List<LevelChunk>());
                }

                chunks[level].AddRange(GetChunks(level));
            }

            return chunks;
        }

        /// <summary>
        /// Decompresses a level from bytes and pushes it to the game tiles. This is how you change maps.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="refreshMetadata"></param>
        public void PopLevel(int levelNumber, bool refreshMetadata = false)
        {
            var bytes = Collection[levelNumber].Bytes;

            Core.QueueAllForDelete();
            Core.PurgeAllDeletedTiles();

            if (bytes.Length > 0)
            {
                var json = Utility.Compress.Unzip(bytes);
                var chunks = JsonConvert.DeserializeObject<List<LevelChunk>>(json);

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
                        if (tileType != null)
                        {
                            tile = (ActorBase)Activator.CreateInstance(tileType, param);
                        }
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
}
