using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    public static class MathExtensions
    {

        public static bool IsAlmostEqualZero(this double number, double tolerance = 1e-5)
        {
            return Math.Abs(number - 0) <= tolerance;
        }

        public static bool IsAlmostEqual(this double number, double target, double tolerance = 1e-5)
        {
            return Math.Abs(number - target) <= tolerance;
        }

        public static double FeetToMm(this double number)
        {
            return UnitUtils.ConvertFromInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
        }

        public static double MmToFeet(this double number)
        {
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
        }

        public static bool IsGreatThan(this double source, double target, double tolerance = 1e-5)
        {
            return source - target > tolerance;
        }

        public static bool IsGreatThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return source - target >= -tolerance;
        }

        public static bool IsLessThan(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreatThanOrEqualWith(source, target, tolerance);
        }

        public static bool IsLessThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreatThan(source, target, tolerance);
        }

        public static double RadianToAngle(this double number)
        {
            return number * (180d / Math.PI);
        }


        public static double AngelToRadian(this double number)
        {
            return number * Math.PI / 180d;
        }

        public static double ToAngle(this double angle)
        {
            return angle * (Math.PI / 180d);
        }

    }
}
