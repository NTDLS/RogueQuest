using Library.Engine.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Library.Engine
{
    public class TileMetadata
    {
        /// <summary>
        /// This is only populated for tiles that need it.
        /// </summary>
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public bool? CanWalkOn { get; set; }
        public bool? CanTakeDamage { get; set; }
        public int? Experience { get; set; }
        public int? HitPoints { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BasicTileType? BasicType { get; set; }

        public void OverrideWith(TileMetadata with)
        {
            this.Tag = with.Tag ?? this.Tag;
            this.CanWalkOn = with.CanWalkOn ?? this.CanWalkOn;
            this.CanTakeDamage = with.CanTakeDamage ?? this.CanTakeDamage;
            this.HitPoints = with.HitPoints ?? this.HitPoints;
            this.BasicType = with.BasicType ?? this.BasicType;
        }
    }
}
