using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    /// <summary>
    /// 数学类扩展方法
    /// </summary>
    public static class MathExtensions
    {
        private static readonly string _typeId = "autodesk.unit.unit:millimeters-1.0.1";

        /// <summary>
        /// 判断一个数是否接近于零
        /// </summary>
        /// <param name="number">要判断的数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果数接近于零则返回true，否则为false</returns>
        public static bool IsAlmostEqualZero(this double number, double tolerance = 1e-5)
        {
            return Math.Abs(number) <= tolerance;
        }

        /// <summary>
        /// 判断两个数是否接近
        /// </summary>
        /// <param name="number">要比较的数</param>
        /// <param name="target">目标数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果两个数接近则返回true，否则为false</returns>
        public static bool IsAlmostEqual(this double number, double target, double tolerance = 1e-5)
        {
            return Math.Abs(number - target) <= tolerance;
        }

        /// <summary>
        /// 将英尺转换为毫米
        /// </summary>
        /// <param name="number">要转换的英尺数</param>
        /// <returns>转换后的毫米数</returns>
        public static double FeetToMm(this double number)
        {
#if R2018 || R2019 || R2020
            return UnitUtils.ConvertFromInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertFromInternalUnits(number, new ForgeTypeId(_typeId));
#endif
        }

        /// <summary>
        /// 将毫米转换为英尺
        /// </summary>
        /// <param name="number">要转换的毫米数</param>
        /// <returns>转换后的英尺数</returns>
        public static double MmToFeet(this double number)
        {
#if R2018 || R2019 || R2020
            return UnitUtils.ConvertToInternalUnits(number, DisplayUnitType.DUT_MILLIMETERS);
#else
            return UnitUtils.ConvertToInternalUnits(number, new ForgeTypeId(_typeId));
#endif
        }

        /// <summary>
        /// 判断一个数是否大于另一个数
        /// </summary>
        /// <param name="source">源数</param>
        /// <param name="target">目标数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果源数大于目标数则返回true，否则为false</returns>
        public static bool IsGreatThan(this double source, double target, double tolerance = 1e-5)
        {
            return source - target > tolerance;
        }

        /// <summary>
        /// 判断一个数是否大于等于另一个数
        /// </summary>
        /// <param name="source">源数</param>
        /// <param name="target">目标数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果源数大于等于目标数则返回true，否则为false</returns>
        public static bool IsGreatThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return source - target >= -tolerance;
        }

        /// <summary>
        /// 判断一个数是否小于另一个数
        /// </summary>
        /// <param name="source">源数</param>
        /// <param name="target">目标数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果源数小于目标数则返回true，否则为false</returns>
        public static bool IsLessThan(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreatThanOrEqualWith(source, target, tolerance);
        }

        /// <summary>
        /// 判断一个数是否小于等于另一个数
        /// </summary>
        /// <param name="source">源数</param>   
        /// <param name="target">目标数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果源数小于等于目标数则返回true，否则为false</returns>
        public static bool IsLessThanOrEqualWith(this double source, double target, double tolerance = 1e-5)
        {
            return !IsGreatThan(source, target, tolerance);
        }

        /// <summary>
        /// 弧度转换为角度
        /// </summary>
        /// <param name="radian">要转换的弧度</param>
        /// <returns>转换后的角度</returns>
        public static double RadianToDegree(this double radian)
        {
            return radian * (180d / Math.PI);
        }

        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="degree">要转换的角度</param>
        /// <returns>转换后的弧度</returns>
        public static double DegreeToRadian(this double degree)
        {
            return degree * Math.PI / 180d;
        }
    }
}
