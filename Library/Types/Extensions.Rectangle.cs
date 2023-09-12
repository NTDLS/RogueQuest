using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;

namespace Library.Types
{
    public static class DrawingRectangle
    {
        public static Rectangle EmptyRectangle { get; } = new Rectangle(0, 0, 0, 0);

        public static Rectangle NormalizeWidth(this Rectangle rc)
        {
            if (rc.Width >= 0)
                return rc;
            Rectangle result = new Rectangle(rc.Left + rc.Width, rc.Top, -rc.Width, rc.Height);
            return result;
        }

        public static Rectangle NormalizeHeight(this Rectangle rc)
        {
            if (rc.Height >= 0)
                return rc;
            Rectangle result = new Rectangle(rc.Left, rc.Top + rc.Height, rc.Width, -rc.Height);
            return result;
        }

        public static Rectangle Normalize(this Rectangle rc)
        {
            Rectangle result = rc.NormalizeWidth().NormalizeHeight();
            return result;
        }

        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
        }

        public static RawRectangleF ToRawRectangleF(this RectangleF value)
        {
            return new RawRectangleF(value.Left, value.Top, value.Right, value.Bottom);
        }

        public static bool IntersectsWith(this RectangleF reference, RectangleF with, float tolerance)
        {
            return with.X < reference.X + reference.Width + tolerance
                && reference.X < with.X + with.Width + tolerance
                && with.Y < reference.Y + reference.Height + tolerance
                && reference.Y < with.Y + with.Height + tolerance;
        }

        /// <summary>
        /// Gets the bounds of rectangle intersections.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="rc2"></param>
        /// <returns></returns>
        public static Rectangle GetIntersection(this Rectangle rc, Rectangle rc2)
        {
            rc = rc.Normalize();
            rc2 = rc2.Normalize();

            if (rc.IntersectsWith(rc2))
            {
                double left = Math.Max(rc.Left, rc2.Left);
                double width = Math.Min(rc.Right, rc2.Right) - left;
                double top = Math.Max(rc.Top, rc2.Top);
                double height = Math.Min(rc.Bottom, rc2.Bottom) - top;

                return new Rectangle((int)left, (int)top, (int)width, (int)height);
            }

            return EmptyRectangle;
        }
    }
}
