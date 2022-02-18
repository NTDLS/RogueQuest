using Assets;
using Game.Actors;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Game.Engine.Types;

namespace Game.Engine
{
    public class EngineTickController
    {
        private int _avatarAnimationFrame = 1;

        public EngineCore Core { get; private set; }
        public EngineTickController(EngineCore core)
        {
            Core = core;
        }

        private void PickupPack(ActorItem packOnGround)
        {
            var pack = Core.State.Character.GetEquipSlot(EquipSlot.Pack); //The default container to add items to.
            Guid putItemsIntoContainerId = (Guid)pack.Tile.Meta.UID;
            var groundContainer = Core.Actors.Tiles.Where(o => o.Meta.UID == (Guid)packOnGround.Meta.UID).First();

            int maxBulk = (int)pack.Tile.Meta.BulkCapacity;
            int maxWeight = (int)pack.Tile.Meta.WeightCapacity;

            var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Weight);
            var sourceContainerWeight = Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID).Sum(o => o.Tile.Meta.Weight);
            if (currentPackWeight + sourceContainerWeight > maxWeight)
            {
                Core.LogLine($"This pack contains items too bulky for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
                return;
            }

            var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Bulk);
            var sourceContainerBulk = Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID).Sum(o => o.Tile.Meta.Bulk);
            if (currentPackBulk + sourceContainerBulk > maxWeight)
            {
                Core.LogLine($"This pack contains items too heavy for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
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

                Core.State.Items.Add(nestedPack);
            }

            List<Guid> itemsToDelete = new List<Guid>();

            //Add all the items in the pack on the ground to the pack in the players inventory pack (nested). :(
            foreach (var item in Core.State.Items.Where(o => o.ContainerId == groundContainer.Meta.UID))
            {
                Core.LogLine($"Picked up" + ((item.Tile.Meta.CanStack == true) ? $" {item.Tile.Meta.Quantity:N0}" : "") + $" {item.Tile.Meta.Name}");

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
            int maxBulk = (int)pack.Tile.Meta.BulkCapacity;
            int maxWeight = (int)pack.Tile.Meta.WeightCapacity;
            var containerContents = Core.State.Items.Where(o => o.ContainerId == containerId);

            var itemsToDelete = new List<Guid>();

            foreach (var item in containerContents)
            {
                //Do weight/bulk math.
                var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Weight);
                if (item.Tile.Meta.Weight + currentPackWeight > maxWeight)
                {
                    Core.LogLine($"{item.Tile.Meta.Name} is too bulky for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
                    break;
                }

                var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == pack.Tile.Meta.UID).Sum(o => o.Tile.Meta.Bulk);
                if (item.Tile.Meta.Bulk + currentPackBulk > maxBulk)
                {
                    Core.LogLine($"{item.Tile.Meta.Name} is too heavy for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
                    break;
                }

                Core.LogLine($"Picked up" + ((item.Tile.Meta.CanStack == true) ? $" {item.Tile.Meta.Quantity:N0}" : "") + $" {item.Tile.Meta.Name}");

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

        public bool UseConsumableItem(Guid itemUid, ActorBase target/*, Guid? rangedProjectile = null*/)
        {
            var item = Core.State.Items.Where(o => o.Tile.Meta.UID == itemUid).First();
            if (item == null || item.Tile.Meta.UID == null)
            {
                return false;
            }

            //var projectile = Core.State.Items.Where(o => o.Tile.Meta.UID == rangedProjectile).FirstOrDefault();

            if (item.Tile.Meta.SubType == ActorSubType.Wand)
            {
                var input = new Types.TickInput()
                {
                    InputType = Types.TickInputType.Ranged,
                    RangedItem = item,
                    RangedTarget = target
                };

                Advance(input);
                WaitOnIdleEngine();
            }
            else if (item.Tile.Meta.SubType == ActorSubType.RangedWeapon)
            {
                var input = new Types.TickInput()
                {
                    InputType = Types.TickInputType.Ranged,
                    RangedItem = item,
                    //RangedProjectile = projectile,
                    RangedTarget = target
                };

                Advance(input);
                WaitOnIdleEngine();
            }
            else if (item.Tile.Meta.SubType == ActorSubType.Potion)
            {
                #region Heal.
                if (item.Tile.Meta.Effect == ItemEffect.Heal)
                {
                    int totalHealing = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        if (formula[0] == '%') //Raise the hitpoints by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            int hpToAdd = (int)((double)Core.State.Character.Hitpoints * pct);
                            totalHealing += hpToAdd;
                            Core.State.Character.AvailableHitpoints += hpToAdd;
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            var hpToAdd = int.Parse(formula);
                            totalHealing += hpToAdd;
                            Core.State.Character.AvailableHitpoints += hpToAdd;
                        }
                    }

                    if (Core.State.Character.AvailableHitpoints > Core.State.Character.Hitpoints)
                    {
                        Core.State.Character.AvailableHitpoints = Core.State.Character.Hitpoints;
                    }

                    Core.LogLine($"Healed {totalHealing} hitpoints.");
                }
                #endregion
                #region IncreaseDexterity
                else if (item.Tile.Meta.Effect == ItemEffect.IncreaseDexterity)
                {
                    int totalAdded = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        int toAdd = 0;

                        if (formula[0] == '%') //Raise the attribute by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            toAdd = (int)((double)Core.State.Character.Dexterity * pct);
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = int.Parse(formula);
                        }

                        Core.State.Character.AugmentedDexterity += toAdd;
                        totalAdded += toAdd;
                    }

                    var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.IncreaseDexterity);
                    state.ModificationAmount = totalAdded;
                    state.ExpireTime = Core.State.TimePassed + item.Tile.Meta.ExpireTime;

                    Core.LogLine($"Dexterity increased by {totalAdded} for {item.Tile.Meta.ExpireTime} minutes.");
                }
                #endregion
                #region IncreaseConstitution
                else if (item.Tile.Meta.Effect == ItemEffect.IncreaseConstitution)
                {
                    int totalAdded = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        int toAdd = 0;

                        if (formula[0] == '%') //Raise the attribute by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            toAdd = (int)((double)Core.State.Character.Hitpoints * pct);
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = int.Parse(formula);
                        }

                        Core.State.Character.AugmentedConstitution += toAdd;
                        totalAdded += toAdd;
                    }

                    var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.IncreaseConstitution);
                    state.ModificationAmount = totalAdded;
                    state.ExpireTime = Core.State.TimePassed + item.Tile.Meta.ExpireTime;

                    Core.LogLine($"Constitution increased by {totalAdded} for {item.Tile.Meta.ExpireTime} minutes.");
                }
                #endregion
                #region IncreaseArmorClass
                else if (item.Tile.Meta.Effect == ItemEffect.IncreaseArmorClass)
                {
                    int totalAdded = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        int toAdd = 0;

                        if (formula[0] == '%') //Raise the attribute by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            toAdd = (int)((double)Core.State.Character.AugmentedAC * pct);
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = int.Parse(formula);
                        }

                        Core.State.Character.AugmentedAC += toAdd;
                        totalAdded += toAdd;
                    }

                    var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.IncreaseArmorClass);
                    state.ModificationAmount = totalAdded;
                    state.ExpireTime = Core.State.TimePassed + item.Tile.Meta.ExpireTime;

                    Core.LogLine($"AC increased by {totalAdded} for {item.Tile.Meta.ExpireTime} minutes.");
                }
                #endregion
                #region IncreaseIntelligence
                else if (item.Tile.Meta.Effect == ItemEffect.IncreaseIntelligence)
                {
                    int totalAdded = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        int toAdd = 0;

                        if (formula[0] == '%') //Raise the attribute by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            toAdd = (int)((double)Core.State.Character.Intelligence * pct);
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = int.Parse(formula);
                        }

                        Core.State.Character.AugmentedIntelligence += toAdd;
                        totalAdded += toAdd;
                    }

                    var stateInt = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.IncreaseIntelligence);
                    stateInt.ModificationAmount = totalAdded;
                    stateInt.ExpireTime = item.Tile.Meta.ExpireTime;

                    Core.LogLine($"Intelligence increased by {totalAdded} for {item.Tile.Meta.ExpireTime} minutes.");
                }
                #endregion
                #region IncreaseStrength
                else if (item.Tile.Meta.Effect == ItemEffect.IncreaseStrength)
                {
                    int totalAdded = 0;

                    var formulas = item.Tile.Meta.EffectFormula.Split(',');
                    foreach (var formula in formulas)
                    {
                        int toAdd = 0;

                        if (formula[0] == '%') //Raise the attribute by a percentage.
                        {
                            var pct = int.Parse(formula.Substring(1)) / 100.0;
                            toAdd = (int)((double)Core.State.Character.Strength * pct);
                        }
                        else //Raise the hitpoints by a fixed ammount.
                        {
                            toAdd = int.Parse(formula);
                        }

                        Core.State.Character.AugmentedStrength += toAdd;
                        totalAdded += toAdd;
                    }

                    var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.IncreaseStrength);
                    state.ModificationAmount = totalAdded;
                    state.ExpireTime = Core.State.TimePassed + item.Tile.Meta.ExpireTime;

                    Core.LogLine($"Strength increased by {totalAdded} for {item.Tile.Meta.ExpireTime} minutes.");
                }
                #endregion
                #region Poison
                else if (item.Tile.Meta.Effect == ItemEffect.Poison)
                {
                    int damage = (int)((double)Core.State.Character.Hitpoints * 0.1);
                    if (damage == 0) damage = 1;
                    Core.State.Character.AvailableHitpoints -= damage;

                    var state = Core.State.ActorStates.AddState(Core.State.Character.UID, StateOfBeing.Poisoned);
                    state.ExpireTime = Core.State.TimePassed + item.Tile.Meta.ExpireTime;
                    Core.LogLine($"You have been poisoned! {damage} damage!", Color.DarkRed);
                }
                #endregion
                #region CurePoison
                else if (item.Tile.Meta.Effect == ItemEffect.CurePoison)
                {
                    if (Core.State.ActorStates.HasState(Core.State.Character.UID, StateOfBeing.Poisoned))
                    {
                        Core.LogLine($"Your body has been purged of all poisons!", Color.DarkGreen);
                        Core.State.ActorStates.RemoveState(Core.State.Character.UID, StateOfBeing.Poisoned);
                    }
                    else
                    {
                        Core.LogLine($"Your needlesly drink a lifesaving potion...");
                    }
                }
                #endregion
                else
                {
                    throw new NotImplementedException();
                }
            }

            if (item.Tile.Meta.SubType == ActorSubType.Wand)
            {
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
                else
                {
                    Core.State.Items.Remove(item);
                    var slotToVacate = Core.State.Character.FindEquipSlotByItemId(item.Tile.Meta.UID);
                    if (slotToVacate != null)
                    {
                        slotToVacate.Tile = null;
                    }
                }
            }

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
                    Core.LogLine($"Picked up {moneyItem.Meta.Quantity} {moneyItem.Meta.Name} and placed them in your purse.");
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

                    Core.LogLine($"Picked up {underfootPack.Meta.Name} and placed it on your back.");
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
            int maxBulk = (int)pack.Tile.Meta.BulkCapacity;
            int maxWeight = (int)pack.Tile.Meta.WeightCapacity;
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
                    var currentPackWeight = Core.State.Items.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Weight);
                    if (item.Meta.Weight + currentPackWeight > maxWeight)
                    {
                        Core.LogLine($"{item.Meta.Name} is too bulky for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
                        break;
                    }

                    var currentPackBulk = Core.State.Items.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Bulk);
                    if (item.Meta.Bulk + currentPackBulk > maxBulk)
                    {
                        Core.LogLine($"{item.Meta.Name} is too heavy for your {pack.Tile.Meta.Name}. Drop something or move to free hand?");
                        break;
                    }

                    if (maxItems != null)
                    {
                        var currentPackItems = Core.State.Items.Where(o => o.ContainerId == containerId).Count();
                        if (currentPackItems + 1 > (int)maxItems)
                        {
                            Core.LogLine($"{pack.Tile.Meta.Name} can only carry {maxItems} items. Drop something or move to free hand?");
                            break;
                        }
                    }

                    Core.LogLine($"Picked up" + ((item.Meta.CanStack == true) ? $" {item.Meta.Quantity:N0}" : "") + $" {item.Meta.Name}");

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

            if (Core.State.Character.AvailableHitpoints >= Core.State.Character.Hitpoints)
            {
                Core.LogLine($"Feeling no need to rest you press on.", Color.DarkGreen);
                Advance(input);
                WaitOnIdleEngine();
                return;
            }

            while (Core.State.Character.AvailableHitpoints < Core.State.Character.Hitpoints)
            {
                Advance(input);
                WaitOnIdleEngine();

                var actorsThatCanSeePlayer = Core.Actors.Intersections(Core.Player, 150)
                    .Where(o => o.Meta.CanTakeDamage == true && o.Meta.ActorClass == ActorClassName.ActorHostileBeing);

                if (actorsThatCanSeePlayer.Any())
                {
                    var firstActor = actorsThatCanSeePlayer.First();
                    Core.LogLine($"Your rest was interrupted by a {firstActor.Meta.Name}!", Color.DarkRed);
                    return;
                }

                Core.State.Character.AvailableHitpoints++;
            }

            Core.LogLine($"You awake feeling refreshed.", Color.DarkGreen);
        }

        public Point<double> Advance(TickInput Input)
        {
            Point<double> appliedOffset = new Point<double>();

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

                string assetPath = Assets.Constants.GetAssetPath(@$"Tiles\Special\@Player\{Core.State.Character.Avatar}\{playerAngle} {_avatarAnimationFrame}.png");
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

            if (intersections.Count == 1
                && intersections.First().Meta.ActorClass == ActorClassName.ActorStore
                && intersections.First().Meta.SubType != ActorSubType.RuinsStore)
            {
                using (var form = new FormStore(Core, intersections.First().Meta))
                {
                    form.ShowDialog();
                }
            }

            System.Threading.Thread thread = new System.Threading.Thread(GameLogicThread);
            thread.Start(new GameLogicParam()
            {
                Intersections = intersections,
                Input = Input
            });

            return appliedOffset;
        }

        class GameLogicParam
        {
            public TickInput Input { get; set; }
            public List<ActorBase> Intersections { get; set; }
        }

        public void WaitOnIdleEngine()
        {
            while (Core.State.IsThreadActive == true)
            {
                System.Threading.Thread.Sleep(1);
            }
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
            //Dant want the player to accidently skip the dialog.
            if ((DateTime.Now - Dialogs.DialogOpenTime).TotalSeconds > 1)
            {
                Core.Actors.Tiles.RemoveAll(o => o.Meta?.ActorClass == Library.Engine.Types.ActorClassName.ActorDialog);
                Core.PurgeAllDeletedTiles();
                Core.Display.DrawingSurface.Invalidate();
                Core.State.IsDialogActive = false;
            }
        }

        /// <summary>
        /// Intersections contains all objects that the player collided with during their turn. If this
        /// contains actors that can be damaged, then these  would be the melee attack target for the player.
        /// </summary>
        /// <param name="intersections"></param>
        void GameLogicThread(object param)
        {
            Core.State.IsThreadActive = true;
            GameLogicThreadEx((GameLogicParam)param);
            Core.State.IsThreadActive = false;
        }

        void GameLogicThreadEx(GameLogicParam p)
        {
            List<ActorBase> intersections = p.Intersections;
            TickInput Input = p.Input;

            var somethingToSay = intersections.Where(o => string.IsNullOrEmpty(o.Meta.Dialog) == false).FirstOrDefault();
            if (somethingToSay != null)
            {
                Dialogs.DrawDialog(Core, somethingToSay.Meta.Dialog);

                if (somethingToSay.Meta.OnlyDialogOnce == true)
                {
                    somethingToSay.Meta.Dialog = "";
                }
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
                return;
            }

            var actorsThatCanSeePlayer = Core.Actors.Intersections(Core.Player, 200)
                .Where(o => o.Meta.CanTakeDamage == true);

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

                if (distance < actor.MaxFollowDistance)
                {
                    Point<double> appliedOtherOffset = new Point<double>();
                    obj.Velocity.Angle.Degrees = actor.AngleTo(Core.Player);
                    MoveActor(actor, out appliedOtherOffset);
                }
            }

            var weapon = Core.State.Character.GetEquipSlot(EquipSlot.Weapon)?.Tile?.Meta;
            TileIdentifier projectile = null;
            ActorBase actorToAttack = null;

            if (Input.RangedTarget != null && Input.RangedItem != null) //Ranged attack.
            {
                weapon = Input.RangedItem.Tile.Meta;
                actorToAttack = Input.RangedTarget;
            }
            else //Melee attack.
            {
                //If this is a ranged weapon but we are in a melee situation, then check the free hand for a melee weapon and use it instead.
                if (weapon != null && weapon.ProjectileType != ProjectileType.Unspecified)
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
                if (weapon != null && weapon.ProjectileType != ProjectileType.Unspecified) //Melee attack with ranged weapon.
                {
                    projectile = Core.State.Character.GetInventoryItemFromQuiverSlotOfType(weapon.ProjectileType)?.Tile;
                    if (projectile == null)
                    {
                        Core.LogLine($"You are out of projectiles for the eqipeed ranged weapon!!!", Color.DarkRed);
                    }
                }
            }

            if (projectile != null)
            {
                AnimateTo(projectile.TilePath, Core.Player, actorToAttack);
            }
            else if (weapon != null && string.IsNullOrEmpty(weapon.ProjectileTilePath) == false)
            {
                AnimateTo(weapon.ProjectileTilePath, Core.Player, actorToAttack);
            }

            string weaponDescription = weapon?.Name;

            if (projectile != null)
            {
                if (projectile.Meta.IsConsumable == true)
                {
                    projectile.Meta.Quantity--;

                    weaponDescription += $" ({projectile.Meta.Name})";

                    if ((projectile.Meta.Quantity ?? 0) == 0)
                    {
                        Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == projectile.Meta.UID);

                        var slotToVacate = Core.State.Character.FindEquipSlotByItemId(projectile.Meta.UID);
                        if (slotToVacate != null)
                        {
                            slotToVacate.Tile = null;
                        }
                    }

                    Core.State.Character.PushFreshInventoryItemsToEquipSlots();
                }
            }

            if (actorToAttack != null && weapon != null)
            {
                //Get the additional damage added by all equipped items (basically looking for enchanted items that add damage, like rings, cloaks, etc).
                //This also gets the additional damage for the equipped weapon. For example, for a +3 Enchanted Long Sword, this is the 3.

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

                additionalDamage += (weapon.DamageAdditional ?? 0);

                var hitType = CalculateHitType(Core.State.Character.Dexterity, (int)actorToAttack.Meta.AC);
                if (hitType == HitType.Hit || hitType == HitType.CriticalHit)
                {
                    int playerHitsFor = 0;

                    if (weapon?.DamageDice > 0)
                    {
                        playerHitsFor = CalculateDealtDamage(hitType, Core.State.Character.Strength,
                            additionalDamage, weapon?.DamageDice ?? 0, weapon?.DamageDiceFaces ?? 0);
                    }

                    if (projectile != null && projectile.Meta?.DamageDice > 0)
                    {
                        playerHitsFor += CalculateDealtDamage(hitType, Core.State.Character.Strength,
                            additionalDamage, projectile.Meta?.DamageDice ?? 0, projectile.Meta?.DamageDiceFaces ?? 0);
                    }

                    if (hitType == HitType.CriticalHit)
                    {
                        Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} with {weaponDescription} for {playerHitsFor}hp {GetCriticalHitText()}", Color.DarkOliveGreen);
                    }
                    else
                    {
                        Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} with {weaponDescription} for {playerHitsFor}hp and hits.", Color.DarkGreen);
                    }

                    if (actorToAttack.ApplyDamage(playerHitsFor))
                    {
                        int experience = 0;

                        if (actorToAttack.Meta != null && actorToAttack.Meta.Experience != null)
                        {
                            experience = (int)actorToAttack.Meta.Experience;
                        }

                        Core.LogLine($"{Core.State.Character.Name} kills {actorToAttack.Meta.Name} gaining {experience}xp!", Color.DarkGreen);

                        if (actorToAttack.Meta.IsContainer == true)
                        {
                            //If the enemy has loot, they drop it when they die.
                            EmptyContainerToGround(actorToAttack.Meta.UID, actorToAttack);
                        }

                        Core.State.Character.Experience += experience;
                    }

                    //Hit animation:
                    if (projectile != null)
                    {
                        if (string.IsNullOrEmpty(projectile.Meta.HitAnimationTilePath) == false)
                        {
                            AnimateAt(projectile.Meta.HitAnimationTilePath, actorToAttack);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(weapon.HitAnimationTilePath) == false)
                        {
                            AnimateAt(weapon.HitAnimationTilePath, actorToAttack);
                        }
                    }
                }
                else if (hitType == HitType.CriticalMiss)
                {
                    Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} {GetCriticalMissText()}", Color.DarkRed);
                }
                else
                {
                    Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} but misses.", Color.DarkRed);
                }
            }

            if (Core.State.Character.Experience > Core.State.Character.NextLevelExperience)
            {
                Core.State.Character.LevelUp();
                Core.LogLine($"{Core.State.Character.Name} reached level {Core.State.Character.Level}! Next level at {Core.State.Character.NextLevelExperience:N0}xp.", Color.Green);
            }

            var totalPlayerAC = Core.State.Character.Equipment.Where(o => o.Tile != null).Sum(o => o.Tile?.Meta?.AC ?? 0);

            //Hostiles attack player. Be sure to look at visible actors only because the player may have killed one before we get here.
            foreach (var hostile in hostileInteractions.Where(o => o.Visible))
            {
                //Monster hit player.
                var hitType = CalculateHitType((int)hostile.Meta.Dexterity, totalPlayerAC);
                if (hitType == HitType.Hit)
                {
                    int hostileHitsFor = CalculateDealtDamage(hitType, (int)hostile.Meta.Strength,
                        0, hostile.Meta.DamageDice ?? 0, hostile.Meta?.DamageDiceFaces ?? 0);

                    if (hitType == HitType.CriticalHit)
                    {
                        Core.LogLine($"{hostile.Meta.Name} attacks {Core.State.Character.Name} for {hostileHitsFor}hp landing a critical hit!", Color.DarkRed);
                    }
                    else
                    {
                        Core.LogLine($"{hostile.Meta.Name} attacks {Core.State.Character.Name} for {hostileHitsFor}hp and hits.", Color.DarkRed);
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
                    Core.LogLine($"{hostile.Meta.Name} attacks {Core.State.Character.Name} resulting in a critical miss!", Color.DarkGreen);
                }
                else
                {
                    Core.LogLine($"{hostile.Meta.Name} attacks {Core.State.Character.Name} but missed.", Color.DarkGreen);
                }
            }

            var states = Core.State.ActorStates.States(Core.State.Character.UID);
            var expiredStates = states.Where(o => Core.State.TimePassed >= o.ExpireTime).ToList();

            foreach (var state in expiredStates)
            {
                #region IncreaseDexterity
                if (state.State == StateOfBeing.IncreaseDexterity)
                {
                    Core.State.ActorStates.RemoveState(state);
                    Core.State.Character.AugmentedDexterity -= (int)state.ModificationAmount;
                    Core.LogLine($"Dexterity augmentation expired, decreased by {state.ModificationAmount}.");
                }
                #endregion
                #region IncreaseConstitution
                else if (state.State == StateOfBeing.IncreaseConstitution)
                {
                    Core.State.ActorStates.RemoveState(state);
                    Core.State.Character.AugmentedConstitution -= (int)state.ModificationAmount;
                    Core.LogLine($"Constitution augmentation expired, decreased by {state.ModificationAmount}.");
                }
                #endregion
                #region IncreaseArmorClass
                else if (state.State == StateOfBeing.IncreaseArmorClass)
                {
                    Core.State.ActorStates.RemoveState(state);
                    Core.State.Character.AugmentedAC -= (int)state.ModificationAmount;
                    Core.LogLine($"AC augmentation expired, decreased by {state.ModificationAmount}.");
                }
                #endregion
                #region IncreaseIntelligence
                else if (state.State == StateOfBeing.IncreaseIntelligence)
                {
                    Core.State.ActorStates.RemoveState(state);
                    Core.State.Character.AugmentedIntelligence -= (int)state.ModificationAmount;
                    Core.LogLine($"Intelligence augmentation expired, decreased by {state.ModificationAmount}.");
                }
                #endregion
                #region IncreaseStrength
                else if (state.State == StateOfBeing.IncreaseStrength)
                {
                    Core.State.ActorStates.RemoveState(state);
                    Core.State.Character.AugmentedStrength -= (int)state.ModificationAmount;
                    Core.LogLine($"Strength augmentation expired, decreased by {state.ModificationAmount}.");
                }
                #endregion
                #region Poison
                else if (state.State == StateOfBeing.Poisoned)
                {
                    int damage = (int)((double)Core.State.Character.Hitpoints * 0.1);
                    if (damage == 0) damage = 1;
                    Core.State.Character.AvailableHitpoints -= damage;

                    Core.State.ActorStates.RemoveState(state);
                    Core.LogLine($"With time, your poison has been cured.", Color.DarkGreen);
                }
                #endregion
                else
                {
                    throw new NotImplementedException();
                }
            }

            Core.PurgeAllDeletedTiles();
        }

        private void AnimateTo(string imagePath, ActorBase from, ActorBase to)
        {
            var item = Core.Actors.AddNew<ActorPlayer>(from.X, from.Y, imagePath);
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
                System.Threading.Thread.Sleep(5);

                if (item.Intersects(to))
                {
                    break;
                }
            }

            item.QueueForDelete();
        }

        private void AnimateAt(string imagePath, ActorBase at)
        {
            var item = Core.Actors.AddNewAnimation<ActorAnimation>(at.X, at.Y, imagePath, new Size(66, 66));

            while (item.ReadyForDeletion == false)
            {
                item.AdvanceImage();
                item.Invalidate();
                System.Threading.Thread.Sleep(25);
            }

            item.QueueForDelete();
        }

        private void EmptyContainerToGround(Guid? containerId, ActorBase at)
        {
            if (containerId == null)
            {
                return;
            }

            var items = Core.State.Items.Where(o => o.ContainerId == containerId).ToList();

            foreach (var item in items)
            {
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
                    var droppedItem = Core.Actors.AddDynamic(item.Tile.Meta.ActorClass.ToString(),
                        at.X, at.Y, item.Tile.TilePath);

                    droppedItem.Meta = item.Tile.Meta;

                    droppedItem.Meta.HasBeenViewed = true;
                    droppedItem.AlwaysRender = true;
                    droppedItem.Invalidate();
                }

                Core.State.Items.RemoveAll(o => o.Tile.Meta.UID == item.Tile.Meta.UID);
            }
        }

        /// <summary>
        /// Moves an actor in the direction of their vector and returns a list of any
        /// encountered collisions as well as passes back distance the actor was moved.
        /// </summary>
        public List<ActorBase> MoveActor(ActorBase actor, out Point<double> finalAppliedOffset)
        {
            if (actor.Meta.UID != null && Core.State.ActorStates.HasState((Guid)actor.Meta.UID, StateOfBeing.Poisoned))
            {
                if (actor == Core.Player)
                {
                    int damage = (int)((double)Core.State.Character.Hitpoints * 0.1);
                    if (damage == 0) damage = 1;
                    Core.State.Character.AvailableHitpoints -= damage;
                    Core.LogLine($"{Core.State.Character.Name} took {damage} poison damage!", Color.DarkRed);

                    if (Core.State.Character.AvailableHitpoints <= 0)
                    {
                        Core.LogLine($"{PlayerDeathText()}", Color.Red);
                        Core.Player.QueueForDelete();

                        finalAppliedOffset = new Point<double>(0, 0);
                        return new List<ActorBase>();
                    }
                }
                else
                {
                    int damage = (int)((actor.Meta.OriginalHitPoints ?? 0) * 0.1);
                    if (damage == 0) damage = 1;
                    Core.LogLine($"{actor.Meta.Name} took {damage} poison damage!", Color.DarkGreen);

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

            Point<double> appliedOffset = new Point<double>(
                (int)(actor.Velocity.Angle.X * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage),
                (int)(actor.Velocity.Angle.Y * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));

            actor.X += appliedOffset.X;
            actor.Y += appliedOffset.Y;

            var intersections = Core.Actors.Intersections(actor)
                .Where(o => o.Meta.ActorClass != ActorClassName.ActorTerrain)
                  .Where(o => o.Meta.CanWalkOn == false).ToList();

            //Only get the top terrain block, we dont want to dig to the ocean.
            var topTerrainBlock = Core.Actors.Intersections(actor)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorTerrain)
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

        string PlayerDeathText()
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

        string GetCriticalHitText()
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

        string GetCriticalMissText()
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

    }
}
