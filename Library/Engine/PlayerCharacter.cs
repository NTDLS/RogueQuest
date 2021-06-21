using System;

namespace Library.Engine
{
    public class PlayerCharacter
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public int StartingConstitution { get; set; }
        public int StartingDexterity { get; set; }
        public int StartingIntelligence { get; set; }
        public int StartingStrength { get; set; }

        public int AdditionalConstitution { get; set; }
        public int AdditionalDexterity { get; set; }
        public int AdditionalIntelligence { get; set; }
        public int AdditionalStrength { get; set; }

        public int Experience { get; set; }
        public int Level { get; set; }
        public int Money { get; set; }
        public int Hitpoints { get; set; } //Remaining
        public int Manna { get; set; } //Remaining
    }
}
