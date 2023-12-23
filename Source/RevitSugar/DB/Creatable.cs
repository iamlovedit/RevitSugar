using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    internal class Creatable : ICreatable
    {
        private readonly Document _doc;
        public Creatable(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public FamilyInstance CreateDoor(XYZ location, FamilySymbol symbol, Element host, StructuralType structuralType = StructuralType.NonStructural)
        {
            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            return _doc.Create.NewFamilyInstance(location, symbol, host, structuralType);
        }

        public Floor CreateFloor(CurveArray profile, bool structural)
        {
            throw new NotImplementedException();
        }

        public Floor CreateFloor(CurveArray profile, FloorType floorType, Level level, bool structural)
        {
            throw new NotImplementedException();
        }

        public Floor CreateFloor(CurveArray profile, FloorType floorType, Level level, bool structural, XYZ normal)
        {
            throw new NotImplementedException();
        }

        public Wall CreateWall(IList<Curve> profile, bool structural)
        {
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return Wall.Create(_doc, profile, structural);
        }

        public Wall CreateWall(Curve curve, ElementId levelId, bool structural)
        {
            if (curve is null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (levelId is null)
            {
                throw new ArgumentNullException(nameof(levelId));
            }

            return Wall.Create(_doc, curve, levelId, structural);
        }

        public Wall CreateWall(IList<Curve> profile, ElementId wallTypeId, ElementId levelId, bool structural)
        {
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (wallTypeId is null)
            {
                throw new ArgumentNullException(nameof(wallTypeId));
            }

            if (levelId is null)
            {
                throw new ArgumentNullException(nameof(levelId));
            }

            return Wall.Create(_doc, profile, wallTypeId, levelId, structural);
        }

        public FamilyInstance CreateWindow(XYZ location, FamilySymbol symbol, Element host, StructuralType structuralType = StructuralType.NonStructural)
        {
            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            return _doc.Create.NewFamilyInstance(location, symbol, host, structuralType);
        }
    }

}
