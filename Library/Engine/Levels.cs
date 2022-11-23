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
                State = Core.State,
                Meta = Core.ScenarioMeta,
                Materials = Core.Materials
            };

            saveFile.Meta.ModifiedDate = DateTime.Now;

            var json = JsonConvert.SerializeObject(saveFile,
                Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var compressed = Native.Compression.Compress(json);
            System.IO.File.WriteAllBytes(fileName, compressed);
            //System.IO.File.WriteAllText(fileName, json);
        }

        /// <summary>
        /// Gets the metadata for a level file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ScenarioMetaData GetMetadata(string fileName)
        {
            var compressedSaveFile = System.IO.File.ReadAllBytes(fileName);
            var json = Native.Compression.DecompressString(compressedSaveFile);
            var saveFile = JsonConvert.DeserializeObject<ScenarioMetaEnumerator>(json);
            return saveFile.Meta;
        }

        /// <summary>
        /// Saves an entire game file, all levels, items and character state. All levels remain compressed in byte arrays.
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            var compressedSaveFile = System.IO.File.ReadAllBytes(fileName);
            var json = Native.Compression.DecompressString(compressedSaveFile);
            //string json = System.IO.File.ReadAllText(fileName);

            //json = json.Replace("\"Weapon\"", "\"MeleeWeapon\"");

            var saveFile = JsonConvert.DeserializeObject<SaveFile>(json);

            Collection = saveFile.Collection;

            Core.State = saveFile.State;
            Core.ScenarioMeta = saveFile.Meta;
            Core.Materials = saveFile.Materials;

            foreach (var item in Core.State.Items)
            {
                item.Tile.Meta = Core.AutoIdentifyItem(item.Tile.Meta);
            }
            foreach (var item in Core.Materials)
            {
                item.Meta = Core.AutoIdentifyItem(item.Meta);
            }

            if (Core.State == null)
            {
                Core.State = new GameState(Core);
            }
            if (Core.State.Character == null)
            {
                Core.State.Character = new PlayerState(Core);
            }

            Core.State.SetCore(Core);
        }

        public static List<LevelChunk> LoadChunks(string fileName, int levelNumber)
        {
            var compressedSaveFile = System.IO.File.ReadAllBytes(fileName);

            var fileJson = Native.Compression.DecompressString(compressedSaveFile);

            var saveFile = JsonConvert.DeserializeObject<SaveFile>(fileJson);

            var bytes = saveFile.Collection[levelNumber].Bytes;

            if (bytes.Length > 0)
            {
                var levelJson = Native.Compression.DecompressString(bytes);
                var chunks = JsonConvert.DeserializeObject<List<LevelChunk>>(levelJson);
                return chunks;
            }

            return new List<LevelChunk>();
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

        public Level ByName(string name)
        {
            return Collection[GetIndex(name)];
        }

        public Level ByIndex(int index)
        {
            return Collection[index];
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

            foreach (var obj in Core.Actors.Tiles.Where(o => o.ReadyForDeletion == false))
            {
                var chunk = new LevelChunk
                {
                    TilePath = obj.TilePath,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Velocity.Angle.Degrees == 0 ? null : obj.Velocity.Angle.Degrees,
                    DrawOrder = obj.DrawOrder == 0 ? null : obj.DrawOrder,
                    Meta = obj.Meta
                };

                if (chunk.Meta.ActorClass == Types.ActorClassName.ActorTerrain && chunk.Meta.UID != null)
                {
                    //We really shouldn't have UIDs for terrain tiles. They just take up space.
                    chunk.Meta.UID = null;
                }

                chunks.Add(chunk);
            }

            var json = JsonConvert.SerializeObject(chunks,
                Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var compressed = Native.Compression.Compress(json);

            this.Collection[levelNumber].Bytes = compressed;
            Collection[levelNumber].LastEditBackgroundOffset = Core.Display.BackgroundOffset;
        }

        public List<LevelChunk> GetChunks(int levelNumber)
        {
            var bytes = Collection[levelNumber].Bytes;

            if (bytes.Length > 0)
            {
                var json = Native.Compression.DecompressString(bytes);
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

        public int FailedToLoadTilesCount { get; set; }
        public List<string> LoadErrors { get; set; } = new List<string>();

        private Assembly _gameAssembly = null;

        /// <summary>
        /// Decompresses a level from bytes and pushes it to the game tiles. This is how you change maps.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="refreshMetadata"></param>
        public void PopLevel(int levelNumber, bool refreshMetadata = false)
        {
            FailedToLoadTilesCount = 0;
            LoadErrors = new List<string>();

            var bytes = Collection[levelNumber].Bytes;

            Core.QueueAllForDelete();
            Core.PurgeAllDeletedTiles();

            if (bytes.Length > 0)
            {
                var json = Native.Compression.DecompressString(bytes);
                var chunks = JsonConvert.DeserializeObject<List<LevelChunk>>(json);

                if (_gameAssembly == null)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    var assemblies = currentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        if (assembly.FullName.StartsWith("Game,"))
                        {
                            _gameAssembly = Assembly.Load("Game");
                        }
                    }
                }

                foreach (var chunk in chunks)
                {
                    string tilePath = string.Empty;

                    try
                    {
                        ActorBase tile = null;
                        object[] param = { Core };

                        tilePath = Constants.GetCommonAssetPath($"{chunk.TilePath}.png");

                        //If the game assembly is loaded them create the actual tile class.
                        if (_gameAssembly != null)
                        {
                            var tileType = _gameAssembly.GetType($"Game.Actors.{chunk.Meta.ActorClass}");
                            if (tileType != null)
                            {
                                tile = (ActorBase)Activator.CreateInstance(tileType, param);
                            }
                        }

                        if (tile == null)
                        {
                            tile = (ActorBase)Activator.CreateInstance(Type.GetType("Library.Engine.ActorBase"), param);
                        }

                        tile.TilePath = chunk.TilePath;
                        tile.X = chunk.X;
                        tile.Y = chunk.Y;
                        tile.Velocity.Angle.Degrees = chunk.Angle ?? 0;
                        tile.DrawOrder = chunk.DrawOrder;
                        tile.Meta = chunk.Meta;
                        tile.SetImage(tile.ImagePath);

                        if (refreshMetadata)
                        {
                            tile.RefreshMetadata(false);
                        }

                        if (_gameAssembly != null && tile.Meta.ActorClass == Types.ActorClassName.ActorSpawner)
                        {
                            tile = Core.GetWeightedLotteryActor(tile);
                        }

                        if (tile != null)
                        {
                            if (tile.Meta.ActorClass == Types.ActorClassName.ActorTerrain && tile.Meta.UID != null)
                            {
                                //We really shouldn't have UIDs for terrain tiles. They just take up space.
                                tile.Meta.UID = null;
                            }

                            tile.Meta = Core.AutoIdentifyItem(tile.Meta);

                            Core.Actors.Add(tile);
                        }
                    }
                    catch
                    {
                        FailedToLoadTilesCount++;
                        LoadErrors.Add($"Failed to load {tilePath}.");
                    }
                }
            }
        }
    }
}
