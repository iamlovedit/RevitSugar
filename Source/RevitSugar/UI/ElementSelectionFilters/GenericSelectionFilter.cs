using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace RevitSugar.UI.ElementSelectionFilters
{
    /// <summary>
    /// 通用选择过滤器
    /// </summary>
    public class GenericSelectionFilter : ISelectionFilter
    {
        private readonly Predicate<Element> _elementPredicate;

        private readonly Predicate<Reference> _referencePredicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementPredicate"></param>
        /// <param name="referencePredicate"></param>
        public GenericSelectionFilter(Predicate<Element> elementPredicate = null, Predicate<Reference> referencePredicate = null)
        {
            _elementPredicate = elementPredicate;
            _referencePredicate = referencePredicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            return _elementPredicate?.Invoke(elem) ?? true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return _referencePredicate?.Invoke(reference) ?? true;
        }
    }
}
