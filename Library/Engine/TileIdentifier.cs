using Assets;
using Library.Engine.Types;
using System;

namespace Library.Engine
{
    public class TileIdentifier
    {
        public string TilePath { get; set; }
        public TileMetadata Meta { get; set; }

        public string ImagePath
        {
            get
            {
                if ((Meta.Enchantment ?? EnchantmentType.Normal) == EnchantmentType.Normal
                    || (Meta.Enchantment ?? EnchantmentType.Normal) == EnchantmentType.Undecided
                    || Meta.IsIdentified == false)
                {
                    return Constants.GetCommonAssetPath($"{TilePath}.png");
                }

                if (Meta.Enchantment == EnchantmentType.Cursed && Meta.IsIdentified == true)
                {
                    if (string.IsNullOrWhiteSpace(Meta.EnchantedImagePath) || string.IsNullOrWhiteSpace(Meta.CursedImagePath))
                    {
                        throw new Exception("Cursed tile must have an [CursedImagePath] specified.");
                    }
                    return Constants.GetCommonAssetPath($"{Meta.CursedImagePath}.png");
                }

                if (Meta.Enchantment == EnchantmentType.Enchanted && Meta.IsIdentified == true)
                {
                    if (string.IsNullOrWhiteSpace(Meta.EnchantedImagePath) || string.IsNullOrWhiteSpace(Meta.CursedImagePath))
                    {
                        throw new Exception("Cursed tile must have an [EnchantedImagePath] specified.");
                    }
                    return Constants.GetCommonAssetPath($"{Meta.EnchantedImagePath}.png");
                }

                return Constants.GetCommonAssetPath($"{TilePath}.png");
            }
        }

        public TileIdentifier()
        {
        }

        public TileIdentifier(string tilePath)
        {
            this.TilePath = tilePath;
        }

        public TileIdentifier(string tilePath, bool loadMetadata)
        {
            this.TilePath = tilePath;
            if (loadMetadata)
            {
                LoadMetadata();
            }
        }

        public void LoadMetadata()
        {
            Meta = TileMetadata.GetFreshMetadata(this.TilePath);
        }

        public TileIdentifier(string tilePath, TileMetadata meta)
        {
            this.TilePath = tilePath;
            this.Meta = meta;
        }

        /// <summary>
        /// Makes a copy of the tile with a seperate copy of the meta-data. The tile will have the same ID as the original.
        /// </summary>
        /// <param name="ensureItemHasUid"></param>
        /// <returns></returns>
        public TileIdentifier Clone(bool ensureItemHasUid = false)
        {
            var identifier = new TileIdentifier(this.TilePath);

            identifier.Meta = new TileMetadata();

            identifier.Meta.OverrideWith(this.Meta);

            if (ensureItemHasUid && identifier.Meta.UID == null)
            {
                identifier.Meta.UID = Guid.NewGuid();
            }

            return identifier;
        }

        /// <summary>
        /// Creates a complete copy of the tile, with a copy of the meta data and a new UID.
        /// </summary>
        /// <returns></returns>
        public TileIdentifier DeriveCopy()
        {
            var identifier = this.Clone();
            identifier.Meta.UID = Guid.NewGuid();
            return identifier;
        }

    }
}
