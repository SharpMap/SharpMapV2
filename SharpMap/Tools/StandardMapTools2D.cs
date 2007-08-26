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
using SharpMap.Presentation;

namespace SharpMap.Tools
{
	public static class StandardMapTools2D
	{
		/// <summary>
		/// No active tool
		/// </summary>
		public static readonly MapTool<IMapView2D> None;

		/// <summary>
		/// Pan
		/// </summary>
		public static readonly MapTool<IMapView2D> Pan;

		/// <summary>
		/// Zoom in
		/// </summary>
		public static readonly MapTool<IMapView2D> ZoomIn;

		/// <summary>
		/// Zoom out
		/// </summary>
		public static readonly MapTool<IMapView2D> ZoomOut;

		/// <summary>
		/// Query tool
		/// </summary>
		public static readonly MapTool<IMapView2D> Query;

		/// <summary>
		/// QueryAdd tool
		/// </summary>
		public static readonly MapTool<IMapView2D> QueryAdd;

		/// <summary>
		/// QueryRemove tool
		/// </summary>
		public static readonly MapTool<IMapView2D> QueryRemove;

		/// <summary>
		/// Add feature tool
		/// </summary>
		public static readonly MapTool<IMapView2D> FeatureAdd;

		/// <summary>
		/// Remove feature tool
		/// </summary>
		public static readonly MapTool<IMapView2D> FeatureRemove;


		static StandardMapTools2D()
		{
			None = new MapTool<IMapView2D>(String.Empty, DoNothing, DoNothing, DoNothing, DoNothing);
			Pan = new MapTool<IMapView2D>("Pan", QueryPan, BeginPan, ContinuePan, EndPan);
			ZoomIn = new MapTool<IMapView2D>("ZoomIn", QueryZoomIn, BeginZoomIn, ContinueZoomIn, EndZoomIn);
			ZoomOut = new MapTool<IMapView2D>("ZoomOut", QueryZoomOut, BeginZoomOut, ContinueZoomOut, EndZoomOut);
			Query = new MapTool<IMapView2D>("Query", QueryQuery, BeginQuery, ContinueQuery, EndQuery);
			QueryAdd = new MapTool<IMapView2D>("QueryAdd", QueryQueryAdd, BeginQueryAdd, ContinueQueryAdd, EndQueryAdd);
			QueryRemove = new MapTool<IMapView2D>("QueryRemove", QueryQueryRemove, BeginQueryRemove, ContinueQueryRemove, EndQueryRemove);
			FeatureAdd = new MapTool<IMapView2D>("FeatureAdd", QueryFeatureAdd, BeginFeatureAdd, ContinueFeatureAdd, EndFeatureAdd);
			FeatureRemove = new MapTool<IMapView2D>("FeatureRemove", QueryFeatureRemove, BeginFeatureRemove, ContinueFeatureRemove, EndFeatureRemove);
		}

		private static void DoNothing(ActionContext<IMapView2D> context)
		{
		}

		#region Panning
		private static void QueryPan(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginPan(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinuePan(ActionContext<IMapView2D> context)
		{
		}

		private static void EndPan(ActionContext<IMapView2D> context)
		{
		}
		#endregion

		#region Zoom in
		private static void QueryZoomIn(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginZoomIn(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueZoomIn(ActionContext<IMapView2D> context)
		{
		}

		private static void EndZoomIn(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Zoom out
		private static void QueryZoomOut(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginZoomOut(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueZoomOut(ActionContext<IMapView2D> context)
		{
		}

		private static void EndZoomOut(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Query
		private static void QueryQuery(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginQuery(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueQuery(ActionContext<IMapView2D> context)
		{
		}

		private static void EndQuery(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Query add
		private static void QueryQueryAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginQueryAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueQueryAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void EndQueryAdd(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Query remove
		private static void QueryQueryRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginQueryRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueQueryRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void EndQueryRemove(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Feature add
		private static void QueryFeatureAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginFeatureAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueFeatureAdd(ActionContext<IMapView2D> context)
		{
		}

		private static void EndFeatureAdd(ActionContext<IMapView2D> context)
		{
		} 
		#endregion

		#region Feature remove
		private static void QueryFeatureRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void BeginFeatureRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void ContinueFeatureRemove(ActionContext<IMapView2D> context)
		{
		}

		private static void EndFeatureRemove(ActionContext<IMapView2D> context)
		{
		} 
		#endregion
	}
}