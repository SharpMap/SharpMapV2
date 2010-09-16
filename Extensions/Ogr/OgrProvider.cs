using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using OSGeo.OGR;
using SharpMap.Expressions;
using SharpMap.Utilities.SridUtility;
using Ogr = OSGeo.OGR.Ogr;
using OgrDatasource = OSGeo.OGR.DataSource;
using OgrLayer = OSGeo.OGR.Layer;
using OgrEnvelope = OSGeo.OGR.Envelope;
using OgrFeature = OSGeo.OGR.Feature;
using OgrFeatureDefn = OSGeo.OGR.FeatureDefn;
using OgrFieldDefn = OSGeo.OGR.FieldDefn;
using OgrFieldType = OSGeo.OGR.FieldType;
using OgrGeometryType = OSGeo.OGR.wkbGeometryType;

namespace SharpMap.Data.Providers
{
    public class OgrProvider : FeatureProviderBase, IWritableFeatureProvider<Int32>
    {
        private string _connectionString;
        private ICoordinateSystemFactory _coordSysFactory;
        private ICoordinateFactory _coordFactory;

        private OgrDatasource _ogrDatasource;
        private OgrLayer _ogrLayer;
        private OgrFeatureDefn _ogrFeatureDefn;
        private readonly bool _isUpdateable;

        static OgrProvider()
        {
            OgrForSharpMap.Configure();
        }

        public static void Configure()
        {}

        public OgrProvider(
            string connectionString,
            int layerIndex,
            IGeometryFactory geometryFactory)
            :this(connectionString, layerIndex, geometryFactory, null)
        {
        }

        public OgrProvider(
            string connectionString,
            int layerIndex, 
            IGeometryFactory geometryFactory,
            ICoordinateSystemFactory coordSysFactory)
        {
            _connectionString = connectionString;
            IGeometryFactory geoFactoryClone = base.GeometryFactory = geometryFactory.Clone();
            //OriginalSpatialReference = geoFactoryClone.SpatialReference;
            //OriginalSrid = geoFactoryClone.Srid;
            _coordSysFactory = coordSysFactory;
            _coordFactory = geoFactoryClone.CoordinateFactory;

            try
            {
                _ogrDatasource = Ogr.OpenShared(connectionString, (int)OSGeo.GDAL.Access.GA_Update);
                _isUpdateable = true;
            }
            catch (Exception)
            {
                _isUpdateable = false;
                _ogrDatasource = Ogr.OpenShared(connectionString, (int)OSGeo.GDAL.Access.GA_ReadOnly);
            }
            
            _ogrLayer = _ogrDatasource.GetLayerByIndex(layerIndex);
            if (String.IsNullOrEmpty(_ogrLayer.GetFIDColumn()))
            {
                string layer = _ogrLayer.GetName();
                _ogrLayer.Dispose();
                _ogrLayer = _ogrDatasource.ExecuteSQL("SELECT FID AS OGR_FID, * FROM " + layer + ";", null, "");
            }

            if (coordSysFactory != null)
                using (OSGeo.OSR.SpatialReference osrSpatialReference = _ogrLayer.GetSpatialRef())
                {
                    if (osrSpatialReference != null)
                    {
                        string wkt = "";
                        osrSpatialReference.ExportToPrettyWkt(out wkt, 1);
                        wkt = wkt.Replace("\r", "");
                        wkt = wkt.Replace("\n", "");
                        OriginalSpatialReference = _coordSysFactory.CreateFromWkt(wkt);
                        GeometryFactory.SpatialReference = OriginalSpatialReference;
                    }
                }

            _ogrFeatureDefn = _ogrLayer.GetLayerDefn();
        }

