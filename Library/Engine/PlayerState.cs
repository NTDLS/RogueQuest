using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Engine
{
    public class PlayerState
    {
        private EngineCoreBase _core;

        public PlayerState(EngineCoreBase core)
        {
            _core = core;
            Equipment = new List<Equip>();
        }

        public void SetCore(EngineCoreBase core)
        {
            _core = core;
        }

        public List<Equip> Equipment { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public int Avatar { get; set; }

        //These are the attributes that the player started with from character creation.
        public int StartingConstitution { get; set; }
        public int StartingDexterity { get; set; }
        public int StartingIntelligence { get; set; }
        public int StartingStrength { get; set; }

        //Augmented attributes can be increased each time the character levels up/
        public int AugmentedConstitution { get; set; }
        public int AugmentedDexterity { get; set; }
        public int AugmentedIntelligence { get; set; }
        public int AugmentedStrength { get; set; }

        //Starting + Augmented attributes.
        public int Constitution => StartingConstitution + AugmentedConstitution;
        public int Dexterity => StartingDexterity + AugmentedDexterity;
        public int Intelligence => StartingIntelligence + AugmentedIntelligence;
        public int Strength => StartingStrength + AugmentedStrength;
        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int Level { get; set; }

        /// <summary>
        /// Gets the aggregate amount of money in gold equivalent.
        /// </summary>
        public int Money
        {
            get
            {
                //Get the purse that is equiped.
                var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
                if (equipSlot != null && equipSlot.Tile != null)
                {
                    //Find all the money in the purse.
                    var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                    int value = (int)(money.Sum(o => o.Tile.Meta.Quantity * o.Tile.Meta.Value) ?? 0.0);

                    return value;
                }
                return 0;
            }
        }

        //Availabe start at the base rates and are reduced as consumed.
        private int _availableHitpoints;
        public int AvailableHitpoints
        {
            get
            {
                return _availableHitpoints;

            }
            set
            {
                if (value < 0)
                {
                    _availableHitpoints = 0;
                }
                else
                { _availableHitpoints = value;
                }
            }
        }
        public int AvailableMana { get; set; }

        //These are the base values.
        public int Hitpoints { get; set; }
        public int Manna { get; set; }
        public int MaxWeight { get; set; }

        public void InitializeState()
        {
            Hitpoints = 5 + StartingConstitution;
            Manna = StartingIntelligence;
            MaxWeight = 250 + (StartingStrength * 10);

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;

            NextLevelExperience = 300;
        }

        public void LevelUp()
        {
            Level++;

            NextLevelExperience = (int)(((float)NextLevelExperience) * 1.5f);

            Hitpoints += 6 + StartingConstitution + (AugmentedConstitution * 3);
            Manna += 6 + StartingIntelligence + (AugmentedIntelligence * 3);
            MaxWeight += 20 + StartingStrength + (AugmentedStrength * 5);

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;
        }

        public Equip GetEquipSlot(EquipSlot slot)
        {
            var equip = this.Equipment.Where(o => o.Slot == slot).FirstOrDefault();
            if (equip == null)
            {
                equip = new Equip() { Slot = slot };
                this.Equipment.Add(equip);
            }

            return equip;
        }
    }
}