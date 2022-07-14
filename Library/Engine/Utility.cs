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

            foreach (var effect in effects)
            {
                int mod = 0;

                switch (effect.EffectType)
                {
                    case ItemEffect.CastFireball:
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            //TODO: Add spalsh damage info.
                            text += $"Cast Fireball\r\n";
                        break;
                    case ItemEffect.CastLight:
                        //TODO: Add seconds
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Cast Light\r\n";
                        break;
                    case ItemEffect.CastPoison:
                        //TODO: Add seconds
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Cast Poison\r\n";
                        break;
                    case ItemEffect.CurePoison:
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Cure Poison\r\n";
                        break;
                    case ItemEffect.Heal:
                        {
                             mod = effects.Where(o => o.EffectType == effect.EffectType && o.ValueType == ItemEffectType.Fixed).Sum(o => o.Value);
                            if (mod > 0) text += $"Heal {mod}\r\n";

                            mod = effects.Where(o => o.EffectType == effect.EffectType && o.ValueType == ItemEffectType.Percent).Sum(o => o.Value);
                            if (mod > 0) text += $"Heal {mod}%\r\n";
                        }
                        break;
                    case ItemEffect.HoldMonster:
                        //TODO: Add seconds
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Hold Monster\r\n";
                        break;
                    case ItemEffect.MagicArrow:
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Magic Arrow\r\n";
                        break;
                    case ItemEffect.Poison:
                        //TODO: Add seconds
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Poison\r\n";
                        break;
                    case ItemEffect.Identify:
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Identify\r\n";
                        break;
                    case ItemEffect.RemoveCurse:
                        if (effects.Where(o => o.EffectType == effect.EffectType).Any())
                            text += $"Remove Curse\r\n";
                        break;
                    case ItemEffect.SummonMonster:
                        if(effects.Where(o => o.EffectType == effect.EffectType).Any())
                        text += $"Summon Monster\r\n";
                        break;
                    case ItemEffect.ArmorClass:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"AC {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.ColdResistance:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Cold Resistance {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.Constitution:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Constitution {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.Dexterity:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Dexterity {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.EarthResistance:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Earth Resistance {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.FireResistance:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Fire Resistance {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.Intelligence:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Intelligence {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.LightningResistance:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Lightning Resistance {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.Speed:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Speed {PosNeg(mod)}\r\n";
                        break;
                    case ItemEffect.Strength:
                        //TODO: Add seconds
                        mod = effects.Where(o => o.EffectType == effect.EffectType).Sum(o => o.Value);
                        if (mod != 0) text += $"Strength {PosNeg(mod)}\r\n";
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
