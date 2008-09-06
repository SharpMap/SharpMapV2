using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGGSharp.Drawing.Interface;

namespace AGGSharp.Drawing
{
    public class Pen : IPen
    {
        private IBrush _brush;
        private IStroke _stroke;

        #region IPen Members

        public IBrush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }

        public IStroke Stroke
        {
            get
            {
                return _stroke;
            }
            set
            {
                _stroke = value;
            }
        }

        #endregion
    }
}
