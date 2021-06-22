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

        public void NewGame(string characterName, int dexterity, int constitution, int intelligence, int strength)
        {
            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();

            Level.Load(this, Assets.Constants.GetAssetPath(@"Maps\MapHome.rqm"));

            this.State = new GameState()
            {
                CurrentMap = "MapHome"
            };

            this.State.Character = new PlayerState()
            {
                UID = Guid.NewGuid(),
                Experience = 0,
                Name = characterName,
                Level = 1,
                StartingDexterity = dexterity,
                StartingConstitution = constitution,
                StartingIntelligence = intelligence,
                StartingStrength = strength
            };

            this.State.Character.InitializeState();

            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
        }

        public void LoadGame(string fileName)
        {
            this.QueueAllForDelete();
            this.PurgeAllDeletedTiles();
            Level.Load(this, fileName);
            this.Player = Actors.OfType<ActorPlayer>().FirstOrDefault();
        }

        public void SaveGame(string fileName)
        {
            Level.Save(this, fileName, this.State);
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

        public void Rest()
        {
            var input = new Types.TickInput() { InputType = Types.TickInputType.Rest };
            Tick.Rest();
            AfterTick?.Invoke(this, input, new Point<double>());
        }

        public override void HandleSingleKeyPress(Keys key)
        {
            if (Player == null || Player.Visible == false)
            {
                return;
            }

            var input = new Types.TickInput()
            {
                InputType = Types.TickInputType.Keyboard,
                Key = key
            };

            BeforeTick?.Invoke(this, input);
            var offsetApplied = Tick.Advance(input);
            AfterTick?.Invoke(this, input, offsetApplied);
        }
    }
}
