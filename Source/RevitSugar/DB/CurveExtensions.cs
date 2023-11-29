using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    public static class CurveExtensions
    {
        /// <summary>
        /// 获取给定曲线与目标曲线的交点的XYZ点集合。
        /// </summary>
        /// <param name="curve">要查找交点的曲线。</param>
        /// <param name="target">要查找交点的目标曲线。</param>
        /// <returns>表示曲线与目标曲线交点的XYZ点的IEnumerable集合。</returns>
        /// <exception cref="ArgumentNullException">当曲线或目标为null时引发异常。</exception>
        /// <remarks>
        /// 该函数使用Intersect方法检查给定曲线和目标曲线是否相交。
        /// 如果结果为SetComparisonResult.Overlap，则从resultArray中提取XYZ点。
        /// 返回表示交点的XYZ点集合。
        /// </remarks>
        public static IEnumerable<XYZ> GetCrorssPoints(this Curve curve, Curve target)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            IEnumerable<XYZ> points = null;
            var result = curve.Intersect(target, out var resultArray);
            if (result == SetComparisonResult.Overlap)
            {
                points = resultArray.OfType<IntersectionResult>().Select(i => i.XYZPoint);
            }
            return points;
        }

        /// <summary>
        /// 获取一组曲线之间的交点。
        /// </summary>
        /// <param name="curves">要查找交点的曲线集合。</param>
        /// <returns>表示交点的XYZ点的List集合。</returns>
        public static IList<XYZ> GetIntersectPoints(this IList<Curve> curves)
        {
            if (curves is null)
            {
                throw new ArgumentNullException(nameof(curves));
            }

            var result = new List<XYZ>();
            for (int i = curves.Count - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (curves[i].Intersect(curves[j], out var resultArray) == SetComparisonResult.Overlap)
                    {
                        for (int k = 0; k < resultArray.Size; k++)
                        {
                            result.Add(resultArray.get_Item(k).XYZPoint);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 判断给定点是否在曲线上，容差为指定的距离。
        /// </summary>
        /// <param name="curve">要检查的曲线。</param>
        /// <param name="point">要检查的点。</param>
        /// <param name="tolerance">曲线与点之间的距离容差。</param>
        /// <returns>如果点在曲线上且在指定容差范围内，则返回true，否则返回false。</returns>
        public static bool IsPointOnCurve(this Curve curve, XYZ point, double tolerance = 1e-3)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return curve.Distance(point) <= tolerance;
        }

        /// <summary>
        /// 判断点是否投影到曲线上。
        /// </summary>
        /// <param name="curve">要投影到的曲线。</param>
        /// <param name="point">要投影到曲线上的点。</param>
        /// <param name="tolerance">用于将投影点与曲线的端点进行比较的容差。</param>
        /// <returns>一个布尔值，指示点是否投影到曲线上。</returns>
        public static bool IsPointProjectAtCurve(this Curve curve, XYZ point, double tolerance = 1e-3)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            var projectPoint = curve.Evaluate(curve.Project(point).Parameter, false);
            return !(projectPoint.IsAlmostEqualTo(curve.GetEndPoint(0), tolerance) || projectPoint.IsAlmostEqualTo(curve.GetEndPoint(1), tolerance));
        }

        /// <summary>
        /// 判断给定曲线是否与目标曲线在指定容差范围内共线。
        /// </summary>
        /// <param name="source">要检查共线性的曲线。</param>
        /// <param name="target">用于比较的参考曲线。</param>
        /// <param name="tolerance">曲线之间允许的最大距离，以判断它们是否共线。 (可选)</param>
        /// <returns>如果曲线在指定容差范围内共线，则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentNullException">如果源曲线或目标曲线为null，则引发异常。</exception>
        public static bool IsCollinearWith(this Curve source, Curve target, double tolerance = 1e-5)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using var curve = source.Clone();
            curve.MakeUnbound();
            return curve.Distance(target.GetStartPoint()) <= tolerance && curve.Distance(target.GetEndPoint()) <= tolerance;
        }

        /// <summary>
        /// 获取曲线的中点。
        /// </summary>
        /// <param name="curve">要获取中点的曲线。</param>
        /// <returns>曲线的中点。</returns>
        public static XYZ GetMiddlePoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.Evaluate(0.5, true);
        }

        /// <summary>
        /// 判断两个圆弧是否平行。
        /// </summary>
        /// <param name="source">要检查的圆弧</param>
        /// <param name="target">用于检查的圆弧</param>
        /// <param name="tolerance">检查容差</param>
        /// <returns>如果两个圆弧平行，则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentNullException">如果源圆弧或目标圆弧为null，则引发异常。</exception>
        public static bool IsParalleWith(this Arc source, Arc target, double tolerance = 1e-5)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.Center.DistanceTo(target.Center).IsAlmostEqualZero(tolerance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XYZ GetStartPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XYZ GetEndPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsParalleWith(this Line source, Line target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var sourceVec = source.GetEndPoint() - source.GetStartPoint();
            var targetVer = target.GetEndPoint() - target.GetStartPoint();
            return sourceVec.IsParallerWith(targetVer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="isExtendZ"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Outline GetOutline(this Curve curve, bool isExtendZ = false)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            IList<XYZ> points = null;
            if (curve.IsBound)
            {
                points = curve.Tessellate();
            }
            else
            {
                points = new XYZ[] { curve.GetStartPoint(), curve.GetEndPoint() };
            }
            var xList = points.Select(p => p.X).OrderBy(x => x);
            var yList = points.Select(p => p.Y).OrderBy(y => y);
            if (isExtendZ)
            {
                return new Outline(new XYZ(xList.First(), yList.First(), double.MinValue), new XYZ(xList.Last(), yList.Last(), double.MaxValue));
            }
            else
            {
                var zList = points.Select(p => p.Z).OrderBy(z => z);
                return new Outline(new XYZ(xList.First(), yList.First(), zList.First()), new XYZ(xList.Last(), yList.Last(), zList.Last()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<Line> GetLines(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            var result = new List<Line>();
            if (curve is Line line)
            {
                result.Add(line);
            }
            else if (curve.IsBound)
            {
                var points = curve.Tessellate();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (points[i].TryMakeLineWith(points[i + 1], out var resultLine))
                    {
                        result.Add(resultLine);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsParalleWith(this Curve source, Curve target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (source is Line sourceLine && target is Line targetLine)
            {
                return sourceLine.Direction.IsParallerWith(targetLine.Direction);
            }

            if (source is Arc sourceArc && target is Arc targetArc)
            {
                if (sourceArc.Center.IsAlmostEqualTo(targetArc.Center))
                {
                    return sourceArc.Normal.IsAlmostEqualTo(targetArc.Normal) || sourceArc.Normal.IsAlmostEqualTo(-targetArc.Normal);
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="pt"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsContainExt(this Curve curve, XYZ pt, double tolerance = 1e-5)
        {
            if (curve is not Line)
            {
                return curve.Distance(pt).IsAlmostEqual(0, tolerance);
            }
            if (curve.IsBound)
            {
                return (pt.DistanceTo(curve.GetStartPoint()) + pt.DistanceTo(curve.GetEndPoint())).IsAlmostEqual(curve.Length, tolerance) && curve.Distance(pt).IsAlmostEqual(0.0, tolerance);
            }
            return curve.Distance(pt).IsAlmostEqual(0, tolerance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curves"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void SortCurvesContiguous(this IList<Curve> curves)
        {
            var sixteenth = 1d / 12d / 16d;
            if (curves is null)
            {
                throw new ArgumentNullException(nameof(curves));
            }
            var curveCount = curves.Count;
            for (int i = 0; i < curveCount; i++)
            {
                var curve = curves[i];
                var endPoint = curve.GetEndPoint();

                var found = i + 1 >= curveCount;
                for (int j = i + 1; j < curveCount; ++j)
                {
                    var point = curves[j].GetStartPoint();

                    if (point.DistanceTo(endPoint).IsGreatThan(sixteenth))
                    {
                        if (i + 1 != j)
                        {
                            var tempCurve = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tempCurve;
                        }
                        found = true;
                        break;
                    }
                    point = curves[j].GetEndPoint();
                    if (point.DistanceTo(endPoint).IsGreatThan(sixteenth))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = curves[j].CreateReversedCurve();
                        }
                        else
                        {
                            var tempCurve = curves[i + 1];
                            curves[i + 1] = curves[j].CreateReversedCurve();
                            curves[j] = tempCurve;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous:" + " non-contiguous input curves");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        private static Curve CreateReversedCurve(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            if (!(curve is Line || curve is Arc))
            {
                throw new NotImplementedException($"CreateReversedCurve for type {curve.GetType().Name}");
            }
            switch (curve)
            {
                case Line line:
                    return Line.CreateBound(line.GetEndPoint(), line.GetStartPoint());
                case Arc arc:
                    return Arc.Create(arc.GetEndPoint(), arc.GetStartPoint(), arc.Evaluate(0.5, true));
            }
            throw new Exception("CreateReversedCurve - Unreachable");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Curve GetLocationCurve(this Element element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (element.Location is LocationCurve locationCurve)
            {
                return locationCurve.Curve;
            }
            throw new ArgumentException($"{element.Name} is not a curve base element");
        }
    }
}
