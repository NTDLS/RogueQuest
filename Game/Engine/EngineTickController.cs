﻿using Library.Types;
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

            #region Keyboard handler.
            if (Input.InputType == TickInputType.Keyboard)
            {
                if (Input.Key == Keys.NumPad1 || Input.Key == Keys.Z)
                {
                    appliedOffset.X -= 10;
                    appliedOffset.Y += 10;
                }
                else if (Input.Key == Keys.NumPad2 || Input.Key == Keys.Down || Input.Key == Keys.S)
                {
                    appliedOffset.Y += 10;
                }
                else if (Input.Key == Keys.NumPad3 || Input.Key == Keys.X)
                {
                    appliedOffset.X += 10;
                    appliedOffset.Y += 10;
                }
                else if (Input.Key == Keys.NumPad4 || Input.Key == Keys.Left || Input.Key == Keys.A)
                {
                    appliedOffset.X -= 10;
                }
                else if (Input.Key == Keys.NumPad6 || Input.Key == Keys.Right || Input.Key == Keys.D)
                {
                    appliedOffset.X += 10;
                }
                else if (Input.Key == Keys.NumPad7 || Input.Key == Keys.Q)
                {
                    appliedOffset.X -= 10;
                    appliedOffset.Y -= 10;
                }
                else if (Input.Key == Keys.NumPad8 || Input.Key == Keys.Up || Input.Key == Keys.W)
                {
                    appliedOffset.Y -= 10;
                }
                else if (Input.Key == Keys.NumPad9 || Input.Key == Keys.E)
                {
                    appliedOffset.X += 10;
                    appliedOffset.Y -= 10;
                }
            }
            #endregion

            Core.Player.X += appliedOffset.X;
            Core.Player.Y += appliedOffset.Y;

            var intersections = Core.Terrain.Intersections(Core.Player).OrderBy(o => o.DrawOrder);


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
                Core.Display.BackgroundOffset.X += 10;
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.Y > 0 && (Core.Player.Y - Core.Display.BackgroundOffset.Y) > Core.Display.VisibleSize.Height - boxSize)
            {
                Core.Display.BackgroundOffset.Y += 10;
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.X < 0 && (Core.Player.X - Core.Display.BackgroundOffset.X) < boxSize)
            {
                Core.Display.BackgroundOffset.X -= 10;
                Core.Display.DrawingSurface.Invalidate();
            }
            if (appliedOffset.Y < 0 && (Core.Player.Y - Core.Display.BackgroundOffset.Y) < boxSize)
            {
                Core.Display.BackgroundOffset.Y -= 10;
                Core.Display.DrawingSurface.Invalidate();
            }
        }
    }
}
