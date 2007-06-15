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
