using System;
using System.Collections;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    public class GdalRasterLayer : Layer, IRasterLayer
    {
        public GdalRasterLayer(IProvider dataSource) : base(dataSource)
        {
            if ((dataSource as GdalRasterProvider) == null)
                throw new ArgumentException("dataSource");
            LayerName = dataSource.ConnectionId;
        }

        private RasterStyle rs;
        public GdalRasterLayer(string layerName, IProvider dataSource) : base(layerName, dataSource)
        {
            if ((dataSource as GdalRasterProvider) == null)
                throw new ArgumentException("dataSource");

        }

        public GdalRasterLayer(string layerName, IStyle style, IProvider dataSource) : base(layerName, style, dataSource)
        {
            if ((dataSource as GdalRasterProvider) == null)
                throw new ArgumentException("dataSource");
        }

        public GdalRasterLayer(string layerName, IStyle style, IProvider dataSource, IGeometryFactory geometryFactory, ILayer parent) : base(layerName, style, dataSource, geometryFactory, parent)
        {
            if ((dataSource as GdalRasterProvider) == null)
                throw new ArgumentException("dataSource");
        }

        #region Overrides of Layer

        public new IRasterProvider DataSource
        {
            get { return base.DataSource as IRasterProvider; }
        }

        private readonly Queue<GdalRasterRecord> _rasters = new Queue<GdalRasterRecord>();

        public override IEnumerable Select(Expression query)
        {
            _rasters.Clear();
            SpatialBinaryExpression sbe = query as SpatialBinaryExpression;
            if (sbe == null)
                sbe = SpatialBinaryExpression.Intersects(base.DataSource.GetExtents());
            handleRastersSelectRequested(this, new SelectRequestedEventArgs(GetQueryFromSpatialBinaryExpression(sbe)));
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
                    _rasters.Enqueue(record as GdalRasterRecord);
                }
        }

        protected override QueryExpression GetQueryFromSpatialBinaryExpression(SpatialBinaryExpression exp)
        {
            return new RasterQueryExpression(exp);
        }

        internal GdalRasterProvider InternalProvider
        {
            get { return DataSource as GdalRasterProvider; }
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