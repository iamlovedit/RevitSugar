using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public static class CurveExtensions
    {
        /// <summary>
        /// 获取给定曲线与目标曲线的交点的XYZ点集合。
        /// </summary>
        /// <param name="curve">要查找交点的曲线。</param>
        /// <param name="target">要查找交点的目标曲线。</param>
        /// <returns>表示曲线与目标曲线交点的XYZ点的IEnumerable集合。</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">当曲线或目标为null时引发异常。</exception>
        /// <remarks>
        /// 该函数使用Intersect方法检查给定曲线和目标曲线是否相交。
        /// 如果结果为SetComparisonResult.Overlap，则从resultArray中提取XYZ点。
        /// 返回表示交点的XYZ点集合。
        /// </remarks>
        public static IEnumerable<XYZ> GetCrossPoints(this Curve curve, Curve target)
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
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果源曲线或目标曲线为null，则引发异常。</exception>
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
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果源圆弧或目标圆弧为null，则引发异常。</exception>
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
        ///  获取曲线的起点
        /// </summary>
        /// <param name="curve">用于获取的曲线</param>
        /// <returns>曲线的起点</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果曲线null，引发异常。</exception>
        public static XYZ GetStartPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(0);
        }

        /// <summary>
        /// 获取曲线的终点
        /// </summary>
        /// <param name="curve">用于获取的曲线</param>
        /// <returns>曲线的终点</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果曲线null，引发异常。</exception>
        public static XYZ GetEndPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(1);
        }

        /// <summary>
        /// 判断直线是否与目标直线平行
        /// </summary>
        /// <param name="source">要检查的直线</param>
        /// <param name="target">用于检查的直线</param>
        /// <returns>两直线平行返回true，否则返回false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果源直线或目标直线为null，引发异常</exception>
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
            var sourceVector = source.GetEndPoint() - source.GetStartPoint();
            var targetVector = target.GetEndPoint() - target.GetStartPoint();
            return sourceVector.IsParallerWith(targetVector);
        }

        /// <summary>
        /// 获取目标曲线的Outline
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="extendZ">是否设置扩展Z方向</param>
        /// <returns>返回Outline </returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果曲线null，引发异常。</exception>
        public static Outline GetOutline(this Curve curve, bool extendZ = false)
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
            if (extendZ)
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
        /// 获取曲线中的直线组
        /// </summary>
        /// <param name="curve">目标曲线</param>
        /// <returns>返回直线列表</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果曲线null，引发异常。</exception>
        public static IList<Line> GetLines(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            var lines = new List<Line>();
            if (curve is Line line)
            {
                lines.Add(line);
            }
            else if (curve.IsBound)
            {
                var points = curve.Tessellate();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (points[i].TryMakeLineWith(points[i + 1], out var resultLine))
                    {
                        lines.Add(resultLine);
                    }
                }
            }
            return lines;
        }

        /// <summary>
        /// 两线是否平行
        /// </summary>
        /// <param name="source">用于检验的线</param>
        /// <param name="target">目标曲线</param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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
        /// 点是否位于线上
        /// </summary>
        /// <param name="curve">线</param>
        /// <param name="point">用于检测的点</param>
        /// <param name="tolerance">容差</param>
        /// <returns>如果点在线上则返回true，否则为false</returns>
        public static bool ContainsPoint(this Curve curve, XYZ point, double tolerance = 1e-5)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (curve is not Line)
            {
                return curve.Distance(point).IsAlmostEqualZero(tolerance);
            }
            if (curve.IsBound)
            {
                return (point.DistanceTo(curve.GetStartPoint()) + point.DistanceTo(curve.GetEndPoint()))
                    .IsAlmostEqual(curve.Length, tolerance) && curve.Distance(point).IsAlmostEqualZero(tolerance);
            }
            return curve.Distance(point).IsAlmostEqualZero(tolerance);
        }

        /// <summary>
        /// 将持续的线排序
        /// </summary>
        /// <param name="curves">线的列表</param>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentException"></exception>
        public static void SortCurvesContiguous(this IList<Curve> curves)
        {
            if (curves is null)
            {
                throw new ArgumentNullException(nameof(curves));
            }
            var sixteenth = 1d / 12d / 16d;
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
                    throw new ArgumentException("SortCurvesContiguous:" + " non-contiguous input curves");
                }
            }
        }

        /// <summary>
        /// 获取与线方向相反的线
        /// </summary>
        /// <param name="curve">目标线</param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果线为null，已发异常</exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentException"></exception>
        private static Curve CreateReversedCurve(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            if (curve is not Line || curve is not Arc)
            {
                throw new NotImplementedException($"CreateReversedCurve for type {curve.GetType().Name}");
            }
            return curve switch
            {
                Line line => Line.CreateBound(line.GetEndPoint(), line.GetStartPoint()),
                Arc arc => Arc.Create(arc.GetEndPoint(), arc.GetStartPoint(), arc.Evaluate(0.5, true)),
                _ => throw new ArgumentException("CreateReversedCurve - Unreachable"),
            };
        }

        /// <summary>
        /// 获取图元的位置线
        /// </summary>
        /// <param name="element">目标图元</param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果图元为null，则引发异常</exception>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentException">如果图元不是基于线的则会引发异常</exception>
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

        /// <summary>
        /// 将直线拍平到某个高度
        /// </summary>
        /// <param name="line"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        public static Line Flatten(this Line line, double elevation = 0)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            var startPoint = line.GetStartPoint().Flat(elevation);
            var endPoint = line.GetEndPoint().Flat(elevation);
            return Line.CreateBound(startPoint, endPoint);
        }

        /// <summary>
        /// 将曲线偏移一定的高度
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        public static Curve OffsetVertical(this Curve curve, double elevation)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            var midPoint = curve.GetMiddlePoint();
            var transform = Transform.CreateTranslation(midPoint.Flat(elevation) - midPoint);
            return curve.CreateTransformed(transform);
        }
    }
}
