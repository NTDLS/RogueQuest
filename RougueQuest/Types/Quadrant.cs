using System.Drawing;

namespace AI2D.Engine
{
    public class Quadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public Quadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }
    }
}