using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="structural"></param>
        /// <returns></returns>
        Wall CreateWall(IList<Curve> profile, bool structural);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="levelId"></param>
        /// <param name="structural"></param>
        /// <returns></returns>
        Wall CreateWall(Curve curve, ElementId levelId, bool structural);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="wallTypeId"></param>
        /// <param name="levelId"></param>
        /// <param name="structural"></param>
        /// <returns></returns>
        Wall CreateWall(IList<Curve> profile, ElementId wallTypeId, ElementId levelId, bool structural);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="symbol"></param>
        /// <param name="host"></param>
        /// <param name="structuralType"></param>
        /// <returns></returns>
        FamilyInstance CreateDoor(XYZ location, FamilySymbol symbol, Element host, StructuralType structuralType = StructuralType.NonStructural);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="symbol"></param>
        /// <param name="host"></param>
        /// <param name="structuralType"></param>
        /// <returns></returns>
        FamilyInstance CreateWindow(XYZ location, FamilySymbol symbol, Element host, StructuralType structuralType = StructuralType.NonStructural);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="structural"></param>
        /// <returns></returns>
        Floor CreateFloor(CurveArray profile, bool structural);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <param name="structural"></param>
        /// <returns></returns>
        Floor CreateFloor(CurveArray profile, FloorType floorType, Level level, bool structural);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <param name="structural"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        Floor CreateFloor(CurveArray profile, FloorType floorType, Level level, bool structural, XYZ normal);
    }

}
