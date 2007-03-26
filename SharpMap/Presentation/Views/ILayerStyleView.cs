using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public interface ILayerStyleView
    {
        event EventHandler<LayerStyleChangedEventArgs> LayerStyleChanged;
        Style Style { get; set; }
    }

    public class LayerStyleChangedEventArgs : EventArgs
    {
        private Style _style;

        public Style Style
        {
            get { return _style; }
            set { _style = value; }
        }
	
    }
}
