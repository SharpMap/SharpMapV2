using System;
using System.ComponentModel;
using GeoAPI.Coordinates;
using GeoAPI.Diagnostics;
using NPack.Interfaces;
using SharpMap.Diagnostics;

namespace SharpMap.Presentation.Presenters
{
    /// <typeparam name="TView">Type of view to manage.</typeparam>
    public abstract class MapLayersListenerPresenter<TCoordinate, TView> : BasePresenter<TCoordinate, TView>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
        where TView : class
    {
        #region Instance Fields
        #endregion

        #region Object construction / disposal
        protected MapLayersListenerPresenter(Map<TCoordinate> map, TView view)
            : base(map, view)
        {
            map.Layers.ListChanged += handleLayersChanged;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (Map != null)
            {
                Map.Layers.ListChanged -= handleLayersChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Public Members
        #endregion

        protected virtual void NotifyLayersChanged(ListChangedType listChangedType,
                                                   Int32 oldIndex,
                                                   Int32 newIndex,
                                                   PropertyDescriptor propertyDescriptor) { }

        #region Private helper methods
        #region Event handlers
        private void handleLayersChanged(Object sender, ListChangedEventArgs e)
        {
            Trace.Info(TraceCategories.Presentation, "MapLayersListenerPresenter handling " +
                                                     "layer collection changed");
            Trace.Debug(TraceCategories.Presentation, "Layer collection change: " +
                                                      e.ListChangedType);

            NotifyLayersChanged(e.ListChangedType, e.OldIndex, e.NewIndex, e.PropertyDescriptor);
        }
        #endregion
        #endregion
    }
}
