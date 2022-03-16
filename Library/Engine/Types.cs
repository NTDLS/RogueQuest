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
        Unspecified, //¯\_(ツ)_/¯
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
        ActorHiddenMessage,
        ActorLevelWarpVisible,
        ActorBlockadeHidden,
        ActorDialog,
        ActorSpawner, //This is used to spawn random actors.
        ActorBlockaid, //This is like a wall. It cannot be intersected at all.
        ActorWarpTarget,
        ActorAnimation
    }

    public enum ActorSubType
    {
        Unspecified, //¯\_(ツ)_/¯
        Pack, //Container
        Belt, //Container for quick-use slots
        Ring, //Enchanted/cursed item.
        MeleeWeapon, //Weapon that you hit things with by hand,
        RangedWeapon, //Weapon you hit things with from afar.
        Bracers, //Body armor, can be enchanted/cursed.
        Armor, //Body armor, can be enchanted/cursed.
        Necklace, //Enchanted/cursed item.
        Garment, //Body armor, can be enchanted/cursed.
        Helment, //Body armor, can be enchanted/cursed.
        Shield, //Body armor, can be enchanted/cursed.
        Gauntlets, //Body armor, can be enchanted/cursed.
        Boots, //Body armor, can be enchanted/cursed.
        Potion, //Potions always affect the consumer.
        Scroll, //Scrolls can affect either the consumer or a ranged target.
        Wand, //Wands always affect a ranged target.
        Book, //Cause a spell to be learned.
        Projectile, //Items projected from bows, wands and crossbows.
        Money, //Hard earned cash.
        Chest, //Container
        Purse, //Container for money.
        AlchemistStore, //A place to spend your hard earned cash.
        MageStore, //A place to spend your hard earned cash.
        ArmorSmithStore, //A place to spend your hard earned cash.
        GeneralStore, //A place to spend your hard earned cash.
        WeaponSmithStore, //A place to spend your hard earned cash.
        RuinsStore //Once a to spend your hard earned cash, now in ruins.
    }

    public enum TargetType
    {
        Unspecified, //¯\_(ツ)_/¯
        Self, //Healing, etc.
        HostileBeing, //Attacking, etc.
        Terrain //Summon being, etc.
    }

    public enum EnchantmentType
    {
        Undecided, //Will be decided at runtime.
        Normal, //Not cursed or enchanted, if set explicitly - does not need to be identified when found.
        Enchanted, //Boosted stats
        Cursed //Negative boosted stats
    }

    public enum ProjectileType
    {
        Unspecified, //¯\_(ツ)_/¯
        Arrow,
        Bolt
    }

    public enum DamageType
    {
        Unspecified, //¯\_(ツ)_/¯
        Ice,
        Fire,
        Earth,
        Electric,
        Poison
    }
    public enum ItemEffectType
    {
        Fixed,
        Percent
    }

    public enum ItemEffect
    {
        Unspecified, //¯\_(ツ)_/¯
        Heal,
        Poison,
        CurePoison,
        SummonMonster,

        ModSpeed,
        ModStrength,
        ModDexterity,
        ModConstitution,
        ModArmorClass,
        ModIntelligence,
        ModIceResistance,
        ModElectricResistance,
        ModFireResistance,
        ModEarthResistance,

        MagicArrow,
        CastFireball,
        HoldMonster,
        CastPoison,
        RemoveCurse
    }

    public enum StateOfBeing
    {
        Unspecified, //¯\_(ツ)_/¯
        Poisoned,
        ModSpeed,
        Held,

        ModStrength,
        ModDexterity,
        ModConstitution,
        ModArmorClass,
        ModIntelligence, //Also increases mana.

        ModEarthResistance,
        ModIceResistance,
        ModElectricResistance,
        ModFireResistance
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }
}
