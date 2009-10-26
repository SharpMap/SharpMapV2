using Cairo;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Cairo
{
    public abstract class CairoRasterizer : IRasterizer<Surface, Context>
    {
        private Context _context;
        private Surface _surface;

        protected CairoRasterizer(Surface surface, Context context)
        {
            Surface = surface;
            Context = context;
        }

        #region IRasterizer<Surface,Context> Members

        public Surface Surface
        {
            get { return _surface; }
            protected set { _surface = value; }
        }

        public Context Context
        {
            get { return _context; }
            protected set { _context = value; }
        }

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