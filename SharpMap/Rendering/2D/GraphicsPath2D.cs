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

namespace SharpMap.Rendering.Rendering2D
{
    public class GraphicsPath2D : GraphicsPath<ViewPoint2D, ViewRectangle2D>
    {
        public GraphicsPath2D() 
            :base() { }

        public GraphicsPath2D(IEnumerable<ViewPoint2D> points)
            : base(points) { }

        public GraphicsPath2D(IEnumerable<ViewPoint2D> points, bool closeFigure)
            : base(points, closeFigure) { }

        public GraphicsPath2D(IEnumerable<GraphicsFigure<ViewPoint2D, ViewRectangle2D>> figures)
            : base(figures) { }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath(IEnumerable<ViewPoint2D> points, bool closeFigure)
        {
            return new GraphicsPath2D(points, closeFigure);
        }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath(IEnumerable<GraphicsFigure<ViewPoint2D, ViewRectangle2D>> figures)
        {
            return new GraphicsPath2D(figures);
        }

        protected override GraphicsFigure<ViewPoint2D, ViewRectangle2D> CreateFigure(IEnumerable<ViewPoint2D> points, bool closeFigure)
        {
            return new GraphicsFigure2D(points, closeFigure);
        }

        protected override ViewRectangle2D ComputeBounds()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
