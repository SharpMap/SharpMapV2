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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

using SharpMap.Rendering;

namespace SharpMap.Styles
{
    public class LinearGradientStyleBrush : StyleBrush
    {
        private ColorBlend _colorBlend;
        private StyleColor _startColor = StyleColor.Black;
        private StyleColor _endColor = StyleColor.White;
        private IMatrixD _transform;

        public ColorBlend ColorBlend
        {
            get { return _colorBlend; }
            set { _colorBlend = value; }
        }

        public StyleColor StartColor
        {
            get { return _startColor; }
            set { _startColor = value; }
        }

        public StyleColor EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        public IMatrixD Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public override string ToString()
        {
            return String.Format("LinearGradientStyleBrush - Start: {0}; End: {1}", StartColor, EndColor);
        }
    }
}
