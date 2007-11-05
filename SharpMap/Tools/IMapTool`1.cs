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
using NPack;
using NPack.Interfaces;

namespace SharpMap.Tools
{
	public interface IMapTool<TMapView, TPoint>
		where TPoint : IVector<DoubleComponent>
	{
		string Name { get; }
		Action<ActionContext<TMapView, TPoint>> QueryAction { get; }
		Action<ActionContext<TMapView, TPoint>> BeginAction { get; }
		Action<ActionContext<TMapView, TPoint>> ExtendAction { get; }
		Action<ActionContext<TMapView, TPoint>> EndAction { get; }
	}
}