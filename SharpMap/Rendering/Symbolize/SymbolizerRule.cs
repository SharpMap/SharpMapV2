using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class SymbolizerRule : ISymbolizerRule
    {
        private bool _enabled;
        private double _maxVisible = double.PositiveInfinity;
        private double _minVisible = 0.0;

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

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}