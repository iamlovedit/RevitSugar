using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    public static class DocumentExtensions
    {
        /// <summary>
        /// 根据元素ID获取指定类型的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="doc">文档对象</param>
        /// <param name="id">元素ID</param>
        /// <returns>指定类型的元素</returns>
        public static T GetElement<T>(this Document doc, ElementId id) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (ElementId.InvalidElementId == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return doc.GetElement(id) as T;
        }

        /// <summary>
        /// 根据元素ID整数值获取指定类型的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="doc">文档对象</param>
        /// <param name="id">元素ID整数值</param>
        /// <returns>指定类型的元素</returns>
        public static T GetElement<T>(this Document doc, int id) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (ElementId.InvalidElementId.IntegerValue == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return doc.GetElement(new ElementId(id)) as T;
        }

        /// <summary>
        /// 根据元素GUID获取指定类型的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="doc">文档对象</param>
        /// <param name="guid">元素GUID</param>
        /// <returns>指定类型的元素</returns>
        public static T GetElement<T>(this Document doc, string guid) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }
            return doc.GetElement(guid) as T;
        }

        /// <summary>
        /// 根据引用获取指定类型的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="doc">文档对象</param>
        /// <param name="reference">引用对象</param>
        /// <returns>指定类型的元素</returns>
        public static T GetElement<T>(this Document doc, Reference reference) where T : Element
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            return doc.GetElement(reference) as T;
        }
    }
}
