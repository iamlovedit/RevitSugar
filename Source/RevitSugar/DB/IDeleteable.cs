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
        ICollection<ElementId> DeleteElements(IEnumerable<Element> elements);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        ICollection<ElementId> DeleteElement(Element element);

        
    }

    public class Deleteable : IDeleteable
    {
        private readonly Document _doc;

        public Deleteable(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        /// <inheritdoc/>
        public ICollection<ElementId> DeleteElement(Element element)
        {
            return _doc.Delete(element.Id);
        }

        /// <inheritdoc/>
        public ICollection<ElementId> DeleteElements(IEnumerable<Element> elements)
        {
            var ids = elements.Select(i => i.Id).ToArray();
            return _doc.Delete(ids);
        }
    }
}
