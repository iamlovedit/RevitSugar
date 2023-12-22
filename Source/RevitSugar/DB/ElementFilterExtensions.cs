using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public static class ElementFilterExtensions
    {
        /// <summary>
        /// 创建一个 ElementCollector <see cref="Autodesk.Revit.DB.FilteredElementCollector"/>。
        /// </summary>
        /// <param name="doc">文档对象。</param>
        /// <param name="view">视图对象（可选）。</param>
        /// <returns>返回一个 ElementCollector 对象。</returns>
        private static FilteredElementCollector GetCollector(Document doc, View view = null)
        {
            return view == null ? new FilteredElementCollector(doc) : new FilteredElementCollector(doc, view.Id);
        }

        /// <summary>
        /// 根据元素类型收集元素，如果 view 不为空，则只收集当前视图中可见的元素。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="doc">文档对象。</param>
        /// <param name="view">视图对象（可选）。</param>
        /// <param name="predicate"></param>
        /// <returns>返回一个元素集合。</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果 doc 参数为空，则抛出此异常。</exception>
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
        /// 根据元素的类别收集元素，如果 view 不为空，则只收集当前视图中可见的元素。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="doc">文档对象。</param>
        /// <param name="builtInCategory">内置类别。</param>
        /// <param name="view">视图对象（可选）。</param>
        /// <param name="predicate"></param>
        /// <returns>返回一个元素集合。</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果 doc 参数为空，则抛出此异常。</exception>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentException">如果 builtInCategory 参数为 BuiltInCategory.INVALID，则抛出此异常。</exception>
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
        /// 根据元素的类别收集元素，如果 view 不为空，则只收集当前视图中可见的元素。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="doc">文档对象。</param>
        /// <param name="category">元素类别。</param>
        /// <param name="view">视图对象（可选）。</param>
        /// <param name="predicate"></param> 
        /// <returns>返回一个元素集合。</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果 doc 参数为空，则抛出此异常。</exception>
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
        /// 根据元素过滤器收集元素，如果 view 不为空，则只收集当前视图中可见的元素。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="doc">文档对象。</param>
        /// <param name="filter">元素过滤器。</param>
        /// <param name="view">视图对象（可选）。</param>
        /// <param name="predicate"></param> 
        /// <returns>返回一个元素集合。</returns>
        /// <exception cref="Autodesk.Revit.Exceptions.ArgumentNullException">如果 doc 参数为空，则抛出此异常。</exception>
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
