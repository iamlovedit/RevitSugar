using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUpdateable
    {

    }

    public class Updateable : IUpdateable
    {
        private readonly Document _doc;

        public Updateable(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }


    }
}
