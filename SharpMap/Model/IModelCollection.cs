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
using System.ComponentModel;
using System.Text;

namespace SharpMap
{
    public interface IModelCollection<TItem> : IList<TItem>
    {
        event EventHandler<ModelCollectionChangedEventArgs<TItem>> CollectionChanged;
    }

    public class ModelCollectionChangedEventArgs<TItem> : EventArgs
	{
        private readonly CollectionChangeAction _action;
        private readonly List<TItem> _elements;

        public ModelCollectionChangedEventArgs(IEnumerable<TItem> elements, CollectionChangeAction action)
        {
            _elements = new List<TItem>(elements);
            _action = action;
        }

        public IList<TItem> Elements
        {
            get { return _elements; }
        }

	    public CollectionChangeAction Action
	    {
		    get { return _action;}
	    }
	}
}
