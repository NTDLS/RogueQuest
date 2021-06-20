using Game.Actors;
using Library.Engine;
using Library.Engine.Types;
using Library.Types;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Game.Engine.Types;

namespace Game.Engine
{
    public class EngineTickController
    {
        public delegate void LogEvent(EngineCore core, string text, Color color);
        public event LogEvent OnLog;

        public EngineCore Core { get; private set; }

        public int TimePassed { get; set; }


        public EngineTickController(EngineCore core)
        {
            Core = core;
        }

        public Point<double> Advance(TickInput Input)
        {
            Core.debugRects.Clear();

            Point<double> appliedOffset = new Point<double>();

            int movementSpeed = 10;

            bool isValidInput = false;

            #region Keyboard handler.
            if (Input.InputType == TickInputType.Keyboard)
            {
                if (Input.Key == Keys.NumPad1 || Input.Key == Keys.Z)
                {
                    appliedOffset.X -= movementSpeed;
                    appliedOffset.Y += movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad2 || Input.Key == Keys.Down || Input.Key == Keys.S)
                {
                    appliedOffset.Y += movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad3 || Input.Key == Keys.X)
                {
                    appliedOffset.X += movementSpeed;
                    appliedOffset.Y += movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad4 || Input.Key == Keys.Left || Input.Key == Keys.A)
                {
                    appliedOffset.X -= movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad6 || Input.Key == Keys.Right || Input.Key == Keys.D)
                {
                    appliedOffset.X += movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad7 || Input.Key == Keys.Q)
                {
                    appliedOffset.X -= movementSpeed;
                    appliedOffset.Y -= movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad8 || Input.Key == Keys.Up || Input.Key == Keys.W)
                {
                    appliedOffset.Y -= movementSpeed;
                    isValidInput = true;
                }
                else if (Input.Key == Keys.NumPad9 || Input.Key == Keys.E)
                {
                    appliedOffset.X += movementSpeed;
                    appliedOffset.Y -= movementSpeed;
                    isValidInput = true;
                }
            }
            #endregion

            if (isValidInput == false)
            {
                return new Point<double>(0, 0);
            }

            Core.Player.X += appliedOffset.X;
            Core.Player.Y += appliedOffset.Y;

            var intersections = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.BasicType != BasicTileType.ActorTerrain)
                .Where(o => o.Meta.CanWalkOn == false).ToList();

            //Only get the top terrain block, we dont want to dig to the ocean.
            var topTerrainBlock = Core.Actors.Intersections(Core.Player)
                .Where(o => o.Meta.BasicType == BasicTileType.ActorTerrain)
                .OrderBy(o => o.DrawOrder).LastOrDefault();

            //Only act on the top terrain block if it turns out to be one we cant walk on.
            if (topTerrainBlock.Meta.CanWalkOn == false)
            {
                intersections.Add(topTerrainBlock);
            }

            //Do basic collision detection and back off the player movement
            //from any that might have caused an overlap with tiles that can not be walked on.
            //This includes both terrain and beings.
            foreach (var intersection in intersections)
            {
                //Figure out how much overlap we have.
                var delta = Core.Player.ScreenBounds.GetIntersection(intersection.ScreenBounds);

                //Back the player off of the overalpping collision.
                Core.Player.X -= appliedOffset.X;
                Core.Player.Y -= appliedOffset.Y;

                //Butt the rectangles up against each other and adjust the applied offset to what was actually done.
                if (delta.X > 0 && delta.Y > 0)
                {
                    Core.debugRects.Add(new Rectangle((int)delta.X, (int)delta.Y, (int)delta.Width, (int)delta.Height));

                    if (appliedOffset.X > 0)
                    {
                        appliedOffset.X -= delta.Width;
                    }
                    if (appliedOffset.X < 0)
                    {
                        appliedOffset.X += delta.Width;
                    }
                    if (appliedOffset.Y > 0)
                    {
                        appliedOffset.Y -= delta.Height;
                    }
                    if (appliedOffset.Y < 0)
                    {
                        appliedOffset.Y += delta.Height;
                    }
                }

                Core.Player.X += appliedOffset.X;
                Core.Player.Y += appliedOffset.Y;
            }

            ScrollBackground(appliedOffset);
            TimePassed++;

            GameLogic(intersections);

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
            foreach (var obj in actorsThatCanSeePlayer.Where(o => o.Meta.BasicType == BasicTileType.ActorHostileBeing))
            {
                var largerBounds = new Rectangle(obj.ScreenBounds.X - 1, obj.ScreenBounds.Y - 1, obj.ScreenBounds.Width + 2, obj.ScreenBounds.Height + 2);
                if (Core.Player.ScreenBounds.IntersectsWith(largerBounds))
                {
                    hostileInteractions.Add(obj as ActorHostileBeing);
                }
            }

            //Hostile actors will follow player.
            foreach (var obj in actorsThatCanSeePlayer.Where(o => o.Meta.BasicType == BasicTileType.ActorHostileBeing))
            {
                double distance = Core.Player.DistanceTo(obj);

                if (distance > 200)
                {
                    //obj.Velocity.Angle.Degrees = obj.AngleTo(Core.Player);
                    //obj.X += (obj.Velocity.Angle.X * obj.Velocity.MaxSpeed * obj.Velocity.ThrottlePercentage);
                    //obj.Y += (obj.Velocity.Angle.Y * obj.Velocity.MaxSpeed * obj.Velocity.ThrottlePercentage);
                }
            }

            //Player attack other actor.
            var actorToAttack = intersections.Where(o => o.Meta.CanTakeDamage == true).FirstOrDefault();
            if (actorToAttack != null)
            {
                int playerHitsFor = MathUtility.RandomNumber(1, 5);

                if (MathUtility.ChanceIn(4))
                {
                    //actorToAttack.Hit(playerHitsFor);
                    OnLog?.Invoke(Core, $"Player attacks for {playerHitsFor} and HITS!\r\n", Color.DarkGreen);
                }
                else
                {
                    OnLog?.Invoke(Core, $"Player attacks for {playerHitsFor} and MISSES!\r\n", Color.DarkRed);
                }
            }


            //Hostiles attack player. Be sure to look at visible actors only because the player may have killed one before we get here.
            foreach (var actor in hostileInteractions.Where(o => o.Visible))
            {
                int actorHitsFor = MathUtility.RandomNumber(1, 5);

                //Monster hit player.
                if (MathUtility.ChanceIn(4))
                {
                    OnLog?.Invoke(Core, $"Monster attacks for {actorHitsFor} and HITS!\r\n", Color.DarkRed);
                    //Core.Player.Hit(actorHitsFor);
                }
                else
                {
                    OnLog?.Invoke(Core, $"Monster attacks for {actorHitsFor} and Misses!\r\n", Color.DarkGreen);
                }
            }
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
    }
}
