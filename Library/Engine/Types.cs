namespace Library.Engine.Types
{
    public enum ReplayMode
    {
        SinglePlay,
        LoopedPlay,
        StillFrame
    }

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
        Purse,
        Projectile1,
        Projectile2,
        Projectile3,
        Projectile4,
        Projectile5
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
        ActorWarpTarget,
        ActorAnimation
    }

    public enum ActorSubType
    {
        Unspecified,
        Pack,
        Belt,
        Ring,
        MeleeWeapon,
        RangedWeapon,
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
        Projectile,
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

    public enum ProjectileType
    {
        Unspecified,
        Arrow,
        Bolt
    }

    public enum ItemEffect
    {
        Unspecified,
        Heal,
        Poison,
        CurePoison,
        IncreaseStrength,
        IncreaseDexterity,
        IncreaseConstitution,
        IncreaseArmorClass,
        IncreaseIntelligence,
        MagicArrow
    }

    public enum StateOfBeing
    {
        Unspecified,
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
