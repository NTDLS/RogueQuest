using Library.Engine.Types;
using Library.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Engine
{
    public static class Utility
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

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

        public static string PrevalenceText(int prevalence)
        {
            if (prevalence >= 40) return "Common";
            else if (prevalence >= 20) return "Uncommon";
            else if (prevalence >= 10) return "Rare";
            else if (prevalence >= 1) return "Ultra Rare";
            else if (prevalence >= 0) return "Legendary";
            return "n/a";
        }

        public static string GetEffectsText(List<MetaEffect> effects)
        {
            if (effects == null)
            {
                return "";
            }

            string text = string.Empty;

            foreach (var effect in effects.GroupBy(o => o.EffectType))
            {
                int val = 0;

                switch (effect.Key)
                {
                    case ItemEffect.CastFireball:
                        text += $"Cast Fireball\r\n";
                        break;
                    case ItemEffect.CastLight:
                        text += $"Cast Light\r\n";
                        break;
                    case ItemEffect.CastPoison:
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                        if (val > 0) text += $"Cast Poison, {val}s\r\n";
                        break;
                    case ItemEffect.CurePoison:
                        text += $"Cure Poison\r\n";
                        break;
                    case ItemEffect.Heal:
                        {
                            val = effects.Where(o => o.EffectType == effect.Key && o.ValueType == ItemEffectType.Fixed).Sum(o => o.Value);
                            if (val > 0) text += $"Heal {val}\r\n";
                            val = effects.Where(o => o.EffectType == effect.Key && o.ValueType == ItemEffectType.Percent).Sum(o => o.Value);
                            if (val > 0) text += $"Heal {val}%\r\n";
                        }
                        break;
                    case ItemEffect.HoldMonster:
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                        if (val > 0) text += $"Hold Monster, {val}s\r\n";
                        break;
                    case ItemEffect.MagicArrow:
                        text += $"Magic Arrow\r\n";
                        break;
                    case ItemEffect.Poison:
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                        if (val > 0) text += $"Poison, {val}s\r\n";
                        break;
                    case ItemEffect.Identify:
                        text += $"Identify\r\n";
                        break;
                    case ItemEffect.RemoveCurse:
                        text += $"Remove Curse\r\n";
                        break;
                    case ItemEffect.SummonMonster:
                        text += $"Summon Monster\r\n";
                        break;
                    case ItemEffect.ArmorClass:
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"AC {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.ColdResistance:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Cold Resistance {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.Constitution:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Constitution {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.Dexterity:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Dexterity {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.EarthResistance:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Earth Resistance {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.FireResistance:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Fire Resistance {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.Intelligence:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Intelligence {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.LightningResistance:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Lightning Resistance {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.Speed:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Speed {PosNeg(val)}\r\n";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                    case ItemEffect.Strength:
                        //TODO: Add seconds
                        val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Value);
                        if (val != 0)
                        {
                            text += $"Strength {PosNeg(val)}";
                            val = effects.Where(o => o.EffectType == effect.Key).Sum(o => o.Duration) ?? 0;
                            if (val != 0) text += $", {val}s";
                            text += "\r\n";
                        }
                        break;
                }
            }

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
