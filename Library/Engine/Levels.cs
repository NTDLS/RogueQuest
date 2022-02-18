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

            var compressed = Utility.Compress.Zip(json);
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
            var json = Utility.Compress.Unzip(compressedSaveFile);
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
            var json = Utility.Compress.Unzip(compressedSaveFile);
            //string json = System.IO.File.ReadAllText(fileName);
            var saveFile = JsonConvert.DeserializeObject<SaveFile>(json);

            Collection = saveFile.Collection;

            Core.State = saveFile.State;
            Core.ScenarioMeta = saveFile.Meta;
            Core.Materials = saveFile.Materials;

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

            var fileJson = Utility.Compress.Unzip(compressedSaveFile);

            var saveFile = JsonConvert.DeserializeObject<SaveFile>(fileJson);

            var bytes = saveFile.Collection[levelNumber].Bytes;

            if (bytes.Length > 0)
            {
                var levelJson = Utility.Compress.Unzip(bytes);
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
                chunks.Add(new LevelChunk
                {
                    TilePath = obj.TilePath,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Velocity.Angle.Degrees == 0 ? null : obj.Velocity.Angle.Degrees,
                    DrawOrder = obj.DrawOrder == 0 ? null : obj.DrawOrder,
                    Meta = obj.Meta
                });
            }

            var json = JsonConvert.SerializeObject(chunks,
                Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var compressed = Utility.Compress.Zip(json);

            this.Collection[levelNumber].Bytes = compressed;
            Collection[levelNumber].LastEditBackgroundOffset = Core.Display.BackgroundOffset;
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
                var json = Utility.Compress.Unzip(bytes);
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

                        tilePath = Constants.GetAssetPath($"{chunk.TilePath}.png");

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

                        tile.SetImage(tilePath);
                        tile.X = chunk.X;
                        tile.Y = chunk.Y;
                        tile.TilePath = chunk.TilePath;
                        tile.Velocity.Angle.Degrees = chunk.Angle ?? 0;
                        tile.DrawOrder = chunk.DrawOrder;
                        tile.Meta = chunk.Meta;

                        if (refreshMetadata)
                        {
                            tile.RefreshMetadata(false);
                        }

                        if (_gameAssembly != null && tile.Meta.ActorClass == Types.ActorClassName.ActorSpawner)
                        {
                            tile = InsertSpawnedTile(tile);
                        }

                        if (tile != null)
                        {
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

        /// <summary>
        /// Inserts a spawn tile. (e.g. a random tile of the type specified by the spawn tile).
        /// </summary>
        /// <param name="spawner"></param>
        /// <returns></returns>
        private ActorBase InsertSpawnedTile(ActorBase spawner)
        {
            ActorBase tile = null;
            TileIdentifier randomTile = null;

            object[] param = { Core };

            if (spawner.Meta.SpawnType == Types.ActorClassName.ActorHostileBeing)
            {
                var randos = Core.Materials.Where(o => o.Meta.ActorClass == spawner.Meta.SpawnType
                    && o.Meta.Level >= (o.Meta.MinLevel ?? 1)
                    && o.Meta.Level <= (o.Meta.MaxLevel ?? 1)).ToList();

                if (randos.Count > 0)
                {
                    int rand = Utility.MathUtility.RandomNumber(0, randos.Count);
                    randomTile = randos[rand];

                    var tileType = _gameAssembly.GetType($"Game.Actors.{randomTile.Meta.ActorClass}");

                    tile = (ActorBase)Activator.CreateInstance(tileType, param);
                }
                else
                {

                }
            }
            else
            {
                throw new NotImplementedException();
            }

            if (randomTile != null)
            {
                tile.SetImage(Constants.GetAssetPath($"{randomTile.TilePath}.png"));
                tile.X = spawner.X;
                tile.Y = spawner.Y;
                tile.TilePath = randomTile.TilePath;
                tile.Velocity.Angle.Degrees = tile.Velocity.Angle.Degrees;
                tile.DrawOrder = spawner.DrawOrder;
                tile.Meta = TileMetadata.GetFreshMetadata(randomTile.TilePath);

                var ownedItems = Core.State.Items.Where(o => o.ContainerId == spawner.Meta.UID).ToList();
                foreach (var ownedItem in ownedItems)
                {
                    ownedItem.ContainerId = tile.Meta.UID;
                }
            }

            return tile;
        }
    }
}
