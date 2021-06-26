using Library.Engine.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Engine
{
    public class PlayerState
    {
        public List<Equip> Equipment { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }

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

        //Availabe start at the base rates and are reduced as consumed.
        public int AvailableHitpoints { get; set; }
        public int AvailableMana { get; set; }

        //These are the base values.
        public int Hitpoints { get; private set; }
        public int Manna { get; private set; }
        public int MaxWeight { get; private set; }

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

            NextLevelExperience = (int)(((float)Experience) * 1.5f);

            Hitpoints += 6 + StartingConstitution + (AugmentedConstitution * 3);
            Manna += 6 + StartingIntelligence + (AugmentedIntelligence * 3);
            MaxWeight += 20 + StartingStrength + (AugmentedStrength * 5);

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;
        }

        public PlayerState()
        {
            Equipment = new List<Equip>();
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
