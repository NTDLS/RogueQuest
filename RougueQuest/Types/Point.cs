using System;
using System.Drawing;

namespace RougueQuest.Types
{
    public class Point<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Point()
        {
        }
        public Point(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(Point<T> p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        public static T DistanceTo(Point<T> from, Point<T> to)
        {
            var deltaX = Math.Pow((to.X - (dynamic)from.X), 2);
            var deltaY = Math.Pow((to.Y - (dynamic)from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        public static T AngleTo(Point<T> from, Point<T> to)
        {
            var fRadians = Math.Atan2((to.Y - (dynamic)from.Y), (to.X - (dynamic)from.X));
            var fDegrees = ((Angle<double>.RadiansToDegrees(fRadians) + 360.0) + Angle<double>.DegreeOffset) % 360.0;
            return fDegrees;
        }

        #region  Unary Operator Overloading.

        public static Point<T> operator -(Point<T> original, Point<T> modifier)
        {
            return new Point<T>(original.X - (dynamic)modifier.X, original.Y - (dynamic)modifier.Y);
        }

        public static Point<T> operator -(Point<T> original, T modifier)
        {
            return new Point<T>(original.X - (dynamic)modifier, original.Y - (dynamic)modifier);
        }

        public static Point<T> operator +(Point<T> original, Point<T> modifier)
        {
            return new Point<T>(original.X + (dynamic)modifier.X, original.Y + (dynamic)modifier.Y);
        }

        public static Point<T> operator +(Point<T> original, T modifier)
        {
            return new Point<T>(original.X + (dynamic)modifier, original.Y + (dynamic)modifier);
        }

        public static Point<T> operator *(Point<T> original, Point<T> modifier)
        {
            return new Point<T>(original.X * (dynamic)modifier.X, original.Y * (dynamic)modifier.Y);
        }

        public static Point<T> operator *(Point<T> original, T modifier)
        {
            return new Point<T>(original.X * (dynamic)modifier, original.Y * (dynamic)modifier);
        }

        public override bool Equals(object o)
        {
            return (Math.Round((dynamic)((Point<T>)o).X, 4) == this.X && Math.Round((dynamic)((Point<T>)o).Y, 4) == this.Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round((dynamic)X, 4).ToString("#.####")},{Math.Round((dynamic)Y, 4).ToString("#.####")}}}";
        }
    }
}
