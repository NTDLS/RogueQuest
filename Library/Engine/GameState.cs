using System.Collections.Generic;

namespace Library.Engine
{
    public class GameState
    {
        private EngineCoreBase _core;

        public GameState(EngineCoreBase core)
        {
            _core = core;
            Character = new PlayerState(core);
        }

        public void SetCore(EngineCoreBase core)
        {
            _core = core;
            Character.SetCore(core);
        }

        public PlayerState Character { get; set; }
        public List<CustodyItem> Items { get; private set; } = new List<CustodyItem>();

        /// <summary>
        /// The level/map-number that the player is currently on.
        /// </summary>
        public int CurrentLevel { get; set; }
        public int DefaultLevel { get; set; }
        public bool IsDialogActive { get; set; }
        public int TimePassed { get; set; } = 0;
        public List<PersistentStore> Stores { get; set; } = new List<PersistentStore>();
    }
}
