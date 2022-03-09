using Assets;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Library.Engine
{
    public class EngineCoreBase
    {
        #region Public properties.

        /// <summary>
        /// Whether the map will be slowly revealed to the player.
        /// </summary>
        public bool BlindPlay { get; set; } = false;
        public int BlindPlayDistance { get; set; } = 200;
        public bool DrawMinimap { get; set; } = false;

        public Levels Levels { get; set; }
        public GameState State { get; set; }
        /// <summary>
        /// A list of all materials in the tile library when this level was initially created. Good for populating stores, spawns and drops.
        /// </summary>
        public List<TileIdentifier> Materials { get; set; }
        public ScenarioMetaData ScenarioMeta { get; set; }
        public bool IsRendering { get; private set; }
        public bool IsRunning { get; private set; }
        public EngineDisplay Display { get; private set; }
        public object CollectionSemaphore { get; private set; } = new object();
        public object DrawingSemaphore { get; private set; } = new object();
        public ActorController Actors { get; private set; }
        public Color BackgroundColor { get; private set; } = Color.FromArgb(46, 32, 60);

        #endregion

        private Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();

        #region Events.

        public delegate void StartEvent(EngineCoreBase sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(EngineCoreBase sender);
        public event StopEvent OnStop;

        #endregion

        public void Save(string fileName)
        {
            PushCurrentLevel();
            Levels.Save(fileName);
        }

        public void Load(string fileName)
        {
            Levels.Load(fileName);
            Display.DrawingSurface.Invalidate();
        }

        public void PushCurrentLevel()
        {
            PurgeAllDeletedTiles();
            Levels.PushLevel(State.CurrentLevel);
        }

        public void PopCurrentLevel()
        {
            Levels.PopLevel(State.CurrentLevel);
            Display.DrawingSurface.Invalidate();
        }

        public void SelectLevel(int levelIndex)
        {
            Levels.PushLevel(State.CurrentLevel);
            Levels.PopLevel(levelIndex);
            State.CurrentLevel = levelIndex;
        }

        public void SelectLevel(string levelName)
        {
            int levelIndex = Levels.GetIndex(levelName);
            Levels.PushLevel(State.CurrentLevel);
            Levels.PopLevel(levelIndex);
            State.CurrentLevel = levelIndex;
        }

        public void DeleteLevel(int levelIndex)
        {
            if (Levels.Collection.Count <= 1)
            {
                return;//Can't delete the last level.
            }

            if (levelIndex == State.CurrentLevel)
            {
                Levels.PopLevel(0);
                State.CurrentLevel = 0;
            }

            if (levelIndex == State.DefaultLevel)
            {
                State.DefaultLevel = 0;
            }

            Levels.Collection.RemoveAt(levelIndex);
        }

        /// <summary>
        /// Set the level number for the start of a new game.
        /// </summary>
        /// <param name="levelIndex"></param>
        public void SetDefaultLevel(int levelIndex)
        {
            State.DefaultLevel = levelIndex;
        }

        public void Reset()
        {
            QueueAllForDelete();
            PurgeAllDeletedTiles();

            Levels.Collection.Clear();
            Levels.AddNew("Home");

            State = new GameState(this);

            ScenarioMeta = new ScenarioMetaData()
            {
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Name = "",
                CreatedBy = "",
                Description = ""
            };

            Display.BackgroundOffset = new Point<double>();
        }

        public EngineCoreBase(Control drawingSurface, Size visibleSize)
        {
            Display = new EngineDisplay(drawingSurface, visibleSize);
            Levels = new Levels(this);
            State = new GameState(this);

            lock (CollectionSemaphore)
            {
                Actors = new ActorController(this);
            }
        }

        public void Start()
        {
            OnStart?.Invoke(this);
            IsRunning = true;
        }

        public void Stop()
        {
            if (IsRunning == false)
            {
                return;
            }

            IsRunning = false;

            OnStop?.Invoke(this);
        }

        public virtual void HandleSingleKeyPress(Keys key)
        {
        }

        /// <summary>
        /// Resets the metadata on all tiles and removes not-found / container-orphaned items.
        /// </summary>
        public void ResetAllTilesMetadata()
        {
            Actors.ResetAllTilesMetadata();

            var orphanedItems = new List<CustodyItem>();

            var _allLevelChunks = Levels.GetAllLevelsChunks();

            foreach (var obj in this.State.Items)
            {
                if (obj.ContainerId != null)
                {
                    bool found = false;

                    for (int level = 0; level < _allLevelChunks.Count; level++)
                    {
                        var parent = _allLevelChunks[level].Where(o => o.Meta.UID == obj.ContainerId).FirstOrDefault();

                        if (parent != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found == false)
                    {
                        orphanedItems.Add(obj);
                        continue;
                    }
                }

                if (File.Exists(Assets.Constants.GetAssetPath($"{obj.Tile.TilePath}.png")) == false)
                {
                    orphanedItems.Add(obj);
                }
                else
                {
                    Guid? uid = obj.Tile.Meta.UID;

                    var freshMeta = TileMetadata.GetFreshMetadata(obj.Tile.TilePath);
                    obj.Tile.Meta.OverrideWith(freshMeta);

                    if (uid != null)
                    {
                        obj.Tile.Meta.UID = (Guid)uid; //Never change the UID once it is set.
                    }

                    //All items need UIDs.
                    if (obj.Tile.Meta.UID == null)
                    {
                        obj.Tile.Meta.UID = Guid.NewGuid();
                    }
                }
            }

            foreach (var item in orphanedItems)
            {
                this.State.Items.Remove(item);
            }
        }

        public void ResizeDrawingSurface(Size visibleSize)
        {
            Display.ResizeDrawingSurface(visibleSize);

            lock (CollectionSemaphore)
            {
                _ScreenBitmap = null;
            }
        }

        public void QueueAllForDelete()
        {
            lock (this.CollectionSemaphore)
            {
                Actors.QueueAllForDelete();
            }
        }

        public void PurgeAllDeletedTiles()
        {
            lock (this.CollectionSemaphore)
            {
                var tilesToDelete = Actors.Tiles.Where(o => o.ReadyForDeletion);
                foreach (var tile in tilesToDelete)
                {
                    if (tile?.Meta?.UID != null)
                    {
                        State.ActorStates.RemoveAll((Guid)tile.Meta.UID);
                    }
                }

                Actors.Tiles.RemoveAll(o => o.ReadyForDeletion);
            }
        }

        private Bitmap _ScreenBitmap = null;
        private Graphics _ScreenDC = null;

        private Object _LatestFrameLock = new Object();

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public Bitmap Render()
        {
            IsRendering = true;

            bool lockTaken = false;
            var timeout = TimeSpan.FromMilliseconds(1);

            try
            {
                Monitor.TryEnter(DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (CollectionSemaphore)
                    {
                        if (_ScreenBitmap == null)
                        {
                            _ScreenBitmap = new Bitmap(Display.DrawingSurface.Width, Display.DrawingSurface.Height);

                            _ScreenDC = Graphics.FromImage(_ScreenBitmap);
                            _ScreenDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            _ScreenDC.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                            _ScreenDC.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            _ScreenDC.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            _ScreenDC.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        }

                        _ScreenDC.Clear(BackgroundColor);

                        Actors.Render(_ScreenDC);
                    }
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(DrawingSemaphore);
                }
            }

            IsRendering = false;

            return _ScreenBitmap;
        }


        private Assembly _gameAssembly = null;
        private bool _gameAssemblyAttempted = false;

        public Assembly GameAssembly
        {
            get
            {
                if (_gameAssembly == null && _gameAssemblyAttempted == false)
                {
                    _gameAssemblyAttempted = true;
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

                return _gameAssembly;
            }
        }


        /// <summary>
        /// Inserts a spawn tile. (e.g. a random tile of the type specified by the spawn tile).
        /// </summary>
        /// <param name="spawner"></param>
        /// <returns></returns>
        public TileIdentifier GetWeightedLotteryTile(TileMetadata spawnerMeta)
        {
            object[] param = { this };

            int rarity = Utility.MathUtility.RandomNumber(1, 100);

            var randos = Materials.Where(o => o.Meta.ActorClass == spawnerMeta.SpawnType
                && ((spawnerMeta.SpawnSubTypes?.Length ?? 0) == 0 || spawnerMeta.SpawnSubTypes.Contains(o.Meta.SubType ?? Types.ActorSubType.Unspecified))
                && o.Meta.Rarity >= rarity
                && (o.Meta.Level ?? 1) >= (o.Meta.MinLevel ?? 1)
                && (o.Meta.Level ?? 1) <= (o.Meta.MaxLevel ?? 1)).ToList();

            if (randos.Count > 0)
            {
                int rand = Utility.MathUtility.RandomNumber(0, randos.Count);
                var tile = randos[rand];

                tile.Meta = TileMetadata.GetFreshMetadata(tile.TilePath);

                if (tile.Meta.SubType == Types.ActorSubType.Money)
                {
                    double divisor = (100.0 * (tile.Meta.Value ?? 1.0));
                    tile.Meta.Quantity = Utility.MathUtility.RandomNumber(1, 1000);
                    tile.Meta.Quantity = (int)(tile.Meta.Quantity / divisor);
                    if ((tile.Meta.Quantity ?? 0) <= 0)
                    {
                        tile.Meta.Quantity = 1;
                    }
                }
                else if (tile.Meta.CanStack == true)
                {
                    tile.Meta.Quantity = 1;
                }

                return tile;
            }

            return null;
        }

        /// <summary>
        /// Inserts a spawn tile. (e.g. a random tile of the type specified by the spawn tile).
        /// </summary>
        /// <param name="spawner"></param>
        /// <returns></returns>
        public ActorBase GetWeightedLotteryActor(ActorBase spawner)
        {
            TileIdentifier randomTile = GetWeightedLotteryTile(spawner.Meta);

            if (randomTile != null)
            {
                object[] param = { this };

                var tileType = GameAssembly.GetType($"Game.Actors.{randomTile.Meta.ActorClass}");
                var actor = (ActorBase)Activator.CreateInstance(tileType, param);

                actor.SetImage(Constants.GetAssetPath($"{randomTile.TilePath}.png"));
                actor.X = spawner.X;
                actor.Y = spawner.Y;
                actor.TilePath = randomTile.TilePath;
                actor.Velocity.Angle.Degrees = actor.Velocity.Angle.Degrees;
                actor.DrawOrder = spawner.DrawOrder;
                actor.Meta.OverrideWith(randomTile.Meta);

                var ownedItems = State.Items.Where(o => o.ContainerId == spawner.Meta.UID).ToList();
                foreach (var ownedItem in ownedItems)
                {
                    ownedItem.ContainerId = actor.Meta.UID;
                }

                return actor;
            }

            return null;
        }
    }
}