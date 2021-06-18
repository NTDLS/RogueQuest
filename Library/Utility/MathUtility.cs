using Library.Engine;
using Library.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Library.Utility
{
    public class MathUtility
    {

        #region Math.

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Point<double> AngleFromPointAtDistance(Angle<double> angle, Point<double> distance)
        {
            return new Point<double>(
                (Math.Cos(angle.Radians) * distance.X),
                (Math.Sin(angle.Radians) * distance.Y));
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double AngleTo(ActorBase from, ActorBase to)
        {
            return Point<double>.AngleTo(from.Location, to.Location);
        }

        public static double AngleTo(Point<double> from, ActorBase to)
        {
            return Point<double>.AngleTo(from, to.Location);
        }

        public static double AngleTo(ActorBase from, Point<double> to)
        {
            return Point<double>.AngleTo(from.Location, to);
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj));
            return deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees);
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees))
            {
                double distance = DistanceTo(fromObj, atObj);

                return distance <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromObj"></param>
        /// <param name="atObj"></param>
        /// <param name="offsetAngle">-90 degrees would be looking off te left-hand side of the object</param>
        /// <returns></returns>
        public static double DeltaAngle(ActorBase fromObj, ActorBase atObj, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, atObj);

            if (fromAngle < 0) fromAngle = (0 - fromAngle);
            if (angleTo < 0)
            {
                angleTo = (0 - angleTo);
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - (Math.Abs(angleTo) % 360.0);
            }

            return angleTo;
        }

        public static double DistanceTo(ActorBase from, ActorBase to)
        {
            return Point<double>.DistanceTo(from.Location, to.Location);
        }

        #endregion

        #region Random.


        public static bool ChanceIn(int n)
        {
            return (Random.Next(0, n * 10) % n) == n/2;
        }


        public static Random Random = new Random();
        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static Double RandomNumber(double min, double max)
        {
            return Random.NextDouble() * (max - min) + min;
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static int RandomNumberNegative(int min, int max)
        {
            if (FlipCoin())
            {
                return -(Random.Next(0, 1000) % max);
            }
            return Random.Next(0, 1000) % max;
        }

        #endregion
    }
}
