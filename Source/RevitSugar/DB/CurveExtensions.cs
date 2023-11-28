using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    public static class CurveExtensions
    {
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

            using (var curve = source.Clone())
            {
                curve.MakeUnbound();
                return curve.Distance(target.GetEndPoint(0)) <= tolerance && curve.Distance(target.GetEndPoint(1)) <= tolerance;
            }
        }

        public static XYZ GetMiddlePoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.Evaluate(0.5, true);
        }

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

        public static XYZ GetStartPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(0);
        }

        public static XYZ GetEndPoint(this Curve curve)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }
            return curve.GetEndPoint(1);
        }

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
