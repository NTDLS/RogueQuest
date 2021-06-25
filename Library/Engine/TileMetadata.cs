using Assets;
using Library.Engine.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace Library.Engine
{
    public class TileMetadata
    {
        /// <summary>
        /// This is only populated for tiles that need it.
        /// </summary>
        public Guid? UID { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int Quantity { get; set; } //Stacking because things like money only really matter in multiples.
        public bool? CanWalkOn { get; set; }
        public bool? CanTakeDamage { get; set; }
        public int? Experience { get; set; }
        public int? HitPoints { get; set; }
        public int? DamageReduction { get; set; }
        public int? DamageDice { get; set; }
        public int? DamageDiceFaces { get; set; }
        public int? DamageAdditional { get; set; }
        public int? OriginalHitPoints { get; set; }        
        public bool? IsContainer { get; set; }
        public bool? CanStack { get; set; }
        public int? BulkCapacity { get; set; }
        public int? WeightCapacity { get; set; }
        public int? Weight { get; set; }
        public int? Bulk { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public ActorClassName? ActorClass { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActorSubType? SubType { get; set; }

        public void OverrideWith(TileMetadata with)
        {
            this.UID = with.UID ?? this.UID;
            this.Tag = with.Tag ?? this.Tag;
            this.CanWalkOn = with.CanWalkOn ?? this.CanWalkOn;
            this.CanTakeDamage = with.CanTakeDamage ?? this.CanTakeDamage;
            this.HitPoints = with.HitPoints ?? this.HitPoints;
            this.Experience = with.Experience ?? this.Experience;
            this.ActorClass = with.ActorClass ?? this.ActorClass;
            this.Name = with.Name ?? this.Name;
            this.IsContainer = with.IsContainer ?? this.IsContainer;
            this.CanStack = with.CanStack ?? this.CanStack;
            this.SubType = with.SubType ?? this.SubType;
            this.DamageReduction = with.DamageReduction ?? this.DamageReduction;
            this.BulkCapacity = with.BulkCapacity ?? this.BulkCapacity;
            this.WeightCapacity = with.WeightCapacity ?? this.WeightCapacity;
            this.Bulk = with.Bulk ?? this.Bulk;
            this.Weight = with.Weight ?? this.Weight;
        }

        /// <summary>
        /// Traverses up the directories looking for metadata files that describe the assets and allowing for overriding of metadata values.
        /// </summary>
        /// <param name="tilePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static TileMetadata FindFirstMetafile(string tilePath, string fileName)
        {
            string globalMetaFile = Path.Combine(tilePath, fileName);

            if (File.Exists(globalMetaFile))
            {
                var text = File.ReadAllText(globalMetaFile);
                return JsonConvert.DeserializeObject<TileMetadata>(text);
            }
            else
            {
                string parentPath = Directory.GetParent(tilePath)?.FullName;
                if (string.IsNullOrWhiteSpace(parentPath) == false)
                {
                    return FindFirstMetafile(parentPath, fileName);
                }
            }

            return null;
        }

        /// <summary>
        /// Reads metadata from the asset directories and refreshes the values stored with the actor.
        /// </summary>
        public static TileMetadata GetFreshMetadata(string tilePath)
        {
            string fileSystemPath = Path.GetDirectoryName(Constants.GetAssetPath($"{tilePath}"));
            string exactMetaFileName = Constants.GetAssetPath($"{tilePath}.txt");

            var meta = FindFirstMetafile(fileSystemPath, "_GlobalMeta.txt") ?? new TileMetadata();

            var localMeta = FindFirstMetafile(fileSystemPath, "_LocalMeta.txt");
            if (localMeta != null)
            {
                meta.OverrideWith(localMeta);
            }

            if (File.Exists(exactMetaFileName))
            {
                var text = File.ReadAllText(exactMetaFileName);
                var exactMeta = JsonConvert.DeserializeObject<TileMetadata>(text);
                if (exactMeta != null)
                {
                    meta.OverrideWith(exactMeta);
                }
            }

            if (meta.UID == null && meta.ActorClass != ActorClassName.ActorTerrain)
            {
                meta.UID = Guid.NewGuid();
            }

            meta.OriginalHitPoints = meta.HitPoints;

            return meta;
        }

    }
}
