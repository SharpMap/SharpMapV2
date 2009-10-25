using System.ComponentModel;

namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizer : INotifyPropertyChanged
    {
        bool Enabled { get; set; }
    }
}