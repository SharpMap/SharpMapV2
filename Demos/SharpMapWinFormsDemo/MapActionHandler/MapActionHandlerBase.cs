/*
 *	This file is part of SharpMap.MapViewer
 *  SharpMapMapViewer is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using SharpMap.Rendering.Rendering2D;

namespace MapViewer.MapActionHandler
{
    public class MapActionHandler : IMapActionHandler
    {
        private Point2D beginPoint;
        private Point2D endPoint;
        private Point2D hoverPoint;

        #region IMapActionHandler Members

        public event EventHandler<MapActionHandlerEventArgs> Begin;
        public event EventHandler<MapActionHandlerEventArgs> Hover;
        public event EventHandler<MapActionHandlerEventArgs> End;

        public Point2D BeginPoint
        {
            get { return beginPoint; }
            set
            {
                beginPoint = value;
                HoverPoint = value;
                OnBeginPointSet();
                EndPoint = Point2D.Empty;
            }
        }

        public Point2D HoverPoint
        {
            get { return hoverPoint; }
            set
            {
                hoverPoint = value;
                OnHoverPontSet();
            }
        }

        public Point2D EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;
                OnEndPointSet();
            }
        }

        #endregion

        protected void OnBeginPointSet()
        {
            if (!BeginPoint.IsEmpty && Begin != null) Begin(this, new MapActionHandlerEventArgs(BeginPoint));
        }

        protected void OnHoverPontSet()
        {
            if (!HoverPoint.IsEmpty && Hover != null) Hover(this, new MapActionHandlerEventArgs(HoverPoint));
        }

        protected void OnEndPointSet()
        {
            if (!EndPoint.IsEmpty && End != null) End(this, new MapActionHandlerEventArgs(EndPoint));
        }
    }
}