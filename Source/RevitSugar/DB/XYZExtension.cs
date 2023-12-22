using Autodesk.Revit.DB;
using System;
using System.Diagnostics;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public static class XYZExtension
    {

        /// <summary>
        /// 判断三点是否共线
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="tolerance"></param>
        /// <returns>如果三点共线则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 判断两个向量是否垂直
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns>如果两个向量垂直则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 判断两个向量是否同向
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns>如果两个向量同向则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 判断两个向量是否平行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <param name="considerZeroVector"></param>
        /// <returns>如果两个向量平行则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 判断两个向量是否为相反的方向
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns>如果两个向量为相反的方向则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        ///  判断两个向量是否相等
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <param name="ingoreZ">是否考虑Z轴</param>
        /// <returns>如果两个向量相等则返回true，否则为false</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        public static bool IsAlmostEqualWith(this XYZ source, XYZ target, double tolerance = 1e-3, bool ingoreZ = false)
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
            if (ingoreZ)
            {
                return isEqual;
            }
            return isEqual && source.Z.IsAlmostEqual(target.Z, tolerance);
        }

        /// <summary>
        /// 获取两点之间的方向
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>返回两点之间的方向</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 将点拍平到某个z值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        public static XYZ Flat(this XYZ source, double z = 0)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new XYZ(source.X, source.Y, z);
        }

        /// <summary>
        /// 深拷贝一个XYZ
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        public static XYZ DeepClone(this XYZ source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new XYZ(source.X, source.Y, source.Z);
        }

        /// <summary>
        /// 判断是否可以投影到平面
        /// </summary>
        /// <param name="source"></param>
        /// <param name="face"></param>
        /// <param name="result"></param>
        /// <returns>如果可以投影返回true和投影结果</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取点到plane之间的距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 将点投影到平面
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取点在指定方向上与plane的交点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取点在平面上的投影点
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="xyz"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 尝试创建直线
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取点到曲线的距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static double GetDistanceFromCurve(this XYZ point, Curve curve)
        {
            return curve is Arc arc && arc.Center.DistanceTo(point).IsAlmostEqualZero(1e-3) ? arc.Radius : curve.Distance(point);
        }

        /// <summary>
        /// 获取两个向量的最小夹角
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 将向量在指定方向上偏移一定的距离
        /// </summary>
        /// <param name="source"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取垂直于直线的交点
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 将向量围绕一个方向旋转角度
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
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

        /// <summary>
        /// 获取图元的放置点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException"></exception>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentException"></exception>

        public static XYZ GetLocationPoint(this Element element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.Location is LocationPoint locationPoint)
            {
                return locationPoint.Point;
            }
            throw new ArgumentException($"{element.Name} is not a point base element");
        }
    }
}
