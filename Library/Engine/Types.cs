using Library.Types;
using System.Collections.Generic;
using System.Linq;
using static Library.Engine.TileMetadata;

namespace Library.Engine.Types
{
    public static class Utility
    {
        public static string PosNeg(int val)
        {
            if (val > 0)
            {
                return $"+{val:N0}";
            }
            else if (val > 0)
            {
                return $"-{val:N0}";
            }

            return $"{val:N0}";
        }

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

        public static string RarityText(int rarity)
        {
            if (rarity >= 40) return "Common";
            else if (rarity >= 20) return "Uncommon";
            else if (rarity >= 10) return "Rare";
            else if (rarity >= 1) return "Ultra Rare";
            else if (rarity >= 0) return "Legendary";
            return "n/a";
        }

        public static string GetEffectsText(List<MetaEffect> effects)
        {
            if (effects == null)
            {
                return "";
            }

            string text = string.Empty;

            var modArmorClass = effects.Where(o => o.EffectType == ItemEffect.ModArmorClass).Sum(o => o.Value);
            var modConstitution = effects.Where(o => o.EffectType == ItemEffect.ModConstitution).Sum(o => o.Value);
            var modDexterity = effects.Where(o => o.EffectType == ItemEffect.ModDexterity).Sum(o => o.Value);
            var modIntelligence = effects.Where(o => o.EffectType == ItemEffect.ModIntelligence).Sum(o => o.Value);
            var modStrength = effects.Where(o => o.EffectType == ItemEffect.ModStrength).Sum(o => o.Value);
            var modSpeed = effects.Where(o => o.EffectType == ItemEffect.ModSpeed).Sum(o => o.Value);
            var modEarthResistance = effects.Where(o => o.EffectType == ItemEffect.ModEarthResistance).Sum(o => o.Value);
            var modElectricResistance = effects.Where(o => o.EffectType == ItemEffect.ModElectricResistance).Sum(o => o.Value);
            var modFireResistance = effects.Where(o => o.EffectType == ItemEffect.ModFireResistance).Sum(o => o.Value);
            var modIceResistance = effects.Where(o => o.EffectType == ItemEffect.ModIceResistance).Sum(o => o.Value);

            if (modArmorClass != 0) text += $"AC {PosNeg(modArmorClass)}\r\n";
            if (modConstitution != 0) text += $"CON {PosNeg(modConstitution)}\r\n";
            if (modDexterity != 0) text += $"DEX {PosNeg(modDexterity)}\r\n";
            if (modIntelligence != 0) text += $"INT {PosNeg(modIntelligence)}\r\n";
            if (modStrength != 0) text += $"STR {PosNeg(modStrength)}\r\n";
            if (modSpeed != 0) text += $"SPD {PosNeg(modSpeed)}\r\n";
            if (modEarthResistance != 0) text += $"EARTH {PosNeg(modEarthResistance)}\r\n";
            if (modElectricResistance != 0) text += $"ELECTRIC {PosNeg(modElectricResistance)}\r\n";
            if (modFireResistance != 0) text += $"FIRE {PosNeg(modFireResistance)}\r\n";
            if (modIceResistance != 0) text += $"ICE {PosNeg(modIceResistance)}\r\n";

            return text.Trim().Replace("\r\n", "|");
        }

        public static ActorSubType[] RandomDropSubTypes
        {
            get
            {
                return new ActorSubType[]
                {
                    ActorSubType.Armor,
                    ActorSubType.Belt,
                    ActorSubType.Book,
                    ActorSubType.Boots,
                    ActorSubType.Bracers,
                    ActorSubType.Garment,
                    ActorSubType.Gauntlets,
                    ActorSubType.Helment,
                    ActorSubType.MeleeWeapon,
                    ActorSubType.Money,
                    ActorSubType.Necklace,
                    ActorSubType.Pack,
                    ActorSubType.Potion,
                    ActorSubType.Projectile,
                    ActorSubType.RangedWeapon,
                    ActorSubType.Ring,
                    ActorSubType.Scroll,
                    ActorSubType.Shield,
                    ActorSubType.Wand
                };
            }
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
