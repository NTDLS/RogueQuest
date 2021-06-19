using Game.Actors;
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

            var intersection = Core.Actors.Intersections(Core.Player).OrderBy(o => o.DrawOrder).LastOrDefault();
            if (intersection == null || intersection.Meta.CanWalkOn == false)
            {
                //Back the player off of the collision and zero out the applied offset.
                Core.Player.X -= appliedOffset.X;
                Core.Player.Y -= appliedOffset.Y;
                appliedOffset = new Point<double>(0, 0);
            }

            ScrollBackground(appliedOffset);
            TimePassed++;

            GameLogic();

            return appliedOffset;
        }

        public class Interactions
        {
            public ActorHostileBeing Actor { get; set; }
            public double Distance { get; set; }
        }

        void GameLogic()
        {
            var withinVisibleRange = Core.Actors.Intersections(Core.Player, 150)
                .Where(o => o.Meta.BasicType == BasicTileType.ActorHostileBeing);

            var interactions = new List<Interactions>();

            foreach (var obj in withinVisibleRange)
            {
                double playerSize = ((Core.Player.Size.Height / 2) + (Core.Player.Size.Width / 2)) / 2;
                double objSize = ((obj.Size.Height / 2) + (obj.Size.Width / 2)) / 2;
                double interactionDistance = (playerSize + objSize + 20);

                double distance = Core.Player.DistanceTo(obj);

                //Follow the player, but don't pile on top of them.
                if (distance > (playerSize + objSize) + 10)
                {
                    obj.Velocity.Angle.Degrees = obj.AngleTo(Core.Player);
                    obj.X += (obj.Velocity.Angle.X * obj.Velocity.MaxSpeed * obj.Velocity.ThrottlePercentage);
                    obj.Y += (obj.Velocity.Angle.Y * obj.Velocity.MaxSpeed * obj.Velocity.ThrottlePercentage);
                }

                //these are the hostiles that are close enough for melee attacks.
                if (distance <= interactionDistance)
                {
                    interactions.Add(new Interactions()
                    {
                        Actor = obj as ActorHostileBeing,
                        Distance = distance
                    });
                }
            }

            if (interactions.Count == 0)
            {
                return;
            }

            var actorToAttack = interactions.OrderByDescending(o => o.Distance).First();

            int playerHitsFor = MathUtility.RandomNumber(1, 5);

            //Player hit hostile
            if (MathUtility.ChanceIn(4))
            {
                actorToAttack.Actor.Hit(playerHitsFor);
                OnLog?.Invoke(Core, $"Player attacks for {playerHitsFor} and HITS!\r\n", Color.DarkGreen);
            }
            else
            {
                OnLog?.Invoke(Core, $"Player attacks for {playerHitsFor} and MISSES!\r\n", Color.DarkRed);
            }

            foreach (var actor in interactions.Where(o => o.Actor.Visible))
            {
                int actorHitsFor = MathUtility.RandomNumber(1, 5);

                //Monster hit player.
                if (MathUtility.ChanceIn(4))
                {
                    OnLog?.Invoke(Core, $"Monster attacks for {actorHitsFor} and HITS!\r\n", Color.DarkRed);
                    Core.Player.Hit(actorHitsFor);
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
