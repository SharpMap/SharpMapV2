// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;

namespace SharpMap.Styles
{
	/// <summary>
	/// Defines a style used for rendering vector data
	/// </summary>
	public abstract class FeatureStyle : Style
	{
		#region Private fields

		private StylePen _lineStyle;
		private StylePen _highlightLineStyle;
		private StylePen _selectLineStyle;
		private Boolean _outline;
		private StylePen _outlineStyle;
		private StylePen _highlightOutlineStyle;
		private StylePen _selectOutlineStyle;
		private StyleBrush _fillStyle;
		private StyleBrush _highlightFillStyle;
		private StyleBrush _selectionFillStyle;
		private StyleRenderingMode _smoothingMode;
		private Boolean _areFeaturesSelectable = true;
		private string _labelFormatExpression;

		#endregion

		/// <summary>
		/// Initializes a new VectorStyle with default values.
		/// </summary>
		/// <remarks>
		/// Default style values when initialized:<br/>
		/// <list type="table">
		/// <item>
		/// <term>AreFeaturesSelectable</term>
		/// <description>True</description>
		/// </item>
		/// <item>
		/// <term>LineStyle</term>
		/// <description>1px solid black</description>
		/// </item>
		/// <item>
		/// <term>FillStyle</term>
		/// <description>Solid black</description>
		/// </item>
		/// <item>
		/// <term>Outline</term>
		/// <description>No Outline</description>
		/// </item>
		/// <item>
		/// <term>Symbol</term>
		/// <description>Null reference (uses the geometry renderer default)</description>
		/// </item>
		/// </list>
		/// </remarks>
		protected FeatureStyle()
		{
			Outline = new StylePen(StyleColor.Black, 1);
			Line = new StylePen(StyleColor.Black, 1);
			Fill = new SolidStyleBrush(StyleColor.Black);
			EnableOutline = false;
		}

		#region Properties

		/// <summary>
		/// Linestyle for line geometries
		/// </summary>
		public StylePen Line
		{
			get { return _lineStyle; }
			set { _lineStyle = value; }
		}

		/// <summary>
		/// Highlighted line style for line geometries
		/// </summary>
		public StylePen HighlightLine
		{
			get { return _highlightLineStyle; }
			set { _highlightLineStyle = value; }
		}

		/// <summary>
		/// Selected line style for line geometries
		/// </summary>
		public StylePen SelectLine
		{
			get { return _selectLineStyle; }
			set { _selectLineStyle = value; }
		}

		/// <summary>
		/// Specified whether the objects are rendered with or without outlining
		/// </summary>
		public Boolean EnableOutline
		{
			get { return _outline; }
			set { _outline = value; }
		}

		/// <summary>
		/// Normal outline style for line and polygon geometries
		/// </summary>
		public StylePen Outline
		{
			get { return _outlineStyle; }
			set { _outlineStyle = value; }
		}

		/// <summary>
		/// Highlighted outline style for line and polygon geometries
		/// </summary>
		public StylePen HighlightOutline
		{
			get { return _highlightOutlineStyle; }
			set { _highlightOutlineStyle = value; }
		}

		/// <summary>
		/// Selected outline style for line and polygon geometries.
		/// </summary>
		public StylePen SelectOutline
		{
			get { return _selectOutlineStyle; }
			set { _selectOutlineStyle = value; }
		}

		/// <summary>
		/// Fill style for closed geometries.
		/// </summary>
		public StyleBrush Fill
		{
			get { return _fillStyle; }
			set { _fillStyle = value; }
		}

        /// <summary>
        /// Fill style for closed geometries when they are in a selected state.
        /// </summary>
		public StyleBrush SelectFill
		{
			get { return _selectionFillStyle; }
			set { _selectionFillStyle = value; }
		}

        /// <summary>
        /// Fill style for closed geometries when they are in a highlighted state.
        /// </summary>
		public StyleBrush HighlightFill
		{
			get { return _highlightFillStyle; }
			set { _highlightFillStyle = value; }
		}

		/// <summary>
		/// Render whether smoothing (antialiasing) is applied to lines 
		/// and curves and the edges of filled areas
		/// </summary>
		public StyleRenderingMode RenderingMode
		{
			get { return _smoothingMode; }
			set { _smoothingMode = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string LabelFormatExpression
		{
			get { return _labelFormatExpression; }
			set { _labelFormatExpression = value; }
		}

		/// <summary>
		/// Gets or sets a value to determine if features can 
		/// be selected on this layer.
		/// </summary>
		public Boolean AreFeaturesSelectable
		{
			get { return _areFeaturesSelectable; }
			set { _areFeaturesSelectable = value; }
		}

		#endregion
	}
}