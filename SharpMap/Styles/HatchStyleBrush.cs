using System;

namespace SharpMap.Styles
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HatchStyleBrush : SolidStyleBrush
    {
        /// <summary>
        /// Creates a hatch pattern brush, setting <see cref="HatchStyleBrush.Color"/> to <see cref="StyleColor.Black"/>, <see cref="BackColor"/> to <see cref="StyleColor.Transparent"/> and <see cref="HatchPattern"/> to <see cref="StyleHatchPattern.Default"/>.
        /// </summary>
        public HatchStyleBrush()
            : this(StyleColor.Black, StyleColor.Transparent, StyleHatchPattern.Default)
        {
        }

        /// <summary>
        /// Creates a hatch pattern brush
        /// </summary>
        /// <param name="color">The foreground color</param>
        /// <param name="backColor">The background color</param>
        /// <param name="hatchPattern">The hatch pattern</param>
        public HatchStyleBrush(StyleColor color, StyleColor backColor, StyleHatchPattern hatchPattern)
            :base(color)
        {
            BackColor = backColor;
            HatchPattern = hatchPattern;
        }
        /// <summary>
        /// Gets or sets the back color
        /// </summary>
        public StyleColor BackColor { get; set; }

        /// <summary>
        /// Gets or sets the hatch pattern
        /// </summary>
        public StyleHatchPattern HatchPattern { get; set; }

        public override String ToString()
        {
            return String.Format("[HatchStyleBrush] Color: {0}; BackColor {1}; HatchPattern: {2}", Color, BackColor, HatchPattern);
        }

    }
}