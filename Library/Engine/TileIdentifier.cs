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

        public TileIdentifier(string tilePath, TileMetadata meta)
        {
            this.TilePath = tilePath;
            this.Meta = meta;
        }

        public TileIdentifier Clone(bool ensureItemHasUid = false)
        {
            var identifier = new TileIdentifier(this.TilePath)
            {
                Meta = this.Meta
            };

            if (ensureItemHasUid && identifier.Meta.UID == null)
            {
                identifier.Meta.UID = Guid.NewGuid();
            }

            return identifier;
        }
    }
}
