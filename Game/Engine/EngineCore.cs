using Library.Types;
using Game.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Engine;

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
            : base (drawingSurface, visibleSize)
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

        public void NewGame(string characterName, int avatar,
            int dexterity, int constitution, int intelligence, int strength)
        {
            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();

            Load(Assets.Constants.GetAssetPath(@"Scenario\Default Scenario.rqm"));

            this.State.Character = new PlayerState()
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

            Actors.AddNew<ActorPlayer>(spawnPoint.X, spawnPoint.Y, @$"Tiles\Player\{avatar}\Front 1");
            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
            this.Player.DrawOrder = Actors.Tiles.Max(o => o.DrawOrder) + 1;
            spawnPoint.QueueForDelete();

            this.Display.BackgroundOffset.Y = Player.Y / 2;
            this.Display.BackgroundOffset.X = Player.X / 2;
        }

        /// <summary>
        /// This is the user friendly load method. Dont call the base.Load directly because it leaves the character off screen n' stuff.
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadGame(string fileName)
        {
            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();

            Load(fileName);

            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();

            this.Display.BackgroundOffset.Y = Player.Y / 2;
            this.Display.BackgroundOffset.X = Player.X / 2;

            LogLine("Game loaded.");
        }

        /// <summary>
        /// This is the user frendly save game method, do not call save direclty from user code.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveGame(string fileName)
        {
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
