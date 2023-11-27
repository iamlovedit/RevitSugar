using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    public static class ElementFilterExtensions
    {
        /// <summary>
        /// Create a ElementCollector <see cref="Autodesk.Revit.DB.FilteredElementCollector"/>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        private static FilteredElementCollector GetCollector(Autodesk.Revit.DB.Document doc, View view = null)
        {
            return view == null ? new FilteredElementCollector(doc) : new FilteredElementCollector(doc, view.Id);
        }

        /// <summary>
        /// Collect elements by type of element, If view is not null,Only the visible elements of the current view 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> GetElementsByClass<T>(this Autodesk.Revit.DB.Document doc, View view = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            using (var collector = GetCollector(doc, view))
            {
                return collector.OfClass(typeof(T)).OfType<T>();
            }
        }

        /// <summary>
        /// Collect elements by category of element, If view is not null,Only the visible elements of the current view 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="builtInCategory"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> GetElementsByCategory<T>(this Autodesk.Revit.DB.Document doc, BuiltInCategory builtInCategory, View view = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (builtInCategory == BuiltInCategory.INVALID)
            {
                throw new ArgumentException(nameof(builtInCategory));
            }

            using (var collector = GetCollector(doc, view))
            {
                return collector.OfCategory(builtInCategory).OfType<T>();
            }
        }

        /// <summary>
        /// Collect elements by category of element, If view is not null,Only the visible elements of the current view 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> GetElementsByCategory<T>(this Autodesk.Revit.DB.Document doc, Category category, View view = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            using (var collector = GetCollector(doc, view))
            {
                return collector.OfCategoryId(category.Id).OfType<T>();
            }
        }

        /// <summary>
        /// Collect elements by a element filter, If view is not null,Only the visible elements of the current view 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> GetElementsByFilter<T>(this Autodesk.Revit.DB.Document doc, Autodesk.Revit.DB.ElementFilter filter, View view = null) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            using (var collector = GetCollector(doc, view))
            using (filter)
            {
                return collector.WherePasses(filter).OfType<T>();
            }

        }
    }
}
