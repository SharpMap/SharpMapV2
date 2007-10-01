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
using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

using IMatrix2D = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using System.Collections;

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// The base class for 2D feature renderers.
	/// </summary>
	/// <typeparam name="TStyle">The type of style to use.</typeparam>
	/// <typeparam name="TRenderObject">The type of render object produced.</typeparam>
	public abstract class FeatureRenderer2D<TStyle, TRenderObject> : IFeatureRenderer<TRenderObject>
		where TStyle : class, IStyle
	{
		private Matrix2D _viewMatrix;
		private StyleRenderingMode _renderMode;
		private readonly VectorRenderer2D<TRenderObject> _vectorRenderer;
		private ITheme _theme;
		private bool _disposed;
		private TStyle _defaultStyle;

		#region Object construction and disposal
		protected FeatureRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer)
			: this(vectorRenderer, null)
		{
		}

		protected FeatureRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer, ITheme theme)
		{
			_vectorRenderer = vectorRenderer;
			_theme = theme;
		}

		~FeatureRenderer2D()
		{
			Dispose(false);
		}

		#region Dispose Pattern
		#region IDisposable Members

		public void Dispose()
		{
			if (!Disposed)
			{
				Dispose(true);
				Disposed = true;
				GC.SuppressFinalize(this);
			}
		}
		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}

		protected bool Disposed
		{
			get { return _disposed; }
			set { _disposed = value; }
		}
		#endregion
		#endregion

		/// <summary>
		/// Gets the VectorRenderer2D which this featurer renderer 
		/// uses to render graphics.
		/// </summary>
		public VectorRenderer2D<TRenderObject> VectorRenderer
		{
			get { return _vectorRenderer; }
		}

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

        #region IRenderer<TRenderObject> Members
        /// <summary>
		/// Gets or sets a <see cref="StyleRenderingMode"/> 
		/// value used to render objects.
		/// </summary>
		public StyleRenderingMode StyleRenderingMode
		{
			get { return _renderMode; }
			set { _renderMode = value; }
		}
		#endregion

        #region IFeatureRenderer<TRenderObject> Members

        /// <summary>
		/// Renders a feature into displayable render objects.
		/// </summary>
		/// <param name="feature">The feature to render.</param>
		/// <returns>An enumeration of positioned render objects for display.</returns>
        public IEnumerable<TRenderObject> RenderFeature(IFeatureDataRecord feature)
		{
			TStyle style;

			if (Theme == null)
			{
				if (DefaultStyle == null)
				{
					throw new InvalidOperationException(
                        "Cannot render feature without style. Both Theme and DefaultStyle are null.");
				}

				style = DefaultStyle;
			}
			else
			{
				style = (TStyle)Theme.GetStyle(feature);
			}

			return RenderFeature(feature, style, RenderState.Normal);
		}

		/// <summary>
		/// Renders a feature into displayable render objects.
		/// </summary>
		/// <param name="feature">The feature to render.</param>
		/// <param name="style">The style to use to render the feature.</param>
		/// <returns>An enumeration of positioned render objects for display.</returns>
        public IEnumerable<TRenderObject> RenderFeature(IFeatureDataRecord feature, TStyle style, RenderState renderState)
		{
			bool cancel = false;

			OnFeatureRendering(ref cancel);

			if (cancel)
			{
				yield break;
			}

			IEnumerable<TRenderObject> renderedObjects = DoRenderFeature(feature, style, renderState);

			OnFeatureRendered();

			foreach (TRenderObject renderObject in renderedObjects)
			{
				yield return renderObject;
			}
		}

		/// <summary>
		/// Gets or sets the theme used to generate styles for rendered features.
		/// </summary>
		public ITheme Theme
		{
			get { return _theme; }
			set { _theme = value; }
		}
        #endregion

	    /// <summary>
	    /// Gets or sets the default style if no style or theme information is provided.
	    /// </summary>
	    public TStyle DefaultStyle
	    {
	        get { return _defaultStyle; }
	        set
	        {
	            if (value == null) throw new ArgumentNullException("value");

	            _defaultStyle = value;
	        }
	    }

	    /// <summary>
        /// Gets or sets a matrix used to transform world 
        /// coordinates to graphical display coordinates.
        /// </summary>
        public Matrix2D ToViewTransform
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }

	    /// <summary>
		/// Template method to perform the actual geometry rendering.
		/// </summary>
		/// <param name="feature">Feature to render.</param>
		/// <param name="style">Style to use in rendering geometry.</param>
		/// <param name="state">
		/// A <see cref="RenderState"/> value to indicate how to render the feature.
		/// </param>
		/// <returns></returns>
        protected abstract IEnumerable<TRenderObject> DoRenderFeature(IFeatureDataRecord feature, TStyle style, RenderState state);

		#region Protected virtual methods
		/// <summary>
		/// Called when a feature is rendered.
		/// </summary>
		protected virtual void OnFeatureRendered()
		{
			EventHandler @event = FeatureRendered;

			if (@event != null)
			{
				@event(this, EventArgs.Empty); //Fire event
			}
		}

		/// <summary>
		/// Called when a feature is being rendered.
		/// </summary>
		/// <param name="cancel">
		/// Value which can be set to indicate that the feature shouldn't be rendered.
		/// </param>
        protected virtual void OnFeatureRendering(ref bool cancel)
		{
			CancelEventHandler @event = FeatureRendering;

			if (@event != null)
			{
				CancelEventArgs args = new CancelEventArgs(cancel);
				@event(this, args); //Fire event

				cancel = args.Cancel;
			}
		}
		#endregion

		#region Explicit Interface Implementation
		#region IRenderer<Point2D,ViewSize2D,Rectangle2D,TRenderObject> Members

		IMatrix2D IRenderer.RenderTransform
		{
			get
			{
				return ToViewTransform;
			}
			set
			{
				if (!(value is Matrix2D))
				{
					throw new NotSupportedException("Only a Matrix2D is supported on a FeatureRenderer2D.");
				}

				ToViewTransform = value as Matrix2D;
			}
		}

		#endregion

        #region IFeatureRenderer Members

        IStyle IFeatureRenderer.DefaultStyle
        {
            get
            {
                return DefaultStyle;
            }
            set
            {
                if (!(value is TStyle))
                {
                    throw new ArgumentException("DefaultStyle must be of type " + typeof(TStyle));
                }

                DefaultStyle = (TStyle)value;
            }
        }

        IEnumerable IFeatureRenderer.RenderFeature(IFeatureDataRecord feature)
        {
            return RenderFeature(feature);
        }

        IEnumerable IFeatureRenderer.RenderFeature(IFeatureDataRecord feature, IStyle style, RenderState renderState)
        {
            return RenderFeature(feature, style as TStyle, renderState);
        }

        #endregion

		#region IFeatureRenderer<TRenderObject> Members

		IEnumerable<TRenderObject> IFeatureRenderer<TRenderObject>.RenderFeature(
            IFeatureDataRecord feature, IStyle style, RenderState renderState)
		{
			return RenderFeature(feature, style as TStyle, renderState);
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		#endregion
		#endregion
    }
}
