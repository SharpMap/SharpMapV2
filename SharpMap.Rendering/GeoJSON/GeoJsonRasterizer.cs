using System.IO;
using System.Text;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterizer : IRasterizer<StringBuilder, TextWriter>
    {
        private TextWriter _context;
        private StringBuilder _surface;

        protected GeoJsonRasterizer(StringBuilder sb, TextWriter writer)
        {
            Surface = sb;
            Context = writer;
        }

        #region IRasterizer<StringBuilder,TextWriter> Members

        public StringBuilder Surface
        {
            get { return _surface; }
            protected set { _surface = value; }
        }

        public TextWriter Context
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

        public void BeginPass()
        {
            //do nothing
        }

        public void EndPass()
        {
            //do nothing
        }

        #endregion
    }
}