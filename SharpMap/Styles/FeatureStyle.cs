// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
	/// Defines a style used for rendering features.
	/// </summary>
	public abstract class FeatureStyle : Style
	{
		#region Private instance fields
        private StyleRenderingMode _smoothingMode;
        private Boolean _areFeaturesSelectable = true;

		#endregion


		#region Properties
        /// <summary>
        /// Gets or sets whether smoothing (anti-aliasing or ClearType) is applied to lines 
        /// and curves and the edges of filled areas.
        /// </summary>
        public StyleRenderingMode RenderingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value; }
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