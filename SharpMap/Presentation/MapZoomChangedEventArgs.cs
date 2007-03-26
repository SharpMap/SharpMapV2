using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Presentation
{
    [Serializable]
    public class MapZoomChangedEventArgs : EventArgs
    {
        private double _previousZoom;
        private double _currentZoom;
        private GeoPoint _previousCenter;
        private GeoPoint _currentCenter;

        public MapZoomChangedEventArgs(double previousZoom, double currentZoom, GeoPoint previousCenter, GeoPoint currentCenter)
        {
            _previousZoom = previousZoom;
            _currentZoom = currentZoom;
            _previousCenter = previousCenter;
            _currentCenter = currentCenter;
        }

        public double PreviousZoom
        {
            get { return _previousZoom; }
        }

        public double CurrentZoom
        {
            get { return _currentZoom; }
        }

        public GeoPoint PreviousCenter
        {
            get { return _previousCenter; }
        }

        public GeoPoint CurrentCenter
        {
            get { return _currentCenter; }
        }
    }
}
