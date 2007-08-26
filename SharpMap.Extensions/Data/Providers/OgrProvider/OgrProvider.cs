// Portions copyright 2005, 2006 - Christian Gräfe (www.sharptools.de)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using System.Diagnostics;
using OSGeo.OGR;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
using Geometry=SharpMap.Geometries.Geometry;
using OgrGeometry = OSGeo.OGR.Geometry;

namespace SharpMap.Extensions.Data.Providers.OgrProvider
{
	/// <summary>
	/// Ogr provider for SharpMap
	/// using the C# SWIG wrapper of GDAL/OGR
	/// <code>
	/// SharpMap.Layers.VectorLayer vLayerOgr = new SharpMap.Layers.VectorLayer("MapInfoLayer");
	/// vLayerOgr.DataSource = new SharpMap.Data.Providers.Ogr(@"D:\GeoData\myWorld.tab");
	/// </code>
	/// </summary>
	public class OgrProvider : IWritableVectorLayerProvider<int>
	{
		private BoundingBox _bbox;
		private String _filename;
		private readonly DataSource _ogrDataSource;
		private readonly Layer _ogrLayer;
		private bool _isOpen;
		private int? _srid = null;
		private bool _isDisposed = false;

		#region Object construction and disposal

		#region Constructors

		/// <summary>
		/// Loads a Ogr datasource with the specified layer
		/// </summary>
		/// <param name="Filename">datasource</param>
		/// <param name="LayerName">name of layer</param>
		public OgrProvider(string Filename, string LayerName)
		{
			this.Filename = Filename;

			Ogr.RegisterAll();
			_ogrDataSource = Ogr.Open(this.Filename, 1);
			_ogrLayer = _ogrDataSource.GetLayerByName(LayerName);
		}


		/// <summary>
		/// Loads a Ogr datasource with the specified layer
		/// </summary>
		/// <param name="Filename">datasource</param>
		/// <param name="LayerNum">number of layer</param>
		public OgrProvider(string Filename, int LayerNum)
		{
			this.Filename = Filename;
			Ogr.RegisterAll();

			_ogrDataSource = Ogr.Open(this.Filename, 0);
			_ogrLayer = _ogrDataSource.GetLayerByIndex(LayerNum);
		}

		/// <summary>
		/// Loads a Ogr datasource with the specified layer
		/// </summary>
		/// <param name="Filename">datasource</param>
		/// <param name="LayerNum">number of layer</param>
		/// <param name="name">Returns the name of the loaded layer</param>
		public OgrProvider(string Filename, int LayerNum, out string name)
			: this(Filename, LayerNum)
		{
			name = _ogrLayer.GetName();
		}

		/// <summary>
		/// Loads a Ogr datasource with the first layer
		/// </summary>
		/// <param name="Filename">datasource</param>
		public OgrProvider(string Filename)
			: this(Filename, 0)
		{
		}

		/// <summary>
		/// Loads a Ogr datasource with the first layer
		/// </summary>
		/// <param name="Filename">datasource</param>
		/// <param name="name">Returns the name of the loaded layer</param>
		public OgrProvider(string Filename, out string name)
			: this(Filename, 0, out name)
		{
		}

		#endregion

		#region Disposers and finalizers

		/// <summary>
		/// Finalizer
		/// </summary>
		~OgrProvider()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

