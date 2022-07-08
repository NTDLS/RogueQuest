using Assets;
using Library.Engine.Types;
using Library.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Engine
{
    public class TileMetadata
    {
        #region Backend fields.

        private Guid? _UID;
        private string _ProjectileImagePath;
        private string _AnimationImagePath;
        private string _Name;
        private string _SpellName;
        private int? _Quantity;
        private int? _Experience;
        private int? _HitPoints;
        private int? _AC;
        private int? _DamageDice;
        private int? _DamageDiceFaces;
        private int? _DamageAdditional;
        private int? _OriginalHitPoints;
        private int? _BulkCapacity;
        private int? _WeightCapacity;
        private int? _ItemCapacity;
        private double? _Weight;
        private double? _Bulk;
        private int? _Dexterity;
        private int? _Charges;
        private int? _Strength;
        private string _Dialog;
        private string _Special;
        private int? _MinLevel;
        private int? _MaxLevel;
        private int? _Level;
        private double? _Value;
        private string _LevelWarpName;
        private Guid? _LevelWarpTargetTileUID;
        private ActorSubType? _SubType;

        #endregion

        public int UIDHash
        {
            get
            {
                return UID.ToString().ToCharArray().Sum(x => x);
            }
        }

        /// <summary>
        /// For enchanted or cursed items, they will be unidentified when found unless bought from a store.
        /// </summary>
        /// 
        public bool? IsIdentified { get; set; }


        class WeightedLottery
        {
            public ItemEffect Effect { get; set; }
            public int Chance { get; set; }
            public int Max { get; set; }
            public int MonitaryValue { get; set; }
        }

        public void Identify(EngineCoreBase core)
        {
            if (IsIdentified == true || Enchantment != EnchantmentType.Undecided)
            {
                return;
            }

            //We use the UID hash here to make sure that the enchantments are always the same of this item even if the player reloads.
            Random rand = new Random(UIDHash);

            IsIdentified = true;

            if ((Enchantment ?? EnchantmentType.Undecided) != EnchantmentType.Undecided)
            {
                return;
            }

            if (rand.NextDouble() * 100 >= 70) //30% chance or being enchanted or cursed.
            {
                //We have to get this early so that the random number will not be affected by the loop below it.
                bool willBeCursed = rand.NextDouble() * 100 < 50;

                int bonusRolls = rand.Next(1, 4 + core.State.Character.Level); //Add some enchantment.
                int bonusPointsApplied = 0;

                if (Effects == null)
                {
                    Effects = new List<MetaEffect>();
                }

                if (SubType != ActorSubType.Projectile)
                {
                    WeightedLottery[] effectLottery =
                    {
                          new WeightedLottery() { Effect = ItemEffect.Constitution, Chance = 20, Max = 3, MonitaryValue = 3 },
                          new WeightedLottery() { Effect = ItemEffect.Dexterity, Chance = 20, Max = 3, MonitaryValue = 3 },
                          new WeightedLottery() { Effect = ItemEffect.Intelligence, Chance = 20, Max = 3, MonitaryValue = 3 },
                          new WeightedLottery() { Effect = ItemEffect.Strength, Chance = 20, Max = 3, MonitaryValue = 3 },
                          new WeightedLottery() { Effect = ItemEffect.EarthResistance, Chance = 30, Max = 5, MonitaryValue = 2 },
                          new WeightedLottery() { Effect = ItemEffect.LightningResistance, Chance = 30, Max = 5, MonitaryValue = 2 },
                          new WeightedLottery() { Effect = ItemEffect.FireResistance, Chance = 30, Max = 5, MonitaryValue = 2 },
                          new WeightedLottery() { Effect = ItemEffect.ColdResistance, Chance = 30, Max = 5, MonitaryValue = 1 },
                          new WeightedLottery() { Effect = ItemEffect.ArmorClass, Chance = 100, Max = 10, MonitaryValue = 1 },
                          new WeightedLottery() { Effect = ItemEffect.Speed, Chance = 10, Max = 2, MonitaryValue = 5 },
                    };

                    do
                    {
                        for (int i = 0; i < bonusRolls; i++)
                        {
                            int itemIndex = rand.Next(0, effectLottery.Length - 1);

                            effectLottery = effectLottery.OrderBy(x => rand.Next()).ToArray();

                            var effect = effectLottery[itemIndex];
                            if (rand.NextDouble() * 100 >= 100 - effect.Chance)
                            {
                                if (Effects.Where(o => o.EffectType == effect.Effect).Sum(o => o.Value) < effect.Max)
                                {
                                    Effects.Add(new MetaEffect() { EffectType = effect.Effect, ValueType = ItemEffectType.Fixed, Value = 1 });
                                    bonusPointsApplied += effect.MonitaryValue;
                                }
                            }
                        }
                    } while (bonusPointsApplied == 0);

                    EnchantmentBonus = (EnchantmentBonus ?? 0) + bonusPointsApplied;
                }

                if (willBeCursed)
                {
                    Enchantment = EnchantmentType.Cursed;

                    foreach (var effect in Effects)
                    {
                        effect.Value *= -1; //Make this item suck!
                    }

                    //Make it really suck.
                    if (SubType == ActorSubType.MeleeWeapon || SubType == ActorSubType.RangedWeapon || SubType == ActorSubType.Projectile)
                    {
                        bonusRolls = rand.Next(1, 1 + core.State.Character.Level) * 3; //Add some de-enchantment.
                        bonusPointsApplied = 0;

                        for (int i = 0; i < bonusRolls; i++)
                        {
                            if (rand.NextDouble() * 100 >= 60) { DamageDiceFaces--; bonusPointsApplied += 1; }
                            if (rand.NextDouble() * 100 >= 70) { DamageAdditional--; bonusPointsApplied += 2; }
                            if (rand.NextDouble() * 100 >= 80) { DamageDice--; bonusPointsApplied += 3; }
                        }

                        EnchantmentBonus = (EnchantmentBonus ?? 0) + bonusPointsApplied;
                    }

                    DamageDice ??= 1;
                    DamageDiceFaces ??= 1;
                }
                else
                {
                    Enchantment = EnchantmentType.Enchanted;

                    if (SubType == ActorSubType.MeleeWeapon || SubType == ActorSubType.RangedWeapon || SubType == ActorSubType.Projectile)
                    {
                        //Apply some damage enchantments.
                        bonusRolls = rand.Next(1, core.State.Character.Level + 1); //Add some enchantment.
                        bonusPointsApplied = 0;
                        for (int i = 0; i < bonusRolls; i++)
                        {
                            if (rand.NextDouble() * 100 >= 60) { DamageDiceFaces++; bonusPointsApplied += 1; }
                            if (rand.NextDouble() * 100 >= 70) { DamageAdditional++; bonusPointsApplied += 2; }
                            if (rand.NextDouble() * 100 >= 80) { DamageDice++; bonusPointsApplied += 3; }
                        }

                        EnchantmentBonus = (EnchantmentBonus ?? 0) + bonusPointsApplied;
                    }

                    //Apply a damage type.
                    if (rand.NextDouble() * 100 >= 80 && (DamageType ?? Types.DamageType.Unspecified) == Types.DamageType.Unspecified)
                    {
                        DamageType = (DamageType)rand.Next(0, (int)Enum.GetValues<DamageType>().ToList().Max());
                        EnchantmentBonus = (EnchantmentBonus ?? 0) + 1;
                    }
                }
            }
            else
            {
                Enchantment = EnchantmentType.Normal;
            }
        }

        /// <summary>
        /// The extra amount that one or more stats were randomly increased to.
        /// </summary>
        public int? EnchantmentBonus { get; set; }
        /// <summary>
        /// The image to use for enchanted item.
        /// </summary>
        public string EnchantedImagePath { get; set; }
        /// <summary>
        /// The image to use for cursed item.
        /// </summary>
        public string CursedImagePath { get; set; }
        /// <summary>
        /// Whether is iten is normal, cursed or enchanted.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EnchantmentType? Enchantment { get; set; }
        /// <summary>
        /// Set to true when Core.BlindPlay is enabled. Allows the map to be slowly revealed.
        /// </summary>
        public bool HasBeenViewed { get; set; }
        /// <summary>
        /// Tells us that a scroll in the inventory is actually a memorized spell and not a scroll. It consumes mana and not quantity.
        /// </summary>
        public bool? IsMemoriziedSpell { get; set; }
        /// <summary>
        /// Liklihood in drops and stores.
        /// 100 being common and 0 being literally impossible (not in stores or dropped randomly).
        /// </summary>
        public int? Prevalence { get; set; }
        /// <summary>
        /// Whether an item with a dialog text attribute will only show the text once.
        /// </summary>
        public bool? OnlyDialogOnce { get; set; }
        /// <summary>
        /// Amount of mana required to cast spell (only used for memorized scrolls).
        /// </summary>
        public int? Mana { get; set; }
        /// <summary>
        /// Whether the tile is a container or not. Containers such as bags, chests and belts can hold other tiles.
        /// Contents of containers (even the characters inventory) is tracked via [Core.State.Items].
        /// </summary>
        public bool? IsContainer { get; set; }
        /// <summary>
        /// Whether the item can be stacked with a variable amount of quantity. Items with "Charges" are NOT stackable.
        /// </summary>
        public bool? CanStack { get; set; }
        /// <summary>
        /// Whether or not a tile can be walked on or blocks travel
        /// </summary>
        public bool? CanWalkOn { get; set; }
        /// <summary>
        /// The number of moves it takes to activate an item
        /// </summary>
        public int? CastTime { get; set; }
        /// <summary>
        /// The range in pixiles that a weapon applies splash damage. The damage is applied as a percentage from the center of the selected target area.
        /// </summary>
        public int? SplashDamageRange { get; set; }
        /// <summary>
        /// The type of damage being done (fire, ice, earth, electrical, etc.)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DamageType? DamageType { get; set; }
        /// <summary>
        /// Whether the item (actor) can be attacked and take damage then be destroyed when HitPoints fall to 0.
        /// </summary>
        public bool? CanTakeDamage { get; set; }
        /// <summary>
        /// The globally unique identifier of the tile. Not populated for terrain tiles.
        /// </summary>
        public Guid? UID { get { return _UID == Guid.Empty ? null : _UID; } set { _UID = value; } }
        /// <summary>
        /// The path to the tile to use for "animating" a projectile. This is a single PNG, not an animation.
        /// </summary>
        public string ProjectileImagePath { get { return _ProjectileImagePath == string.Empty ? null : _ProjectileImagePath; } set { _ProjectileImagePath = value; } }
        /// <summary>
        /// The path to the animation frames that will be used on a successful hit with this tile. Such as an explosion.
        /// </summary>
        public string AnimationImagePath { get { return _AnimationImagePath == string.Empty ? null : _AnimationImagePath; } set { _AnimationImagePath = value; } }
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get { return _Name == string.Empty ? null : _Name; } set { _Name = value; } }

        public string DisplayName
        {
            get
            {
                if (_Name == string.Empty)
                {
                    return null;
                }
                else
                {
                    switch (Enchantment)
                    {
                        case EnchantmentType.Enchanted:
                            return $"{_Name} (Enchanted)";
                        case EnchantmentType.Cursed:
                            return $"{_Name} (Cursed)";
                        case EnchantmentType.Undecided:
                            return $"{_Name} (Unidentified)";
                    }
                    return _Name;
                }
            }
        }

        /// <summary>
        /// For scrolls, this is the name of the spell when it is learned and cast. This is also what spell a book will teach.
        /// </summary>
        public string SpellName { get { return _SpellName == string.Empty ? null : _SpellName; } set { _SpellName = value; } }
        /// <summary>
        /// Used to track the number in a stack when CanStack=true because things like money, arrows, etc only matter in multiples.
        /// </summary>
        public int? Quantity { get { return _Quantity == 0 ? null : _Quantity; } set { _Quantity = value; } }
        /// <summary>
        /// Used for magical items, tells the engine what effect the tile will have (Poison, magic arrow, etc.)
        /// </summary>
        public List<MetaEffect> Effects { get; set; }
        /// <summary>
        /// Whether the ranged weapon uses a bolt or an arrow. Used to search the quiver for an appropriate projectile.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProjectileType? ProjectileType { get; set; }
        /// <summary>
        /// Used for magical items to determine what they affect (the player, terrain, or another actor)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetType? TargetType { get; set; }
        /// <summary>
        /// Whether the item is consumable. e.g. wands have a finite number of charges and scrolls can only be used once.
        /// </summary>
        public bool? IsConsumable { get; set; }
        /// <summary>
        /// The number of charges left on a wand.
        /// </summary>
        public int? Charges { get { return _Charges == 0 ? null : _Charges; } set { _Charges = value; } }
        /// <summary>
        /// How much expereicne the player will receive when destroying this tile.
        /// </summary>
        public int? Experience { get { return _Experience == 0 ? null : _Experience; } set { _Experience = value; } }
        /// <summary>
        /// The number of hitpoints that are required to destroy this tile.
        /// </summary>
        public int? HitPoints { get { return _HitPoints == 0 ? null : _HitPoints; } set { _HitPoints = value; } }
        /// <summary>
        /// The armor class of this tile (also the AC modifier of this tile when equipped).
        /// </summary>
        public int? AC { get { return _AC == 0 ? null : _AC; } set { _AC = value; } }
        /// <summary>
        /// The number of dice used when calcuating damage.
        /// </summary>
        public int? DamageDice { get { return _DamageDice == 0 ? null : _DamageDice; } set { _DamageDice = value; } }
        /// <summary>
        /// The number of faces on the dice used when calcuating damage.
        /// </summary>
        public int? DamageDiceFaces { get { return _DamageDiceFaces == 0 ? null : _DamageDiceFaces; } set { _DamageDiceFaces = value; } }
        /// <summary>
        /// The number extra hitpoints that are added when calcuating damage.
        /// </summary>
        public int? DamageAdditional { get { return _DamageAdditional == 0 ? null : _DamageAdditional; } set { _DamageAdditional = value; } }
        /// <summary>
        /// The original number of hitpoints a tile had. Used to determine the damage percentage.
        /// </summary>
        public int? OriginalHitPoints { get { return _OriginalHitPoints == 0 ? null : _OriginalHitPoints; } set { _OriginalHitPoints = value; } }
        /// <summary>
        /// For containers: the amount of bulk it can carry.
        /// </summary>
        public int? BulkCapacity { get { return _BulkCapacity == 0 ? null : _BulkCapacity; } set { _BulkCapacity = value; } }
        /// <summary>
        /// For containers: the amount of weight it can carry.
        /// </summary>
        public int? WeightCapacity { get { return _WeightCapacity == 0 ? null : _WeightCapacity; } set { _WeightCapacity = value; } }
        /// <summary>
        /// For containers: the count of items it can carry.
        /// </summary>
        public int? ItemCapacity { get { return _ItemCapacity == 0 ? null : _ItemCapacity; } set { _ItemCapacity = value; } }
        /// <summary>
        /// The weight in grams that an item weighs
        /// </summary>
        public double? Weight { get { return _Weight == 0 ? null : _Weight; } set { _Weight = value; } }
        /// <summary>
        /// The bulk in cubic centimeters of an item.
        /// </summary>
        public double? Bulk { get { return _Bulk == 0 ? null : _Bulk; } set { _Bulk = value; } }
        /// <summary>
        /// The dexterity of an actor. Used when determining if a player can hit.
        /// </summary>
        public int? Dexterity { get { return _Dexterity == 0 ? null : _Dexterity; } set { _Dexterity = value; } }
        /// <summary>
        /// The strength of an actor. Used when determining how much damage will be done to player.
        /// </summary>
        public int? Strength { get { return _Strength == 0 ? null : _Strength; } set { _Strength = value; } }
        /// <summary>
        /// The type of tile that will be spawned by the ActorSpawner
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActorClassName? SpawnType { get; set; }
        /// <summary>
        /// The type of tile that will be spawned by the ActorSpawner
        /// </summary>
        public ActorSubType[] SpawnSubTypes;
        /// <summary>
        /// The minimum level of tile that will be spawned by the ActorSpawner
        /// </summary>
        public int? MinLevel { get { return _MinLevel == 0 ? null : _MinLevel; } set { _MinLevel = value; } }
        /// <summary>
        /// The maximum level of tile that will be spawned by the ActorSpawner
        /// </summary>
        public int? MaxLevel { get { return _MaxLevel == 0 ? null : _MaxLevel; } set { _MaxLevel = value; } }
        /// <summary>
        /// Used to know when we should show items, enemies and what to populate in shops.
        /// </summary>
        public int? Level { get { return _Level == 0 ? null : _Level; } set { _Level = value; } }
        /// <summary>
        /// Rough monetary value of the item in a shop.
        /// </summary>
        public double? Value { get { return _Value == 0 ? null : _Value; } set { _Value = value; } }
        /// <summary>
        /// Used for level warp tiles. This tells the engine which level to load.
        /// </summary>
        public string LevelWarpName { get { return _LevelWarpName == string.Empty ? null : _LevelWarpName; } set { _LevelWarpName = value; } }
        /// <summary>
        /// Used for level warp tiles. This tells the engine which tile to spawn to.
        /// </summary>
        public Guid? LevelWarpTargetTileUID { get { return _LevelWarpTargetTileUID == Guid.Empty ? null : _LevelWarpTargetTileUID; } set { _LevelWarpTargetTileUID = value; } }
        /// <summary>
        /// The class that is used to drive the behaivior of this tile.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActorClassName? ActorClass { get; set; }
        /// <summary>
        /// The subtype of this tile. Belt, pack, weapon, etc.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActorSubType? SubType { get { return _SubType == ActorSubType.Unspecified ? null : _SubType; } set { _SubType = value; } }
        /// <summary>
        /// This is what the object will say to the player when approached.
        /// </summary>
        public string Dialog { get { return _Dialog == string.Empty ? null : _Dialog; } set { _Dialog = value; } }

        /// <summary>
        /// Used like a user data object. Like, which key opens this door.
        /// </summary>
        public string Special { get { return _Special == string.Empty ? null : _Special; } set { _Special = value; } }

        [JsonIgnore]
        public string DndDamageText
        {
            get
            {
                if ((DamageDice ?? 0) > 0 && (DamageDiceFaces ?? 0) > 0)
                {
                    var text = $"{DamageDice}d{DamageDiceFaces}";

                    if ((DamageType ?? Types.DamageType.Unspecified) != Types.DamageType.Unspecified)
                    {
                        text += $" ({DamageType})";
                    }

                    if (DamageAdditional > 0)
                    {
                        text += $" +{DamageAdditional}";
                    }

                    if (SplashDamageRange > 0)
                    {
                        text += $" (Splash Range {SplashDamageRange:N0})";
                    }

                    return text;
                }
                return "";
            }
        }

        [JsonIgnore]
        public string PrevalenceText
        {
            get
            {
                return Utility.PrevalenceText(Prevalence ?? -1);
            }
        }

        [JsonIgnore]
        public string EffectText
        {
            get
            {

                return Utility.GetEffectsText(this.Effects).Replace("|", "\r\n");
            }
        }

        public void OverrideWith(TileMetadata with)
        {
            if (with.Effects != null && (this.Effects?.Count ?? 0) == 0)
            {
                this.Effects = new List<MetaEffect>();
                foreach (var effect in with.Effects)
                {
                    this.Effects.Add(new MetaEffect() { EffectType = effect.EffectType, ValueType = effect.ValueType, Value = effect.Value, Duration = effect.Duration });
                }
            }

            this.UID = with.UID ?? this.UID;
            this.CanWalkOn = with.CanWalkOn ?? this.CanWalkOn;
            this.CastTime = with.CastTime ?? this.CastTime;
            this.CanTakeDamage = with.CanTakeDamage ?? this.CanTakeDamage;
            this.ProjectileType = with.ProjectileType ?? this.ProjectileType;
            this.TargetType = with.TargetType ?? this.TargetType;
            this.IsConsumable = with.IsConsumable ?? this.IsConsumable;
            this.Charges = with.Charges ?? this.Charges;
            this.Quantity = with.Quantity ?? this.Quantity;
            this.SpellName = with.SpellName ?? this.SpellName;
            this.HitPoints = with.HitPoints ?? this.HitPoints;
            this.Experience = with.Experience ?? this.Experience;
            this.ActorClass = with.ActorClass ?? this.ActorClass;
            this.Name = with.Name ?? this.Name;
            this.ProjectileImagePath = with.ProjectileImagePath ?? this.ProjectileImagePath;
            this.AnimationImagePath = with.AnimationImagePath ?? this.AnimationImagePath;
            this.SplashDamageRange = with.SplashDamageRange ?? this.SplashDamageRange;
            this.IsMemoriziedSpell = with.IsMemoriziedSpell ?? this.IsMemoriziedSpell;
            this.Enchantment = with.Enchantment ?? this.Enchantment;
            this.IsIdentified = with.IsIdentified ?? this.IsIdentified;
            this.EnchantedImagePath = with.EnchantedImagePath ?? this.EnchantedImagePath;
            this.CursedImagePath = with.CursedImagePath ?? this.CursedImagePath;
            this.Prevalence = with.Prevalence ?? this.Prevalence;
            this.DamageType = with.DamageType ?? this.DamageType;
            this.Special = with.Special ?? this.Special;
            this.Dialog = with.Dialog ?? this.Dialog;
            this.OnlyDialogOnce = with.OnlyDialogOnce ?? this.OnlyDialogOnce;
            this.Mana = with.Mana ?? this.Mana;
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
            this.SpawnSubTypes = with.SpawnSubTypes ?? this.SpawnSubTypes;
            this.MinLevel = with.MinLevel ?? this.MinLevel;
            this.MaxLevel = with.MaxLevel ?? this.MaxLevel;
            this.Level = with.Level ?? this.Level;
            this.Value = with.Value ?? this.Value;

            this.OriginalHitPoints = with.OriginalHitPoints ?? this.OriginalHitPoints;
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
            var fileSystemPath = Path.GetDirectoryName(Constants.GetCommonAssetPath($"{tilePath}"));
            var exactMetaFileName = Constants.GetCommonAssetPath($"{tilePath}.txt");
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

            if (meta.ActorClass == ActorClassName.ActorItem)
            {
                var fileCheck = $"{Constants.GetCommonAssetPath(tilePath)}.Enchanted.png";
                if (File.Exists(fileCheck)) meta.EnchantedImagePath = $"{tilePath}.Enchanted";

                fileCheck = $"{Constants.GetCommonAssetPath(tilePath)}.Cursed.png";
                if (File.Exists(fileCheck)) meta.CursedImagePath = $"{tilePath}.Cursed";

                fileCheck = $"{Constants.GetCommonAssetPath(tilePath)}.Projectile.png";
                if (File.Exists(fileCheck)) meta.ProjectileImagePath = $"{tilePath}.Projectile";

                fileCheck = $"{Constants.GetCommonAssetPath(tilePath)}.Animation.png";
                if (File.Exists(fileCheck)) meta.AnimationImagePath = $"{tilePath}.Animation";

                if (meta.Enchantment == null) //Pick a good default for the enchantment type.
                {
                    if (!string.IsNullOrWhiteSpace(meta.EnchantedImagePath) && string.IsNullOrWhiteSpace(meta.CursedImagePath))
                        meta.Enchantment = EnchantmentType.Enchanted;
                    else if (string.IsNullOrWhiteSpace(meta.EnchantedImagePath) && !string.IsNullOrWhiteSpace(meta.CursedImagePath))
                        meta.Enchantment = EnchantmentType.Cursed;
                    else if (!string.IsNullOrWhiteSpace(meta.EnchantedImagePath) || !string.IsNullOrWhiteSpace(meta.CursedImagePath))
                        meta.Enchantment = EnchantmentType.Undecided;
                    else
                        meta.Enchantment = EnchantmentType.Normal;
                }

                if (meta.Enchantment != EnchantmentType.Undecided && meta.IsIdentified == null)
                {
                    //If the enchantment is not explicitly set, then the item is already identified.
                    meta.IsIdentified = true;
                }
            }

            #region Sanity checks...

            if (meta.ActorClass == ActorClassName.ActorItem
                  && (meta._AC != null || meta._Dexterity != null || meta._Strength != null || meta._HitPoints != null || meta._Experience != null))
                throw new Exception("Items cannot have AC, Dexterity, Strength, HitPoints or Experience. Use [Effects] instead?");

            if (meta.ActorClass == ActorClassName.ActorHostileBeing
                  && ((meta._HitPoints ?? 0) == 0 || (meta._Experience ?? 0) == 0 || (meta._Dexterity ?? 0) == 0
                  || (meta._Strength ?? 0) == 0 || (meta.DamageDice ?? 0) == 0 || (meta.DamageDiceFaces ?? 0) == 0))
                throw new Exception("Hostile actors must have HitPoints, Experience, Dexterity, Strength, DamageDice and DamageDiceFaces.");

            if (((meta.DamageDice ?? 0) != 0 && (meta.DamageDiceFaces ?? 0) == 0) || ((meta.DamageDice ?? 0) == 0 && (meta.DamageDiceFaces ?? 0) != 0))
                throw new Exception("If either DamageDice or DamageDiceFaces is specified, then both must be non-zero.");

            if (meta._Charges > 0 && meta.CanStack == true)
                throw new Exception("Stackable items cannot have Charges.");

            if (meta.Enchantment == EnchantmentType.Undecided && (string.IsNullOrWhiteSpace(meta.EnchantedImagePath) || string.IsNullOrWhiteSpace(meta.CursedImagePath)))
                throw new Exception("If enchantment is Undecided, the EnchantedImagePath and CursedImagePath must be set.");

            if (meta.Enchantment == EnchantmentType.Enchanted && string.IsNullOrWhiteSpace(meta.EnchantedImagePath))
                throw new Exception("If enchantment is Enchanted, the EnchantedImagePath must be set.");

            if (meta.Enchantment == EnchantmentType.Cursed && string.IsNullOrWhiteSpace(meta.CursedImagePath))
                throw new Exception("If enchantment is Cursed, the CursedImagePath must be set.");

            #endregion

            return meta;
        }
    }
}
