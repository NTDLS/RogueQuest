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
        public EngineCore Core { get; private set; }
        public int TimePassed { get; set; }
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
                        existingItem.Tile.Meta.Quantity += item.Tile.Meta.Quantity;
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
                        existingItem.Tile.Meta.Quantity += item.Tile.Meta.Quantity;

                        itemsToDelete.Add((Guid)item.Tile.Meta.UID);
                        continue;
                    }
                }

                item.ContainerId = pack.Tile.Meta.UID;
            }

            Core.State.Items.RemoveAll(o => itemsToDelete.Contains((Guid)o.Tile?.Meta?.UID));
        }

        public void Get()
        {
            var itemsUnderfoot = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.ActorClass == ActorClassName.ActorItem)
                .Cast<ActorItem>();

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

            foreach (var item in itemsUnderfoot)
            {
                if (item.Meta.IsContainer == true)
                {
                    //We take stuff out of all containers except for bags. For bags, we pick up the entire thing.
                    if (item.Meta.SubType == ActorSubType.Pack)
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

                    Core.LogLine($"Picked up" + ((item.Meta.CanStack == true) ? $" {item.Meta.Quantity:N0}" : "") + $" {item.Meta.Name}");

                    if (item.Meta.CanStack == true)
                    {
                        var existingItem = Core.State.Items
                            .Where(o => o.Tile.TilePath == item.TilePath && o.ContainerId == pack.Tile.Meta.UID).FirstOrDefault();
                        if (existingItem != null)
                        {
                            existingItem.Tile.Meta.Quantity += item.Meta.Quantity;
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
                return;
            }

            while (Core.State.Character.AvailableHitpoints < Core.State.Character.Hitpoints)
            {
                Advance(input);

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

                intersections.AddRange(MoveActor(Core.Player, out appliedOffset));
                ScrollBackground(appliedOffset);
            }
            else if (Input.InputType == TickInputType.Rest)
            {
                Core.Player.Velocity.ThrottlePercentage = 0;
            }

            TimePassed++;

            GameLogic(intersections);

            Core.PurgeAllDeletedTiles();

            return appliedOffset;
        }

        /// <summary>
        /// Intersections contains all objects that the player collided with during their turn. If this
        /// contains actors that can be damaged, then these  would be the melee attack target for the player.
        /// </summary>
        /// <param name="intersections"></param>
        void GameLogic(List<ActorBase> intersections)
        {
            var actorsThatCanSeePlayer = Core.Actors.Intersections(Core.Player, 150)
                .Where(o => o.Meta.CanTakeDamage == true);

            var hostileInteractions = new List<ActorHostileBeing>();

            //Find out which of the hostile beings in visible range are touching the
            //  player bounds (not intersecting, because these should be none, but just touching.
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

            //Player attack other actor.
            var actorToAttack = intersections.Where(o => o.Meta.CanTakeDamage == true).FirstOrDefault();
            if (actorToAttack != null)
            {
                int playerHitsFor = MathUtility.RandomNumber(1, Core.State.Character.BaseDamage);

                if (MathUtility.ChanceIn(4))
                {
                    Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} for {playerHitsFor}hp {GetStrikeFlair()}", Color.DarkGreen);

                    if (actorToAttack.Hit(playerHitsFor))
                    {
                        int experience = 0;

                        if (actorToAttack.Meta != null && actorToAttack.Meta.Experience != null)
                        {
                            experience = (int)actorToAttack.Meta.Experience;
                        }

                        Core.LogLine($"{Core.State.Character.Name} kills {actorToAttack.Meta.Name} gaining {experience}xp!", Color.DarkGreen);

                        Core.State.Character.Experience += experience;
                    }
                }
                else
                {
                    Core.LogLine($"{Core.State.Character.Name} attacks {actorToAttack.Meta.Name} for {playerHitsFor}hp {GetMissFlair()}", Color.DarkRed);
                }
            }


            if (Core.State.Character.Experience > Core.State.Character.NextLevelExperience)
            {
                Core.State.Character.LevelUp();
            }

            //Hostiles attack player. Be sure to look at visible actors only because the player may have killed one before we get here.
            foreach (var actor in hostileInteractions.Where(o => o.Visible))
            {
                int actorHitsFor = MathUtility.RandomNumber(1, 5);

                //Monster hit player.
                if (MathUtility.ChanceIn(4))
                {
                    Core.LogLine($"{actor.Meta.Name} attacks {Core.State.Character.Name} for {actorHitsFor}hp and hits!", Color.DarkRed);
                    Core.State.Character.AvailableHitpoints -= actorHitsFor;
                    if (Core.State.Character.AvailableHitpoints <= 0)
                    {
                        Core.Player.QueueForDelete();
                    }
                }
                else
                {
                    Core.LogLine($"{actor.Meta.Name} attacks {Core.State.Character.Name} for {actorHitsFor}hp but missed!", Color.DarkGreen);
                }
            }
        }
        
        /// <summary>
        /// Moves an actor in the direction of their vector and returns a list of any
        /// encountered colissions as well as passes back distanct the axtor was moved.
        /// </summary>
        public List<ActorBase> MoveActor(ActorBase actor, out Point<double> finalAppliedOffset)
        {
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
                .OrderBy(o => o.DrawOrder).LastOrDefault();

            //Only act on the top terrain block if it turns out to be one we cant walk on.
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
                        {
                            appliedOffset.X = 0;
                        }
                        else
                        {
                            appliedOffset.X -= delta.Width;
                        }
                    }

                    if (appliedOffset.X < 0)
                    {
                        if (Math.Abs(delta.Width) > Math.Abs(appliedOffset.X))
                        {
                            appliedOffset.X = 0;
                        }
                        else
                        {
                            appliedOffset.X += delta.Width;
                        }
                    }

                    if (appliedOffset.Y > 0)
                    {
                        if (Math.Abs(delta.Height) > Math.Abs(appliedOffset.Y))
                        {
                            appliedOffset.Y = 0;
                        }
                        else
                        {
                            appliedOffset.Y -= delta.Height;
                        }
                    }

                    if (appliedOffset.Y < 0)
                    {
                        if (Math.Abs(delta.Height) > Math.Abs(appliedOffset.Y))
                        {
                            appliedOffset.Y = 0;
                        }
                        else
                        {
                            appliedOffset.Y += delta.Height;
                        }
                    }
                }

                actor.X += appliedOffset.X;
                actor.Y += appliedOffset.Y;
            }

            finalAppliedOffset = new Point<double>(appliedOffset);

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

        string GetStrikeFlair()
        {
            var strs = new string[] {
                "hitting them in the leg!",
                "landing a crushing blow!.",
                "hitting them in the head!.",
                "hitting them in the torso!",
                "nearly removing a limb!",
                "causing them to stumble back!",
                "knocking them to the ground!"};

            if (MathUtility.ChanceIn(25))
            {
                return strs[MathUtility.RandomNumber(0, strs.Count() - 1)];
            }
            else
            {
                return "and hits.";
            }
        }

        string GetMissFlair()
        {
            var strs = new string[] {
                "but they evade your clumsy blow!",
                "but they were faster then you expected!",
                "but they pull just out of your path!",
                "but you were bested by their agility!"
            };

            if (MathUtility.ChanceIn(25))
            {
                return strs[MathUtility.RandomNumber(0, strs.Count() - 1)];
            }
            else
            {
                return "but missed.";
            }
        }

        #endregion

    }
}