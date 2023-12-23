using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class Queryable : IQueryable
    {
        private readonly Document _doc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Queryable(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryElementsByCategory<T>(BuiltInCategory builtInCategory, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            var collector = GetCollector(_doc, view);
            var elements = collector.OfCategory(builtInCategory).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryElementsByCategory<T>(Category category, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var collector = GetCollector(_doc, view);
            var elements = collector.OfCategoryId(category.Id).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryElementsByClass<T>(View view = null, Func<T, bool> predicate = null) where T : Element
        {
            var collector = GetCollector(_doc, view);
            var elements = collector.OfClass(typeof(T)).OfType<T>();
            return predicate is null ? elements : elements.Where(predicate);
        }
        /// <inheritdoc/>
        public IEnumerable<T> QueryElementsByFilter<T>(ElementFilter elementFilter, View view = null, Func<T, bool> predicate = null) where T : Element
        {
            if (elementFilter is null)
            {
                throw new ArgumentNullException(nameof(elementFilter));
            }

            var collector = GetCollector(_doc, view);
            var elements = collector.WherePasses(elementFilter).OfType<T>();
            return elements is null ? elements : elements.Where(predicate);
        }

        private FilteredElementCollector GetCollector(Document doc, View view = null)
        {
            return view == null ? new FilteredElementCollector(doc) : new FilteredElementCollector(doc, view.Id);
        }
    }
}
