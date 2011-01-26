namespace SharpMap.Rendering.Web.Gdi
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal class DrawParams : IDisposable
    {
        // NOTE all params can be null
        public DrawParams(GraphicsPath path, Pen line, Brush fill, Pen outline, Font font)
        {
            this.Path = path;
            this.Line = CloneOf(line);                
            this.Fill = CloneOf(fill);
            this.Outline = CloneOf(outline);
            this.Font = CloneOf(font);
        }            

        public GraphicsPath Path { get; private set; }

        public Pen Line { get; private set; }            
        public Brush Fill { get; private set; }
        public Pen Outline { get; private set; }
        public Font Font { get; private set; }

        private static T CloneOf<T>(T item) where T : class, ICloneable 
        {
            if (item == null)
                return null;
            lock (item)
                return (T)item.Clone();
        }

        public void Dispose()
        {
            if (this.Path != null)
            {
                this.Path.Dispose();
                this.Path = null;
            }

            if (this.Line != null)
            {
                this.Line.Dispose();
                this.Line = null;
            }
            if (this.Outline != null)
            {
                this.Outline.Dispose();
                this.Outline = null;
            }
            if (this.Fill != null)
            {
                this.Fill.Dispose();
                this.Fill = null;
            }
            if (this.Font != null)
            {
                this.Font.Dispose();
                this.Font = null;
            }
        }
    }
}