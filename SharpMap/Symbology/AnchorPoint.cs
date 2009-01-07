// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Xml.Serialization;

namespace SharpMap.Symbology
{
    /// <summary>
    /// Gets the location inside of a graphic or label text to use for anchoring 
    /// the it to the main-geometry point.
    /// </summary>
    /// <remarks>
    /// The coordinates are given as two floating-point numbers in the 
    /// <see cref="SharpMap.Symbology.AnchorPoint.AnchorPointX"/> and 
    /// <see cref="SharpMap.Symbology.AnchorPoint.AnchorPointY"/> properties 
    /// each with values between 0.0 and 1.0 inclusive. 
    /// The bounding box of the graphic or label text to be rendered is considered to be in 
    /// a coordinate space from 0.0 (lower-left corner) to 1.0 (upper-right corner), 
    /// and the anchor position is specified as a point in this space. 
    /// The default point is X=0.5, Y=0.5, which is at the middle height 
    /// and middle length of the graphic or label text.
    /// </remarks>
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "AnchorPoint")]
    [XmlRoot("AnchorPoint", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class AnchorPoint
    {
        private ParameterValue _anchorPointX;
        private ParameterValue _anchorPointY;

        public ParameterValue AnchorPointX
        {
            get { return _anchorPointX; }
            set { _anchorPointX = value; }
        }

        public ParameterValue AnchorPointY
        {
            get { return _anchorPointY; }
            set { _anchorPointY = value; }
        }
    }
}