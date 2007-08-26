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

namespace SharpMap.Tools
{
	public class MapTool<TMapView> : MapTool, IMapTool<TMapView>
	{
		private readonly Action<ActionContext<TMapView>> _queryAction;
		private readonly Action<ActionContext<TMapView>> _beginAction;
		private readonly Action<ActionContext<TMapView>> _extendAction;
		private readonly Action<ActionContext<TMapView>> _endAction;

		public MapTool(string name, Action<ActionContext<TMapView>> queryAction,
			Action<ActionContext<TMapView>> beginAction,
			Action<ActionContext<TMapView>> extendAction,
			Action<ActionContext<TMapView>> endAction)
			: base(name)
		{
			_queryAction = queryAction;
			_beginAction = beginAction;
			_extendAction = extendAction;
			_endAction = endAction;
		}

		public override string ToString()
		{
			return String.Format("MapTool: {0}", String.IsNullOrEmpty(Name) ? "<None>" : Name);
		}

		#region IMapTool Members

		public Action<ActionContext<TMapView>> QueryAction
		{
			get { return _queryAction; }
		}

		public Action<ActionContext<TMapView>> BeginAction
		{
			get { return _beginAction; }
		}

		public Action<ActionContext<TMapView>> ExtendAction
		{
			get { return _extendAction; }
		}

		public Action<ActionContext<TMapView>> EndAction
		{
			get { return _endAction; }
		}

		#endregion
	}
}