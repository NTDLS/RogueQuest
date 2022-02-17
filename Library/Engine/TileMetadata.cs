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
        #region Backend fields.

        private Guid? _UID;
        private string _ProjectileTilePath;
        private string _HitAnimationTilePath;
        private string _Name;
        private string _Tag;
        private int? _Quantity; //Stacking because things like money only really matter in multiples.
        private int? _Experience;
        private int? _HitPoints;
        private int? _AC; //Armor Class. yay.
        private int? _DamageDice;
        private int? _DamageDiceFaces;
        private int? _DamageAdditional;
        private int? _OriginalHitPoints;
        private int? _BulkCapacity;
        private int? _WeightCapacity;
        private int? _ItemCapacity;
        private int? _Weight;
        private int? _Bulk;
        private int? _Dexterity;
        private int? _Charges;
        private int? _Strength;
        private ActorSubType? _SpawnSubType;
        private string _Dialog;
        private int? _MinLevel;
        private int? _MaxLevel;
        private int? _Level;
        private double? _Value;
        private string _LevelWarpName;
        private Guid? _LevelWarpTargetTileUID;
        private ActorSubType? _SubType;

        #endregion

        /// <summary>
        /// Set to true when Core.BlindPlay is enabled. Allows the map to be slowly revealed.
        /// </summary>
        public bool HasBeenViewed { get; set; }
        public bool? OnlyDialogOnce { get; set; }
        public bool? IsContainer { get; set; }
        public bool? CanStack { get; set; } //Remember that items with charges are NOT stackable.
        public bool? CanWalkOn { get; set; }
        /// <summary>
        /// There can only be one.
        /// </summary>
        public bool? IsUnique { get; set; }
        public bool? CanTakeDamage { get; set; }
        // This is only populated for tiles that need it.
        public Guid? UID { get { return _UID == Guid.Empty ? null : _UID; } set { _UID = value; } }
        public string ProjectileTilePath { get { return _ProjectileTilePath == string.Empty ? null : _ProjectileTilePath; } set { _ProjectileTilePath = value; } }
        public string HitAnimationTilePath { get { return _HitAnimationTilePath == string.Empty ? null : _HitAnimationTilePath; } set { _HitAnimationTilePath = value; } }

        public string Name { get { return _Name == string.Empty ? null : _Name; } set { _Name = value; } }
        public string Tag { get { return _Tag == string.Empty ? null : _Tag; } set { _Tag = value; } }
        //Stacking because things like money only really matter in multiples.
        public int? Quantity { get { return _Quantity == 0 ? null : _Quantity; } set { _Quantity = value; } }
        public ItemEffect Effect { get; set; }
        public ProjectileType ProjectileType { get; set; }
        public string EffectFormula { get; set; }
        public bool? IsConsumable { get; set; }
        /// <summary>
        /// Game time for expiration.
        /// </summary>
        public int? ExpireTime { get; set; }
        public int? Charges { get { return _Charges == 0 ? null : _Charges; } set { _Charges = value; } } //Remember that items with charges are NOT stackable.

        public int? Experience { get { return _Experience == 0 ? null : _Experience; } set { _Experience = value; } }

        public int? HitPoints { get { return _HitPoints == 0 ? null : _HitPoints; } set { _HitPoints = value; } }

        public int? AC { get { return _AC == 0 ? null : _AC; } set { _AC = value; } }

        public int? DamageDice { get { return _DamageDice == 0 ? null : _DamageDice; } set { _DamageDice = value; } }

        public int? DamageDiceFaces { get { return _DamageDiceFaces == 0 ? null : _DamageDiceFaces; } set { _DamageDiceFaces = value; } }

        public int? DamageAdditional { get { return _DamageAdditional == 0 ? null : _DamageAdditional; } set { _DamageAdditional = value; } }

        public int? OriginalHitPoints { get { return _OriginalHitPoints == 0 ? null : _OriginalHitPoints; } set { _OriginalHitPoints = value; } }

        public int? BulkCapacity { get { return _BulkCapacity == 0 ? null : _BulkCapacity; } set { _BulkCapacity = value; } }

        public int? WeightCapacity { get { return _WeightCapacity == 0 ? null : _WeightCapacity; } set { _WeightCapacity = value; } }

        public int? ItemCapacity { get { return _ItemCapacity == 0 ? null : _ItemCapacity; } set { _ItemCapacity = value; } }

        public int? Weight { get { return _Weight == 0 ? null : _Weight; } set { _Weight = value; } }

        public int? Bulk { get { return _Bulk == 0 ? null : _Bulk; } set { _Bulk = value; } }

        public int? Dexterity { get { return _Dexterity == 0 ? null : _Dexterity; } set { _Dexterity = value; } }

        public int? Strength { get { return _Strength == 0 ? null : _Strength; } set { _Strength = value; } }

        //Used by the ActorSpawner
        [JsonConverter(typeof(StringEnumConverter))]
        public ActorClassName? SpawnType { get; set; }

        //Used by the ActorSpawner
        [JsonConverter(typeof(StringEnumConverter))]
        public ActorSubType? SpawnSubType { get { return _SpawnSubType ==  ActorSubType.Unspecified ? null : _SpawnSubType; } set { _SpawnSubType = value; } }

        public int? MinLevel { get { return _MinLevel == 0 ? null : _MinLevel; } set { _MinLevel = value; } } //Used by the ActorSpawner

        public int? MaxLevel { get { return _MaxLevel == 0 ? null : _MaxLevel; } set { _MaxLevel = value; } } //Used by the ActorSpawner

        public int? Level { get { return _Level == 0 ? null : _Level; } set { _Level = value; } } //Used to know when we should show items, enemies and what to populate in shops.

        public double? Value { get { return _Value == 0 ? null : _Value; } set { _Value = value; } } //Rough monetary value of the item in a shop.

        // Used for level warp tiles. This tells the engine which level to load.
        public string LevelWarpName { get { return _LevelWarpName == string.Empty ? null : _LevelWarpName; } set { _LevelWarpName = value; } }

        /// Used for level warp tiles. This tells the engine which tile to spawn to.
        public Guid? LevelWarpTargetTileUID { get { return _LevelWarpTargetTileUID == Guid.Empty ? null : _LevelWarpTargetTileUID; } set { _LevelWarpTargetTileUID = value; } }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActorClassName? ActorClass { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActorSubType? SubType { get { return _SubType == ActorSubType.Unspecified ? null : _SubType; } set { _SubType = value; } }

        // This is what the object will say to the player when approached.
        public string Dialog { get { return _Dialog == string.Empty ? null : _Dialog; } set { _Dialog = value; } }


        public string DndDamageText
        {
            get
            {
                var text = $"{this.DamageDice}d{this.DamageDiceFaces}";

                if (this.DamageAdditional > 0)
                {
                    text += $" +{this.DamageAdditional}";
                }

                return text;
            }
        }

        public void OverrideWith(TileMetadata with)
        {
            this.UID = with.UID ?? this.UID;
            this.Tag = with.Tag ?? this.Tag;
            this.CanWalkOn = with.CanWalkOn ?? this.CanWalkOn;
            this.CanTakeDamage = with.CanTakeDamage ?? this.CanTakeDamage;
            this.ProjectileType = with.ProjectileType;
            this.Effect = with.Effect;
            this.EffectFormula = with.EffectFormula ?? this.EffectFormula;
            this.IsConsumable = with.IsConsumable ?? this.IsConsumable;
            this.Charges = with.Charges ?? this.Charges;
            this.ExpireTime = with.ExpireTime ?? this.ExpireTime;
            this.HitPoints = with.HitPoints ?? this.HitPoints;
            this.Experience = with.Experience ?? this.Experience;
            this.ActorClass = with.ActorClass ?? this.ActorClass;
            this.Name = with.Name ?? this.Name;
            this.ProjectileTilePath = with.ProjectileTilePath ?? this.ProjectileTilePath;
            this.HitAnimationTilePath = with.HitAnimationTilePath ?? this.HitAnimationTilePath;
            this.IsUnique = with.IsUnique ?? this.IsUnique;
            this.Dialog = with.Dialog ?? this.Dialog;
            this.OnlyDialogOnce = with.OnlyDialogOnce ?? this.OnlyDialogOnce;
            this.IsContainer = with.IsContainer ?? this.IsContainer;
            this.CanStack = with.CanStack ?? this.CanStack;
            this.SubType = with.SubType ?? this.SubType;
            this.AC = with.AC ?? this.AC;
            this.BulkCapacity = with.BulkCapacity ?? this.BulkCapacity;
            this.WeightCapacity = with.WeightCapacity ?? this.WeightCapacity;
            this.ItemCapacity = with.ItemCapacity ?? this.ItemCapacity;
            this.Bulk = with.Bulk ?? this.Bulk;
            this.Weight = with.Weight ?? this.Weight;
            this.DamageDice = with.DamageDice ?? this.DamageDice;
            this.DamageDiceFaces = with.DamageDiceFaces ?? this.DamageDiceFaces;
            this.DamageAdditional = with.DamageAdditional ?? this.DamageAdditional;
            this.Strength = with.Strength ?? this.Strength;
            this.Dexterity = with.Dexterity ?? this.Dexterity;
            this.SpawnType = with.SpawnType ?? this.SpawnType;
            this.SpawnSubType = with.SpawnSubType ?? this.SpawnSubType;
            this.MinLevel = with.MinLevel ?? this.MinLevel;
            this.MaxLevel = with.MaxLevel ?? this.MaxLevel;
            this.Level = with.Level ?? this.Level;
            this.Value = with.Value ?? this.Value;
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
