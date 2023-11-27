using Autodesk.Revit.DB;
using System;
using System.Diagnostics;

namespace RevitSugar.DB
{
    public static class XYZExtension
    {
        public static bool AreThreePointsCollinear(this XYZ point1, XYZ point2, XYZ point3, double tolerance = 1e-6)
        {
            if (point1 is null)
            {
                throw new ArgumentNullException(nameof(point1));
            }

            if (point2 is null)
            {
                throw new ArgumentNullException(nameof(point2));
            }

            if (point3 is null)
            {
                throw new ArgumentNullException(nameof(point3));
            }

            var x1 = point1.X;
            var x2 = point2.X;
            var x3 = point3.X;
            var y1 = point1.Y;
            var y2 = point2.Y;
            var y3 = point3.Y;
            return Math.Abs((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1)) <= tolerance;
        }

        public static bool IsVerticalWith(this XYZ source, XYZ target, double tolerance = 1e-6)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return Math.Abs(source.DotProduct(target)) <= Math.Abs(tolerance);
        }


        public static bool IsSameDirectionWith(this XYZ source, XYZ target, double tolerance = 1e-6)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.AngleTo(target) <= Math.Abs(tolerance);
        }


        public static bool IsParallerWith(this XYZ source, XYZ target, double tolerance = 1e-6, bool considerZeroVector = false)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return (considerZeroVector && (source.IsZeroLength() || target.IsZeroLength())) ||
                source.IsSameDirectionWith(target, tolerance) ||
                source.IsOppositeDirectionWith(target, tolerance);
        }

        public static bool IsOppositeDirectionWith(this XYZ source, XYZ target, double tolerance = 1e-6)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return source.AngleTo(target.Negate()) <= Math.Abs(tolerance);
        }

        public static bool IsAlmostEqualWith(this XYZ source, XYZ target, double tolerance = 1e-3, bool isIngoreZ = false)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var isEqual = source.X.IsAlmostEqual(target.X, tolerance) && source.Y.IsAlmostEqual(target.Y, tolerance);
            if (isIngoreZ)
            {
                return isEqual;
            }
            return isEqual && source.Z.IsAlmostEqual(target.Z, tolerance);
        }

        public static XYZ GetDirectionWith(this XYZ source, XYZ target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return (target - source).Normalize();
        }


        public static XYZ Flat(this XYZ source, double z = 0)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new XYZ(source.X, source.Y, z);
        }


        public static XYZ DeepClone(this XYZ source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new XYZ(source.X, source.Y, source.Z);
        }


        public static bool CanProjectToFace(this XYZ source, Face face, out IntersectionResult result)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (face is null)
            {
                throw new ArgumentNullException(nameof(face));
            }
            result = face.Project(source);
            return result != null;
        }


        public static double GetDistanceToPlane(this XYZ point, Plane plane)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (plane is null)
            {
                throw new ArgumentNullException(nameof(plane));
            }
            var distance = plane.Normal.DotProduct(plane.Origin);
            return plane.Normal.DotProduct(point) - distance;
        }


        public static XYZ ProjectToPlane(this XYZ point, Plane plane)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (plane is null)
            {
                throw new ArgumentNullException(nameof(plane));
            }
            return point - plane.Normal * point.GetDistanceToPlane(plane);
        }

        public static XYZ GetIntersertWithPlane(this XYZ point, XYZ direction, Plane plane)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (direction is null)
            {
                throw new ArgumentNullException(nameof(direction));
            }

            if (plane is null)
            {
                throw new ArgumentNullException(nameof(plane));
            }

            if (Math.Abs(direction.DotProduct(plane.Normal)) <= 1e-6)
            {
                return null;
            }
            var number = plane.Normal.DotProduct(plane.Origin - point) / plane.Normal.DotProduct(direction);
            return point + number * direction;
        }

        public static XYZ GetProjectPoint(this Plane plane, XYZ xyz)
        {
            if (plane is null)
            {
                throw new ArgumentNullException(nameof(plane));
            }

            if (xyz is null)
            {
                throw new ArgumentNullException(nameof(xyz));
            }
            var transform = Transform.Identity;
            transform.BasisX = plane.XVec;
            transform.BasisY = plane.YVec;
            transform.BasisZ = plane.Normal;
            transform.Origin = plane.Origin;
            var point = transform.Inverse.OfPoint(xyz);
            point = point.Flat();
            return transform.OfPoint(point);
        }

        public static bool TryMakeLineWith(this XYZ source, XYZ target, out Line line)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            line = null;
            try
            {
                line = Line.CreateBound(source, target);
                return line != null;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }


        public static double GetDistanceFromCurve(this XYZ point, Curve curve)
        {
            return curve is Arc arc && arc.Center.DistanceTo(point).IsAlmostEqualZero(1e-3) ? arc.Radius : curve.Distance(point);
        }

        public static double GetMinAngleWithVector(this XYZ source, XYZ target)
        {
            var angle = Math.Abs(source.AngleTo(target));
            if (angle <= Math.PI * 0.5)
            {
                return angle;
            }
            if (angle <= Math.PI)
            {
                return Math.Abs(Math.PI - angle);
            }
            if (angle <= Math.PI * 1.5)
            {
                return angle - Math.PI;
            }
            return Math.Abs(Math.PI * 2 - angle);
        }

        public static XYZ Offset(this XYZ source, XYZ direction, double distance)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (direction is null)
            {
                throw new ArgumentNullException(nameof(direction));
            }
            return source + direction * distance;
        }

        public static XYZ GetVerticalIntersection(this XYZ source, Line line)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            var normal = line.Direction.CrossProduct(XYZ.BasisZ);
            using var tempLine = Line.CreateUnbound(source, normal);
            var result = tempLine.Intersect(line, out var resultArray);
            if (result != SetComparisonResult.Overlap)
            {
                return default;
            }
            if (resultArray is null || resultArray.Size != 1)
            {
                return default;
            }
            return resultArray.get_Item(0).XYZPoint;
        }

        public static XYZ GetPointToLineVecIntersection(this XYZ point, Line line)
        {
            var normal = line.Direction.Normalize();
            var cross = normal.CrossProduct(XYZ.BasisZ);
            Line crossLine = Line.CreateUnbound(point, cross);
            SetComparisonResult result = line.Intersect(crossLine, out IntersectionResultArray results);
            if (result != SetComparisonResult.Overlap)
            {
                return line.GetEndPoint();
            }
            if (results == null || results.Size != 1)
            {
                return line.GetEndPoint();
            }
            return results.get_Item(0).XYZPoint;
        }


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

        public static XYZ Rotate(this XYZ vector, XYZ axis, double angle)
        {
            if (vector is null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            if (axis is null)
            {
                throw new ArgumentNullException(nameof(axis));
            }
            return Transform.CreateRotation(axis, angle).OfVector(vector);
        }

    }
}
