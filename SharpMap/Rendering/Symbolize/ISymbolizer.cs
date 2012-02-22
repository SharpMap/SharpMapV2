using System.ComponentModel;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Base interface for all symbolizer classes
    /// </summary>
    public interface ISymbolizer : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets whether the symbolizer is enabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}