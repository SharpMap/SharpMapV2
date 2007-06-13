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
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Tools
{
    public interface IMapTool
    {
        string Name { get; }
        Action<ActionContext> BeginAction { get; }
        Action<ActionContext> ExtendAction { get; }
        Action<ActionContext> EndAction { get; }
    }

    public class MapTool : IMapTool
    {
        /// <summary>
        /// No active tool
        /// </summary>
        public static readonly MapTool None;

        /// <summary>
        /// Pan
        /// </summary>
        public static readonly MapTool Pan;

        /// <summary>
        /// Zoom in
        /// </summary>
        public static readonly MapTool ZoomIn;

        /// <summary>
        /// Zoom out
        /// </summary>
        public static readonly MapTool ZoomOut;

        /// <summary>
        /// Query tool
        /// </summary>
        public static readonly MapTool Query;

        /// <summary>
        /// QueryAdd tool
        /// </summary>
        public static readonly MapTool QueryAdd;

        /// <summary>
        /// QueryRemove tool
        /// </summary>
        public static readonly MapTool QueryRemove;

        /// <summary>
        /// Add feature tool
        /// </summary>
        public static readonly MapTool FeatureAdd;

        /// <summary>
        /// Remove feature tool
        /// </summary>
        public static readonly MapTool FeatureRemove;

        static MapTool()
        {
            None = new MapTool(String.Empty, delegate(ActionContext context) { }, delegate(ActionContext context) { }, delegate(ActionContext context) { });
            Pan = new MapTool("Pan", BeginPan, ContinuePan, EndPan);
            ZoomIn = new MapTool("ZoomIn", BeginZoomIn, ContinueZoomIn, EndZoomIn);
            ZoomOut = new MapTool("ZoomOut", BeginZoomOut, ContinueZoomOut, EndZoomOut);
            Query = new MapTool("Query", BeginQuery, ContinueQuery, EndQuery);
            QueryAdd = new MapTool("QueryAdd", BeginQueryAdd, ContinueQueryAdd, EndQueryAdd);
            QueryRemove = new MapTool("QueryRemove", BeginQueryRemove, ContinueQueryRemove, EndQueryRemove);
            FeatureAdd = new MapTool("FeatureAdd", BeginFeatureAdd, ContinueFeatureAdd, EndFeatureAdd);
            FeatureRemove = new MapTool("FeatureRemove", BeginFeatureRemove, ContinueFeatureRemove, EndFeatureRemove);
        }

        private static void BeginPan(ActionContext context)
        {
        }

        private static void ContinuePan(ActionContext context)
        {
        }

        private static void EndPan(ActionContext context)
        {
        }

        private static void BeginZoomIn(ActionContext context)
        {
        }

        private static void ContinueZoomIn(ActionContext context)
        {
        }

        private static void EndZoomIn(ActionContext context)
        {
        }

        private static void BeginZoomOut(ActionContext context)
        {
        }

        private static void ContinueZoomOut(ActionContext context)
        {
        }

        private static void EndZoomOut(ActionContext context)
        {
        }

        private static void BeginQuery(ActionContext context)
        {
        }

        private static void ContinueQuery(ActionContext context)
        {
        }

        private static void EndQuery(ActionContext context)
        {
        }

        private static void BeginQueryAdd(ActionContext context)
        {
        }

        private static void ContinueQueryAdd(ActionContext context)
        {
        }

        private static void EndQueryAdd(ActionContext context)
        {
        }

        private static void BeginQueryRemove(ActionContext context)
        {
        }

        private static void ContinueQueryRemove(ActionContext context)
        {
        }

        private static void EndQueryRemove(ActionContext context)
        {
        }

        private static void BeginFeatureAdd(ActionContext context)
        {
        }

        private static void ContinueFeatureAdd(ActionContext context)
        {
        }

        private static void EndFeatureAdd(ActionContext context)
        {
        }

        private static void BeginFeatureRemove(ActionContext context)
        {
        }

        private static void ContinueFeatureRemove(ActionContext context)
        {
        }

        private static void EndFeatureRemove(ActionContext context)
        {
        }

        private readonly string _name;
        private readonly Action<ActionContext> _beginAction;
        private readonly Action<ActionContext> _extendAction;
        private readonly Action<ActionContext> _endAction;

        public MapTool(string name, Action<ActionContext> beginAction, Action<ActionContext> extendAction, Action<ActionContext> endAction)
        {
            _name = name;
            _beginAction = beginAction;
            _extendAction = extendAction;
            _endAction = endAction;
        }

        public override string ToString()
        {
            return String.Format("MapTool: {0}", String.IsNullOrEmpty(Name) ? "<None>" : Name);
        }

        public string Name
        {
            get { return _name; }
        }

        #region IMapTool Members

        public Action<ActionContext> BeginAction
        {
            get { return _beginAction; }
        }

        public Action<ActionContext> ExtendAction
        {
            get { return _extendAction; }
        }

        public Action<ActionContext> EndAction
        {
            get { return _endAction; }
        }

        #endregion
    }
}
