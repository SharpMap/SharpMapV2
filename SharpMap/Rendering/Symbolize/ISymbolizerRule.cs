using System;
using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizerRule : INotifyPropertyChanged
    {
        bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the minimum zoom value where the style is applied.
        /// </summary>
        Double MinVisible { get; set; }

        /// <summary>
        /// Gets or sets the maximum zoom value where the style is applied.
        /// </summary>
        Double MaxVisible { get; set; }

        bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out IStyle style);
    }
}