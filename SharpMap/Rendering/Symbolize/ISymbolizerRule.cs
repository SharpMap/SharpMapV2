using System;
using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    ///<summary>
    /// Base interface for symbolizer rules
    ///</summary>
    public interface ISymbolizerRule : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets whether this rule is enabled.
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum zoom value where this rule has to be applied.
        /// </summary>
        Double MinVisible { get; set; }

        /// <summary>
        /// Gets or sets the maximum zoom value where this rule has to be applied.
        /// </summary>
        Double MaxVisible { get; set; }

        /// <summary>
        /// Function to evaluate the <paramref name="style"/> for a given <paramref name="record"/> and the <paramref name="phase"/> at which it is to be rendered. 
        /// </summary>
        /// <param name="record">The feature data record</param>
        /// <param name="phase">The rendering phase</param>
        /// <param name="style">The evaluated style</param>
        /// <returns><c>true</c> if the evaluation succeded</returns>
        bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out IStyle style);
    }
}