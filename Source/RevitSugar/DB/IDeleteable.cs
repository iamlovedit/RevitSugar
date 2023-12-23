using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeleteable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<ElementId> DeleteElements(IEnumerable<Element> elements);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        IEnumerable<ElementId> DeleteElement(Element element);
    }

    public class Deleteable : IDeleteable
    {
        private readonly Document _doc;

        public Deleteable(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        /// <inheritdoc/>
        public IEnumerable<ElementId> DeleteElement(Element element)
        {
            return _doc.Delete(element.Id).Select(i => i);
        }

        /// <inheritdoc/>
        public IEnumerable<ElementId> DeleteElements(IEnumerable<Element> elements)
        {
            var ids = elements.Select(i => i.Id).ToArray();
            return _doc.Delete(ids).Select(i => i);
        }
    }
}
