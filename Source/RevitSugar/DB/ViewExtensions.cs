using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace RevitSugar.DB
{
    public static class ViewExtensions
    {

        /// <summary>
        /// 从视图获取平面
        /// </summary>
        /// <param name="view">要获取平面的视图</param>
        /// <returns>平面</returns>
        public static Plane GetPlaneFromView(this View view)
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
        }

        /// <summary>
        /// 获取平面视图的边界框
        /// </summary>
        /// <param name="view">要获取边界框的平面视图</param>
        /// <returns>边界框</returns>
        public static BoundingBoxXYZ GetPlanViewBox(this ViewPlan view)
        {
            var doc = view.Document;

            var range = view.GetViewRange();

            double topOffset = range.GetOffset(PlanViewPlane.TopClipPlane);
            var topId = range.GetLevelId(PlanViewPlane.TopClipPlane);
            if (doc.GetElement(topId) is Level topLevel)
            {
                topOffset += topLevel.Elevation;
            }
            else
            {
                topOffset = view.Origin.Z + 1500d.MmToFeet();
            }
            double max = topOffset;

            double depthOffset;
            var depthId = range.GetLevelId(PlanViewPlane.ViewDepthPlane);
            if (doc.GetElement(depthId) is Level depthLevel)
            {
                depthOffset = depthLevel.Elevation;
            }
            else
            {
                depthOffset = view.Origin.Z - 1500d.MmToFeet();
            }
            double min = depthOffset;

            var box = view.get_BoundingBox(view);
            box.Min = box.Min.Flat(min);
            box.Max = box.Max.Flat(max);
            return box;
        }
    }
}
