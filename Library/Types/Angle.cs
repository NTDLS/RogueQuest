using System;

namespace Library.Types
{
    public class Angle<T>
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
        public static double DegreeOffset = 90.0;
        public static double RadianOffset = (Math.PI / 180) * DegreeOffset; //1.5707963267948966

        const double DEG_TO_RAD = Math.PI / 180.0;
        const double RAD_TO_DEG = 180.0 / Math.PI;

        public static double RadiansToDegrees(T rad)
        {
            return rad * (dynamic)RAD_TO_DEG;
        }

        public static T DegreesToRadians(T deg)
        {
            return deg * (dynamic)DEG_TO_RAD;
        }

        public static T XYToRadians(T x, T y)
        {
            return Math.Atan2((dynamic)y, (dynamic)x) + RadianOffset;
        }

        public static T XYToDegrees(T x, T y)
        {
            return RadiansToDegrees(Math.Atan2((dynamic)y, (dynamic)x)) + DegreeOffset;
        }

        public static Point<T> ToXY(Angle<T> angle)
        {
            return new Point<T>(angle.X, angle.Y);
        }

        public static Point<T> DegreesToXY(T degrees)
        {
            T radians = DegreesToRadians(degrees) - (dynamic)RadianOffset;
            return new Point<T>(Math.Cos((dynamic)radians), Math.Sin((dynamic)radians));
        }

        public static Point<T> RadiansToXY(T radians)
        {
            radians -= (dynamic)RadianOffset;
            return new Point<T>(Math.Cos((dynamic)radians), Math.Sin((dynamic)radians));
        }

        #endregion

        #region ~/CTor.

        public Angle()
        {
        }

        public Angle(Angle<T> angle)
        {
            Degrees = angle.Degrees;
        }

        public Angle(T degrees)
        {
            Degrees = degrees;
        }

        public Angle(T x, T y)
        {
            Degrees = RadiansToDegrees(Math.Atan2((dynamic)y, (dynamic)x)) + DegreeOffset;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static Angle<T> operator -(Angle<T> original, Angle<T> modifier)
        {
            return new Angle<T>(original.Degrees - (dynamic)modifier.Degrees);
        }

        public static Angle<T> operator -(Angle<T> original, T degrees)
        {
            return new Angle<T>(original.Degrees - (dynamic)degrees);
        }

        public static Angle<T> operator +(Angle<T> original, Angle<T> modifier)
        {
            return new Angle<T>(original.Degrees + (dynamic)modifier.Degrees);
        }

        public static Angle<T> operator +(Angle<T> original, T degrees)
        {
            return new Angle<T>(original.Degrees + (dynamic)degrees);
        }

        public static Angle<T> operator *(Angle<T> original, Angle<T> modifier)
        {
            return new Angle<T>(original.Degrees * (dynamic)modifier.Degrees);
        }

        public static Angle<T> operator *(Angle<T> original, T degrees)
        {
            return new Angle<T>(original.Degrees * (dynamic)degrees);
        }

        public override bool Equals(object o)
        {
            return (Math.Round((dynamic)((Angle<T>)o).X, 4) == this.X && Math.Round((dynamic)((Angle<T>)o).Y, 4) == this.Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round((dynamic)X, 4):#.####}x,{Math.Round((dynamic)Y, 4):#.####}y}}";
        }


        public T _degrees;
        public T Degrees
        {
            get
            {
                return _degrees;
            }
            set
            {
                if ((dynamic)value < 0)
                {
                    _degrees = (360 - (Math.Abs((dynamic)value) % 360.0));
                }
                else
                {
                    _degrees = ((dynamic)value) % 360;
                }
            }
        }

        public T Radians
        {
            get
            {
                return DegreesToRadians(_degrees) - (dynamic)RadianOffset;
            }
        }

        public T RadiansUnadjusted
        {
            get
            {
                return DegreesToRadians(_degrees);
            }
        }

        public T X
        {
            get
            {
                return Math.Cos((dynamic)Radians);
            }
        }

        public T Y
        {
            get
            {
                return Math.Sin((dynamic)Radians);
            }
        }
    }
}
