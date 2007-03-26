// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
//using System.Drawing;
//using GdiPoint = System.Drawing.Point;
//using GdiPointF = System.Drawing.PointF;
//using System.Drawing.Drawing2D;
using System.Globalization;

using SharpMap.Layers;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap
{
    /// <summary>
    /// Map class
    /// </summary>
    /// <example>
    /// Creating a new map instance, adding layers and rendering the map:
    /// <code lang="C#">
    /// SharpMap.Map myMap = new SharpMap.Map(picMap.Size);
    /// myMap.MinimumZoom = 100;
    /// myMap.BackgroundColor = Color.White;
    /// 
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    ///	string ConnStr = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=password;Database=myGisDb;";
    /// myLayer.DataSource = new SharpMap.Data.Providers.PostGIS(ConnStr, "myTable", "the_geom", 32632);
    /// myLayer.FillStyle = new SolidBrush(Color.FromArgb(240,240,240)); //Applies to polygon types only
    ///	myLayer.OutlineStyle = new Pen(Color.Blue, 1); //Applies to polygon and linetypes only
    /// //Setup linestyle (applies to line types only)
    ///	myLayer.Style.Line.Width = 2;
    ///	myLayer.Style.Line.Color = Color.Black;
    ///	myLayer.Style.Line.EndCap = System.Drawing.Drawing2D.LineCap.Round; //Round end
    ///	myLayer.Style.Line.StartCap = layRailroad.LineStyle.EndCap; //Round start
    ///	myLayer.Style.Line.DashPattern = new float[] { 4.0f, 2.0f }; //Dashed linestyle
    ///	myLayer.Style.EnableOutline = true;
    ///	myLayer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //Render smooth lines
    ///	myLayer.MaxVisible = 40000;
    /// 
    /// myMap.Layers.Add(myLayer);
    /// // [add more layers...]
    /// 
    /// myMap.Center = new SharpMap.Geometries.Point(725000, 6180000); //Set center of map
    ///	myMap.Zoom = 1200; //Set zoom level
    /// myMap.Size = new System.Drawing.Size(300,200); //Set output size
    /// 
    /// System.Drawing.Image imgMap = myMap.GetMap(); //Renders the map
    /// </code>
    /// </example>
    [DesignTimeVisible(false)]
    public class Map : IDisposable
    {
        /// <summary>
        /// Used for converting numbers to/from strings
        /// </summary>
        internal readonly static NumberFormatInfo NumberFormat_EnUS = new CultureInfo("en-US", false).NumberFormat;

        static Map() { }

        private LayersCollection _layers = new LayersCollection();
        private BoundingBox _envelope = BoundingBox.Empty;
        private int _activeLayer;

        /// <summary>
        /// Initializes a new map.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// Disposes the map object
        /// </summary>
        public void Dispose()
        {
            foreach (Layer layer in Layers)
                if (layer is IDisposable && layer != null)
                    ((IDisposable)layer).Dispose();

            _layers.Clear();
        }

        #region Events

        /// <summary>
        /// Event fired when the maps layer list have been changed.
        /// </summary>
        public event EventHandler<LayersChangedEventArgs> LayersChanged;

        #endregion

        #region Methods

        public void AddLayer(ILayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            _layers.Add(layer);
            OnLayersChanged(layer, LayersChangedType.Added);
        }

        public void RemoveLayer(ILayer layer)
        {
            if (layer != null)
            {
                _layers.Remove(layer);
                OnLayersChanged(layer, LayersChangedType.Removed);
            }
        }

        public void RemoveLayer(string name)
        {
            ILayer layer = GetLayerByName(name);
            RemoveLayer(layer);
        }

        /// <summary>
        /// Returns an enumerable set of all layers containing the string <paramref name="layerName"/> in the <see cref="ILayer.LayerName"/> property.
        /// </summary>
        /// <param name="layerName">String to search for.</param>
        /// <returns>IEnumerable{ILayer} of all layers with <see cref="ILayer.LayerName"/> containing <paramref name="layerName"/>.</returns>
        public IEnumerable<ILayer> FindLayers(string layerName)
        {
            foreach (ILayer layer in Layers)
                if (layer.LayerName.Contains(layerName))
                    yield return layer;
        }

        /// <summary>
        /// Returns a layer by its name.
        /// </summary>
        /// <remarks>
        /// Performs culture-sensitive, case-insensitive search.
        /// </remarks>
        /// <param name="name">Name of layer.</param>
        /// <returns>Layer with <see cref="ILayer.LayerName"/> of <paramref name="name"/>.</returns>
        public ILayer GetLayerByName(string name)
        {
            return _layers.Find(delegate(ILayer layer) 
            {
                return String.Compare(layer.LayerName, name, StringComparison.CurrentCultureIgnoreCase) == 0;
            });
        }

        #endregion

        #region Properties

        public BoundingBox Envelope
        {
            get { return _envelope; }
            private set { _envelope = value; }
        }

        /// <summary>
        /// Gets a collection of layers. The first layer in the list is drawn first, the last one on top.
        /// </summary>
        public IList<ILayer> Layers
        {
            get 
            {
                return _layers as IList<ILayer>;
            }
            private set 
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _layers = new LayersCollection(value);
                BoundingBox envelope = new BoundingBox();

                foreach (ILayer layer in _layers)
                    envelope.ExpandToInclude(layer.Envelope);

                Envelope = envelope;
            }
        }

        //public ILayer ActiveLayer
        //{
        //    get 
        //    {
        //        if (_activeLayer < 0 || _activeLayer >= Layers.Count)
        //            return null;

        //        return Layers[_activeLayer]; 
        //    }
        //    set 
        //    {
        //        if (value == null)
        //        {
        //            _activeLayer = -1;
        //            return;
        //        }

        //        _activeLayer = Layers.IndexOf(value); 
        //    }
        //}

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        public GeoPoint Center
        {
            get { return _envelope.GetCentroid(); }
        }

        /// <summary>
        /// Gets the extents of the map based on the extents of all the layers 
        /// in the layers collection.
        /// </summary>
        /// <returns>Full map extents.</returns>
        public BoundingBox GetExtents()
        {
            return Envelope;
        }
        #endregion

        private void OnLayersChanged(ILayer layer, LayersChangedType changeType)
        {
            EventHandler<LayersChangedEventArgs> @event = LayersChanged;
            if (@event != null)
                @event(this, new LayersChangedEventArgs(layer, changeType));
        }

        #region LayersCollection class
        private class LayersCollection : IList<ILayer>
        {
            private List<ILayer> _layers = new List<ILayer>();

            public LayersCollection() { }

            public LayersCollection(IEnumerable<ILayer> layers)
            {
                _layers.AddRange(layers);
            }

            public ILayer Find(Predicate<ILayer> predicate)
            {
                return _layers.Find(predicate);
            }

            #region IList Implementation
            public int Add(ILayer layer)
            {
                if (layer == null)
                    throw new ArgumentNullException("layer");

                _layers.Add(layer);
                return _layers.Count - 1;
            }

            public void Clear()
            {
                _layers.Clear();
            }

            public int Count
            {
                get { return _layers.Count; }
            }

            public bool Contains(ILayer layer)
            {
                return _layers.Contains(layer);
            }

            public int IndexOf(ILayer layer)
            {
                return _layers.IndexOf(layer);
            }

            public void Insert(int index, ILayer layer)
            {
                if (layer == null)
                    throw new ArgumentNullException("layer");

                _layers.Insert(index, layer);
            }

            public void Remove(ILayer layer)
            {
                if (layer == null)
                    throw new ArgumentNullException("layer");

                _layers.Remove(layer);
            }

            public void RemoveAt(int index)
            {
                ILayer layer = this[index];
                _layers.RemoveAt(index);
            }

            public ILayer this[int index]
            {
                get { return _layers[index]; }
                set { _layers[index] = value; }
            }

            public ILayer this[string layerName]
            {
                get
                {
                    return _layers.Find(delegate(ILayer layer)
                        {
                            return String.Compare(layer.LayerName, layerName, StringComparison.CurrentCultureIgnoreCase) == 0;
                        });
                }
                set
                {
                    ILayer layer = _layers.Find(delegate(ILayer candidateLayer)
                        {
                            return String.Compare(candidateLayer.LayerName, layerName, StringComparison.CurrentCultureIgnoreCase) == 0;
                        });

                    int index = this.IndexOf(layer);
                    _layers[index] = value;
                }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void CopyTo(ILayer[] array, int index)
            {
                _layers.CopyTo(array, index);
            }

            public IEnumerator<ILayer> GetEnumerator()
            {
                return _layers.GetEnumerator();
            }
            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region ICollection<ILayer> Members

            void ICollection<ILayer>.Add(ILayer item)
            {
                this.Add(item);
            }

            bool ICollection<ILayer>.Remove(ILayer item)
            {
                return _layers.Remove(item);
            }

            #endregion
        }
        #endregion
    }

    public enum LayersChangedType
    {
        Unknown = 0,
        Added,
        Removed
    }

    public class LayersChangedEventArgs : EventArgs
    {
        private IEnumerable<ILayer> _changedLayers;
        private LayersChangedType _changeType;

        internal LayersChangedEventArgs(ILayer changedLayer, LayersChangedType changeType)
        {
            List<ILayer> layers = new List<ILayer>();
            layers.Add(changedLayer);
            _changedLayers = layers;
            _changeType = changeType;
        }

        internal LayersChangedEventArgs(IEnumerable<ILayer> changedLayers, LayersChangedType changeType)
        {
            _changedLayers = changedLayers;
            _changeType = changeType;
        }

        public IEnumerable<ILayer> ChangedLayers
        {
            get { return _changedLayers; }
        }

        public LayersChangedType ChangeType
        {
            get { return _changeType; }
        }
    }
}
