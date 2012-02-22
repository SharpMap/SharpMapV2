using System;
using SharpMap.Utilities;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Base class for all symbolizer classes
    /// </summary>
    [Serializable]
    public abstract class Symbolizer : NotificationObject, ISymbolizer
    {
        private bool _enabled = true;

        #region ISymbolizer Members

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (Enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }

        #endregion

    }
}