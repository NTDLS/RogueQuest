using Library.Native;
using Library.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Library.Engine
{
    public class EngineDisplay
    {
        public Dictionary<Point, Quadrant> Quadrants = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; set; }
        public Point<double> BackgroundOffset { get; set; } = new Point<double>(); //Offset of background, all calls must take into account.
        public RectangleF VisibleBounds { get; private set; }


        /// <summary>
        /// The number of extra pixles to draw beyond the NatrualScreenSize.
        /// </summary>
        public Size OverdrawSize { get; private set; }

        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size TotalCanvasSize { get; private set; }

        /// <summary>
        /// The size of the screen with no scaling.
        /// </summary>
        public Size NatrualScreenSize { get; private set; }

        /// <summary>
        /// The bounds of the screen with no scaling.
        /// </summary>
        public RectangleF NatrualScreenBounds
        {
            get
            {
                return new RectangleF(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f,
                        NatrualScreenSize.Width, NatrualScreenSize.Height
                );
            }
        }

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natrual + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalScreenBounds
        {
            get
            {
                return new RectangleF(0, 0, TotalCanvasSize.Width, TotalCanvasSize.Height
                );
            }
        }

        private Size _visibleSize;
        public Size VisibleSize
        {
            get
            {
                return _visibleSize;
            }
        }

        private Control _drawingSurface;
        public Control DrawingSurface
        {
            get
            {
                return _drawingSurface;
            }
        }

        public Point<double> RandomOnScreenLocation()
        {
            return new Point<double>(MathUtility.Random.Next(0, VisibleSize.Width), MathUtility.Random.Next(0, VisibleSize.Height));
        }

        public Point<double> RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (MathUtility.FlipCoin())
            {
                if (MathUtility.FlipCoin())
                {
                    x = -MathUtility.RandomNumber(min, max);
                    y = MathUtility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = -MathUtility.RandomNumber(min, max);
                    x = MathUtility.RandomNumber(0, VisibleSize.Width);
                }
            }
            else
            {
                if (MathUtility.FlipCoin())
                {
                    x = VisibleSize.Width + MathUtility.RandomNumber(min, max);
                    y = MathUtility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = VisibleSize.Height + MathUtility.RandomNumber(min, max);
                    x = MathUtility.RandomNumber(0, VisibleSize.Width);
                }

            }

            return new Point<double>(x, y);
        }

        public EngineDisplay(Control drawingSurface, Size visibleSize)
        {
            _drawingSurface = drawingSurface;
            _visibleSize = visibleSize;
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);


            int totalSizeX = (int)(visibleSize.Width * 2);
            int totalSizeY = (int)(visibleSize.Height * 2);

            if (totalSizeX % 2 != 0) totalSizeX++;
            if (totalSizeY % 2 != 0) totalSizeY++;

            TotalCanvasSize = new Size(totalSizeX, totalSizeY);

            OverdrawSize = new Size(totalSizeX - NatrualScreenSize.Width, totalSizeY - NatrualScreenSize.Height);
        }

        public void ResizeDrawingSurface(Size visibleSize)
        {
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);
        }

        public Quadrant GetQuadrant(double x, double y)
        {
            var coord = new Point(
                    (int)(x / VisibleSize.Width),
                    (int)(y / VisibleSize.Height)
                );

            if (Quadrants.ContainsKey(coord) == false)
            {
                var absoluteBounds = new Rectangle(
                    VisibleSize.Width * coord.X,
                    VisibleSize.Height * coord.Y,
                    VisibleSize.Width,
                    VisibleSize.Height);

                var quad = new Quadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
