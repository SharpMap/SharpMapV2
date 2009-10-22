using System;
using Cairo;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using CairoMatrix = Cairo.Matrix;

namespace SharpMap.Rendering.Cairo
{
    public struct CairoRenderObject
    {
        /// <summary>
        /// The affine transform applied to the symbol before drawing.
        /// </summary>
        public readonly CairoMatrix AffineTransform;

        /// <summary>
        /// The location and size of the symbol or text.
        /// </summary>
        public readonly Rectangle Bounds;

        /// <summary>
        /// The color transform applied to the symbol before drawing.
        /// </summary>
        public readonly CairoMatrix ColorTransform;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly StyleBrush Fill;

        public readonly FontFace Font;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly StyleBrush HighlightFill;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly StylePen HighlightLine;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly StylePen HighlightOutline;

        /// <summary>
        /// The symbol to draw if render object is a symbol.
        /// </summary>
        public readonly Surface Image;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly StylePen Line;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly StylePen Outline;

        /// <summary>
        /// The path to draw if render object is a path.
        /// </summary>
        public readonly Path2D Path;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly StyleBrush SelectFill;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly StylePen SelectLine;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly StylePen SelectOutline;

        public readonly String Text;
        private RenderState _state;

        /// <summary>
        /// Creates a new CairoRenderObject instance.
        /// </summary>
        /// <param name="path">The path to draw.</param>
        /// <param name="fill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="line">
        /// The pen used to draw a line when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightLine">
        /// The pen used to draw a line when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectLine">
        /// The pen used to draw a line when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="outline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        public CairoRenderObject(Path2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
                                 StylePen line, StylePen highlightLine, StylePen selectLine,
                                 StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            _state = RenderState.Normal;
            Path = path;
            Fill = fill;
            HighlightFill = highlightFill;
            SelectFill = selectFill;
            Line = line;
            HighlightLine = highlightLine;
            SelectLine = selectLine;
            Outline = outline;
            HighlightOutline = highlightOutline;
            SelectOutline = selectOutline;

            Image = null;
            Bounds = new Rectangle();
            AffineTransform = new CairoMatrix();
            ColorTransform = null;

            Text = null;
            Font = null;
        }

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="image">The symbol to draw.</param>
        /// <param name="imageBounds">The location and size to draw the symbol.</param>
        /// <param name="transform">The affine transform applied to the symbol before drawing.</param>
        /// <param name="colorTransform">The color transform applied to the symbol before drawing.</param>
        public CairoRenderObject(Surface image, Rectangle imageBounds, CairoMatrix transform, CairoMatrix colorTransform)
        {
            _state = RenderState.Normal;
            Image = image;
            Bounds = imageBounds;
            AffineTransform = transform;
            ColorTransform = colorTransform;

            Path = null;
            Fill = null;
            HighlightFill = null;
            SelectFill = null;
            Line = null;
            HighlightLine = null;
            SelectLine = null;
            Outline = null;
            HighlightOutline = null;
            SelectOutline = null;

            Text = null;
            Font = null;
        }

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The font to use to render the text.</param>
        /// <param name="bounds">The location and size to draw the text.</param>
        /// <param name="fill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="outline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        public CairoRenderObject(String text, FontFace font, Rectangle bounds,
                                 StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
                                 StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            _state = RenderState.Normal;
            Path = null;

            Image = null;
            Bounds = new Rectangle();
            AffineTransform = null;
            ColorTransform = null;

            Text = text;
            Font = font;
            Bounds = bounds;
            Fill = fill;
            HighlightFill = highlightFill;
            SelectFill = selectFill;
            Outline = outline;
            HighlightOutline = highlightOutline;
            SelectOutline = selectOutline;
            Line = null;
            HighlightLine = null;
            SelectLine = null;
        }

        /// <summary>
        /// Gets or sets the render object state of visual appearance.
        /// </summary>
        public RenderState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}