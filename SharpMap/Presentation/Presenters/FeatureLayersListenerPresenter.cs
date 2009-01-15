using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.Coordinates;
using NPack.Interfaces;
using SharpMap.Layers;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// Provides functionality for a presenter which listens to <see cref="IBindingList.ListChanged"/>
    /// events from <see cref="FeatureLayer.SelectedFeatures"/> and <see cref="FeatureLayer.HighlightedFeatures"/>.
    /// </summary>
    /// <typeparam name="TView">Type of view to manage.</typeparam>
    public abstract class FeatureLayersListenerPresenter<TCoordinate, TView> : MapLayersListenerPresenter<TCoordinate, TView>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
        where TView : class
    {
        #region Nested types

        private class LayerWireState
        {
            private Boolean _highlighted;
            private Boolean _selected;
            private readonly IFeatureLayer _layer;

            public LayerWireState(IFeatureLayer layer)
            {
                _layer = layer;
            }

            public Boolean Highlighted
            {
                get { return _highlighted; }
                set { _highlighted = value; }
            }

            public Boolean Selected
            {
                get { return _selected; }
                set { _selected = value; }
            }

            public IFeatureLayer Layer
            {
                get { return _layer; }
            }
        } 
        #endregion

        #region Instance Fields
        private readonly List<LayerWireState> _wiredLayers = new List<LayerWireState>();
        #endregion

        #region Object construction / disposal
        protected FeatureLayersListenerPresenter(Map<TCoordinate> map, TView view)
            : base(map, view)
        {
            wireupExistingLayers(map.Layers);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            unwireLayers();
            base.Dispose(disposing);
        }
        #endregion

        #region Public Members
        #endregion

        #region Protected members

        protected virtual ListChangedEventHandler GetSelectedChangedEventHandler()
        {
            return null;
        }

        protected virtual ListChangedEventHandler GetHighlightedChangedEventHandler()
        {
            return null;
        }

        protected override void NotifyLayersChanged(ListChangedType listChangedType,
                                                    Int32 oldIndex,
                                                    Int32 newIndex,
                                                    PropertyDescriptor propertyDescriptor)
        {
            switch (listChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        ILayer layer = Map.Layers[newIndex];

                        IFeatureLayer featureLayer = layer as IFeatureLayer;

                        if (featureLayer != null)
                        {
                            wireupFeatureLayer(featureLayer);
                        }

                        IEnumerable<ILayer> layers = layer as IEnumerable<ILayer>;

                        if (layers != null)
                        {
                            foreach (IFeatureLayer child in layers)
                            {
                                wireupFeatureLayer(child);
                            }
                        }
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    // LayerCollection defines an e.NewIndex as -1 when an item is being 
                    // deleted and not yet removed from the collection.
                    if (newIndex < 0)
                    {
                        ILayer layer = Map.Layers[oldIndex];
                        IFeatureLayer featureLayer = layer as IFeatureLayer;

                        if (featureLayer != null)
                        {
                            unwireFeatureLayer(featureLayer);
                        }

                        IEnumerable<ILayer> layers = layer as IEnumerable<ILayer>;

                        if (layers != null)
                        {
                            foreach (IFeatureLayer child in layers)
                            {
                                unwireFeatureLayer(child);
                            }
                        }
                    }
                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.Reset:
                    wireupExistingLayers(Map.Layers);
                    break;
                case ListChangedType.ItemChanged:
                    break;
                default:
                    break;
            }

            base.NotifyLayersChanged(listChangedType, oldIndex, newIndex, propertyDescriptor);
        }
        #endregion

        #region Private helper methods
        private void wireupExistingLayers(IEnumerable<ILayer> layerCollection)
        {
            foreach (ILayer layer in layerCollection)
            {
                IFeatureLayer featureLayer = layer as IFeatureLayer;

                if (featureLayer != null)
                {
                    wireupFeatureLayer(featureLayer);
                }

                IEnumerable<ILayer> layers = layer as IEnumerable<ILayer>;

                if (layers != null)
                {
                    foreach (IFeatureLayer child in layers)
                    {
                        if (child != null)
                        {
                            wireupFeatureLayer(child);
                        }
                    }

                    continue;
                }
            }
        }

        private void wireupFeatureLayer(IFeatureLayer layer)
        {
            StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
            Predicate<LayerWireState> sameName = delegate(LayerWireState find)
            {
                return comparer.Equals(layer.LayerName,
                                       find.Layer.LayerName);
            };

            if (_wiredLayers.Exists(sameName))
            {
                return;
            }

            LayerWireState wireState = new LayerWireState(layer);

            ListChangedEventHandler handler = GetSelectedChangedEventHandler();

            if (handler != null)
            {
                layer.SelectedFeatures.ListChanged += handler;
                wireState.Selected = true;
            }

            handler = GetHighlightedChangedEventHandler();

            if (handler != null)
            {
                layer.HighlightedFeatures.ListChanged += handler;
                wireState.Highlighted = true;
            }

            if (wireState.Highlighted || wireState.Selected)
            {
                _wiredLayers.Add(wireState);
            }
        }

        private void unwireLayers()
        {
            foreach (LayerWireState wireState in _wiredLayers)
            {
                unwireByLayerWireState(wireState);
            }
        }

        private void unwireFeatureLayer(IFeatureLayer layer)
        {
            StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
            Predicate<LayerWireState> findByName = delegate(LayerWireState match)
                                                   {
                                                       return comparer.Equals(match.Layer.LayerName, 
                                                                              layer.LayerName);
                                                   };

            LayerWireState wireState = _wiredLayers.Find(findByName);

            if (wireState != null)
            {
                unwireByLayerWireState(wireState);
            }
        }
        
        private void unwireByLayerWireState(LayerWireState wireState)
        {
            IFeatureLayer layer = wireState.Layer;

            if (wireState.Selected)
            {
                layer.SelectedFeatures.ListChanged -= GetSelectedChangedEventHandler();
            }

            if (wireState.Highlighted)
            {
                layer.HighlightedFeatures.ListChanged -= GetHighlightedChangedEventHandler();
            }
        }
        #endregion
    }
}
