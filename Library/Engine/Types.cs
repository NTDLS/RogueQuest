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
        LeftRing
    }

    public enum ActorClassName
    {
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
        Chest
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }
}
