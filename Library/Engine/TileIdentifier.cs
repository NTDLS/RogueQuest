using System;

namespace Library.Engine
{
    public class TileIdentifier
    {
        public string TilePath { get; set; }
        public TileMetadata Meta { get; set; }

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
