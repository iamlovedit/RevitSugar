using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    public static class MathExtensions
    {
        private static readonly string _typeId = "autodesk.unit.unit:millimeters-1.0.1";
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
#if R2018 || R2019 || R2020
            return UnitUtils.ConvertFromInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertFromInternalUnits(number, new ForgeTypeId(_typeId));
#endif
        }

        public static double MmToFeet(this double number)
        {
#if R2018 || R2019 || R2020
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertToInternalUnits(number, new ForgeTypeId(_typeId));
#endif
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
