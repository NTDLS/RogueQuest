using Assets;
using Game.Actors;
using Game.Classes;
using Library.Engine;
using Library.Engine.Types;
using Library.Native;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using static Game.Engine.Types;

namespace Game.Engine
{
    public class EngineTickController
    {
        private bool _isEngineBusy = false;
        public bool IsEngineBusy
        {
            set
            {
                _isEngineBusy = value;
            }
            get
            {
                return Core.State.ActiveThreadCount > 0 || _isEngineBusy;
            }
        }

        private int _avatarAnimationFrame = 1;
        public delegate void GameThreadCallback(object param);

        public EngineCore Core { get; private set; }
        public EngineTickController(EngineCore core)
        {
            Core = core;
        }

        private Assembly _gameAssembly = null;

        public Assembly GameAssembly
        {
            get
            {
                if (_gameAssembly == null)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    var assemblies = currentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        if (assembly.FullName.StartsWith("Game,"))
                        {
                            _gameAssembly = Assembly.Load("Game");
                        }
                    }
                }

                return _gameAssembly;
            }
        }

        private void PickupPack(ActorItem packOnGround)
        {
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack); //The default container to add items to.
            Guid putItemsIntoContainerId = (Guid)pack.Tile.Meta.UID;
            var groundContainer = Core.Actors.Tiles.Where(o => o.Meta.UID == (Guid)packOnGround.Meta.UID).First();

            double maxBulk = pack.Tile.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Tile.Meta.WeightCapacity ?? 0;

            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => (o.Tile.Meta.Weight ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            var sourceContainerWeight = Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID).Sum(o => (o.Tile.Meta.Weight ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            if (currentPackWeight + sourceContainerWeight > maxWeight)
            {
                Core.LogLine($"This pack contains items too bulky for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => (o.Tile.Meta.Bulk ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            var sourceContainerBulk = Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID).Sum(o => (o.Tile.Meta.Bulk ?? 0) * (o.Tile.Meta.Quantity ?? 1));
            if (currentPackBulk + sourceContainerBulk > maxWeight)
            {
                Core.LogLine($"This pack contains items too heavy for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                return;
            }

            //We typically add the pack to our existing pack, unless the pack we are picking up is the one in the players pack slot.
            //This happens when the player picks up a pack when one is not already equipped.
            if (putItemsIntoContainerId != packOnGround.Meta.UID)
            {
                //Add the pack on the ground to the players inventory pack.
                var nestedPack = new CustodyItem()
                {
                    ContainerId = (Guid)pack.Tile.Meta.UID,
                    Tile = packOnGround.CloneIdentifier()
                };

                putItemsIntoContainerId = (Guid)nestedPack.Tile.Meta.UID; //Put items into the new container we just added to inventory.

                Core.LogLine($"Picked up {packOnGround.Meta.DisplayName}.");

                Core.State.Items.Add(nestedPack);
            }

            List<Guid> itemsToDelete = new List<Guid>();

            //Add all the items in the pack on the ground to the pack in the players inventory pack (nested). :(
            foreach (var item in Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID))
            {
                if ((item.Tile.Meta.IsIdentified ?? false) == false && Core.State.IdentifiedItems.Contains(item.Tile.Meta.Name)
                    && item.Tile.Meta.EventualEnchantmentType == EnchantmentType.Normal)
                {
                    item.Tile.Meta.Identify(Core);
                }

                Core.LogLine($"Picked up" + ((item.Tile.Meta.CanStack == true) ? $" {item.Tile.Meta.Quantity:N0}" : "") + $" {item.Tile.Meta.DisplayName}");

                if (item.Tile.Meta.CanStack == true)
                {
                    var existingItem = Core.State.Items
                        .Where(o => o.Tile.TilePath == item.Tile.TilePath && o.ContainerId == pack.Tile.Meta.UID).FirstOrDefault();
                    if (existingItem != null)
                    {
                        existingItem.Tile.Meta.Quantity = (existingItem.Tile.Meta.Quantity ?? 0) + item.Tile.Meta.Quantity;
                        itemsToDelete.Add((Guid)item.Tile.Meta.UID);
                        continue;
                    }
                }

                item.ContainerId = putItemsIntoContainerId;
            }

            //Delete the items that were stacked onto existing items.
            Core.State.Items.RemoveAll(o => itemsToDelete.Contains((Guid)o.Tile?.Meta?.UID));

            packOnGround.QueueForDelete();
        }

        private void PickupChestContents(Guid containerId)
        {
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            double maxBulk = pack.Tile.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Tile.Meta.WeightCapacity ?? 0;
            var containerContents = Core.State.Items.Where(o => o.ContainerId == containerId);

            var itemsToDelete = new List<Guid>();

            foreach (var item in containerContents)
            {
                //Do weight/bulk math.
                var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => (o.Tile.Meta.Weight ?? 0) * (o.Tile.Meta.Quantity ?? 1));
                if (item.Tile.Meta.Weight + currentPackWeight > maxWeight)
                {
                    Core.LogLine($"{item.Tile.Meta.DisplayName} is too bulky for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                    break;
                }

                var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => (o.Tile.Meta.Bulk ?? 0) * (o.Tile.Meta.Quantity ?? 1));
                if (item.Tile.Meta.Bulk + currentPackBulk > maxBulk)
                {
                    Core.LogLine($"{item.Tile.Meta.DisplayName} is too heavy for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                    break;
                }

                if ((item.Tile.Meta.IsIdentified ?? false) == false && Core.State.IdentifiedItems.Contains(item.Tile.Meta.Name)
                    && item.Tile.Meta.EventualEnchantmentType == EnchantmentType.Normal)
                {
                    item.Tile.Meta.Identify(Core);
                }

                Core.LogLine($"Picked up" + ((item.Tile.Meta.CanStack == true) ? $" {item.Tile.Meta.Quantity:N0}" : "") + $" {item.Tile.Meta.DisplayName}");

                if (item.Tile.Meta.CanStack == true)
                {
                    var existingItem = Core.State.Items
                        .Where(o => o.Tile.TilePath == item.Tile.TilePath && o.ContainerId == pack.Tile.Meta.UID).FirstOrDefault();
                    if (existingItem != null)
                    {
                        existingItem.Tile.Meta.Quantity = (existingItem.Tile.Meta.Quantity ?? 0) + item.Tile.Meta.Quantity;
                        itemsToDelete.Add((Guid)item.Tile.Meta.UID);
                        continue;
                    }
                }

                item.ContainerId = pack.Tile.Meta.UID;
            }

            Core.State.Items.RemoveAll(o => itemsToDelete.Contains((Guid)o.Tile?.Meta?.UID));
        }

        /// <summary>
        /// /// This works for potions, wands and scrolls.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="target"></param>
        /// <returns>Return true is spell was cast, return false for cancel.</returns>
        private bool CastNonWeaponMagic(CustodyItem item, ActorBase target)
        {
            if (item.Tile.Meta.TargetType == TargetType.HostileBeing)
            {
                #region Target: HostileBeing
                var hitType = CalculateHitType(Core.State.Character.Dexterity, target.Meta.AC ?? 0);

                if (hitType == HitType.Miss || hitType == HitType.CriticalMiss)
                {
                    Core.LogLine($"{Core.State.Character.Name} casts {item.Tile.Meta.DisplayName} at {target.Meta.DisplayName} but misses.", Color.DarkRed);
                }
                else
                {
                    foreach (var effect in item.Tile.Meta.Effects)
                    {
                        #region HoldMonster
                        if (effect.EffectType == ItemEffect.HoldMonster)
                        {
                            int expireTime = Core.State.TimePassed + (hitType == HitType.CriticalHit ? (effect.Duration ?? 0) * 2 : (effect.Duration ?? 0));
                            var state = Core.State.ActorStates.UpsertState((Guid)target.Meta.UID, StateOfBeing.Held, expireTime);
                            Core.LogLine($"You successfully held the beast{(hitType == HitType.CriticalHit ? " (Critical-hit doubles time)" : "")}!", Color.DarkGreen);
                        }
                        #endregion
                        #region Poison
                        else if (effect.EffectType == ItemEffect.Poison)
                        {
                            int expireTime = Core.State.TimePassed + (hitType == HitType.CriticalHit ? (effect.Duration ?? 0) * 2 : (effect.Duration ?? 0));
                            var state = Core.State.ActorStates.UpsertState((Guid)target.Meta.UID, StateOfBeing.Poisoned, expireTime);
                            Core.LogLine($"You successfully poison the beast{(hitType == HitType.CriticalHit ? " (Critical-hit doubles time)" : "")}!", Color.DarkGreen);
                        }
                        #endregion
                        else throw new NotImplementedException();
                    }

                    if (string.IsNullOrEmpty(item.Tile.Meta.AnimationImagePath) == false)
                    {
                        AnimateAtAsync(item.Tile.Meta.AnimationImagePath, target);

                    }
                }
                #endregion
            }
            else if (item.Tile.Meta.TargetType == TargetType.UnidentifiedItem)
            {
                using (var form = new FormSelectUnidentifiedItem(Core))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var selectedItem = form.SelectedItem;
                        selectedItem.Meta.Identify(Core);
                        if (selectedItem.Meta.Enchantment == EnchantmentType.Normal) Core.State.IdentifiedItems.Add(selectedItem.Meta.Name);

                        Constants.Alert($"You have identifed this as {selectedItem.Meta.DisplayName}!", "Identified");
                        Core.LogLine($"You have identifed {selectedItem.Meta.DisplayName}!");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (item.Tile.Meta.TargetType == TargetType.Any)
            {
                #region Target: Any
                foreach (var effect in item.Tile.Meta.Effects)
                {
                    #region CastLight
                    if (effect.EffectType == ItemEffect.CastLight)
                    {
                        target.Meta.HasBeenViewed = true;
                        target.AlwaysRender = true;
                        target.Invalidate();

                        var intersections = Core.Actors.Intersections(target, effect.Value).ToList();

                        foreach (var intersection in intersections)
                        {
                            if (intersection.DistanceTo(target) <= effect.Value)
                            {
                                intersection.Meta.HasBeenViewed = true;
                                intersection.AlwaysRender = true;
                                intersection.Invalidate();
                            }
                        }
                    }
                    #endregion
                    else throw new NotImplementedException();
                }

                if (string.IsNullOrEmpty(item.Tile.Meta.AnimationImagePath) == false)
                {
                    AnimateAtAsync(item.Tile.Meta.AnimationImagePath, target);
                }
                #endregion
            }
            else if (item.Tile.Meta.TargetType == TargetType.Terrain)
            {
                #region Target: Terrain

                foreach (var effect in item.Tile.Meta.Effects)
                {
                    #region SummonMonster
                    if (effect.EffectType == ItemEffect.SummonMonster)
                    {
                        var randos = Core.Materials.Where(o => o.Meta.ActorClass == ActorClassName.ActorHostileBeing
                            && o.Meta.Level >= 1 && o.Meta.Level <= item.Tile.Meta.Level).ToList();

                        if (randos.Count > 0)
                        {
                            object[] param = { Core };
                            int rand = MathUtility.RandomNumber(0, randos.Count);
                            var randomTile = randos[rand];
                            var tileType = GameAssembly.GetType($"Game.Actors.{randomTile.Meta.ActorClass}");
                            var tile = (ActorBase)Activator.CreateInstance(tileType, param);

                            if (randomTile != null)
                            {
                                tile.SetImage(Assets.Constants.GetCommonAssetPath($"{randomTile.TilePath}.png"));
                                tile.X = target.X;
                                tile.Y = target.Y;
                                tile.TilePath = randomTile.TilePath;
                                tile.Velocity.Angle.Degrees = tile.Velocity.Angle.Degrees;
                                tile.DrawOrder = target.DrawOrder + 100;
                                tile.Meta = TileMetadata.GetFreshMetadata(randomTile.TilePath);
                                tile.Meta.HasBeenViewed = true;

                                /* //Maybe add some random drops?
                                var ownedItems = Core.State.Items.Where(o => o.ContainerId == spawner.Meta.UID).ToList();
                                foreach (var ownedItem in ownedItems)
                                {
                                    ownedItem.ContainerId = tile.Meta.UID;
                                }
                                */

                                Core.Actors.Add(tile);

                                Core.LogLine($"You carelessly summon a level {tile.Meta.Level} {tile.Meta.DisplayName}!", Color.DarkRed);
                            }
                        }
                    }
                    #endregion
                    else throw new NotImplementedException();
                }

                if (string.IsNullOrEmpty(item.Tile.Meta.AnimationImagePath) == false)
                {
                    AnimateAtAsync(item.Tile.Meta.AnimationImagePath, target);
                }
                #endregion
            }
            else if (item.Tile.Meta.TargetType == TargetType.Self)
            {
                #region Target: Self

                foreach (var effect in item.Tile.Meta.Effects)
                {
                    #region ColdResistance
                    if (effect.EffectType == ItemEffect.ColdResistance)
                    {
                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.ColdResistance);
                        state.ModificationAmount = effect.Value;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region LightningResistance
                    else if (effect.EffectType == ItemEffect.LightningResistance)
                    {
                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.LightningResistance);
                        state.ModificationAmount = effect.Value;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region FireResistance
                    else if (effect.EffectType == ItemEffect.FireResistance)
                    {
                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.FireResistance);
                        state.ModificationAmount = effect.Value;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region EarthResistance
                    else if (effect.EffectType == ItemEffect.EarthResistance)
                    {
                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.EarthResistance);
                        state.ModificationAmount = effect.Value;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region Heal
                    else if (effect.EffectType == ItemEffect.Heal)
                    {
                        int totalHealing = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            int hpToAdd = (int)((double)(Core.State.Character.Hitpoints) * (double)(effect.Value / 100.0));
                            totalHealing += hpToAdd;
                            Core.State.Character.AvailableHitpoints += hpToAdd;
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            totalHealing += effect.Value;
                            Core.State.Character.AvailableHitpoints += effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        if (Core.State.Character.AvailableHitpoints > Core.State.Character.Hitpoints)
                        {
                            Core.State.Character.AvailableHitpoints = Core.State.Character.Hitpoints;
                        }

                        Core.LogLine($"Healed {totalHealing} hitpoints.");
                    }
                    #endregion
                    #region Dexterity
                    else if (effect.EffectType == ItemEffect.Dexterity)
                    {
                        int toAdd = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            toAdd = (int)((double)(Core.State.Character.Dexterity) * (double)(effect.Value / 100.0));
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Dexterity);
                        state.ModificationAmount = toAdd;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region Constitution
                    else if (effect.EffectType == ItemEffect.Constitution)
                    {
                        int toAdd = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            toAdd = (int)((double)(Core.State.Character.Hitpoints) * (double)(effect.Value / 100.0));
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Constitution);
                        state.ModificationAmount = toAdd;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region ArmorClass
                    else if (effect.EffectType == ItemEffect.ArmorClass)
                    {
                        int toAdd = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            toAdd = (int)((double)(Core.State.Character.Armorclass) * (double)(effect.Value / 100.0));
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.ArmorClass);
                        state.ModificationAmount = toAdd;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region Intelligence
                    else if (effect.EffectType == ItemEffect.Intelligence)
                    {
                        int toAdd = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            toAdd = (int)((double)(Core.State.Character.Intelligence) * (double)(effect.Value / 100.0));
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Intelligence);
                        state.ModificationAmount = toAdd;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region Strength
                    else if (effect.EffectType == ItemEffect.Strength)
                    {
                        int toAdd = 0;

                        if (effect.ValueType == ItemEffectType.Percent) //Raise the hitpoints by a percentage.
                        {
                            toAdd = (int)((double)(Core.State.Character.Strength) * (double)(effect.Value / 100.0));
                        }
                        else if (effect.ValueType == ItemEffectType.Fixed) //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = effect.Value;
                        }
                        else throw new Exception($"Value type {effect.ValueType} is not implemented.");

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Strength);
                        state.ModificationAmount = toAdd;
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine(GetAugmentationText(effect));
                    }
                    #endregion
                    #region Poison
                    else if (effect.EffectType == ItemEffect.Poison)
                    {
                        int damage = (int)((double)Core.State.Character.AvailableHitpoints * 0.05); //5% of the remaining hit points.
                        if (damage == 0) damage = 1;
                        Core.State.Character.AvailableHitpoints -= damage;

                        var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Poisoned);
                        state.ExpireTime = Core.State.TimePassed + effect.Duration;
                        Core.LogLine($"You have been poisoned! {damage} damage!", Color.DarkRed);
                    }
                    #endregion
                    #region CurePoison
                    else if (effect.EffectType == ItemEffect.CurePoison)
                    {
                        if (Core.State.ActorStates.HasState(Core.State.Character.UID, StateOfBeing.Poisoned))
                        {
                            Core.LogLine($"Your body has been purged of all poisons!", Color.DarkGreen);
                            Core.State.ActorStates.RemoveState(Core.State.Character.UID, StateOfBeing.Poisoned);
                        }
                        else
                        {
                            Core.LogLine($"Your needlesly burn a lifesaving scroll...");
                        }
                    }
                    #endregion
                    #region RemoveCurse
                    else if (effect.EffectType == ItemEffect.RemoveCurse)
                    {
                        TileIdentifier curseToRemove = null;

                        using (var form = new FormPickCursedItem(Core))
                        {
                            if (form.ShowDialog() != DialogResult.OK)
                            {
                                return false;
                            }
                            curseToRemove = form.SelectedCursedItem;
                        }

                        var equip = Core.State.Character.FindEquipSlotByItemId(curseToRemove.Meta.UID);

                        //Place dropped item on the ground.
                        var droppedItem = Core.Actors.AddDynamic(equip.Tile, Core.Player.X, Core.Player.Y);
                        droppedItem.Meta.HasBeenViewed = true;
                        droppedItem.AlwaysRender = true;

                        //Clear the equip slot.
                        equip.Tile = null;

                        Core.LogLine($"The cursed item fell to the ground!", Color.DarkGreen);
                    }
                    #endregion
                    else throw new NotImplementedException();
                }

                if (string.IsNullOrEmpty(item.Tile.Meta.AnimationImagePath) == false)
                {
                    AnimateAtAsync(item.Tile.Meta.AnimationImagePath, Core.Player);
                }

                #endregion
            }

            return true;
        }

        private string GetAugmentationText(MetaEffect effect)
        {
            string text = effect.EffectType.ToString();

            if (effect.Duration == null)
            {
                text += " permanately";
            }

            text += effect.Value > 0 ? "increased" : "decreased";

            text += $" by {effect.Value}";

            if (effect.Duration != null)
            {
                text += $" for {effect.Duration} minutes.";
            }

            return text;
        }

        public bool UseConsumableItem(Guid itemUid, ActorBase target/*, Guid? rangedProjectile = null*/)
        {
            var item = Core.State.Items.Where(o => o.Tile.Meta.UID == itemUid).First();
            if (item == null || item.Tile.Meta.UID == null)
            {
                return false;
            }

            int startTime = Core.State.TimePassed;

            if (item.Tile.Meta.SubType == ActorSubType.RangedWeapon || item.Tile.Meta.DamageDice > 0) //Bow and arrow, crossbow, etc.
            {
                //Items (and spells) with damage dice are counted as weapons and need to call Advance to apply damage to actors. Cast fireball, cast magic arrow, etc.

                var input = new Types.TickInput()
                {
                    InputType = Types.TickInputType.Ranged,
                    RangedItem = item,
                    RangedTarget = target
                };

                Advance(input);
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Scroll
                || item.Tile.Meta.SubType == ActorSubType.Wand
                || item.Tile.Meta.SubType == ActorSubType.Potion)
            {
                var input = new Types.TickInput()
                {
                    InputType = Types.TickInputType.Ranged,
                    RangedItem = item,
                    RangedTarget = target
                };

                if (CastNonWeaponMagic(item, target) == false)
                {
                    return false;
                }
            }
            else throw new Exception(@"This scenario was not implemented ¯\_(ツ)_/¯");

            PassTime(((item.Tile.Meta.CastTime ?? 0) - (Core.State.TimePassed - startTime)) - Core.State.Character.Speed);

            if (item.Tile.Meta.Charges > 0) //Remember that items with charges are NOT stackable.
            {
                item.Tile.Meta.Charges--;

                if ((item.Tile.Meta.Charges ?? 0) == 0)
                {
                    Core.State.Items.Remove(item);
                    var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                    if (slotToVacate != null)
                    {
                        slotToVacate.Tile = null;
                    }
                }
            }
            else if (item.Tile.Meta.Quantity > 0) //Remember that items with charges are NOT stackable.
            {
                item.Tile.Meta.Quantity--;

                if ((item.Tile.Meta.Quantity ?? 0) == 0)
                {
                    Core.State.Items.Remove(item);
                    var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                    if (slotToVacate != null)
                    {
                        slotToVacate.Tile = null;
                    }
                }
            }
            else if (item.Tile.Meta.IsMemoriziedSpell ?? false)
            {
                Core.State.Character.AvailableMana -= (item.Tile.Meta.Mana ?? 0);
            }
            else if (item.Tile.Meta.SubType == ActorSubType.RangedWeapon)
            {
                var projectile = Core.State.Character.GetQuiverSlotOfType((ProjectileType)item.Tile.Meta.ProjectileType);

                if (projectile != null)
                {
                    if (projectile.Tile?.Meta?.IsConsumable == true)
                    {
                        projectile.Tile.Meta.Quantity--;

                        if ((projectile.Tile?.Meta?.Quantity ?? 0) == 0)
                        {
                            Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == projectile.Tile?.Meta?.UID);

                            var slotToVacate = Core.State.Character.FindEquipSlotByItemId(projectile.Tile?.Meta?.UID);
                            if (slotToVacate != null)
                            {
                                slotToVacate.Tile = null;
                            }
                        }

                        Core.State.Character.PushFreshInventoryItemsToEquipSlots();
                    }
                    else throw new Exception(@"A non-consumable projectile is not possible ¯\_(ツ)_/¯");
                }
                else throw new Exception(@"A ranged weapon with no projectile is not possible ¯\_(ツ)_/¯");
            }
            else
            {
                Core.State.Items.Remove(item);
                var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                if (slotToVacate != null)
                {
                    slotToVacate.Tile = null;
                }
            }

            Core.State.Character.PushFreshInventoryItemsToEquipSlots();

            return true;
        }

        public void Get()
        {
            var itemsUnderfoot = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem)
                .Cast<ActorItem>().ToList();

            //If there are money items underfoot, then add them to the purse instead of the pack.
            {
                var moneyItems = itemsUnderfoot.Where(o => o.Meta.SubType == ActorSubType.Money).ToList();

                moneyItems.ForEach(x => x.QueueForDelete());
                moneyItems.ForEach(x => itemsUnderfoot.Remove(x));

                foreach (var moneyItem in moneyItems)
                {
                    Core.State.Character.AddMoney(moneyItem.CloneIdentifier());
                    Core.LogLine($"Picked up {moneyItem.Meta.Quantity} {moneyItem.Meta.DisplayName} and placed them in your purse.");
                }
            }

            if (itemsUnderfoot.Count() == 0)
            {
                return;
            }

            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack);
            if (pack.Tile == null)
            {
                //Make it easy to put our first pack where it goes.
                var underfootPack = itemsUnderfoot.Where(o => o.Meta.SubType == ActorSubType.Pack).FirstOrDefault();
                if (underfootPack != null)
                {
                    //TODO: We need to make sure that the player can even pick up this pack. It might be too heavy?
                    if ((underfootPack.Meta.IsIdentified ?? false) == false)
                    {
                        underfootPack.Meta.Identify(Core);
                    }

                    if (underfootPack.Meta.Enchantment == EnchantmentType.Cursed)
                    {
                        Core.LogLine($"Well this is unfortunate....", Color.DarkRed);
                    }

                    Core.LogLine($"Picked up {underfootPack.Meta.DisplayName} and placed it on your back.");
                    pack.Tile = underfootPack.CloneIdentifier();

                    var inventoryItem = new CustodyItem()
                    {
                        Tile = underfootPack.CloneIdentifier()
                    };

                    Core.State.Items.Add(inventoryItem);

                    PickupPack(underfootPack);

                    underfootPack.QueueForDelete();
                    return;
                }

                Core.LogLine($"You'll need a pack if you want to carry items. Maybe use your free hand?");
                return;
            }

            Guid containerId = (Guid)pack.Tile.Meta.UID;
            double maxBulk = pack.Tile.Meta.BulkCapacity ?? 0;
            double maxWeight = pack.Tile.Meta.WeightCapacity ?? 0;
            int? maxItems = pack.Tile.Meta.ItemCapacity;

            foreach (var item in itemsUnderfoot)
            {
                if (item.Meta.IsContainer == true)
                {
                    //We take stuff out of all containers except for bags. For bags, we pick up the entire thing.
                    if (item.Meta.SubType == ActorSubType.Pack || item.Meta.SubType == ActorSubType.Belt)
                    {
                        PickupPack(item);
                    }
                    else
                    {
                        PickupChestContents((Guid)item.Meta.UID);
                    }
                }
                else
                {
                    //Do weight/bulk math.
                    var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == containerId).Sum(o => (o.Tile.Meta.Weight ?? 0) * (o.Tile.Meta.Quantity ?? 1));
                    if (item.Meta.Weight + currentPackWeight > maxWeight)
                    {
                        Core.LogLine($"{item.Meta.DisplayName} is too bulky for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                        break;
                    }

                    var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == containerId).Sum(o => (o.Tile.Meta.Bulk ?? 0) * (o.Tile.Meta.Quantity ?? 1));
                    if (item.Meta.Bulk + currentPackBulk > maxBulk)
                    {
                        Core.LogLine($"{item.Meta.DisplayName} is too heavy for your {pack.Tile.Meta.DisplayName}. Drop something or move to free hand?");
                        break;
                    }

                    if (maxItems != null)
                    {
                        var currentPackItems = Core.State.Items.Where(o => o.ContainerId == containerId).Count();
                        if (currentPackItems + 1 > (int)maxItems)
                        {
                            Core.LogLine($"{pack.Tile.Meta.DisplayName} can only carry {maxItems} items. Drop something or move to free hand?");
                            break;
                        }
                    }

                    if ((item.Meta.IsIdentified ?? false) == false && Core.State.IdentifiedItems.Contains(item.Meta.Name)
                        && item.Meta.EventualEnchantmentType == EnchantmentType.Normal)
                    {
                        item.Meta.Identify(Core);
                    }

                    Core.LogLine($"Picked up" + ((item.Meta.CanStack == true) ? $" {item.Meta.Quantity:N0}" : "") + $" {item.Meta.DisplayName}");

                    if (item.Meta.CanStack == true)
                    {
                        var existingItem = Core.State.Items
                            .Where(o => o.Tile.TilePath == item.TilePath && o.ContainerId == pack.Tile.Meta.UID).FirstOrDefault();
                        if (existingItem != null)
                        {
                            existingItem.Tile.Meta.Quantity = (existingItem.Tile.Meta.Quantity ?? 0) + item.Meta.Quantity;
                            item.QueueForDelete();
                            continue;
                        }
                    }

                    var inventoryItem = new CustodyItem()
                    {
                        ContainerId = containerId,
                        Tile = item.CloneIdentifier()
                    };

                    Core.State.Items.Add(inventoryItem);

                    item.QueueForDelete();
                }
            }
        }

        public void Rest()
        {
            var input = new Types.TickInput() { InputType = Types.TickInputType.Rest };

            if (Core.State.Character.AvailableHitpoints >= Core.State.Character.Hitpoints
                && Core.State.Character.AvailableMana >= Core.State.Character.Mana)
            {
                Core.LogLine($"Feeling no need to rest you press on.", Color.DarkGreen);
                Advance(input);
                return;
            }

            int statMod = 0;

            while (Core.State.Character.AvailableHitpoints < Core.State.Character.Hitpoints
                || Core.State.Character.AvailableMana < Core.State.Character.Mana)
            {
                Advance(input);

                var actorsThatCanSeePlayer = Core.Actors.Intersections(Core.Player, 250)
                    .Where(o => o.Meta.CanTakeDamage == true && o.Meta.ActorClass == ActorClassName.ActorHostileBeing);

                if (actorsThatCanSeePlayer.Any())
                {
                    var firstActor = actorsThatCanSeePlayer.First();
                    Core.LogLine($"Your rest was interrupted by a {firstActor.Meta.DisplayName}!", Color.DarkRed);
                    return;
                }

                if ((statMod % 2) == 0)
                {
                    Core.State.Character.AvailableHitpoints++;
                }

                if ((statMod % 4) == 0)
                {
                    Core.State.Character.AvailableMana++;
                }
            }

            Core.LogLine($"You awake feeling refreshed.", Color.DarkGreen);
        }

        public void PassTime(int startTime, int? timeToPass)
        {
            if (timeToPass != null && timeToPass > 0)
            {
                int startGameTime = startTime;

                var input = new Types.TickInput() { InputType = Types.TickInputType.Wait };

                while (Core.State.TimePassed - startGameTime < timeToPass)
                {
                    Advance(input);
                    Application.DoEvents(); //The UI thread is a bitch.
                }
                Thread.Sleep(500);
            }
        }

        public void PassTime(int? timeToPass)
        {
            if (timeToPass != null && timeToPass > 0)
            {
                int startGameTime = Core.State.TimePassed;

                var input = new Types.TickInput() { InputType = Types.TickInputType.Wait };

                while (Core.State.TimePassed - startGameTime < timeToPass)
                {
                    Advance(input);
                    Application.DoEvents(); //The UI thread is a bitch.
                    Thread.Sleep(250);
                }
            }
        }

        public Point<double> Advance(TickInput Input, GameThreadCallback callback = null, object callbackParam = null)
        {
            var appliedOffset = new Point<double>();

            List<ActorBase> intersections = new List<ActorBase>();

            if (Input.InputType == TickInputType.Movement)
            {
                Core.Player.Velocity.Angle.Degrees = Input.Degrees;
                Core.Player.Velocity.ThrottlePercentage = Input.Throttle;

                var angle = Core.Player.Velocity.Angle.Degrees;

                string playerAngle = "front";
                if ((angle >= 0 && angle < 45) || (angle > 315 && angle < 359))
                {
                    playerAngle = "Back";
                }
                else if ((angle >= 45 && angle <= 135))
                {
                    playerAngle = "Right";
                }
                else if ((angle >= 135 && angle <= 224))
                {
                    playerAngle = "Front";
                }
                else if ((angle >= 225 && angle <= 315))
                {
                    playerAngle = "Left";
                }

                string assetPath = Assets.Constants.GetCommonAssetPath(@$"Tiles\Special\@Player\{Core.State.Character.Avatar}\{playerAngle} {_avatarAnimationFrame}.png");
                Core.Player.SetImage(SpriteCache.GetBitmapCached(assetPath));
                if (_avatarAnimationFrame++ == 2)
                {
                    _avatarAnimationFrame = 1;
                }

                intersections.AddRange(MoveActor(Core.Player, out appliedOffset));
                ScrollBackground(appliedOffset);
            }
            else if (Input.InputType == TickInputType.Rest)
            {
                Core.Player.Velocity.ThrottlePercentage = 0;
            }

            Core.State.TimePassed++;

            var storeTile = intersections.Where(o => o.Meta.ActorClass == ActorClassName.ActorStore && o.Meta.SubType != ActorSubType.RuinsStore).FirstOrDefault();
            if (storeTile != null)
            {
                using (var form = new FormStore(Core, storeTile.Meta))
                {
                    form.ShowDialog();
                }
            }

            IsEngineBusy = true;

            ParameterizedThreadStart starter = GameLogicThread;
            starter += (param) =>
            {
                if (callback != null)
                {
                    callback(callbackParam);
                }
            };

            var thread = new Thread(starter) { IsBackground = true };
            thread.Start(new GameLogicParam()
            {
                Intersections = intersections,
                Input = Input
            });

            WaitOnThreadWithShamefulDoEvents(thread);

            IsEngineBusy = false;

            return appliedOffset;
        }

        private HitType CalculateHitType(int agressorDexterity, int victimAC)
        {
            //https://roll20.net/compendium/dnd5e/Items:Leather%20Armor/#h-Leather%20Armor

            /*
                Note that attack bonus is your total attack bonus with all modifiers.
                    We can test this out pretty easily. If you have a +7 to hit and are
                    attacking a target with AC of 18, you need an 11 to hit.
                    11 through 20 is 10 faces on the d20, so it's half the die. A 50% chance to hit.

                21 + 7 − 18
               ------------  = 0.5
                    20

                If you are attacking the same AC 18 target and have a +2 attack bonus, you need
                    a 16 or better to hit. You hit on 16, 17, 18, 19, or 20. That's five of the
                    twenty sides of a d20, which is a quarter or 25% of the die.

                21 + 2 - 18
               ------------ = 0.25
                    20
            */

            int diceRoll = MathUtility.RandomNumber(1, 21);

            if (diceRoll == 1)
            {
                return HitType.CriticalMiss;
            }
            else if (diceRoll == 20)
            {
                return HitType.CriticalHit;
            }

            double chanceToHit = (((21.0 + agressorDexterity) - victimAC) / 20.0);

            if (chanceToHit < 0.05) chanceToHit = 0.05;
            if (chanceToHit > 0.95) chanceToHit = 0.95;

            int diceRollBetterThan = (int)(21 - (20 * chanceToHit));

            bool doWeHit = diceRoll >= diceRollBetterThan;

            return doWeHit ? HitType.Hit : HitType.Miss;
        }

        private int CalculateDealtDamage(HitType hitType, int strength, int additionalDamage, int dice, int faces)
        {
            int damage = strength;  //Characters base damage.

            //Get the additional damage added by all equipped items (basically looking for enchanted items that add damage, like rings, cloaks, etc).
            //This also gets the additional damage for the equipped weapon. For example, for a +3 Enchanted Long Sword, this is the 3.
            damage += additionalDamage;

            //Weapon strike damage.
            for (int i = 0; i < dice; i++)
            {
                damage += MathUtility.RandomNumber(1, faces + 1);

                //Critical hit allows for double weapon damage.
                if (hitType == HitType.CriticalHit)
                {
                    damage += MathUtility.RandomNumber(1, faces + 1);
                }
            }

            return damage;
        }

        public void HandleDialogInput()
        {
            //Dont want the player to accidently skip the dialog.
            if ((DateTime.Now - Dialogs.DialogOpenTime).TotalSeconds > 1)
            {
                Core.Actors.Tiles.RemoveAll(o => o.Meta?.ActorClass == ActorClassName.ActorDialog);
                Core.PurgeAllDeletedTiles();
                Core.Display.DrawingSurface.Invalidate();
                Core.State.IsDialogActive = false;
            }
        }

        private object GameLogicThreadLock = new object();

        /// <summary>
        /// Intersections contains all objects that the player collided with during their turn. If this
        /// contains actors that can be damaged, then these  would be the melee attack target for the player.
        /// </summary>
        /// <param name="intersections"></param>
        private void GameLogicThread(object param)
        {
            Core.State.AddThreadReference();
            Thread.CurrentThread.Name = $"GameLogicThread_{Core.State.ActiveThreadCount}";
            GameLogicThreadEx((GameLogicParam)param);
            Core.State.RemoveThreadReference();
        }

        private void GameLogicThreadEx(GameLogicParam p)
        {
            List<ActorBase> intersections = p.Intersections;
            TickInput Input = p.Input;

            bool warpped = false;

            if (intersections.Where(o => o.Meta.ActorClass == ActorClassName.ActorKeyedEntry).Any())
            {
                bool unlocked = false;
                var entry = intersections.Where(o => o.Meta.ActorClass == ActorClassName.ActorKeyedEntry).First();
                var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack); //The default container to add items to.
                if (pack != null)
                {
                    var existingItem = Core.State.Items
                        .Where(o => o.ContainerId == pack.Tile.Meta.UID && o.Tile.Meta?.SubType == ActorSubType.Key
                        && o.Tile?.Meta.Special == entry.Meta?.Special).FirstOrDefault();
                    if (existingItem != null)
                    {
                        Core.LogLine($"With a turn of the {existingItem.Tile.Meta.DisplayName}, the obstacle has been removed - the key vanished with it....", Color.Green);
                        Core.State.Items.Remove(existingItem);
                        entry.QueueForDelete();
                        unlocked = true;
                    }
                }

                if (!unlocked) Core.LogLine($"You will need a {entry.Meta.DisplayName} key to gain access.", Color.Red);
            }

            if (intersections.Where(o => o.Meta.ActorClass == ActorClassName.ActorLevelWarpHidden
                || o.Meta.ActorClass == ActorClassName.ActorLevelWarpVisible).Any())
            {
                var warp = intersections.Where(o => o.Meta.ActorClass == ActorClassName.ActorLevelWarpHidden
                    || o.Meta.ActorClass == ActorClassName.ActorLevelWarpVisible).First();

                if (string.IsNullOrEmpty(warp.Meta.LevelWarpName) == false && warp.Meta.LevelWarpTargetTileUID != null)
                {
                    Core.LogLine($"After a long travel you arrive in {warp.Meta.LevelWarpName}");
                    Core.LevelWarp(warp.Meta.LevelWarpName, (Guid)warp.Meta.LevelWarpTargetTileUID);
                }

                Core.PurgeAllDeletedTiles();

                warpped = true; //We want to return if we are warping, but we also want to show any dialogs first.
            }

            var somethingToSay = intersections.Where(o => string.IsNullOrEmpty(o.Meta.Dialog) == false).FirstOrDefault();
            if (somethingToSay != null)
            {
                Dialogs.DrawDialog(Core, somethingToSay.Meta.Dialog);

                if (somethingToSay.Meta.OnlyDialogOnce == true)
                {
                    somethingToSay.Meta.Dialog = "";
                }
            }

            if (warpped)
            {
                return;
            }

            var allActiveStates = Core.State.ActorStates.States()
                .Where(o => Core.State.TimePassed < o.ExpireTime).ToList();

            foreach (var activeState in allActiveStates)
            {
                //We want do to do this early because we dont want states that are added this second to affect the players the same second.
                ActOnActiveState(activeState);
            }

            List<ActorBase> recentlyEngagedHostiles = new List<ActorBase>();

            var actorsThatCanSeePlayer = Core.Actors.Intersections(Core.Player, 250)
                .Where(o => o.Meta.CanTakeDamage == true).ToList();

            //The recently wngaged hostiles will remain hostile until 2*their dexterity seconds have passed.
            Core.State.RecentlyEngagedHostiles.RemoveAll(o => (Core.State.TimePassed - o.InteractionTime) > o.Hostile.Meta.Dexterity * 2);

            actorsThatCanSeePlayer.AddRange(Core.State.RecentlyEngagedHostiles.Select(o => o.Hostile));

            var hostileInteractions = new List<ActorHostileBeing>();

            //Find out which of the hostile beings in visible range are touching the
            //  player bounds (not intersecting, because there should be none, but just touching.
            foreach (var obj in actorsThatCanSeePlayer.Where(o => o.Meta.ActorClass == ActorClassName.ActorHostileBeing))
            {
                var largerBounds = new Rectangle(obj.ScreenBounds.X - 1, obj.ScreenBounds.Y - 1, obj.ScreenBounds.Width + 2, obj.ScreenBounds.Height + 2);
                if (Core.Player.ScreenBounds.IntersectsWith(largerBounds))
                {
                    hostileInteractions.Add(obj as ActorHostileBeing);
                }
            }

            //Hostile actors will follow player.
            foreach (var obj in actorsThatCanSeePlayer.Where(o => o.Meta.ActorClass == ActorClassName.ActorHostileBeing))
            {
                var actor = obj as ActorHostileBeing;
                double distance = Core.Player.DistanceTo(obj);
                double maxFollowDistance = actor.MaxFollowDistance;

                //If you have recently hit this actor, he is going to be much more compelled to follow you.
                if (Core.State.RecentlyEngagedHostiles.Where(o => o.Hostile == obj).Any())
                {
                    maxFollowDistance *= 2;
                }

                if (distance < maxFollowDistance)
                {
                    var appliedOtherOffset = new Point<double>();
                    obj.Velocity.Angle.Degrees = actor.AngleTo(Core.Player);
                    MoveActor(actor, out appliedOtherOffset);
                }
            }

            /*
             * This proc can be called with an explicit projectile meaning that we are using a ranged weapon as a ranged weapon.
             * We can also attack at close range with a ranged weapon by running into a hostile. In this case we will need to
             * automatically select a projectile and use it. If we have no projectiles, then we do no damage.
             * 
             * Alternatively, if we have a melee weapon in our free-hand, we will use it when attacking at close range.
             */

            var weapon = Core.State.Character.GetEquipSlot(EquipSlot.Weapon)?.Tile?.Meta;
            TileIdentifier projectile = null;
            ActorBase actorToAttack = null;

            if (Input.RangedTarget != null && Input.RangedItem != null && Input.RangedTarget.Meta.ActorClass == ActorClassName.ActorHostileBeing) //Ranged attack.
            {
                weapon = Input.RangedItem.Tile.Meta;
                actorToAttack = Input.RangedTarget;
            }
            else //Melee attack.
            {
                //If this is a ranged weapon but we are in a melee situation, then check the free hand for a melee weapon and use it instead.
                if (weapon != null && weapon.SubType == ActorSubType.RangedWeapon)
                {
                    var freehandWeapon = Core.State.Character.GetEquipSlot(EquipSlot.FreeHand)?.Tile?.Meta;

                    if (freehandWeapon != null && freehandWeapon.ProjectileType == ProjectileType.Unspecified) //This Is this a melee weapon?
                    {
                        weapon = freehandWeapon;
                    }
                }

                //Player attack other actor.
                var actorsToAttack = intersections.Where(o => o.Meta.CanTakeDamage == true).ToList();
                foreach (var otherActor in actorsToAttack)
                {
                    if (Core.Player.IsPointingAt(otherActor, 60))
                    {
                        actorToAttack = otherActor;
                        break;
                    }
                }
            }

            if (actorToAttack != null)
            {
                if (weapon != null && weapon.ProjectileType != null && weapon.ProjectileType != ProjectileType.Unspecified) //Melee attack with ranged weapon.
                {
                    projectile = Core.State.Character.GetInventoryItemFromQuiverSlotOfType((ProjectileType)weapon.ProjectileType)?.Tile;
                    if (projectile == null)
                    {
                        Core.LogLine($"You are out of projectiles for the equipped ranged weapon!!!", Color.DarkRed);
                    }
                }

                if (projectile != null)
                {
                    AnimateTo(projectile.TilePath, Core.Player, actorToAttack);
                }
                else if (weapon != null && string.IsNullOrEmpty(weapon.ProjectileImagePath) == false)
                {
                    AnimateTo(weapon.ProjectileImagePath, Core.Player, actorToAttack);
                }
            }

            string weaponDescription = weapon?.DisplayName;

            if (projectile != null)
            {
                weaponDescription += $" ({projectile.Meta.DisplayName})";
            }

            if (actorToAttack != null && weapon != null)
            {
                var hitType = CalculateHitType(Core.State.Character.Dexterity, (int)actorToAttack.Meta.AC);

                if ((weapon.DamageDice ?? 0) + (projectile?.Meta?.DamageDice ?? 0) > 0)
                {
                    //Get the additional damage added by all equipped items (basically looking for enchanted items that add damage, like rings, cloaks, etc).
                    EquipSlot[] additionalDamageSearchSlots = new EquipSlot[]
                        {
                        EquipSlot.Pack,
                        EquipSlot.Belt,
                        EquipSlot.RightRing,
                        EquipSlot.Bracers,
                        EquipSlot.Armor,
                        EquipSlot.Boots,
                        EquipSlot.Necklace,
                        EquipSlot.Garment,
                        EquipSlot.Helment,
                        EquipSlot.Shield,
                        EquipSlot.Gauntlets,
                        EquipSlot.LeftRing,
                        };

                    int additionalDamage = Core.State.Character.Equipment.Where(o => o.Tile != null && additionalDamageSearchSlots.Contains(o.Slot))
                        .Sum(o => o.Tile.Meta.DamageAdditional) ?? 0;

                    //This also gets the additional damage for the equipped weapon. For example, for a +3 Enchanted Long Sword, this is the 3.
                    additionalDamage += (weapon.DamageAdditional ?? 0) + (projectile?.Meta?.DamageAdditional ?? 0);

                    if (hitType == HitType.Hit || hitType == HitType.CriticalHit)
                    {
                        int playerHitsFor = 0;

                        DamageType? damageType = weapon?.DamageType;

                        //If we have a projectile, we want to use its damage type. (like a bow shooting a fire arrow).
                        if (projectile != null && projectile.Meta?.DamageType != null)
                        {
                            damageType = projectile.Meta?.DamageType;
                        }

                        if ((weapon?.DamageDice ?? 0) + (projectile?.Meta?.DamageDice ?? 0) > 0)
                        {
                            playerHitsFor = CalculateDealtDamage(hitType, Core.State.Character.Strength,
                                additionalDamage, (weapon?.DamageDice ?? 0) + (projectile?.Meta?.DamageDice ?? 0), (weapon?.DamageDiceFaces ?? 0) + (projectile?.Meta?.DamageDiceFaces ?? 0));
                        }

                        if (projectile != null && projectile.Meta?.DamageDice > 0)
                        {
                            playerHitsFor += CalculateDealtDamage(hitType, Core.State.Character.Strength,
                                additionalDamage, projectile.Meta?.DamageDice ?? 0, projectile.Meta?.DamageDiceFaces ?? 0);

                            if (projectile?.Meta.DamageType != null)
                            {
                                damageType = projectile?.Meta.DamageType;
                            }
                        }

                        var actorsTakingDamage = new List<ActorToDamage>
                        {
                            new ActorToDamage()
                            {
                                Actor = actorToAttack,
                                IsPrimaryTarget = true
                            }
                        };

                        if ((weapon?.SplashDamageRange ?? 0) + (projectile?.Meta?.SplashDamageRange ?? 0) > 0)
                        {
                            var splashActors = Core.Actors.Intersections(actorToAttack, (int)(weapon?.SplashDamageRange ?? 0) + (projectile?.Meta?.SplashDamageRange ?? 0))
                                .Where(o => o.Meta.CanTakeDamage == true);

                            foreach (var splashActor in splashActors)
                            {
                                double distanceFromPrimary = splashActor.DistanceTo(actorToAttack);
                                if (distanceFromPrimary < (int)(weapon?.SplashDamageRange ?? 0) + (projectile?.Meta?.SplashDamageRange ?? 0))
                                {
                                    actorsTakingDamage.Add(new ActorToDamage()
                                    {
                                        Actor = splashActor,
                                        DistanceFromPrimary = distanceFromPrimary,
                                        IsPrimaryTarget = false,
                                        IsSplashDamage = true
                                    });
                                }
                            }
                        }

                        //Hit animation:
                        if (projectile != null)
                        {
                            if (string.IsNullOrEmpty(projectile.Meta.AnimationImagePath) == false)
                            {
                                AnimateAt(projectile.Meta.AnimationImagePath, actorToAttack);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(weapon.AnimationImagePath) == false)
                            {
                                AnimateAt(weapon.AnimationImagePath, actorToAttack);
                            }
                        }

                        foreach (var actorTakingDamage in actorsTakingDamage)
                        {
                            //If we hit an enemy, they are going to fight back.
                            var recentInteraction = Core.State.RecentlyEngagedHostiles.Where(o => o.Hostile == actorTakingDamage.Actor).FirstOrDefault();
                            if (recentInteraction == null)
                            {
                                Core.State.RecentlyEngagedHostiles.Add(new RecentlyEngagedHostile()
                                {
                                    Hostile = actorTakingDamage.Actor,
                                    InteractionTime = Core.State.TimePassed
                                });
                            }
                            else
                            {
                                recentInteraction.InteractionTime = Core.State.TimePassed;
                            }

                            int damageToApply = playerHitsFor;

                            if (actorTakingDamage.IsSplashDamage)
                            {
                                damageToApply = (int)(damageToApply * (actorTakingDamage.DistanceFromPrimary / (double)(weapon?.SplashDamageRange ?? 0) + (projectile?.Meta?.SplashDamageRange ?? 0)));
                            }

                            if (actorTakingDamage.Actor.Meta.DamageType != null)
                            {
                                var weakness = Utility.GetOppositeOfDamageType((DamageType)actorTakingDamage.Actor.Meta.DamageType);

                                if (weakness == damageType)
                                {
                                    Core.LogLine($"{actorToAttack.Meta.DisplayName} is weak to {weaponDescription}'s {damageType} which does double damage.", Color.DarkOliveGreen);
                                    damageToApply *= 2;
                                }
                                else if (actorTakingDamage.Actor.Meta.DamageType == damageType)
                                {
                                    Core.LogLine($"{actorToAttack.Meta.DisplayName} is resistant to {weaponDescription}'s {damageType} which does half damage.", Color.DarkRed);
                                    damageToApply = (damageToApply / 2);
                                }
                            }

                            if (damageType == DamageType.Poison)
                            {
                                if (MathUtility.FlipCoin()) //50% chance
                                {
                                    if (Core.State.ActorStates.HasState((Guid)actorTakingDamage.Actor.Meta.UID, StateOfBeing.Poisoned) == false)
                                    {
                                        var state = Core.State.ActorStates.AddState((Guid)actorTakingDamage.Actor.Meta.UID, StateOfBeing.Poisoned);
                                        state.ExpireTime = 10;
                                        Core.LogLine($"{actorToAttack.Meta.DisplayName} has been poisoned!", Color.DarkOliveGreen);
                                    }
                                }
                            }

                            if (hitType == HitType.CriticalHit)
                            {
                                Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.DisplayName} with {weaponDescription} for {damageToApply}hp {GetCriticalHitText()}", Color.DarkOliveGreen);
                            }
                            else
                            {
                                Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.DisplayName} with {weaponDescription} for {damageToApply}hp and hits.", Color.DarkGreen);
                            }

                            if (actorTakingDamage.Actor.ApplyDamage(playerHitsFor))
                            {
                                int experience = 0;

                                if (actorTakingDamage.Actor.Meta != null && actorTakingDamage.Actor.Meta.Experience != null)
                                {
                                    experience = (int)actorTakingDamage.Actor.Meta.Experience;
                                }

                                Core.LogLine($"{Core.State.Character.Name} kills {actorTakingDamage.Actor.Meta.DisplayName} gaining {experience}xp!", Color.DarkGreen);

                                if (actorTakingDamage.Actor.Meta.IsContainer == true)
                                {
                                    //If the enemy has loot, they drop it when they die.
                                    EmptyContainerToGround(actorTakingDamage.Actor.Meta.UID, actorTakingDamage.Actor);
                                }

                                Core.State.Character.Experience += experience;

                                //No need to keep this guy around, hes been killed.
                                Core.State.RecentlyEngagedHostiles.RemoveAll(o => o.Hostile == actorTakingDamage.Actor);
                            }
                        }
                    }
                    else if (hitType == HitType.CriticalMiss)
                    {
                        Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.DisplayName} {GetCriticalMissText()}", Color.DarkRed);
                    }
                    else
                    {
                        Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.DisplayName} but misses.", Color.DarkRed);
                    }
                }
                else
                {
                    Core.LogLine($"This item seems to do no damage nor have any effect?", Color.DarkRed);
                }
            }

            //var activePlayerStates = Core.State.ActorStates.States(Core.State.Character.UID);

            //Hostiles attack player. Be sure to look at visible actors only because the player may have killed one before we get here.
            foreach (var hostile in hostileInteractions.Where(o => o.Visible))
            {
                if (Core.State.ActorStates.HasState((Guid)hostile.Meta.UID, StateOfBeing.Held))
                {
                    Core.LogLine($"{hostile.Meta.DisplayName} is held, cannot attack.", Color.DarkGreen);
                    continue;
                }

                //Monster hit player.
                var hitType = CalculateHitType((int)hostile.Meta.Dexterity, Core.State.Character.Armorclass);
                if (hitType == HitType.Hit)
                {
                    int hostileHitsFor = CalculateDealtDamage(hitType, (int)hostile.Meta.Strength,
                        0, hostile.Meta.DamageDice ?? 0, hostile.Meta?.DamageDiceFaces ?? 0);

                    if (hostile.Meta.DamageType == DamageType.Poison)
                    {
                        if (Core.State.ActorStates.HasState(Core.State.Character.UID, StateOfBeing.Poisoned) == false)
                        {
                            if (MathUtility.FlipCoin())
                            {
                                var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Poisoned);
                                state.ExpireTime = 10;
                                Core.LogLine($"You has been poisoned!", Color.DarkRed);
                            }
                        }
                    }

                    int coldResistance = Core.State.Character.ColdResistance;
                    int fireResistance = Core.State.Character.FireResistance;
                    int earthResistance = Core.State.Character.EarthResistance;
                    int electricResistance = Core.State.Character.LightningResistance;

                    #region Damage resistance...
                    if (hostile.Meta.DamageType == DamageType.Fire && fireResistance > 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < fireResistance; i++)
                        {
                            resistanceFactor = resistanceFactor / 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} resists {resistedDamage:N0}hp fire damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Cold && coldResistance > 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < coldResistance; i++)
                        {
                            resistanceFactor = resistanceFactor / 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} resists {resistedDamage:N0}hp cold damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Earth && earthResistance > 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < earthResistance; i++)
                        {
                            resistanceFactor = resistanceFactor / 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} resists {resistedDamage:N0}hp earth damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Lightning && electricResistance > 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < electricResistance; i++)
                        {
                            resistanceFactor = resistanceFactor / 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} resists {resistedDamage:N0}hp lightning damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    #endregion

                    #region Opposite of damage resistance (weakness)...
                    if (hostile.Meta.DamageType == DamageType.Fire && fireResistance < 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < (fireResistance * -1); i++)
                        {
                            resistanceFactor = resistanceFactor * 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} is weak to {resistedDamage:N0}hp fire damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Cold && coldResistance < 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < (coldResistance * -1); i++)
                        {
                            resistanceFactor = resistanceFactor * 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} is weak to {resistedDamage:N0}hp cold damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Earth && earthResistance < 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < (earthResistance * -1); i++)
                        {
                            resistanceFactor = resistanceFactor * 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} is weak to {resistedDamage:N0}hp earth damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    else if (hostile.Meta.DamageType == DamageType.Lightning && electricResistance < 0)
                    {
                        double resistanceFactor = hostileHitsFor;

                        for (int i = 0; i < (electricResistance * -1); i++)
                        {
                            resistanceFactor = resistanceFactor * 2.0;
                        }

                        int resistedDamage = (hostileHitsFor - (int)resistanceFactor);

                        Core.LogLine($"{Core.State.Character.Name} is weak to {resistedDamage:N0}hp lightning damage.", Color.DarkGreen);

                        hostileHitsFor = hostileHitsFor - resistedDamage;
                    }
                    #endregion

                    string damageType = string.Empty;
                    if (hostile.Meta.DamageType != null && hostile.Meta.DamageType != DamageType.Unspecified)
                    {
                        damageType = $" ({hostile.Meta.DamageType} damage)";
                    }

                    if (hitType == HitType.CriticalHit)
                    {
                        Core.LogLine($"{hostile.Meta.DisplayName} attacks {Core.State.Character.Name} for {hostileHitsFor}hp{damageType} landing a critical hit!", Color.DarkRed);
                    }
                    else
                    {
                        Core.LogLine($"{hostile.Meta.DisplayName} attacks {Core.State.Character.Name} for {hostileHitsFor}hp{damageType} and hits.", Color.DarkRed);
                    }

                    Core.State.Character.AvailableHitpoints -= hostileHitsFor;
                    if (Core.State.Character.AvailableHitpoints <= 0)
                    {
                        Core.LogLine($"{PlayerDeathText()}", Color.Red);
                        Core.Player.QueueForDelete();
                        break;
                    }
                }
                else if (hitType == HitType.CriticalMiss)
                {
                    Core.LogLine($"{hostile.Meta.DisplayName} attacks {Core.State.Character.Name} resulting in a critical miss!", Color.DarkGreen);
                }
                else
                {
                    Core.LogLine($"{hostile.Meta.DisplayName} attacks {Core.State.Character.Name} but missed.", Color.DarkGreen);
                }
            }

            var expiredStates = Core.State.ActorStates.States()
                .Where(o => Core.State.TimePassed >= o.ExpireTime).ToList();

            foreach (var state in expiredStates)
            {
                if (state.ActorUID != Core.State.Character.UID) //Other actor states expired.
                {
                    var actor = Core.Actors.Tiles.Where(o => o.Meta.UID == state.ActorUID).First();
                    Core.LogLine($"{state.State} state expired for {actor.Meta.DisplayName}.");
                    Core.State.ActorStates.RemoveState(state);
                }
                else if (state.ActorUID == Core.State.Character.UID) //Player states expired.
                {
                    if (state.State == StateOfBeing.Dexterity)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Dexterity augmentation expired, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.Constitution)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Constitution augmentation expired, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.ArmorClass)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"AC augmentation expired, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.Intelligence)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Intelligence augmentation expired, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.Strength)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Strength augmentation expired, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.Poisoned)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"With time, your poison has been cured.", Color.DarkGreen);
                    }
                    else if (state.State == StateOfBeing.EarthResistance)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Earth resistance has worn off, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.FireResistance)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Fire resistance has worn off, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.ColdResistance)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Code resistance has worn off, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.LightningResistance)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"Earth resistance has worn off, {(state.ModificationAmount > 0 ? "decreased" : "increased")} by {state.ModificationAmount}.");
                    }
                    else if (state.State == StateOfBeing.Held)
                    {
                        Core.State.ActorStates.RemoveState(state);
                        Core.LogLine($"You are no longer held!");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (Core.Player.Visible && Core.State.Character.Experience > Core.State.Character.NextLevelExperience)
            {
                Core.LevelUp();
            }

            Core.PurgeAllDeletedTiles();
        }

        private void ActOnActiveState(ActorState state)
        {
            if (state.State == StateOfBeing.Poisoned)
            {
                var actor = Core.Actors.Tiles.Where(o => o.Meta.UID == state.ActorUID).FirstOrDefault();
                if (actor == null)
                {
                    return;
                }

                if (actor == Core.Player)
                {
                    int damage = (int)((double)Core.State.Character.AvailableHitpoints * 0.05); //5% of the remaining hit points.
                    if (damage == 0) damage = 1;
                    Core.State.Character.AvailableHitpoints -= damage;
                    Core.LogLine($"{Core.State.Character.Name} took {damage} poison damage!", Color.DarkRed);

                    if (Core.State.Character.AvailableHitpoints <= 0)
                    {
                        Core.LogLine($"{PlayerDeathText()}", Color.Red);
                        Core.Player.QueueForDelete();
                    }
                }
                else
                {
                    int damage = (int)((actor.Meta.HitPoints ?? 0) * 0.05); //5% of the remaining hit points.
                    if (damage == 0) damage = 1;
                    Core.LogLine($"{actor.Meta.DisplayName} took {damage} poison damage!", Color.DarkGreen);

                    if (actor.ApplyDamage(damage))
                    {
                        int experience = 0;

                        if (actor.Meta != null && actor.Meta.Experience != null)
                        {
                            experience = (int)actor.Meta.Experience;
                        }

                        Core.LogLine($"{Core.State.Character.Name} dies of poisoning, gaining you {experience}xp!", Color.DarkGreen);

                        if (actor.Meta.IsContainer == true)
                        {
                            //If the enemy has loot, they drop it when they die.
                            EmptyContainerToGround(actor.Meta.UID, actor);
                        }

                        Core.State.Character.Experience += experience;
                    }
                }
            }
        }

        private void AnimateTo(string imagePath, ActorBase from, ActorBase to)
        {
            var item = Core.Actors.AddNew<ActorBase>(from.X, from.Y, imagePath);
            item.DrawOrder = 1000;

            item.Velocity.Angle.Degrees = item.AngleTo(to);
            item.Velocity.ThrottlePercentage = 100;
            item.Velocity.MaxSpeed = 3;
            item.RotationMode = RotationMode.Upsize;

            while (true)
            {
                item.X += (item.Velocity.Angle.X * (item.Velocity.MaxSpeed * item.Velocity.ThrottlePercentage));
                item.Y += (item.Velocity.Angle.Y * (item.Velocity.MaxSpeed * item.Velocity.ThrottlePercentage));

                item.Invalidate();
                Thread.Sleep(5);

                if (item.Intersects(to))
                {
                    break;
                }
            }

            item.QueueForDelete();
        }

        private void AnimateAt(string imagePath, ActorBase at)
        {
            var nameParts = imagePath.Split('_');

            if (nameParts.Length != 3)
            {
                throw new Exception("Animation image name should be in the form Name_Width_Height.png.");
            }

            int width;
            int height;

            try
            {
                width = int.Parse(nameParts[1]);
                height = int.Parse(nameParts[2]);
            }
            catch (Exception ex)
            {
                throw new Exception($"Animation image name should be in the form Name_Width_Height.png: {ex.Message}");
            }

            var item = Core.Actors.AddNewAnimation<ActorAnimation>(at.X, at.Y, imagePath, new Size(width, height));

            item.DrawOrder = 1000;

            int frameDelay = 1600 / item.FrameCount;
            if (frameDelay > 1000)
            {
                frameDelay = 1000;
            }
            if (frameDelay < 1)
            {
                frameDelay = 1;
            }

            while (item.ReadyForDeletion == false)
            {
                item.AdvanceImage();
                item.Invalidate();
                Thread.Sleep(frameDelay);
            }

            item.QueueForDelete();
        }

        private void AnimateAtAsyncThread(object param)
        {
            Core.State.AddThreadReference();
            Thread.CurrentThread.Name = $"AnimateAtAsyncThread_{Core.State.ActiveThreadCount}";
            var p = (AnimateAtAsyncParam)param;
            AnimateAt(p.ImagePath, p.At);
            Core.State.RemoveThreadReference();
        }

        /// <summary>
        /// This method is stupid, it spawns a thread to perform the animation but blocks in a loop to update the UI.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="at"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        /// <returns></returns>
        private Thread AnimateAtAsync(string imagePath, ActorBase at, GameThreadCallback callback = null, object callbackParam = null)
        {
            var param = new AnimateAtAsyncParam()
            {
                ImagePath = imagePath,
                At = at
            };

            IsEngineBusy = true;

            ParameterizedThreadStart starter = AnimateAtAsyncThread;
            starter += (param) =>
            {
                if (callback != null)
                {
                    callback(callbackParam);
                }
            };
            var thread = new Thread(starter) { IsBackground = true };
            thread.Start(param);

            WaitOnThreadWithShamefulDoEvents(thread);

            IsEngineBusy = false;

            return thread;
        }

        private void EmptyContainerToGround(Guid? containerId, ActorBase at)
        {
            if (containerId == null)
            {
                return;
            }

            var items = Core.State.Items.Where(o => o.ContainerId == containerId).ToList();

            foreach (var continerItem in items)
            {
                CustodyItem item = continerItem;

                //I don't think this is needed now that ActorSpawner is handled on new game creation.
                if (item.Tile.Meta.ActorClass == ActorClassName.ActorSpawner)
                {
                    var spawnedItem = Core.GetWeightedLotteryTile(item.Tile.Meta);
                    if (spawnedItem == null)
                    {
                        continue;
                    }

                    item = new CustodyItem()
                    {
                        Tile = spawnedItem.DeriveCopy()
                    };
                }

                bool wasStacked = false;

                if (item.Tile.Meta.CanStack == true)
                {
                    var itemUnderfoot = Core.Actors.Intersections(at)
                        .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem && o.TilePath == item.Tile.TilePath)
                        .Cast<ActorItem>().FirstOrDefault();

                    if (itemUnderfoot != null)
                    {
                        itemUnderfoot.Meta.Quantity = (itemUnderfoot.Meta.Quantity ?? 0) + item.Tile.Meta.Quantity;
                        wasStacked = true;
                    }
                }

                if (wasStacked == false)
                {
                    var droppedItem = Core.Actors.AddDynamic(item.Tile, at.X, at.Y);
                    droppedItem.Meta.HasBeenViewed = true;
                    droppedItem.AlwaysRender = true;
                }

                Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == item.Tile.Meta.UID);
            }
        }

        /// <summary>
        /// Moves an actor in the direction of their vector and returns a list of any
        /// encountered collisions as well as passes back distance the actor was moved.
        /// </summary>
        private List<ActorBase> MoveActor(ActorBase actor, out Point<double> finalAppliedOffset)
        {
            var appliedOffset = new Point<double>(0, 0);

            if (Core.State.ActorStates.HasState((Guid)actor.Meta.UID, StateOfBeing.Held) == false)
            {
                appliedOffset = new Point<double>(
                    (int)(actor.Velocity.Angle.X * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage),
                    (int)(actor.Velocity.Angle.Y * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));

                actor.X += appliedOffset.X;
                actor.Y += appliedOffset.Y;
            }

            var intersections = Core.Actors.Intersections(actor)
                .Where(o => o.Meta.ActorClass != ActorClassName.ActorTerrain)
                .Where(o => o.Meta.CanWalkOn == false).ToList();

            //Only get the top terrain block, we dont want to dig to the ocean.
            var topTerrainBlock = Core.Actors.Intersections(actor)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorTerrain
                    || o.Meta.ActorClass == ActorClassName.ActorBlockaid
                    || o.Meta.ActorClass == ActorClassName.ActorBlockadeHidden)
                .OrderBy(o => o.DrawOrder ?? 0).LastOrDefault();

            if (topTerrainBlock == null)
            {
                actor.X -= appliedOffset.X;
                actor.Y -= appliedOffset.Y;

                finalAppliedOffset = new Point<double>(0, 0);

                return intersections;
            }

            //Only act on the top terrain block if it turns out to be one we can't walk on.
            if (topTerrainBlock.Meta.CanWalkOn == false)
            {
                actor.X -= appliedOffset.X;
                actor.Y -= appliedOffset.Y;

                intersections.Add(topTerrainBlock);

                finalAppliedOffset = new Point<double>(0, 0);

                return intersections;
            }

            //Do basic collision detection and back off the player movement
            //from any that might have caused an overlap with tiles that can not be walked on.
            //This includes both terrain and beings.
            foreach (var intersection in intersections)
            {
                //We have to keep checking for collisions as we back the actor off
                //  because the actor is moving. Some intersections may no longer be valid.
                if (actor.Bounds.IntersectsWith(intersection.Bounds) == false)
                {
                    continue;
                }

                //Figure out how much overlap we have.
                var delta = actor.ScreenBounds.GetIntersection(intersection.ScreenBounds);

                //Back the player off of the overalpping collision.
                actor.X -= appliedOffset.X;
                actor.Y -= appliedOffset.Y;

                //Butt the rectangles up against each other and adjust the applied offset to what was actually done.
                if (delta.X > 0 && delta.Y > 0)
                {
                    //Core.debugRects.Add(new Rectangle((int)delta.X, (int)delta.Y, (int)delta.Width, (int)delta.Height));

                    if (appliedOffset.X > 0)
                    {
                        if (Math.Abs(delta.Width) > Math.Abs(appliedOffset.X))
                            appliedOffset.X = 0;
                        else
                            appliedOffset.X -= delta.Width;
                    }

                    if (appliedOffset.X < 0)
                    {
                        if (Math.Abs(delta.Width) > Math.Abs(appliedOffset.X))
                            appliedOffset.X = 0;
                        else
                            appliedOffset.X += delta.Width;
                    }

                    if (appliedOffset.Y > 0)
                    {
                        if (Math.Abs(delta.Height) > Math.Abs(appliedOffset.Y))
                            appliedOffset.Y = 0;
                        else
                            appliedOffset.Y -= delta.Height;
                    }

                    if (appliedOffset.Y < 0)
                    {
                        if (Math.Abs(delta.Height) > Math.Abs(appliedOffset.Y))
                            appliedOffset.Y = 0;
                        else
                            appliedOffset.Y += delta.Height;
                    }
                }

                actor.X += appliedOffset.X;
                actor.Y += appliedOffset.Y;
            }

            finalAppliedOffset = new Point<double>(appliedOffset);

            var finalIntersections = Core.Actors.Intersections(actor)
                .Where(o => o.Meta.ActorClass != ActorClassName.ActorTerrain).ToList();

            intersections.AddRange(finalIntersections);

            return intersections;
        }

        /// <summary>
        /// Keep the player from hitting the edge of the map.
        /// </summary>
        /// <param name="appliedOffset"></param>
        private void ScrollBackground(Point<double> appliedOffset)
        {
            int boxSize = 250;

            if (appliedOffset.X > 0 && (Core.Player.X - Core.Display.BackgroundOffset.X) > Core.Display.VisibleSize.Width - boxSize)
            {
                Core.Display.BackgroundOffset.X += Math.Abs(appliedOffset.X);
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.Y > 0 && (Core.Player.Y - Core.Display.BackgroundOffset.Y) > Core.Display.VisibleSize.Height - boxSize)
            {
                Core.Display.BackgroundOffset.Y += Math.Abs(appliedOffset.Y);
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.X < 0 && (Core.Player.X - Core.Display.BackgroundOffset.X) < boxSize)
            {
                Core.Display.BackgroundOffset.X -= Math.Abs(appliedOffset.X);
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.Y < 0 && (Core.Player.Y - Core.Display.BackgroundOffset.Y) < boxSize)
            {
                Core.Display.BackgroundOffset.Y -= Math.Abs(appliedOffset.Y);
                Core.Display.DrawingSurface.Invalidate();
            }
        }

        #region Misc.

        private string PlayerDeathText()
        {
            var strs = new string[] {
                "DEATH! You have been bested!",
                "DEATH! The beast that dealt your final blow laughs as they rummage through their new spoils.",
                "DEATH! Like a candle in the wind, your flame fades quickly. Your body lies still as it cools.",
                "DEATH! With a final blow, your ambitions are flung from your body.",
                "DEATH! The wretched beast dashes all signs of life from your body.",
                "DEATH! Too much damage... Your body caves in as your mind loses hope for the last time.",
                "DEATH! You gave it your all, it wasn't enough.",
                "DEATH! Your final breath was as much as a whimper.",
                "DEATH! Gone too soon, forgotten even sooner.",
            };

            return strs[MathUtility.RandomNumber(0, strs.Count())];
        }

        private string GetCriticalHitText()
        {
            var strs = new string[] {
                "tearing them open!",
                "landing a crushing blow!.",
                "hitting them in the head!.",
                "hitting them in the torso!",
                "riping them open at the joint!",
                "nearly removing a limb!",
                "leaving them in a state of shock!",
                "nearly blinding them!",
                "causing them to stumble!",
                "knocking them to the ground!"
            };

            return strs[MathUtility.RandomNumber(0, strs.Count())];
        }

        private string GetCriticalMissText()
        {
            var strs = new string[] {
                "but they evade your clumsy blow!",
                "but they were faster then you expected!",
                "but they pull just out of your path!",
                "but you are too slow to get an upper hand!",
                "but miss, nearly dropping your weapon!",
                "but you were bested by their agility!"
            };

            return strs[MathUtility.RandomNumber(0, strs.Count())];
        }

        #endregion

        private Thread WaitOnThreadWithShamefulDoEvents(Thread thread)
        {
            while (thread.ThreadState != ThreadState.Stopped)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
            return thread;
        }
    }
}
