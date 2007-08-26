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
	[Serializable]
	public class MapViewPropertyChangeEventArgs<TParam> : EventArgs
	{
		private readonly TParam _currentValue;
		private readonly TParam _requestedValue;

		public MapViewPropertyChangeEventArgs(TParam currentValue, TParam requestedValue)
		{
			_currentValue = currentValue;
			_requestedValue = requestedValue;
		}

		public TParam CurrentValue
		{
			get { return _currentValue; }
		}

		public TParam RequestedValue
		{
			get { return _requestedValue; }
		}
	}

	[Serializable]
	public class MapViewPropertyChangeEventArgs<TViewValue, TGeoValue> : EventArgs
	{
		private readonly TViewValue _currentViewValue;
		private readonly TViewValue _requestedViewValue;
		private readonly TGeoValue _currentGeoValue;
		private readonly TGeoValue _requestedGeoValue;

		public MapViewPropertyChangeEventArgs(
			TGeoValue currentGeoValue, TGeoValue requestedGeoValue,
			TViewValue currentViewValue, TViewValue requestedViewValue)
		{
			_currentViewValue = currentViewValue;
			_requestedViewValue = requestedViewValue;

			_currentGeoValue = currentGeoValue;
			_requestedGeoValue = requestedGeoValue;
		}

		public TGeoValue CurrentGeoValue
		{
			get { return _currentGeoValue; }
		}

		public TViewValue CurrentViewValue
		{
			get { return _currentViewValue; }
		}

		public TGeoValue RequestedGeoValue
		{
			get { return _requestedGeoValue; }
		}

		public TViewValue RequestedViewValue
		{
			get { return _requestedViewValue; }
		}
	}
}