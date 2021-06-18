using Library.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Game.Engine.Types;

namespace Game.Engine
{
    public class EngineTickController
    {
        public EngineCore Core { get; private set; }

        public EngineTickController(EngineCore core)
        {
            Core = core;
        }

        public void Advance(TickInput Input)
        {
            Point<double> appliedOffset = new Point<double>();

            int movementSpeed = 5;

            #region Keyboard handler.
            if (Input.InputType == TickInputType.Keyboard)
            {
                if (Input.Key == Keys.NumPad1 || Input.Key == Keys.Z)
                {
                    appliedOffset.X -= movementSpeed;
                    appliedOffset.Y += movementSpeed;
                }
                else if (Input.Key == Keys.NumPad2 || Input.Key == Keys.Down || Input.Key == Keys.S)
                {
                    appliedOffset.Y += movementSpeed;
                }
                else if (Input.Key == Keys.NumPad3 || Input.Key == Keys.X)
                {
                    appliedOffset.X += movementSpeed;
                    appliedOffset.Y += movementSpeed;
                }
                else if (Input.Key == Keys.NumPad4 || Input.Key == Keys.Left || Input.Key == Keys.A)
                {
                    appliedOffset.X -= movementSpeed;
                }
                else if (Input.Key == Keys.NumPad6 || Input.Key == Keys.Right || Input.Key == Keys.D)
                {
                    appliedOffset.X += movementSpeed;
                }
                else if (Input.Key == Keys.NumPad7 || Input.Key == Keys.Q)
                {
                    appliedOffset.X -= movementSpeed;
                    appliedOffset.Y -= movementSpeed;
                }
                else if (Input.Key == Keys.NumPad8 || Input.Key == Keys.Up || Input.Key == Keys.W)
                {
                    appliedOffset.Y -= movementSpeed;
                }
                else if (Input.Key == Keys.NumPad9 || Input.Key == Keys.E)
                {
                    appliedOffset.X += movementSpeed;
                    appliedOffset.Y -= movementSpeed;
                }
            }
            #endregion

            Core.Player.X += appliedOffset.X;
            Core.Player.Y += appliedOffset.Y;

            var intersection = Core.Actors.Intersections(Core.Player).OrderBy(o => o.DrawOrder).LastOrDefault();
            if (intersection == null || intersection.Meta.CanWalkOn == false)
            {
                Core.Player.X -= appliedOffset.X;
                Core.Player.Y -= appliedOffset.Y;
                return;
            }

            ScrollBackground(appliedOffset);
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
