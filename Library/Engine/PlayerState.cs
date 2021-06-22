using Newtonsoft.Json;
using System;

namespace Library.Engine
{
    public class PlayerState
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public int StartingConstitution { get; set; }
        public int StartingDexterity { get; set; }
        public int StartingIntelligence { get; set; }
        public int StartingStrength { get; set; }

        public int AugmentedConstitution { get; set; }
        public int AugmentedDexterity { get; set; }
        public int AugmentedIntelligence { get; set; }
        public int AugmentedStrength { get; set; }

        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int Level { get; set; }
        public int Money { get; set; }

        public int AvailableHitpoints { get; set; }
        public int AvailableMana { get; set; }

        public int BaseHitpoints { get; set; }
        public int BaseManna { get; set; }
        public int BaseMaxWeight { get; set; }
        public int BaseDamage { get; set; }
        public int BaseDexterity { get; set; }

        public int Hitpoints => BaseHitpoints; //+any weapons, magicical modifiers.
        public int Manna => BaseManna; //+any weapons, magicical modifiers.
        public int MaxWeight => BaseMaxWeight; //+any weapons, magicical modifiers.
        public int Damage => BaseDamage; //+any weapons, magicical modifiers.
        public int Dexterity => BaseDexterity; //+any weapons, magicical modifiers.

        public void InitializeState()
        {
            BaseHitpoints = 10 + StartingConstitution;
            BaseManna = 10 + StartingIntelligence;
            BaseDexterity = 10 + StartingDexterity;
            BaseDamage = 10 + StartingStrength;
            BaseMaxWeight = 10 + StartingStrength;

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;

            NextLevelExperience = 300;
        }

        public void LevelUp()
        {
            Level++;

            NextLevelExperience = (int)(((float)Experience) * 1.5f);

            //NextLevelExperience = Experience + 10;

            BaseHitpoints += 6 + StartingConstitution + AugmentedConstitution;
            BaseManna = +6 + StartingIntelligence + AugmentedIntelligence;
            BaseDexterity += 6 + StartingDexterity + AugmentedDexterity;
            BaseDamage += 6 + StartingStrength + AugmentedStrength;
            BaseMaxWeight += 20 + StartingStrength + AugmentedStrength;

            AvailableHitpoints = Hitpoints;
            AvailableMana = Manna;
        }
    }
}
