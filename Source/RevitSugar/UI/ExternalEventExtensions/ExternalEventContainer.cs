using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace RevitSugar.UI.ExternalEventExtensions
{
    internal class ExternalEventContainer : IExternalEventHandler
    {
        private readonly object _locker = new();
        private string _currentName = string.Empty;
        private readonly ConcurrentQueue<KeyValuePair<string, Action<UIApplication>>> _eventQueue = new();

        public void Append(KeyValuePair<string, Action<UIApplication>> pair)
        {
            lock (_locker)
            {
                _eventQueue.Enqueue(pair);
            }
        }

        public void Execute(UIApplication app)
        {
            while (_eventQueue.Count > 0)
            {
                try
                {
                    if (_eventQueue.TryDequeue(out var pair))
                    {
                        _currentName = pair.Key;
                        pair.Value?.Invoke(app);
                    }
                }
                catch (Exception e)
                {
                    Trace.Write(e);
                }
            }
        }

        public string GetName()
        {
            return _currentName ?? Guid.NewGuid().ToString();
        }
    }
}
