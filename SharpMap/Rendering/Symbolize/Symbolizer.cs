using System.ComponentModel;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class Symbolizer : ISymbolizer
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

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}