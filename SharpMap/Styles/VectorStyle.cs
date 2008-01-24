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

using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
	/// <summary>
	/// Defines a style used for rendering vector data
	/// </summary>
	public class VectorStyle : FeatureStyle
	{
		#region Private fields

		private Symbol2D _symbol;
		private Symbol2D _highlightSymbol;
		private Symbol2D _selectSymbol;

		#endregion

		#region Properties

		/// <summary>
		/// Symbol used for rendering points.
		/// </summary>
		public Symbol2D Symbol
		{
			get { return _symbol; }
			set { _symbol = value; }
		}

		/// <summary>
		/// Symbol used for rendering points.
		/// </summary>
		public Symbol2D HighlightSymbol
		{
			get { return _highlightSymbol; }
			set { _highlightSymbol = value; }
		}

		/// <summary>
		/// Symbol used for rendering points.
		/// </summary>
		public Symbol2D SelectSymbol
		{
			get { return _selectSymbol; }
			set { _selectSymbol = value; }
		}

		#endregion
	}
}