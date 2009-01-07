// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;
using GeoAPI.Coordinates;
using NPack.Interfaces;
using SharpMap.Data;
using SharpMap.Symbology;
using IMatrix2D = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using SharpMap.Layers;

namespace SharpMap.Rendering
{
    /// <summary>
    /// A base class for feature renderers.
    /// </summary>
    public abstract class FeatureRenderer<TCoordinate> : Renderer, IFeatureRenderer<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        //private readonly IVectorRenderer<TCoordinate> _vectorRenderer;
        //private readonly ITextRenderer<TCoordinate> _textRenderer;
        //private FeatureStyle _defaultStyle;

        #region Object construction and disposal

        //protected FeatureRenderer(IVectorRenderer<TCoordinate> vectorRenderer, ITextRenderer<TCoordinate> textRenderer)
        //{
        //    _vectorRenderer = vectorRenderer;
        //    _textRenderer = textRenderer;
        //}

        protected override void Dispose(Boolean disposing)
        {
            if(IsDisposed)
            {
                return;
            }

            //if(_vectorRenderer != null)
            //{
            //    _vectorRenderer.Dispose();
            //}

            base.Dispose(disposing);
        }

        #endregion

        ///// <summary>
        ///// Gets the <see cref="IVectorRenderer{TCoordinate}"/> which the featurer renderer 
        ///// uses to render vector primitives.
        ///// </summary>
        //protected IVectorRenderer<TCoordinate> VectorRenderer
        //{
        //    get { return _vectorRenderer; }
        //}

        ///// <summary>
        ///// Gets the <see cref="IVectorRenderer{TCoordinate}"/> which the featurer renderer 
        ///// uses to render text primitives.
        ///// </summary>
        //protected ITextRenderer<TCoordinate> TextRenderer
        //{
        //    get { return _textRenderer; }
        //}

        #region Events

        /// <summary>
        /// Event fired when a feature is about to render to the render stream.
        /// </summary>
        public event CancelEventHandler FeatureRendering;

        /// <summary>
        /// Event fired when a feature has been rendered.
        /// </summary>
        public event EventHandler FeatureRendered;

        #endregion

        #region Implementation of IFeatureRenderer<TCoordinate>

        public void RenderStroke(IScene scene, IRenderLayer layer, ICoordinateSequence<TCoordinate> coordinates, IPen pen, double perpendicularOffset, RenderState renderState)
        {
            throw new System.NotImplementedException();
        }

        public void RenderFill(IScene scene, IRenderLayer layer, ICoordinateSequence<TCoordinate> coordinates, IBrush fill, TCoordinate displacement, RenderState renderState)
        {
            throw new System.NotImplementedException();
        }

        public void RenderPoints(IScene scene, IRenderLayer layer, ICoordinateSequence<TCoordinate> coordinates, ISymbol<TCoordinate> graphic, RenderState renderState)
        {
            throw new System.NotImplementedException();
        }

        public void RenderPoint(IScene scene, IRenderLayer layer, TCoordinate coordinate, ISymbol<TCoordinate> graphic, RenderState renderState)
        {
            throw new System.NotImplementedException();
        }

        public void RenderTextOnLine(IScene scene, IRenderLayer layer, ICoordinateSequence<TCoordinate> coordinates, string text, IFont font, IHalo halo, IBrush fill, double perpendicularOffset, bool isRepeated, double initialGap, double gap, bool fitToPath)
        {
            throw new System.NotImplementedException();
        }

        public void RenderTextOnPoints(IScene scene, IRenderLayer layer, ICoordinateSequence<TCoordinate> coordinates, string text, IFont font, IHalo halo, IBrush fill, TCoordinate anchorPoint, TCoordinate displacement)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        //#region IFeatureRenderer Members

        ///// <summary>
        ///// Renders a feature into displayable render objects.
        ///// </summary>
        ///// <param name="feature">The feature to render.</param>
        ///// <param name="style">The style to use to render the feature.</param>
        ///// <returns>An enumeration of positioned render objects for display.</returns>
        //public void RenderFeature(IScene scene, ILayer layer, IFeatureDataRecord feature, 
        //                          FeatureStyle style, RenderState renderState)
        //{
        //    Boolean cancel = false;

        //    OnFeatureRendering(ref cancel);

        //    if (cancel)
        //    {
        //        return;
        //    }
            
        //    if (style == null)
        //    {
        //        throw new InvalidOperationException("Cannot render feature without a style.");
        //    }

        //    DoRenderFeature(scene, layer, feature, style, renderState);

        //    OnFeatureRendered();
        //}

        //#endregion

        ///// <summary>
        ///// Gets or sets the default style if no style or theme information is provided.
        ///// </summary>
        //public FeatureStyle DefaultStyle
        //{
        //    get { return _defaultStyle; }
        //    set
        //    {
        //        if (value == null) throw new ArgumentNullException("value");

        //        _defaultStyle = value;
        //    }
        //}

        ///// <summary>
        ///// Template method to perform the actual geometry rendering.
        ///// </summary>
        ///// <param name="feature">Feature to render.</param>
        ///// <param name="style">Style to use in rendering geometry.</param>
        ///// <param name="state">
        ///// A <see cref="RenderState"/> value to indicate how to render the feature.
        ///// </param>
        ///// <returns></returns>
        ///// <param name="layer"></param>
        //protected abstract void DoRenderFeature(IScene scene, ILayer layer, IFeatureDataRecord feature, FeatureStyle style, RenderState state);

        #region Protected virtual methods

        /// <summary>
        /// Called when a feature is rendered.
        /// </summary>
        protected virtual void OnFeatureRendered()
        {
            EventHandler e = FeatureRendered;

            if (e != null)
            {
                e(this, EventArgs.Empty); //Fire event
            }
        }

        /// <summary>
        /// Called when a feature is being rendered.
        /// </summary>
        /// <param name="cancel">
        /// Value which can be set to indicate that the feature shouldn't be rendered.
        /// </param>
        protected virtual void OnFeatureRendering(ref Boolean cancel)
        {
            CancelEventHandler e = FeatureRendering;

            if (e != null)
            {
                CancelEventArgs args = new CancelEventArgs(cancel);
                e(this, args); //Fire event

                cancel = args.Cancel;
            }
        }

        #endregion
    }
}