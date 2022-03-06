namespace Library.Engine.Types
{
    public static class Utility
    {
        public static DamageType GetOppositeOfDamageType(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Ice: return DamageType.Fire;
                case DamageType.Fire: return DamageType.Ice;
                case DamageType.Electric: return DamageType.Earth;
                case DamageType.Earth: return DamageType.Electric;
            }

            return DamageType.Unspecified;
        }
    }

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
        Potion, //Potions always affect the consumer.
        Scroll, //Scrolls can affect either the consumer or a ranged target.
        Wand, //Wands always affect a ranged target.
        Book,
        Projectile,
        Money,
        Chest,
        Purse,
        AlchemistStore,
        MageStore,
        ArmorSmithStore,
        GeneralStore,
        WeaponSmithStore,
        RuinsStore
    }

    public enum TargetType
    {
        Unspecified,
        Self, //Healing, etc.
        HostileBeing, //Attacking, etc.
        Terrain //Summon being, etc.
    }

    public enum EnchantmentType
    {
        Normal,
        Cursed,
        Enchanted
    }

    public enum ProjectileType
    {
        Unspecified,
        Arrow,
        Bolt
    }

    public enum DamageType
    {
        Unspecified,
        Ice,
        Fire,
        Earth,
        Electric,
        Poison
    }

    public enum ItemEffect
    {
        Unspecified,
        Heal,
        Poison,
        CurePoison,
        SummonMonster,
        IncreaseStrength,
        IncreaseDexterity,
        IncreaseConstitution,
        IncreaseArmorClass,
        IncreaseIntelligence,
        IncreaseIceResistance,
        IncreaseElectricResistance,
        IncreaseFireResistance,
        IncreaseEarthResistance,
        PermanentlyIncreaseStrength,
        PermanentlyIncreaseDexterity,
        PermanentlyIncreaseConstitution,
        PermanentlyIncreaseIntelligence,
        DecreaseIceResistance,
        DecreaseElectricResistance,
        DecreaseFireResistance,
        DecreaseEarthResistance,
        MagicArrow,
        CastFireball,
        HoldMonster,
        CastPoison
    }

    public enum StateOfBeing
    {
        Unspecified,
        Poisoned,
        Slowed,
        Held,
        IncreaseStrength,
        IncreaseDexterity,
        IncreaseConstitution,
        IncreaseArmorClass,
        IncreaseIntelligence, //Also increases mana.

        IncreaseEarthResistance,
        IncreaseIceResistance,
        IncreaseElectricResistance,
        IncreaseFireResistance,
        DecreaseIceResistance, //This is a bad thing.
        DecreaseElectricResistance, //This is a bad thing.
        DecreaseFireResistance, //This is a bad thing.
        DecreaseEarthResistance,  //This is a bad thing.
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }
}
