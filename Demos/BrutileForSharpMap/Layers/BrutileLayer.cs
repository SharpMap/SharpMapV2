using System;
using System.Collections;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    public class BrutileRasterLayer : Layer, IRasterLayer
    {
        public BrutileRasterLayer(BrutileProvider dataSource, Presentation.Views.IMapView2D mapView2D)
            : base(dataSource)
        {
            LayerName = dataSource.ConnectionId;
            _mapView2D = mapView2D;
        }

        private IMapView2D _mapView2D;
        private RasterStyle rs;

        #region Overrides of Layer

        public new IRasterProvider DataSource
        {
            get { return base.DataSource as IRasterProvider; }
        }

        private readonly Queue<BrutileRasterRecord> _rasters = new Queue<BrutileRasterRecord>();

        public override IEnumerable Select(Expression query)
        {
            _rasters.Clear();
            RasterQueryExpression rqe = query as RasterQueryExpression;
            if (rqe == null)
                rqe = (RasterQueryExpression)GetQueryFromSpatialBinaryExpression(SpatialBinaryExpression.Intersects(base.DataSource.GetExtents()));
            handleRastersSelectRequested(this,
                                         new SelectRequestedEventArgs(
                                             new BrutileRasterQueryExpression(rqe.SpatialPredicate,
                                                                              new LiteralExpression<double>(
                                                                                  _mapView2D.WorldUnitsPerPixel))));
            return _rasters;
        }

        void handleRastersSelectRequested(Object sender, SelectRequestedEventArgs e)
        {
            if (IsLoadingData)
            {
                return;
            }

            QueryExpression query = e.Query;

            if (!QueryCache.Contains(query))
            {
                query = QueryCache.FilterQuery(query);
                LoadLayerData(query);
            }
        }

        protected override IAsyncProvider CreateAsyncProvider(IProvider dataSource)
        {
            IRasterProvider rasterProvider = dataSource as IRasterProvider;

            if (rasterProvider == null)
            {
                throw new ArgumentException("The data source must be an " +
                                            "IRasterProvider for a GdalRasterLayer.");
            }

            return new AsyncRasterProviderAdapter(rasterProvider);
        }

        protected override void ProcessLoadResults(object results)
        {
            IEnumerable<IRasterRecord> rasters = results as IEnumerable<IRasterRecord>;
            if (rasters != null)
                foreach (IRasterRecord record in rasters)
                {
                    _rasters.Enqueue(record as BrutileRasterRecord);
                }
        }

        protected override QueryExpression GetQueryFromSpatialBinaryExpression(SpatialBinaryExpression exp)
        {
            return new BrutileRasterQueryExpression(exp, new LiteralExpression<Double>(_mapView2D.WorldUnitsPerPixel));
        }

        internal BrutileProvider InternalProvider
        {
            get { return DataSource as BrutileProvider; }
        }

        private IMathTransform _toViewTransform;
        public IMathTransform ToViewTransform
        {
            get { return _toViewTransform; }
            set { _toViewTransform = value; }
        }
        #endregion
    }
}