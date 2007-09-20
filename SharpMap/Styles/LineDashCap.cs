// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Styles
{
    /// <summary>
    /// Describes how to form the end of a dash in a drawn line.
    /// </summary>
    public enum LineDashCap
    {
        /// <summary>
        /// Makes the cap of the dash flat, so it is square to the sides of the dash.
        /// </summary>
        Flat = 0,

        /// <summary>
        /// Makes the cap of the dash rounded, so it forms an arc from the sides
        /// of the dash.
        /// </summary>
        Round = 2,

        /// <summary>
        /// Makes each side of the cap of the dash an obtuse angle 
        /// from the side of the dash, so a triangle is formed for the cap
        /// when they meet.
        /// </summary>
        Triangle = 3
    }
}