using Game.Actors;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game.Engine
{
    public class EngineCore : EngineCoreBase
    {
        public delegate void BeforeTickEvent(EngineCore core, Types.TickInput input);
        public event BeforeTickEvent BeforeTick;

        public delegate void AfterTickEvent(EngineCore core, Types.TickInput input, Point<double> offsetApplied);
        public event AfterTickEvent AfterTick;

        public EngineTickController Tick { get; set; }

        public ActorPlayer Player { get; set; }

        public delegate void LogEvent(EngineCore core, string text, Color color);
        public event LogEvent OnLog;

        public EngineCore(Control drawingSurface, Size visibleSize)
            : base(drawingSurface, visibleSize)
        {
            Tick = new EngineTickController(this);
        }

        public void Log(string text, Color color)
        {
            OnLog?.Invoke(this, text, color);
        }

        public void Log(string text)
        {
            OnLog?.Invoke(this, text, Color.Black);
        }

        public void LogLine(string text)
        {
            OnLog?.Invoke(this, "\r\n" + text, Color.Black);
        }

        public void LogLine(string text, Color color)
        {
            OnLog?.Invoke(this, "\r\n" + text, color);
        }

        public void LevelWarp(string levelName, Guid targetTileUID)
        {
            SelectLevel(levelName);

            var spawnPoint = Actors.Tiles.Where(o => o.Meta.UID == targetTileUID).FirstOrDefault();
            if (spawnPoint == null)
            {
                MessageBox.Show("This level contains no Spawn Point and cannot be played.");
                return;
            }

            var existingPlayers = Actors.OfType<ActorPlayer>().ToList();
            foreach (var player in existingPlayers)
            {
                player.QueueForDelete();
            }
            PurgeAllDeletedTiles();

            Actors.AddNew<ActorPlayer>(spawnPoint.X, spawnPoint.Y, @$"Tiles\Special\@Player\{this.State.Character.Avatar}\Front 1");
            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
            this.Player.DrawOrder = Actors.Tiles.Max(o => o.DrawOrder) + 1;
            this.Player.Meta = GetPlayerMeta();
            this.Display.BackgroundOffset.Y = Player.Y - (this.Display.DrawingSurface.Height / 2);
            this.Display.BackgroundOffset.X = Player.X - (this.Display.DrawingSurface.Width / 2);
        }

        private TileMetadata GetPlayerMeta()
        {
            return new TileMetadata()
            {
                ActorClass = Library.Engine.Types.ActorClassName.ActorPlayer,
                CanWalkOn = false,
                UID = this.State.Character.UID
            };
        }

        public void NewGame(string scenarioFile, string characterName, int avatar,
            int dexterity, int constitution, int intelligence, int strength)
        {
            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();

            Load(scenarioFile);

            this.State.Character = new PlayerState(this)
            {
                UID = Guid.NewGuid(),
                Experience = 0,
                Name = characterName,
                Avatar = avatar,
                Level = 1,
                StartingDexterity = dexterity,
                StartingConstitution = constitution,
                StartingIntelligence = intelligence,
                StartingStrength = strength
            };

            this.State.Character.InitializeState();

            this.State.CurrentLevel = this.State.DefaultLevel;

            Levels.PopLevel(this.State.CurrentLevel);

            var spawnPoint = Actors.OfType<ActorSpawnPoint>().FirstOrDefault();
            if (spawnPoint == null)
            {
                MessageBox.Show("This level contains no Spawn Point and cannot be played.");
                return;
            }

            Actors.AddNew<ActorPlayer>(spawnPoint.X, spawnPoint.Y, @$"Tiles\Special\@Player\{this.State.Character.Avatar}\Front 1");
            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
            this.Player.DrawOrder = Actors.Tiles.Max(o => o.DrawOrder) + 1;
            this.Player.Meta = GetPlayerMeta();
            this.Display.BackgroundOffset.Y = this.Player.Y - (this.Display.DrawingSurface.Height / 2.0);
            this.Display.BackgroundOffset.X = this.Player.X - (this.Display.DrawingSurface.Width / 2.0);

            var purseTile = this.Materials.Where(o => o.Meta.SubType == ActorSubType.Purse).First().Clone(true);

            var goldTile = this.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Gold")).First().Clone(true);
            goldTile.Meta.Quantity = 8;
            var silverTile = this.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Silver")).First().Clone(true);
            silverTile.Meta.Quantity = 22;
            var copperTile = this.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Copper")).First().Clone(true);
            copperTile.Meta.Quantity = 142;

            this.State.Items.Add(new CustodyItem() { Tile = purseTile });
            this.State.Items.Add(new CustodyItem() { Tile = goldTile, ContainerId = purseTile.Meta.UID });
            this.State.Items.Add(new CustodyItem() { Tile = silverTile, ContainerId = purseTile.Meta.UID });
            this.State.Items.Add(new CustodyItem() { Tile = copperTile, ContainerId = purseTile.Meta.UID });

            var equipSlot = this.State.Character.GetEquipSlot(EquipSlot.Purse);
            equipSlot.Tile = purseTile;
        }

        /// <summary>
        /// This is the user friendly load method. Dont call the base.Load directly because it leaves the character off screen n' stuff.
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadGame(string fileName)
        {
            FormWelcome.AddToRecentList(fileName);

            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();

            Load(fileName);

            PopCurrentLevel();

            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();

            if (this.Player == null)
            {
                //This is really only used when opening a scenario directly with the game - e.g. for debugging.
                var spawnPoint = Actors.OfType<ActorSpawnPoint>().FirstOrDefault();
                if (spawnPoint == null)
                {
                    MessageBox.Show("This level contains no Spawn Point and cannot be played.");
                    return;
                }

                this.State.Character = new PlayerState(this)
                {
                    UID = Guid.NewGuid(),
                    Experience = 0,
                    Name = "Bug Slayer",
                    Avatar = 1,
                    Level = 1,
                    StartingDexterity = 10,
                    StartingConstitution = 10,
                    StartingIntelligence = 10,
                    StartingStrength = 10
                };

                this.State.Character.InitializeState();

                Actors.AddNew<ActorPlayer>(spawnPoint.X, spawnPoint.Y, @$"Tiles\Special\@Player\{this.State.Character.Avatar}\Front 1");
                this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
                this.Player.DrawOrder = Actors.Tiles.Max(o => o.DrawOrder) + 1;
                this.Player.Meta = GetPlayerMeta();
            }

            this.Display.BackgroundOffset.Y = this.Player.Y - (this.Display.DrawingSurface.Height / 2.0);
            this.Display.BackgroundOffset.X = this.Player.X - (this.Display.DrawingSurface.Width / 2.0);

            LogLine("Game loaded.");
        }

        /// <summary>
        /// This is the user frendly save game method, do not call save direclty from user code.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveGame(string fileName)
        {
            FormWelcome.AddToRecentList(fileName);
            Save(fileName);
            LogLine("Game saved.");
        }

        public ActorTextBlock AddNewTextBlock(string font, Brush color, double size, double x, double y, bool isPositionStatic, string text = "")
        {
            lock (CollectionSemaphore)
            {
                var obj = new ActorTextBlock(this, font, color, size, new Point<double>(x, y), isPositionStatic)
                {
                    Text = text
                };
                Actors.Add(obj);
                return obj;
            }
        }

        public void ActionRest()
        {
            var input = new Types.TickInput() { InputType = Types.TickInputType.Rest };
            Tick.Rest();
            AfterTick?.Invoke(this, input, new Point<double>());
        }

        public void ActionGet()
        {
            var input = new Types.TickInput() { InputType = Types.TickInputType.Get };
            Tick.Get();
            AfterTick?.Invoke(this, input, new Point<double>());
        }

        public void ActionDialogInput()
        {
            var input = new Types.TickInput() { InputType = Types.TickInputType.DialogInput };
            Tick.HandleDialogInput();
            AfterTick?.Invoke(this, input, new Point<double>());
        }

        public override void HandleSingleKeyPress(Keys key)
        {
            if (Player == null || Player.Visible == false)
            {
                return;
            }

            Types.TickInput input = null;

            if (key == Keys.NumPad1 || key == Keys.Z)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 225, Throttle = 1 };
            }
            else if (key == Keys.NumPad2 || key == Keys.Down || key == Keys.S)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 180, Throttle = 1 };
            }
            else if (key == Keys.NumPad3 || key == Keys.X)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 135, Throttle = 1 };
            }
            else if (key == Keys.NumPad4 || key == Keys.Left || key == Keys.A)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 270, Throttle = 1 };
            }
            else if (key == Keys.NumPad6 || key == Keys.Right || key == Keys.D)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 90, Throttle = 1 };
            }
            else if (key == Keys.NumPad7 || key == Keys.Q)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 315, Throttle = 1 };
            }
            else if (key == Keys.NumPad8 || key == Keys.Up || key == Keys.W)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 0, Throttle = 1 };
            }
            else if (key == Keys.NumPad9 || key == Keys.E)
            {
                input = new Types.TickInput { InputType = Types.TickInputType.Movement, Degrees = 45, Throttle = 1 };
            }

            if (input != null)
            {
                BeforeTick?.Invoke(this, input);
                var offsetApplied = Tick.Advance(input);
                AfterTick?.Invoke(this, input, offsetApplied);
            }
        }
    }
}
