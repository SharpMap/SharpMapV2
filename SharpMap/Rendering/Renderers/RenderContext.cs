using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public class RenderContext : IRenderContext
    {
        Dictionary<ContextItemKey, object> _contextCache = new Dictionary<ContextItemKey, object>();

        #region IRenderContext Members

        public void Add<TContextItem>(TContextItem item)
        {
            Add(item, null);
        }

        public void Add<TContextItem>(TContextItem item, string id)
        {
            ContextItemKey key = new ContextItemKey(typeof(TContextItem), id);
            _contextCache[key] = item;
        }

        public TContextItem Get<TContextItem>()
        {
            return Get<TContextItem>(null);
        }

        public TContextItem Get<TContextItem>(string id)
        {
            ContextItemKey key = new ContextItemKey(typeof(TContextItem), id);
            object item = null;
            _contextCache.TryGetValue(key, out item);
            if (item is TContextItem)
                return (TContextItem)item;
            else
                return default(TContextItem);
        }

        #endregion
    }
}
