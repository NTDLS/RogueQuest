using System.Collections.Generic;
using System.Linq;

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
            ActiveThreadCount = 0;
        }

        public ActorStates ActorStates { get; set; } = new ActorStates();
        public PlayerState Character { get; set; }
        public List<CustodyItem> Items { get; private set; } = new List<CustodyItem>();

        /// <summary>
        /// The level/map-number that the player is currently on.
        /// </summary>
        public int CurrentLevel { get; set; }
        public int DefaultLevel { get; set; }
        public bool IsDialogActive { get; set; }
        public int ActiveThreadCount { get; private set; } = 0;
        public int TimePassed { get; set; } = 0;
        public List<PersistentStore> Stores { get; set; } = new List<PersistentStore>();


        public void AddThreadReference()
        {
            lock (this)
            {
                ActiveThreadCount++;
            }
        }

        public void RemoveThreadReference()
        {
            lock (this)
            {
                ActiveThreadCount--;
            }
        }


        public CustodyItem GetOrCreateInventoryItem(TileIdentifier tile)
        {
            var inventoryItem = _core.State.Items.Where(o => o.Tile.Meta.UID == tile.Meta.UID).FirstOrDefault();
            if (inventoryItem == null)
            {
                inventoryItem = new CustodyItem()
                {
                    Tile = tile.Clone()
                };

                _core.State.Items.Add(inventoryItem);
            }
            return inventoryItem;
        }
    }
}
