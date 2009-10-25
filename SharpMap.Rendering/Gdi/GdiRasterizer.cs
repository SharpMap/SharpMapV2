using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Gdi
{
    public abstract class GdiRasterizer : IRasterizer<Bitmap, Graphics>
    {
        private Bitmap _surface;
        private Graphics _context;

        protected GdiRasterizer(Bitmap surface, Graphics context)
        {
            _surface = surface;
            _context = context;
        }

        #region IRasterizer<Bitmap,Graphics> Members

        public Bitmap Surface
        {
            get { return _surface; }
        }

        public Graphics Context
        {
            get { return _context; }
        }

        #endregion

        #region IRasterizer Members

        object IRasterizer.Surface
        {
            get { return Surface; }
        }

        object IRasterizer.Context
        {
            get { return Context; }
        }

        public virtual void BeginPass()
        {

        }

        public virtual void EndPass()
        {

        }

        #endregion
    }
}
