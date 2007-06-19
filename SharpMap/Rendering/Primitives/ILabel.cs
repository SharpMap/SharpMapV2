using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Rendering
{
	public interface ILabel<TViewPoint, TViewRectangle, TGraphicsPath>
		where TViewPoint : IViewVector
		where TViewRectangle : IViewMatrix
		where TGraphicsPath : GraphicsPath<TViewPoint, TViewRectangle>
	{
		/// <summary>
		/// The text of the label.
		/// </summary>
		string Text { get; set; }

		/// <summary>
		/// Label position.
		/// </summary>
		TViewPoint LabelPoint { get; set; }

		/// <summary>
		/// Label font.
		/// </summary>
		StyleFont Font { get; set; }

		/// <summary>
		/// Label rotation.
		/// </summary>
		float Rotation { get; set; }

		/// <summary>
        /// Priority in layout.
		/// </summary>
		int Priority { get; set; }

		/// <summary>
		/// Label collision bounds.
		/// </summary>
		TViewRectangle CollisionBounds { get; set; }

		/// <summary>
		/// Path along which label runs.
		/// </summary>
		GraphicsPath<TViewPoint, TViewRectangle> FlowPath { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="SharpMap.Styles.LabelStyle"/> of this label.
		/// </summary>
		LabelStyle Style { get; set; }
	}
}
