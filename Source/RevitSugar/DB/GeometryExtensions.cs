using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RevitSugar.DB
{
    public static class GeometryExtensions
    {
        public static Solid MergeSolids(this IList<Solid> solids)
        {
            if (solids is null)
            {
                throw new ArgumentNullException(nameof(solids));
            }
            if (solids.Any() && solids.Count == 1)
            {
                return solids.First();
            }
            Solid result = null;
            for (int i = 0; i < solids.Count - 1; i++)
            {
                if (i == 0)
                {
                    result = BooleanOperationsUtils.ExecuteBooleanOperation(solids[i], solids[i + 1], BooleanOperationsType.Union);
                }
                else
                {
                    BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(result, solids[i + 1], BooleanOperationsType.Union);
                }
            }
            return result;
        }

        public static IList<Solid> GetSolids(this Element element, Options options, bool getFamilyInstanceOriginGeometry = false, Predicate<Solid> predicate = null)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var solidsList = new List<Solid>();
            try
            {
                GeometryElement geometryElement;
                if (getFamilyInstanceOriginGeometry && element is FamilyInstance familyInstance)
                {
                    geometryElement = familyInstance.get_Geometry(options);
                    using var transform = familyInstance.GetTransform();
                    if (!transform.IsIdentity)
                    {
                        geometryElement = geometryElement.GetTransformed(transform);
                    }
                }
                else
                {
                    geometryElement = element.get_Geometry(options);
                }
                if (geometryElement is null)
                {
                    return solidsList;
                }
                foreach (var geoObj in geometryElement)
                {
                    solidsList.AddRange(geoObj.GetSolids(predicate));
                }
                return solidsList;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return solidsList;
            }
        }

        public static IList<Solid> GetSolids(this GeometryObject geoObject, Predicate<Solid> predicate = null)
        {
            if (geoObject is null)
            {
                throw new ArgumentNullException(nameof(geoObject));
            }

            var solidList = new List<Solid>();
            switch (geoObject)
            {
                case Solid solid:
                    if (predicate is null)
                    {
                        solidList.Add(solid);
                    }
                    else
                    {
                        if (predicate(solid))
                        {
                            solidList.Add(solid);
                        }
                    }
                    break;
                case GeometryInstance geometryInstance:
                    var enumerator = geometryInstance.GetInstanceGeometry().GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        solidList.AddRange(GetSolids(enumerator.Current, predicate));
                    }
                    break;
                case GeometryElement geometryElement:
                    var enumerator1 = geometryElement.GetEnumerator();
                    enumerator1.Reset();
                    while (enumerator1.MoveNext())
                    {
                        solidList.AddRange(GetSolids(enumerator1.Current, predicate));
                    }
                    break;
            }
            return solidList;
        }

        public static IList<Face> GetFacesByCondition(this Element element, Options options, Func<Face, bool> condition, bool getFamilyInstanceOriginGeometry = false)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var solids = element.GetSolids(options, getFamilyInstanceOriginGeometry);

            return (from solid in solids from Face face in solid.Faces where condition.Invoke(face) select face).ToList();
        }

        public static Face GetPickedFace(this Document document, Reference pickedReference)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (pickedReference is null)
            {
                throw new ArgumentNullException(nameof(pickedReference));
            }
            var uvPoint = pickedReference.UVPoint;
            var hostElement = document.GetElement(pickedReference.ElementId);
            using (var options = new Options() { ComputeReferences = true })
            {
                foreach (var solid in hostElement.GetSolids(options))
                {
                    foreach (Face face in solid.Faces)
                    {
                        var result = face.Project(pickedReference.GlobalPoint);
                        if (face.IsInside(pickedReference.UVPoint) && (result?.Distance.IsAlmostEqualZero() ?? false))
                        {
                            return face;
                        }
                    }
                }
            }
            return hostElement.GetGeometryObjectFromReference(pickedReference) as Face;
        }

        public static double GetVolume(this Solid solid)
        {
            if (solid is null)
            {
                throw new ArgumentNullException(nameof(solid));
            }
            if (solid.Faces is null || solid.Faces.Size == 0)
            {
                return -1;
            }
            return solid.Volume;
        }

        public static IList<Edge> GetEdgesFromFace(this Face face)
        {
            if (face is null)
            {
                throw new ArgumentNullException(nameof(face));
            }
            var edges = new List<Edge>();
            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }

        public static Plane GetPlaneFromFace(this Face face)
        {
            if (face is null)
            {
                throw new ArgumentNullException(nameof(face));
            }
            if (face is PlanarFace planarFace)
            {
                return Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
            }
            return null;
        }

        public static void ExtendByPoint(this BoundingBoxXYZ boundingBox, XYZ point)
        {
            if (boundingBox is null)
            {
                throw new ArgumentNullException(nameof(boundingBox));
            }

            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            boundingBox.Min = new XYZ(Math.Min(boundingBox.Min.X, point.X),
                                      Math.Min(boundingBox.Min.Y, point.Y),
                                      Math.Min(boundingBox.Min.Z, point.Z));
            boundingBox.Max = new XYZ(Math.Max(boundingBox.Max.X, point.X),
                                      Math.Max(boundingBox.Max.Y, point.Y),
                                      Math.Min(boundingBox.Max.Z, point.Z));
        }

        public static void ExtendByAnother(this BoundingBoxXYZ source, BoundingBoxXYZ target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            source.ExtendByPoint(target.Min);
            source.ExtendByPoint(target.Max);
        }

        public static BoundingBoxXYZ GetMaxBoundingBox(this IEnumerable<Element> elements, View view = null)
        {
            var boundingBox = new BoundingBoxXYZ();
            foreach (var elem in elements)
            {
                using var box = elem.get_BoundingBox(view);
                if (box != null)
                {
                    boundingBox.ExtendByAnother(box);
                }
            }
            return boundingBox;
        }
    }
}
