using Library.Engine;
using Library.Types;
using Library.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Library.Engine
{
    public class EngineDisplay
    {
        public Dictionary<Point, Quadrant> Quadrants = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; set; }
        public Point<double> BackgroundOffset { get; set; } = new Point<double>(); //Offset of background, all cals must take into account.
        public RectangleF VisibleBounds { get; private set; }

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
