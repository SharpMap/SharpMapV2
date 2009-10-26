/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

using System;
using System.Collections;
using System.IO;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Presentation.AspNet.MVP
{
    public class WebMapView : IMapView2D, IDisposable
    {
        private readonly IWebMap _webMap;
        private Boolean _disposed;
        private Boolean _enabled = true;
        private MapPresenter _presenter;
        private Boolean _visible = true;

        public WebMapView(IWebMap webMap)
        {
            _webMap = webMap;
        }

        public IWebMapRenderer WebMapRenderer { get; set; }

        public MapPresenter Presenter
        {
            get { return _presenter; }
        }

        public Map Map
        {
            get { return _presenter.Map; }
            set
            {
                if (_presenter != null)
                {
                    _presenter.Dispose();
                }

                if (value != null)
                {
                    _presenter = new MapPresenter(value, this);
                }
            }
        }

        public Boolean ClientDisconnected
        {
            get { return !_webMap.Context.Response.IsClientConnected; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IMapView2D Members

        /// <summary>
        /// interface member but unused
        /// </summary>
        public event EventHandler<MapActionEventArgs<Point2D>> Hover;

        /// <summary>
        /// interface member but unused
        /// </summary>
        public event EventHandler<MapActionEventArgs<Point2D>> BeginAction;

        /// <summary>
        /// interface member but unused
        /// </summary>
        public event EventHandler<MapActionEventArgs<Point2D>> MoveTo;

        /// <summary>
        /// interface member but unused
        /// </summary>
        public event EventHandler<MapActionEventArgs<Point2D>> EndAction;

        /// <summary>
        /// interface member but unused
        /// </summary>
        public event EventHandler SizeChanged;

        public event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>> GeoCenterChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>> MaximumWorldWidthChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>> MinimumWorldWidthChangeRequested;

        public event EventHandler<LocationEventArgs> IdentifyLocationRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ViewEnvelopeChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>> WorldAspectRatioChangeRequested;

        public event EventHandler ZoomToExtentsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ZoomToWorldBoundsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>> ZoomToWorldWidthRequested;

        public StyleColor BackgroundColor
        {
            get { return _presenter.BackgroundColor; }
            set { onRequestBackgroundColorChange(BackgroundColor, value); }
        }

        public Double Dpi
        {
            get { return WebMapRenderer.Dpi; }
        }

        public ICoordinate GeoCenter
        {
            get { return _presenter.GeoCenter; }
            set { onRequestGeoCenterChange(GeoCenter, value); }
        }

        public void IdentifyLocation(ICoordinate worldPoint)
        {
            onRequestIdentifyLocation(worldPoint);
        }

        public Double MaximumWorldWidth
        {
            get { return _presenter.MaximumWorldWidth; }
            set { onRequestMaximumWorldWidthChange(MaximumWorldWidth, value); }
        }

        public Double MinimumWorldWidth
        {
            get { return _presenter.MinimumWorldWidth; }
            set { onRequestMinimumWorldWidthChange(MinimumWorldWidth, value); }
        }

        public void Offset(Point2D offsetVector)
        {
            onRequestOffset(offsetVector);
        }

        public Double PixelWorldHeight
        {
            get { return _presenter.PixelWorldHeight; }
        }

        public Double PixelWorldWidth
        {
            get { return _presenter.PixelWorldWidth; }
        }

        public ViewSelection2D Selection
        {
            get { return _presenter.Selection; }
        }

        public Matrix2D ToViewTransform
        {
            get { return _presenter.ToViewTransform; }
        }

        public Matrix2D ToWorldTransform
        {
            get { return _presenter.ToWorldTransform; }
        }

        public Point2D ToView(ICoordinate point)
        {
            return _presenter.ToView(point);
        }

        public Point2D ToView(Double x, Double y)
        {
            return _presenter.ToView(x, y);
        }

        public ICoordinate ToWorld(Point2D point)
        {
            return _presenter.ToWorld(point);
        }

        public ICoordinate ToWorld(Double x, Double y)
        {
            return _presenter.ToWorld(x, y);
        }

        public IExtents2D ViewEnvelope
        {
            get { return _presenter.ViewEnvelope; }
            set { onRequestViewEnvelopeChange(ViewEnvelope, value); }
        }


        public Size2D ViewSize { get; set; }

        public Double WorldAspectRatio
        {
            get { return _presenter.WorldAspectRatio; }
            set { onRequestWorldAspectRatioChange(WorldAspectRatio, value); }
        }

        public Double WorldHeight
        {
            get { return _presenter.WorldHeight; }
        }

        public Double WorldWidth
        {
            get { return _presenter.WorldWidth; }
        }

        public Double WorldUnitsPerPixel
        {
            get { return _presenter.WorldUnitsPerPixel; }
        }

        public void ZoomToExtents()
        {
            onRequestZoomToExtents();
        }

        public void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            onRequestZoomToViewBounds(viewBounds);
        }

        public void ZoomToWorldBounds(IExtents2D zoomBox)
        {
            onRequestZoomToWorldBounds(zoomBox);
        }

        public void ZoomToWorldWidth(Double newWorldWidth)
        {
            onRequestZoomToWorldWidth(newWorldWidth);
        }

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public Boolean Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public String Title { get; set; }

        public IRasterizeSurface RasterizeSurface
        {
            get { return WebMapRenderer.RasterizeSurface; }
        }

        #endregion


        private void onRequestBackgroundColorChange(StyleColor current, StyleColor requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> e = BackgroundColorChangeRequested;

            if (e != null)
            {
                e(this, new MapViewPropertyChangeEventArgs<StyleColor>(current, requested));
            }
        }

        private void onRequestGeoCenterChange(ICoordinate current, ICoordinate requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>> e = GeoCenterChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<ICoordinate> args =
                    new MapViewPropertyChangeEventArgs<ICoordinate>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMaximumWorldWidthChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = MaximumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMinimumWorldWidthChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = MinimumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestIdentifyLocation(ICoordinate location)
        {
            EventHandler<LocationEventArgs> e = IdentifyLocationRequested;

            if (e != null)
            {
                LocationEventArgs args = new LocationEventArgs(location);
                e(this, args);
            }
        }

        private void onRequestOffset(Point2D offset)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Point2D>> e = OffsetChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Point2D> args =
                    new MapViewPropertyChangeEventArgs<Point2D>(Point2D.Zero, offset);

                e(this, args);
            }
        }

        private void onRequestViewEnvelopeChange(IExtents2D current, IExtents2D requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> e = ViewEnvelopeChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<IExtents2D> args =
                    new MapViewPropertyChangeEventArgs<IExtents2D>(current, requested);

                e(this, args);
            }
        }

        private void onRequestWorldAspectRatioChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = WorldAspectRatioChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestZoomToExtents()
        {
            EventHandler e = ZoomToExtentsRequested;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        private void onRequestZoomToViewBounds(Rectangle2D viewBounds)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> e = ZoomToViewBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Rectangle2D> args =
                    new MapViewPropertyChangeEventArgs<Rectangle2D>(
                        new Rectangle2D(0, 0, ViewSize.Width, ViewSize.Height), viewBounds);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldBounds(IExtents2D zoomBox)
        {
            EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> e = ZoomToWorldBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<IExtents2D> args =
                    new MapViewPropertyChangeEventArgs<IExtents2D>(ViewEnvelope, zoomBox);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldWidth(Double newWorldWidth)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = ZoomToWorldWidthRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<Double>(WorldWidth, newWorldWidth);

                e(this, args);
            }
        }


        public Stream Render(out String mimeType)
        {
            return (WebMapRenderer).Render(this, out mimeType);
        }

        ~WebMapView()
        {
            Dispose(false);
        }

        private void Dispose(Boolean disposing)
        {
            if (!_disposed && disposing)
            {
                WebMapRenderer = null;

                if (_presenter != null)
                {
                    _presenter.Dispose();
                }

                _presenter = null;
                _disposed = true;
            }
        }

        #region IMapView2D Members


        public void Display(object viewData)
        {
            _renderedData = viewData;
        }

        private object _renderedData;
        public object RenderedData
        {
            get
            {
                return _renderedData;
            }
        }

        #endregion
    }
}