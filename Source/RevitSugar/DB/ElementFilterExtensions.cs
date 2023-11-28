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
        private static FilteredElementCollector GetCollector(Document doc, View view = null)
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
        public static IEnumerable<T> GetElementsByClass<T>(this Document doc, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            using var collector = GetCollector(doc, view);
            var elements = collector.OfClass(typeof(T)).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
        }

        /// <summary>
        /// Collect elements by element's category, If view is not null,Only the visible elements of the current view 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="builtInCategory"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> GetElementsByCategory<T>(this Document doc, BuiltInCategory builtInCategory, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (builtInCategory == BuiltInCategory.INVALID)
            {
                throw new ArgumentException(nameof(builtInCategory));
            }

            using var collector = GetCollector(doc, view);
            var elements = collector.OfCategory(builtInCategory).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
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
        public static IEnumerable<T> GetElementsByCategory<T>(this Document doc, Category category, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            using var collector = GetCollector(doc, view);
            var elements = collector.OfCategoryId(category.Id).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
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
        public static IEnumerable<T> GetElementsByFilter<T>(this Document doc, ElementFilter filter, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            using var collector = GetCollector(doc, view);
            var elements = collector.WherePasses(filter).OfType<T>();
            return elements is null ? elements : elements.Where(predicate);
        }
    }
}
