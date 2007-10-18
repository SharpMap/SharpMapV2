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

using NPack;
using NPack.Interfaces;

namespace SharpMap.Tools
{
	public struct ActionContext<TMapView, TPoint>
		where TPoint : IVector<DoubleComponent>
	{
		private readonly Map _map;
        private readonly TMapView _view;
        private readonly TPoint _previousPoint;
	    private readonly TPoint _currentPoint;

		public ActionContext(Map map, TMapView view, TPoint previousPoint, TPoint currentPoint)
		{
			_map = map;
			_view = view;
		    _previousPoint = previousPoint;
		    _currentPoint = currentPoint;
		}

		public Map Map
		{
			get { return _map; }
		}

		public TMapView MapView
		{
			get { return _view; }
		}

	    public TPoint PreviousPoint
	    {
	        get { return _previousPoint; }
	    }

	    public TPoint CurrentPoint
	    {
	        get { return _currentPoint; }
	    }
	}
}