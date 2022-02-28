using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Classes
{
    public static class StoreAndInventory
    {
        public static string RarityText(TileIdentifier tile)
        {
            if (tile.Meta.Rarity >= 40)
            {
                return "Common";
            }
            else if (tile.Meta.Rarity >= 20)
            {
                return "Uncommon";
            }
            else if (tile.Meta.Rarity >= 10)
            {
                return "Rare";
            }
            else if (tile.Meta.Rarity >= 1)
            {
                return "Ultra Rare";
            }
            else if (tile.Meta.Rarity >= 0)
            {
                return "Legendary";
            }
            return "n/a";
        }

        public static double UnitPrice(EngineCoreBase core, TileIdentifier tile)
        {
            if (tile.Meta.Value == null)
            {
                return 0;
            }

            double intelligenceDiscount = (core.State.Character.Intelligence / 100.0);
            return ((tile.Meta.Value ?? 0.0) - ((tile.Meta.Value ?? 0.0) * intelligenceDiscount));
        }

        public static int AskingPrice(EngineCoreBase core, TileIdentifier tile, int? quantity)
        {
            if (tile.Meta.Value == null)
            {
                return 0;
            }

            double value = UnitPrice(core, tile);
            double qty = (double)(quantity ?? 0);

            if (qty > 0)
            {
                value *= qty;
            }

            if (value >= 1)
            {
                return (int)value;
            }

            return 1;
        }


        public static int AskingPrice(EngineCoreBase core, TileIdentifier tile)
        {
            if (tile.Meta.Value == null)
            {
                return 0;
            }

            return AskingPrice(core, tile, (tile.Meta.Charges ?? 0) + (tile.Meta.Quantity ?? 0));
        }

        public static int OfferPrice(EngineCoreBase core, TileIdentifier tile)
        {
            if (tile.Meta.Value == null)
            {
                return 0;
            }

            double intelligenceBonus = (core.State.Character.Intelligence / 100.0);
            double halfValue = ((tile.Meta.Value ?? 0.0) / 2.0);
            double value = (halfValue + (halfValue * intelligenceBonus));
            int qty = (tile.Meta.Charges ?? 0) + (tile.Meta.Quantity ?? 0);

            if (qty > 0)
            {
                value *= qty;
            }

            if (value >= 1)
            {
                return (int)value;
            }

            return 1;
        }
    }
}