        #region Dispose pattern

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (_ogrFeatureDefn != null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("OgrProvider.Dispose _ogrFeatureDefn: {0}", _ogrFeatureDefn.GetReferenceCount()));
                    _ogrFeatureDefn.Dispose();
                    _ogrFeatureDefn = null;
                }
                if (_ogrLayer != null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("OgrProvider.Dispose _ogrLayer: {0}", _ogrLayer.GetRefCount()));
                    _ogrLayer.Dispose();
                    _ogrLayer = null;
                }
                if (_ogrDatasource != null)
                {
                    _ogrDatasource.Dispose();
                    _ogrDatasource = null;
                }
            }
            base.Close();
        }

        public override IExtents GetExtents()
        {
            OgrEnvelope envelope = new OgrEnvelope();
            _ogrLayer.GetExtent(envelope, 1);
            return GeometryFactory.CreateExtents2D(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
        }

        public override string ConnectionId
        {
            get { return _connectionString;}
        }

        public override object ExecuteQuery(Expression query)
        {
            FeatureQueryExpression featureQuery = query as FeatureQueryExpression;

            if (featureQuery == null)
            {
                throw new ArgumentException("The query must be a non-null FeatureQueryExpression.");
            }

            return ExecuteFeatureQuery(featureQuery);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("OgrProvider({0}, {1})", _ogrDatasource.name, _ogrLayer.GetName());
        }

        private static FeatureDataTable<int> GetFeatureTableForFields(OgrLayer layer, IGeometryFactory factory)
        {
            string oidColumnName = layer.GetFIDColumn();
            if (string.IsNullOrEmpty(oidColumnName))
                oidColumnName = "OGR_FID";

            FeatureDataTable<int> table = new FeatureDataTable<int>(oidColumnName, factory);

            using (OgrFeatureDefn layerDefn = layer.GetLayerDefn())
            {
                for (int i = 0; i < layerDefn.GetFieldCount(); i++)
                {
                    using (OgrFieldDefn fieldDefn = layerDefn.GetFieldDefn(i))
                    {
                        string columnName = fieldDefn.GetName();
                        if (string.Compare(oidColumnName, columnName, true) != 0)
                        {
                            DataColumn col = table.Columns.Add(columnName, ToNetType(fieldDefn.GetFieldType()));
                            /*
                            if (col.DataType == typeof(string))
                                col.MaxLength = fieldDefn.GetWidth();
                            else
                                col.ExtendedProperties[ProviderSchemaHelper.LengthExtendedProperty] =
                                    fieldDefn.GetWidth();

                            if (fieldDefn.GetPrecision() > 0)
                                col.ExtendedProperties[ProviderSchemaHelper.NumericPrecisionExtendedProperty] =
                                    fieldDefn.GetPrecision();
                             */
                        }
                    }
                }
                System.Diagnostics.Debug.Assert(table.Columns.Count == layerDefn.GetFieldCount());
            }
            return table;
        }


        public override FeatureDataTable CreateNewTable()
        {
            IGeometryFactory fact;
            if (CoordinateTransformation == null)
                fact = GeometryFactory;
            else
            {
                fact = GeometryFactory.Clone();
                fact.SpatialReference = CoordinateTransformation.Target;
                fact.Srid = SridMap.DefaultInstance.Process(fact.SpatialReference, "");
            }

            return GetFeatureTableForFields(_ogrLayer, fact);
            //string fidColumn = _ogrLayer.GetFIDColumn();
            //if (string.IsNullOrEmpty(fidColumn)) fidColumn = "OGR_FID";
            //FeatureDataTable<Int32> tbl = new FeatureDataTable<Int32>(fidColumn, fact);
            //SetTableSchema(tbl, SchemaMergeAction.AddAll | SchemaMergeAction.Key);
            //return tbl;
        }

        public override int GetFeatureCount()
        {
            return _ogrLayer.GetFeatureCount(1);
        }

        public override DataTable GetSchemaTable()
        {
            DataTable schemaTable = ProviderSchemaHelper.CreateSchemaTable();

            OgrFeatureDefn featureDefn = _ogrLayer.GetLayerDefn();
            for (int i = 0; i < featureDefn.GetFieldCount(); i++)
            {
                OgrFieldDefn field = featureDefn.GetFieldDefn(i);
                DataRow dataRow = schemaTable.NewRow();
                dataRow[ProviderSchemaHelper.ColumnNameColumn] = field.GetName();
                dataRow[ProviderSchemaHelper.ColumnSizeColumn] = field.GetWidth();
                dataRow[ProviderSchemaHelper.ColumnOrdinalColumn] = i;
                dataRow[ProviderSchemaHelper.NumericPrecisionColumn] = 0;
                dataRow[ProviderSchemaHelper.NumericScaleColumn] = 0;
                dataRow[ProviderSchemaHelper.DataTypeColumn] = ToNetType(field.GetFieldType());
                dataRow[ProviderSchemaHelper.AllowDBNullColumn] = true;
                dataRow[ProviderSchemaHelper.IsReadOnlyColumn] = !_isUpdateable;
                dataRow[ProviderSchemaHelper.IsUniqueColumn] = false;
                dataRow[ProviderSchemaHelper.IsRowVersionColumn] = false;
                dataRow[ProviderSchemaHelper.IsKeyColumn] = false;
                dataRow[ProviderSchemaHelper.IsAutoIncrementColumn] = false;
                dataRow[ProviderSchemaHelper.IsLongColumn] = false;

                schemaTable.Rows.Add(dataRow);
            }
            return schemaTable;
        }

        public override CultureInfo Locale
        {
            get
            {
                //ToDo: Check if there is more to it
                return CultureInfo.InvariantCulture;
            }
        }

        public override void SetTableSchema(FeatureDataTable table)
        {
            table.Merge(GetSchemaTable());
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            OgrFeatureDataReader reader = new OgrFeatureDataReader(this, query, options);
            //reader.Disposed += readerDisposed;
            reader.CoordinateTransformation = CoordinateTransformation;
            return reader;
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return InternalExecuteFeatureQuery(query, FeatureQueryExecutionOptions.FullFeature);
        }

        private static Type ToNetType(OgrFieldType fieldType)
        {
            switch (fieldType)
            {
                case OgrFieldType.OFTDate:
                case OgrFieldType.OFTDateTime:
                case OgrFieldType.OFTTime:
                    return typeof (DateTime);

                case OgrFieldType.OFTInteger:
                    return typeof (Int32);
                case OgrFieldType.OFTIntegerList:
                    return typeof(Int32[]);

                case OgrFieldType.OFTReal:
                    return typeof(Double);
                case OgrFieldType.OFTRealList:
                    return typeof(Double[]);

                case OgrFieldType.OFTString:
                case OgrFieldType.OFTWideString:
                    return typeof(String);
                case OgrFieldType.OFTStringList:
                case OgrFieldType.OFTWideStringList:
                    return typeof(String[]);

                default:
                    throw new OgrProviderException("Should never reach here!");
            }
        }

        internal OgrDatasource OgrDataSource
        {
            get { return _ogrDatasource; }
        }
        internal OgrLayer OgrLayer
        {
            get { return _ogrLayer; }
        }

        internal OgrFeatureDefn OgrFeatureDefn
        {
            get { return _ogrFeatureDefn; }
        }

        #region Implementation of IWritableFeatureProvider

        public void Insert(FeatureDataRow feature)
        {
            Insert((FeatureDataRow<int>) feature);
            //OgrFeature ogrFeature = new OgrFeature(_ogrFeatureDefn);
            //    OgrFeatureDataRecord ogrFeatureDataRecord = new OgrFeatureDataRecord(
            //        GeometryFactory, CoordinateTransformation, System.Text.Encoding.Default,
            //        ogrFeature, _ogrLayer.GetLayerDefn());
            //    object[] values = null;
            //    int count = feature.GetValues(values);
            //    ogrFeatureDataRecord.SetColumnValues(0, count - 1, values);
            //    _ogrLayer.CreateFeature(ogrFeature);
            //    //this disposes ogrFeature as well;
            //    ogrFeatureDataRecord.Dispose();
            //feature.AcceptChanges();
        }

        public void Insert(IEnumerable<FeatureDataRow> features)
        {
            Insert(Caster.Downcast<FeatureDataRow<int>, FeatureDataRow>(features));
            //_ogrLayer.StartTransaction();
            //foreach (FeatureDataRow featureDataRow in features)
            //{
            //    Insert(featureDataRow);
            //}
            //if (_ogrLayer.CommitTransaction() != 0)
            //    _ogrLayer.RollbackTransaction();
        }

        public void Update(FeatureDataRow feature)
        {
            Update((FeatureDataRow<int>)feature);
            //using (OgrFeatureDefn featureDefn = _ogrLayer.GetLayerDefn())
            //{
            //    OgrFeature ogrFeature = _ogrLayer.GetFeature((int) feature.GetOid());
            //    OgrFeatureDataRecord ogrFeatureDataRecord = new OgrFeatureDataRecord(
            //        GeometryFactory, CoordinateTransformation, System.Text.Encoding.Default,
            //        ogrFeature, _ogrLayer.GetLayerDefn());
            //    object[] values = null;
            //    int count = feature.GetValues(values);
            //    ogrFeatureDataRecord.SetColumnValues(0, count - 1, values);
            //    _ogrLayer.SetFeature(ogrFeature);
            //    //this disposes ogrFeature as well;
            //    ogrFeatureDataRecord.Dispose();
            //}
            //feature.AcceptChanges();

        }

        public void Update(IEnumerable<FeatureDataRow> features)
        {
            Update(Caster.Downcast<FeatureDataRow<int>, FeatureDataRow>(features));
            //_ogrLayer.StartTransaction();
            //foreach (FeatureDataRow featureDataRow in features)
            //{
            //    Update(featureDataRow);
            //}
            //if (_ogrLayer.CommitTransaction() != 0)
            //    _ogrLayer.RollbackTransaction();
        }

        public void Delete(FeatureDataRow feature)
        {
            Delete((FeatureDataRow<int>)feature);
            //_ogrLayer.DeleteFeature((int) feature.GetOid());
            //feature.AcceptChanges();
        }

        public void Delete(IEnumerable<FeatureDataRow> features)
        {
            Delete(Caster.Downcast<FeatureDataRow<int>, FeatureDataRow>(features));
            //_ogrLayer.StartTransaction();
            //foreach (FeatureDataRow featureDataRow in features)
            //{
            //    Update(featureDataRow);
            //}
            //if (_ogrLayer.CommitTransaction() != 0)
            //    _ogrLayer.RollbackTransaction();
        }

        #endregion

        #region Implementation of IFeatureProvider<uint>

        public IEnumerable<int> ExecuteOidQuery(SpatialBinaryExpression query)
        {
            IExtents ext = query.SpatialExpression.Extents;
            _ogrLayer.SetSpatialFilterRect(ext.Min[Ordinates.X], ext.Min[Ordinates.Y],
                                           ext.Max[Ordinates.X], ext.Max[Ordinates.Y]);
            _ogrLayer.SetAttributeFilter("");
            _ogrLayer.ResetReading();
            OgrFeature feature = null;
            while((feature =_ogrLayer.GetNextFeature()) != null)
                yield return feature.GetFID();
        }

        public IExtents GetExtentsByOid(int oid)
        {
            IGeometry ret = GetGeometryByOid(oid);
            if (ret != null)
                return ret.Extents;
            return GeometryFactory.CreateExtents();
        }

        public IGeometry GetGeometryByOid(int oid)
        {
            IGeometry retval = null;
            _ogrLayer.SetSpatialFilterRect(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);
            _ogrLayer.SetAttributeFilter(string.Format("{0}={1}", _ogrLayer.GetFIDColumn(), oid));
            _ogrLayer.ResetReading();
            OgrFeature feature = _ogrLayer.GetNextFeature();
            if (feature != null)
            {
                byte[] wkb = null;
                feature.GetGeometryRef().ExportToWkb(wkb);
                retval = GeometryFactory.WkbReader.Read(wkb);
            }
            return retval;
        }

        public IFeatureDataRecord GetFeatureByOid(int oid)
        {
            _ogrLayer.SetSpatialFilterRect(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);
            _ogrLayer.SetAttributeFilter(string.Format("{0}={1}", _ogrLayer.GetFIDColumn(), oid));
            _ogrLayer.ResetReading();
            OgrFeature feature = _ogrLayer.GetNextFeature();
            if (feature != null)
            {
                return new OgrFeatureDataRecord(GeometryFactory, CoordinateTransformation, Encoding.Default,
                                                feature, _ogrFeatureDefn);
            }
            return null;
        }

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        public void SetTableSchema(FeatureDataTable<int> table)
        {
            SetTableSchema(table, SchemaMergeAction.AddAll | SchemaMergeAction.Key);
        }

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        /// <param name="schemaAction">Indicates how to merge schema information.</param>
        public void SetTableSchema(FeatureDataTable<int> table, SchemaMergeAction schemaAction)
        {
            DataTable dt = GetSchemaTable();
            table.MergeSchemaFrom(dt);
        }

        #endregion

        #region Implementation of IWritableFeatureProvider<uint>

        public void Insert(FeatureDataRow<int> feature)
        {
            if (!_isUpdateable) return;
            OgrFeature ogrFeature = new OgrFeature(_ogrFeatureDefn);
            OgrFeatureDataRecord ogrFeatureDataRecord = new OgrFeatureDataRecord(
                GeometryFactory, CoordinateTransformation, System.Text.Encoding.Default,
                ogrFeature, _ogrFeatureDefn);
            object[] values = null;
            int count = feature.GetValues(values);
            ogrFeatureDataRecord.SetColumnValues(0, count - 1, values);
            _ogrLayer.CreateFeature(ogrFeature);
            //this disposes ogrFeature as well;
            ogrFeatureDataRecord.Dispose();
            feature.AcceptChanges();
        }

        public void Insert(IEnumerable<FeatureDataRow<int>> features)
        {
            if (!_isUpdateable) return;

            _ogrLayer.StartTransaction();
            foreach (FeatureDataRow<int> featureDataRow in features)
            {
                Insert(featureDataRow);
            }
            if (_ogrLayer.CommitTransaction() != 0)
                _ogrLayer.RollbackTransaction();
        }

        public void Update(FeatureDataRow<int> feature)
        {
            if (!_isUpdateable) return;

            OgrFeature ogrFeature = _ogrLayer.GetFeature((int) feature.GetOid());
            OgrFeatureDataRecord ogrFeatureDataRecord = new OgrFeatureDataRecord(
                GeometryFactory, CoordinateTransformation, System.Text.Encoding.Default,
                ogrFeature, _ogrFeatureDefn);
            object[] values = null;
            int count = feature.GetValues(values);
            ogrFeatureDataRecord.SetColumnValues(0, count - 1, values);
            _ogrLayer.SetFeature(ogrFeature);
            //this disposes ogrFeature as well;
            ogrFeatureDataRecord.Dispose();
            feature.AcceptChanges();
        }

        public void Update(IEnumerable<FeatureDataRow<int>> features)
        {
            if (!_isUpdateable) return;

            _ogrLayer.StartTransaction();
            foreach (FeatureDataRow<int> featureDataRow in features)
            {
                Update(featureDataRow);
            }
            if (_ogrLayer.CommitTransaction() != 0)
                _ogrLayer.RollbackTransaction();
        }

        public void Delete(FeatureDataRow<int> feature)
        {
            if (!_isUpdateable) return;

            _ogrLayer.DeleteFeature(feature.GetOid<int>());
            feature.AcceptChanges();
        }

        public void Delete(IEnumerable<FeatureDataRow<int>> features)
        {
            if (!_isUpdateable) return;

            _ogrLayer.StartTransaction();
            foreach (FeatureDataRow<int> featureDataRow in features)
            {
                Delete(featureDataRow);
            }
            if (_ogrLayer.CommitTransaction() != 0)
                _ogrLayer.RollbackTransaction();
        }

        public struct OgrLayerDescriptor
        {
            public readonly int LayerIndex;
            public readonly string LayerName;
            public readonly string FID;
            public readonly string GeometryName;
            public readonly OgcGeometryType GeometryTypes;

            public OgrLayerDescriptor(int layerIndex, string layerName, string fid, string geometryName, OgcGeometryType geometryTypes)
            {
                LayerIndex = layerIndex;
                LayerName = layerName;
                FID = fid;
                GeometryName = geometryName;
                GeometryTypes = geometryTypes;
            }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}, {4}", LayerIndex, LayerName, FID, GeometryName, GeometryTypes);
            }
        }

        private static OgcGeometryType OgrGeometryTypeToOgc(wkbGeometryType type)
        {
            switch (type)
            {
                case OgrGeometryType.wkbNone:
                case OgrGeometryType.wkbUnknown:
                case OgrGeometryType.wkbGeometryCollection:
                case OgrGeometryType.wkbGeometryCollection25D:
                    return OgcGeometryType.GeometryCollection;
                case OgrGeometryType.wkbLinearRing:
                case OgrGeometryType.wkbLineString:
                case OgrGeometryType.wkbLineString25D:
                    return OgcGeometryType.LineString;
                case OgrGeometryType.wkbMultiLineString:
                case OgrGeometryType.wkbMultiLineString25D:
                    return OgcGeometryType.MultiLineString;
                case OgrGeometryType.wkbMultiPoint:
                case OgrGeometryType.wkbMultiPoint25D:
                    return OgcGeometryType.MultiPoint;
                case OgrGeometryType.wkbMultiPolygon:
                case OgrGeometryType.wkbMultiPolygon25D:
                    return OgcGeometryType.MultiPolygon;
                case OgrGeometryType.wkbPoint:
                case OgrGeometryType.wkbPoint25D:
                    return OgcGeometryType.Point;
                case OgrGeometryType.wkbPolygon:
                case OgrGeometryType.wkbPolygon25D:
                    return OgcGeometryType.Polygon;
            }
            throw new OgrProviderException("Should never reach here");
        }
        
        public static IEnumerable<OgrLayerDescriptor> GetLayers(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))yield break;
            using (OgrDatasource ds = Ogr.OpenShared(connectionString, 0))
            {
                int numberOfLayers = ds.GetLayerCount();
                for (int i = 0; i < numberOfLayers; i++)
                {
                    using (OgrLayer lyr = ds.GetLayerByIndex(i))
                    {
                        using (OgrFeatureDefn featureDefn = lyr.GetLayerDefn())
                        {
                            string layerName = lyr.GetName();
                            string geometryColumn = lyr.GetGeometryColumn();
                            OgrGeometryType geometryType = featureDefn.GetGeomType();
                            if (geometryType == OgrGeometryType.wkbNone || geometryType == OgrGeometryType.wkbUnknown) continue;
                            yield return new OgrLayerDescriptor(i, layerName, lyr.GetFIDColumn(), geometryColumn, OgrGeometryTypeToOgc(geometryType));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function to generate a set of providers for multi layer datasources.
        /// </summary>
        /// <param name="connectionString">Information on how to connect to the datasource</param>
        /// <param name="geometryFactory">Factoy to create geometries</param>
        /// <param name="layerIds">series of layer ids</param>
        /// <returns>a series of <see cref="OgrProvider"/>s</returns>
        public static IEnumerable<OgrProvider> GetProviders(string connectionString, IGeometryFactory geometryFactory, params int[] layerIds)
        {
            using (OgrDatasource ds = Ogr.OpenShared(connectionString, 0))
            {
                int layerCount = ds.GetLayerCount();
                foreach (int layerId in layerIds)
                {
                    if (0 <= layerId && layerId < layerCount)
                        yield return new OgrProvider(connectionString, layerId, geometryFactory);
                }
            }
        }

        #endregion
    }
}