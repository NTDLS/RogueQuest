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
        public int Damage { get; private set; }
        public int Dexterity { get; private set; }

        public void InitializeState()
        {
            Hitpoints = 10 + StartingConstitution;
            Manna = 5 + StartingIntelligence;
            Dexterity = 1 + StartingDexterity;
            Damage = 1 + StartingStrength;
            MaxWeight = 250 + StartingStrength;

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;

            NextLevelExperience = 300;
        }

        public void LevelUp()
        {
            Level++;

            NextLevelExperience = (int)(((float)Experience) * 1.5f);

            Hitpoints += 6 + StartingConstitution + AugmentedConstitution;
            Manna += 6 + StartingIntelligence + AugmentedIntelligence;
            Dexterity += 6 + StartingDexterity + AugmentedDexterity;
            Damage += 6 + StartingStrength + AugmentedStrength;
            MaxWeight += 20 + StartingStrength + AugmentedStrength;

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
