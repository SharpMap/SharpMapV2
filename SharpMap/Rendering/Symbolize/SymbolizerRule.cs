using System;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Rendering.Symbolize
{
    ///<summary>
    /// abstract base class for all classes implementing <see cref="ISymbolizerRule"/>
    ///</summary>
    [Serializable]
    public abstract class SymbolizerRule : NotificationObject, ISymbolizerRule
    {
        private bool _enabled = true;
        private double _maxVisible = double.PositiveInfinity;
        private double _minVisible;

        #region ISymbolizerRule Members

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

        public double MinVisible
        {
            get { return _minVisible; }
            set
            {
                if (MinVisible != value)
                {
                    _minVisible = value;
                    OnPropertyChanged("MinVisible");
                }
            }
        }

        public double MaxVisible
        {
            get { return _maxVisible; }
            set
            {
                if (_maxVisible != value)
                {
                    _maxVisible = value;
                    OnPropertyChanged("MaxVisible");
                }
            }
        }

        public abstract bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out IStyle style);

        #endregion

    }
}