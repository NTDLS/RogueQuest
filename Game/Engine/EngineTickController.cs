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
                    Core.LogLine($"Picked up {underfootPack.Meta.Name} and placed it on your back.");
                    pack.Tile = underfootPack.CloneIdentifier();
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
                var itemsRemovedFromContainer = new List<Guid>();

                if (item.Meta.IsContainer == true)
                {
                    var container = Core.Actors.Containers.GetContainer((Guid)item.Meta.UID);

                    foreach (var contItem in container.Contents)
                    {
                        //Do weight/bulk math.
                        var currentContainerWeight = Core.State.Character.Inventory.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Weight);
                        if (contItem.Meta.Weight + currentContainerWeight > maxWeight)
                        {
                            Core.LogLine($"{item.Meta.Name} is too bulky for your {pack.Tile.Meta.Name}. Drop something?");
                            break;
                        }

                        var currentContainerBulk = Core.State.Character.Inventory.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Bulk);
                        if (contItem.Meta.Bulk + currentContainerBulk > maxBulk)
                        {
                            Core.LogLine($"{item.Meta.Name} is too heavy for your {pack.Tile.Meta.Name}. Drop something?");
                            break;
                        }

                        Core.LogLine($"Picked up" + ((contItem.Meta.CanStack == true) ? $" {contItem.Meta.Quantity:N0}" : "") + $" {contItem.Meta.Name}");

                        if (contItem.Meta.CanStack == true)
                        {
                            var existingItem = Core.State.Character.Inventory.Where(o => o.Tile.TilePath == contItem.TilePath).FirstOrDefault();
                            if (existingItem != null)
                            {
                                existingItem.Tile.Meta.Quantity += contItem.Meta.Quantity;
                                continue;
                            }
                        }

                        var inventoryItem = new InventoryItem()
                        {
                            ContainerId = containerId,
                            Tile = contItem.Clone()
                        };

                        itemsRemovedFromContainer.Add((Guid)inventoryItem.Tile.Meta.UID);
                        Core.State.Character.Inventory.Add(inventoryItem);
                    }

                    //Remove the items from the container that were place into inventory.
                    foreach (var itemId in itemsRemovedFromContainer)
                    {
                        container.Contents.RemoveAll(o => o.Meta.UID == itemId);
                    }
                }
                else
                {
                    //Do weight/bulk math.
                    var currentContainerWeight = Core.State.Character.Inventory.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Weight);
                    if (item.Meta.Weight + currentContainerWeight > maxWeight)
                    {
                        Core.LogLine($"{item.Meta.Name} is too bulky for your {pack.Tile.Meta.Name}. Drop something?");
                        break;
                    }

                    var currentContainerBulk = Core.State.Character.Inventory.Where(o => o.ContainerId == containerId).Sum(o => o.Tile.Meta.Bulk);
                    if (item.Meta.Bulk + currentContainerBulk > maxBulk)
                    {
                        Core.LogLine($"{item.Meta.Name} is too heavy for your {pack.Tile.Meta.Name}. Drop something?");
                        break;
                    }

                    Core.LogLine($"Picked up" + ((item.Meta.CanStack == true) ? $" {item.Meta.Quantity:N0}" : "") + $" {item.Meta.Name}");

                    if (item.Meta.CanStack == true)
                    {
                        var existingItem = Core.State.Character.Inventory.Where(o => o.Tile.TilePath == item.TilePath).FirstOrDefault();
                        if (existingItem != null)
                        {
                            existingItem.Tile.Meta.Quantity += item.Meta.Quantity;
                            item.QueueForDelete();
                            continue;
                        }
                    }

                    var inventoryItem = new InventoryItem()
                    {
                        ContainerId = containerId,
                        Tile = item.CloneIdentifier()
                    };

                    Core.State.Character.Inventory.Add(inventoryItem);

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