		internal void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				if (_ogrDataSource != null) _ogrDataSource.Dispose();
			}
		}

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		#endregion

		#endregion

		/// <summary>
		/// return the file name of the datasource
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		/// <summary>
		/// Boundingbox of the dataset
		/// </summary>
		/// <returns>boundingbox</returns>
		public BoundingBox GetExtents()
		{
			if (_bbox.IsEmpty)
			{
				Envelope _OgrEnvelope = new Envelope();
				int i = _ogrLayer.GetExtent(_OgrEnvelope, 1);

				_bbox = new BoundingBox(_OgrEnvelope.MinX,
				                        _OgrEnvelope.MinY,
				                        _OgrEnvelope.MaxX,
				                        _OgrEnvelope.MaxY);
			}

			return _bbox;
		}

		public FeatureDataSet ExecuteQuery(string query)
		{
			return ExecuteQuery(query, null);
		}

		public FeatureDataSet ExecuteQuery(string query, OgrGeometry filter)
		{
			try
			{
				FeatureDataSet ds = new FeatureDataSet();
				FeatureDataTable myDt = new FeatureDataTable();

				Layer results = _ogrDataSource.ExecuteSQL(query, filter, "");

				//reads the column definition of the layer/feature
				readColumnDefinition(myDt, results);

				Feature _OgrFeature;
				results.ResetReading();
				while ((_OgrFeature = results.GetNextFeature()) != null)
				{
					FeatureDataRow _dr = myDt.NewRow();
					for (int iField = 0; iField < _OgrFeature.GetFieldCount(); iField++)
					{
						if (myDt.Columns[iField].DataType == typeof (String))
							_dr[iField] = _OgrFeature.GetFieldAsString(iField);
						else if (myDt.Columns[iField].GetType() == typeof (Int32))
							_dr[iField] = _OgrFeature.GetFieldAsInteger(iField);
						else if (myDt.Columns[iField].GetType() == typeof (Double))
							_dr[iField] = _OgrFeature.GetFieldAsDouble(iField);
						else
							_dr[iField] = _OgrFeature.GetFieldAsString(iField);
					}

					_dr.Geometry = parseOgrGeometry(_OgrFeature.GetGeometryRef());
					myDt.AddRow(_dr);
				}
				ds.Tables.Add(myDt);
				_ogrDataSource.ReleaseResultSet(results);

				return ds;
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.ToString());
				return new FeatureDataSet();
			}
		}

		/// <summary>
		/// Returns the number of features in the dataset
		/// </summary>
		/// <returns>number of features</returns>
		public int GetFeatureCount()
		{
			return _ogrLayer.GetFeatureCount(1);
		}

		/// <summary>
		/// Returns the features that intersects with <paramref name="geometry"/>.
		/// </summary>
		/// <param name="geometry">Geometry</param>
		/// <param name="table"></param>
		public void ExecuteIntersectionQuery(Geometry geometry, FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a FeatureDataRow based on an object id.
		/// </summary>
		/// <param name="oid">The id of the feature to retrieve.</param>
		/// <returns>FeatureDataRow</returns>
		public FeatureDataRow<int> GetFeature(int oid)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the connection ID of the datasource
		/// </summary>
		public string ConnectionId
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Opens the datasource
		/// </summary>
		public void Open()
		{
			_isOpen = true;
		}

		/// <summary>
		/// Closes the datasource
		/// </summary>
		public void Close()
		{
			_isOpen = false;
		}

		/// <summary>
		/// Returns true if the datasource is currently open
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen && !_isDisposed; }
		}

		/// <summary>
		/// Returns geometry Object IDs whose bounding box intersects 'bbox'
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<int> GetObjectIdsInView(BoundingBox bbox)
		{
			_ogrLayer.SetSpatialFilterRect(bbox.Min.X, bbox.Min.Y, bbox.Max.X, bbox.Max.Y);
			Feature _OgrFeature = null;
			_ogrLayer.ResetReading();

			List<int> _ObjectIDs = new List<int>();

			while ((_OgrFeature = _ogrLayer.GetNextFeature()) != null)
			{
				_ObjectIDs.Add(_OgrFeature.GetFID());
				_OgrFeature.Dispose();
			}

			return _ObjectIDs;
		}

		/// <summary>
		/// Returns the geometry corresponding to the Object ID
		/// </summary>
		/// <param name="oid">Object ID</param>
		/// <returns>geometry</returns>
		public Geometry GetGeometryById(int oid)
		{
			using (Feature _OgrFeature = _ogrLayer.GetFeature(oid))
			{
				return parseOgrGeometry(_OgrFeature.GetGeometryRef());
			}
		}

		/// <summary>
		/// Returns geometries within the specified bounding box
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox bbox)
		{
			List<Geometry> geoms = new List<Geometry>();

			_ogrLayer.SetSpatialFilterRect(bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
			Feature _OgrFeature;

			_ogrLayer.ResetReading();

			while ((_OgrFeature = _ogrLayer.GetNextFeature()) != null)
			{
				geoms.Add(parseOgrGeometry(_OgrFeature.GetGeometryRef()));
				_OgrFeature.Dispose();
			}

			return geoms;
		}

		/// <summary>
		/// The id of a well-known spatial reference system.
		/// </summary>
		public int? Srid
		{
			get { return _srid; }
			set { _srid = value; }
		}

		/// <summary>
		/// Returns the data associated with all the geometries that are intersected by 'geom'
		/// </summary>
		/// <param name="bbox">Geometry to intersect with</param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(BoundingBox bbox, FeatureDataSet ds)
		{
			FeatureDataTable myDt = new FeatureDataTable();

			_ogrLayer.SetSpatialFilterRect(bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);

			//reads the column definition of the layer/feature
			readColumnDefinition(myDt, _ogrLayer);

			Feature _OgrFeature;
			_ogrLayer.ResetReading();
			while ((_OgrFeature = _ogrLayer.GetNextFeature()) != null)
			{
				FeatureDataRow _dr = myDt.NewRow();
				for (int iField = 0; iField < _OgrFeature.GetFieldCount(); iField++)
				{
					if (myDt.Columns[iField].DataType == Type.GetType("System.String"))
						_dr[iField] = _OgrFeature.GetFieldAsString(iField);
					else if (myDt.Columns[iField].GetType() == Type.GetType("System.Int32"))
						_dr[iField] = _OgrFeature.GetFieldAsInteger(iField);
					else if (myDt.Columns[iField].GetType() == Type.GetType("System.Double"))
						_dr[iField] = _OgrFeature.GetFieldAsDouble(iField);
					else
						_dr[iField] = _OgrFeature.GetFieldAsString(iField);
				}

				_dr.Geometry = parseOgrGeometry(_OgrFeature.GetGeometryRef());
				myDt.AddRow(_dr);
			}

			ds.Tables.Add(myDt);
		}

		/// <summary>
		/// Returns the data associated with all the geometries that are intersected by 'geom'
		/// </summary>
		/// <param name="geom">Geometry to intersect with</param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
		{
			throw new NotImplementedException();
		}

		#region IWritableVectorLayerProvider<int> Members

		public void Insert(FeatureDataRow<int> feature)
		{
			throw new NotImplementedException();
		}

		public void Insert(IEnumerable<FeatureDataRow<int>> features)
		{
			throw new NotImplementedException();
		}

		public void Update(FeatureDataRow<int> feature)
		{
			throw new NotImplementedException();
		}

		public void Update(IEnumerable<FeatureDataRow<int>> features)
		{
			throw new NotImplementedException();
		}

		public void Delete(FeatureDataRow<int> feature)
		{
			throw new NotImplementedException();
		}

		public void Delete(IEnumerable<FeatureDataRow<int>> features)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVectorLayerProvider<int> Members

		public void SetTableSchema(FeatureDataTable<int> table)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVectorLayerProvider Members

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		public IFeatureDataReader ExecuteIntersectionQuery(Geometry geom)
		{
			throw new NotImplementedException();
		}

		public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
		{
			throw new NotImplementedException();
		}

		public void SetTableSchema(FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILayerProvider Members

		public ICoordinateTransformation CoordinateTransformation
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public ICoordinateSystem SpatialReference
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Private helper methods

		/// <summary>
		/// Reads the field types from the OgrFeatureDefinition -> OgrFieldDefinition
		/// </summary>
		/// <param name="fdt">FeatureDatatTable</param>
		/// <param name="oLayer">OgrLayer</param>
		private void readColumnDefinition(FeatureDataTable fdt, Layer oLayer)
		{
			using (FeatureDefn _OgrFeatureDefn = oLayer.GetLayerDefn())
			{
				int iField;

				for (iField = 0; iField < _OgrFeatureDefn.GetFieldCount(); iField++)
				{
					using (FieldDefn _OgrFldDef = _OgrFeatureDefn.GetFieldDefn(iField))
					{
						FieldType type;
						switch ((type = _OgrFldDef.GetFieldType()))
						{
							case FieldType.OFTInteger:
								fdt.Columns.Add(_OgrFldDef.GetName(), Type.GetType("System.Int32"));
								break;
							case FieldType.OFTReal:
								fdt.Columns.Add(_OgrFldDef.GetName(), Type.GetType("System.Double"));
								break;
							case FieldType.OFTString:
								fdt.Columns.Add(_OgrFldDef.GetName(), Type.GetType("System.String"));
								break;
							case FieldType.OFTWideString:
								fdt.Columns.Add(_OgrFldDef.GetName(), Type.GetType("System.String"));
								break;
							default:
								{
									//fdt.Columns.Add(_OgrFldDef.GetName(), System.Type.GetType("System.String"));
									Debug.WriteLine("Not supported type: " + type + " [" + _OgrFldDef.GetName() + "]");
									break;
								}
						}
					}
				}
			}
		}

		private Geometry parseOgrGeometry(OgrGeometry OgrGeometry)
		{
			byte[] wkbBuffer = new byte[OgrGeometry.WkbSize()];
			int i = OgrGeometry.ExportToWkb(wkbBuffer);
			return GeometryFromWkb.Parse(wkbBuffer);
		}

		#endregion
	}
}