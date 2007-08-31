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

using System;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Represents arguments for map view property changes.
    /// </summary>
    /// <typeparam name="TParam">Type of the propery which changed.</typeparam>
	[Serializable]
	public class MapViewPropertyChangeEventArgs<TParam> : EventArgs
	{
		private readonly TParam _currentValue;
		private readonly TParam _requestedValue;

        /// <summary>
        /// Creates a new instance of the MapViewPropertyChangeEventArgs class.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="requestedValue"></param>
		public MapViewPropertyChangeEventArgs(TParam currentValue, TParam requestedValue)
		{
			_currentValue = currentValue;
			_requestedValue = requestedValue;
		}

        /// <summary>
        /// Gets the current value of the view property.
        /// </summary>
		public TParam CurrentValue
		{
			get { return _currentValue; }
		}

        /// <summary>
        /// Gets the requested value of the property.
        /// </summary>
		public TParam RequestedValue
		{
			get { return _requestedValue; }
		}
	}
}