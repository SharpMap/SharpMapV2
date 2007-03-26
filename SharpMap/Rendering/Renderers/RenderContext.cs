// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

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
