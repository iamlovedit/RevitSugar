using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> QueryElementsByClass<T>(View view = null, Func<T, bool> predicate = null) where T : Element;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builtInCategory"></param>
        /// <param name="view"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> QueryElementsByCategory<T>(BuiltInCategory builtInCategory, View view = null, Func<T, bool> predicate = null) where T : Element;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category"></param>
        /// <param name="view"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> QueryElementsByCategory<T>(Category category, View view = null, Func<T, bool> predicate = null) where T : Element;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementFilter"></param>
        /// <param name="view"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> QueryElementsByFilter<T>(ElementFilter elementFilter, View view = null, Func<T, bool> predicate = null) where T : Element;
    }
}
