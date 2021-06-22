﻿namespace Library.Engine.Types
{
    public enum KeyPressState
    {
        Up,
        Down
    }

    public enum BasicTileType
    {
        ActorBuilding,
        ActorFriendyBeing,
        ActorHostileBeing,
        ActorItem,
        ActorPlayer,
        ActorTerrain,
        ActorTextBlock
    }

    public enum FiredFromType
    {
        Unspecified,
        Player,
        Enemy
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }

    public enum PlayerKey
    {
        SpeedBoost,
        Forward,
        Reverse,
        PrimaryFire,
        SecondaryFire,
        RotateClockwise,
        RotateCounterClockwise,
        Escape,
        Left,
        Right,
        Up,
        Down,
        Enter
    }
}