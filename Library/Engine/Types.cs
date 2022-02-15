namespace Library.Engine.Types
{
    public enum EquipSlot
    {
        Pack,
        Belt,
        RightRing,
        Weapon,
        Bracers,
        Armor,
        Boots,
        Necklace,
        Garment,
        Helment,
        Shield,
        Gauntlets,
        FreeHand,
        LeftRing,
        Purse
    }

    public enum ActorClassName
    {
        ActorStore,
        ActorBuilding,
        ActorFriendyBeing,
        ActorHostileBeing,
        ActorItem,
        ActorPlayer,
        ActorTerrain,
        ActorTextBlock,
        ActorSpawnPoint, //This is where the player spawns on each level.
        ActorLevelWarpHidden,
        ActorLevelWarpVisible,
        ActorDialog,
        ActorSpawner, //This is used to spawn random actors.
        ActorBlockaid, //This is like a wall. It cannot be intersected at all.
        ActorWarpTarget
    }

    public enum ActorSubType
    {
        Unspecified,
        Pack,
        Belt,
        Ring,
        Weapon,
        Bracers,
        Armor,
        Necklace,
        Garment,
        Helment,
        Shield,
        Gauntlets,
        Boots,
        Potion,
        Scroll,
        Wand,
        Money,
        Chest,
        Purse,
        AlchemistStore,
        MageStore,
        ArmorSmithStore,
        WeaponSmithStore,
        RuinsStore
    }

    public enum ItemEffect
    {
        Heal,
        Poison,
        CurePoison,
        IncreaseStrength,
        IncreaseDexterity,
        IncreaseConstitution,
        IncreaseArmorClass,
        IncreaseIntelligence
    }

    public enum StateOfBeing
    {
        Poisoned,
        Slowed,
        Frozen,
        IncreaseStrength,
        IncreaseDexterity,
        IncreaseConstitution,
        IncreaseArmorClass,
        IncreaseIntelligence //Also increases mana.
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }
}
