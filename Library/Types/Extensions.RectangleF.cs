using System;
using System.Drawing;

namespace Library.Types
{
    public static class DrawingRectangleF
    {
        public static RectangleF EmptyRectangleF { get; } = new RectangleF(0, 0, 0, 0);

        public static RectangleF NormalizeWidth(this RectangleF rc)
        {
            if (rc.Width >= 0)
                return rc;
            RectangleF result = new RectangleF(rc.Left + rc.Width, rc.Top, -rc.Width, rc.Height);
            return result;
        }

        public static RectangleF NormalizeHeight(this RectangleF rc)
        {
            if (rc.Height >= 0)
                return rc;
            RectangleF result = new RectangleF(rc.Left, rc.Top + rc.Height, rc.Width, -rc.Height);
            return result;
        }

        public static RectangleF Normalize(this RectangleF rc)
        {
            RectangleF result = rc.NormalizeWidth().NormalizeHeight();
            return result;
        }

        public static RectangleF GetIntersection(this RectangleF rc, RectangleF rc2)
        {
            rc = rc.Normalize();
            rc2 = rc2.Normalize();

            if (rc.IntersectsWith(rc2))
            {
                double left = Math.Max(rc.Left, rc2.Left);
                double width = Math.Min(rc.Right, rc2.Right) - left;
                double top = Math.Max(rc.Top, rc2.Top);
                double height = Math.Min(rc.Bottom, rc2.Bottom) - top;

                return new RectangleF((float)left, (float)top, (float)width, (float)height);
            }

            return EmptyRectangleF;
        }
    }
}
