using Library.Engine.Types;
using Library.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Engine
{
    public static class Utility
    {
        public static bool IgnoreFileName(string name)
        {
            name = name.ToLower();

            string fileName = Path.GetFileName(name);

            return fileName.StartsWith("@") || fileName == "player"
                || name.ToLower().Contains(".cursed.") || name.ToLower().Contains(".enchanted.")
                || name.ToLower().Contains(".projectile.") || name.ToLower().Contains(".animation.");
        }

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
                case DamageType.Cold: return DamageType.Fire;
                case DamageType.Fire: return DamageType.Cold;
                case DamageType.Lightning: return DamageType.Earth;
                case DamageType.Earth: return DamageType.Lightning;
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

            var modArmorClass = effects.Where(o => o.EffectType == ItemEffect.ArmorClass).Sum(o => o.Value);
            var modConstitution = effects.Where(o => o.EffectType == ItemEffect.Constitution).Sum(o => o.Value);
            var modDexterity = effects.Where(o => o.EffectType == ItemEffect.Dexterity).Sum(o => o.Value);
            var modIntelligence = effects.Where(o => o.EffectType == ItemEffect.Intelligence).Sum(o => o.Value);
            var modStrength = effects.Where(o => o.EffectType == ItemEffect.Strength).Sum(o => o.Value);
            var modSpeed = effects.Where(o => o.EffectType == ItemEffect.Speed).Sum(o => o.Value);
            var modEarthResistance = effects.Where(o => o.EffectType == ItemEffect.EarthResistance).Sum(o => o.Value);
            var modElectricResistance = effects.Where(o => o.EffectType == ItemEffect.LightningResistance).Sum(o => o.Value);
            var modFireResistance = effects.Where(o => o.EffectType == ItemEffect.FireResistance).Sum(o => o.Value);
            var modIceResistance = effects.Where(o => o.EffectType == ItemEffect.ColdResistance).Sum(o => o.Value);

            if (modArmorClass != 0) text += $"AC {PosNeg(modArmorClass)}\r\n";
            if (modConstitution != 0) text += $"CON {PosNeg(modConstitution)}\r\n";
            if (modDexterity != 0) text += $"DEX {PosNeg(modDexterity)}\r\n";
            if (modIntelligence != 0) text += $"INT {PosNeg(modIntelligence)}\r\n";
            if (modStrength != 0) text += $"STR {PosNeg(modStrength)}\r\n";
            if (modSpeed != 0) text += $"SPD {PosNeg(modSpeed)}\r\n";
            if (modEarthResistance != 0) text += $"EARTH {PosNeg(modEarthResistance)}\r\n";
            if (modElectricResistance != 0) text += $"ELECTRIC {PosNeg(modElectricResistance)}\r\n";
            if (modFireResistance != 0) text += $"FIRE {PosNeg(modFireResistance)}\r\n";
            if (modIceResistance != 0) text += $"COLD {PosNeg(modIceResistance)}\r\n";

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
}
