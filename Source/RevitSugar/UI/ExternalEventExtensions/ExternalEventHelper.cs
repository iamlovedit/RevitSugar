using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RevitSugar.UI.ExternalEventExtensions
{
    /// <summary>
    /// 外部事件封装类
    /// </summary>
    public class ExternalEventHelper
    {
        private static bool _initialized;
        private static ExternalEvent _externalEvent;
        private static ExternalEventContainer _container;

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            if (!_initialized)
            {
                try
                {
                    _container = new ExternalEventContainer();
                    _externalEvent = ExternalEvent.Create(_container);
                    _initialized = true;
                }
                catch (Exception e)
                {
                    Trace.Write(e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="externalName"></param>
        /// <returns></returns>
        public static ExternalEventRequest Invoke(Action<UIApplication> action, string externalName = null)
        {
            _container.Append(new KeyValuePair<string, Action<UIApplication>>(externalName ?? new Guid().ToString(), action));
            return _externalEvent.Raise();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Dispose()
        {
            if (_initialized)
            {
                _externalEvent.Dispose();
                _initialized = false;
            }
        }
    }
}
