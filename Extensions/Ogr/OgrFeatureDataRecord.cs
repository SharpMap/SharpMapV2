using System;
using System.Data;
using System.IO;
using System.Text;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Styles;
using OgrFeature = OSGeo.OGR.Feature;
using OgrFeatureDefn = OSGeo.OGR.FeatureDefn;
using OgrFieldType = OSGeo.OGR.FieldType;
using OgrGeometry = OSGeo.OGR.Geometry;

namespace SharpMap.Data.Providers
{

    public class OgrFeatureDataRecord : IFeatureDataRecord, IDisposable
    {
        private readonly Encoding _ogrEncoding;
        private readonly IGeometryFactory _geometryFactory;
        private ICoordinateTransformation _coordinateTransformation;
        private OgrFeature _ogrFeature;
        private readonly OgrFeatureDefn _ogrFeatureDefn;

        internal OgrFeatureDataRecord(IGeometryFactory geomFactory, ICoordinateTransformation coordinateTransformation, Encoding ogrEncoding, OgrFeatureDefn ogrFeatureDefn)
            :this(geomFactory, coordinateTransformation, ogrEncoding, ogrFeatureDefn, null)
        {
            
        }
        internal OgrFeatureDataRecord(IGeometryFactory geomFactory, ICoordinateTransformation coordinateTransformation, Encoding ogrEncoding, OgrFeatureDefn ogrFeatureDefn, OgrFeature feature)
        {
            _ogrEncoding = ogrEncoding;
            _geometryFactory = geomFactory;
            _coordinateTransformation = coordinateTransformation;
            _ogrFeatureDefn = ogrFeatureDefn;
            _ogrFeature = feature;
        }

        protected OgrFeature Feature
        {
            get { return _ogrFeature; }
            set
            {
                if (_ogrFeature != null)
                    _ogrFeature.Dispose();
                _ogrFeature = value;
            }
        }

        public void SetColumnValue(Int32 ordinal, Object value)
        {
            DateTime dat;
            Int32 intValue;
            Double dblValue;

            CheckIndex(ordinal);
            switch (_ogrFeatureDefn.GetFieldDefn(ordinal).GetFieldType())
            {
                case OgrFieldType.OFTDate:
                    dat = (DateTime)value;
                    _ogrFeature.SetField(ordinal, dat.Year, dat.Month, dat.Day, 0, 0, 0, 0);
                    break;

                case OgrFieldType.OFTTime:
                    dat = (DateTime)value;
                    _ogrFeature.SetField(ordinal, 0, 0, 0, dat.Hour, dat.Minute, dat.Second, 0);
                    break;

                case OgrFieldType.OFTDateTime:
                    dat = (DateTime)value;
                    _ogrFeature.SetField(ordinal, dat.Year, dat.Month, dat.Day, dat.Hour, dat.Minute, dat.Second, 0);
                    break;

                case OgrFieldType.OFTInteger:
                    intValue = Convert.ToInt32(value);
                    _ogrFeature.SetField(ordinal, intValue);
                    break;

                case OgrFieldType.OFTReal:
                    dblValue = Convert.ToDouble(value);
                    _ogrFeature.SetField(ordinal, dblValue);
                    break;

                case OgrFieldType.OFTString:
                case OgrFieldType.OFTWideString:
                    String strValue = Convert.ToString(value);
                    //byte[] str = UTF8Encoding.Convert(Encoding.UTF8, _ogrEncoding, strValue.)
                    //System.Text.(CultureInfo.CurrentUICulture.)
                    _ogrFeature.SetField(ordinal, strValue);
                    break;

                case OgrFieldType.OFTIntegerList:
                    Int32[] intArray = (int[])value;
                    _ogrFeature.SetFieldIntegerList(ordinal, intArray.Length, intArray);
                    break;

                case OgrFieldType.OFTRealList:
                    Double[] dblArray = (Double[])value;
                    _ogrFeature.SetFieldDoubleList(ordinal, dblArray.Length, dblArray);
                    break;

                case OgrFieldType.OFTStringList:
                case OgrFieldType.OFTWideStringList:
                    String[] strArray = (String[])value;
                    _ogrFeature.SetFieldStringList(ordinal, strArray);
                    break;
                default:
                    throw new InvalidDataException();
            }
        }

        public void SetColumnValues(Int32 startOrdinal, Int32 endOrdinal, Object[] values)
        {
            for (int i = startOrdinal; i < endOrdinal; i++)
                SetColumnValue(i, values[i]);
        }

        private void CheckIndex(Int32 ordinal)
        {
            if (ordinal < 1 || _ogrFeatureDefn.GetFieldCount() <= ordinal)
                throw new IndexOutOfRangeException();
        }
        #region Implementation of IDataRecord

        /// <summary>
        /// Ruft den Namen des zu suchenden Felds ab.
        /// </summary>
        /// <returns>
        /// Der Name des Felds oder eine leere Zeichenfolge (""), wenn kein zurückzugebender Wert vorhanden ist.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public string GetName(int i)
        {
            return _ogrFeatureDefn.GetFieldDefn(i).GetName();
        }

        /// <summary>
        /// Ruft die Datentypinformationen für das angegebene Feld ab.
        /// </summary>
        /// <returns>
        /// Die Datentypinformationen für das angegebene Feld.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        /// </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception><filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        /// <summary>
        /// Ruft die <see cref="T:System.Type"/>-Informationen ab, die dem Typ des <see cref="T:System.Object"/> entsprechen, das von <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/> zurückgegeben wird.
        /// </summary>
        /// <returns>
        /// Die <see cref="T:System.Type"/>-Informationen, die dem Typ des <see cref="T:System.Object"/> entsprechen, das von <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/> zurückgegeben wird.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public Type GetFieldType(int i)
        {
            OgrFieldType ft =
                _ogrFeatureDefn.GetFieldDefn(i).GetFieldType();

            switch (ft)
            {
                case (OgrFieldType.OFTDate):
                case (OgrFieldType.OFTTime):
                case (OgrFieldType.OFTDateTime):
                    return typeof(DateTime);
                case OgrFieldType.OFTInteger:
                    return typeof(Int32);
                case OgrFieldType.OFTIntegerList:
                    return typeof(Int32[]);
                case OgrFieldType.OFTReal:
                    return typeof(Double);
                case OgrFieldType.OFTRealList:
                    return typeof(Int32[]);
                case OgrFieldType.OFTString:
                case OgrFieldType.OFTWideString:
                    return typeof(String);
                case OgrFieldType.OFTStringList:
                case OgrFieldType.OFTWideStringList:
                    return typeof(String[]);
                case OgrFieldType.OFTBinary:
                    return typeof(Byte[]);
                default:
                    throw new OgrProviderException("Unknown OgrFieldType");
            }
        }

        /// <summary>
        /// Zurückgeben des Werts des angegebenen Felds.
        /// </summary>
        /// <returns>
        /// Das <see cref="T:System.Object"/>, das bei Rückgabe den Feldwert enthält.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public object GetValue(int i)
        {
            OgrFieldType ft =
                _ogrFeatureDefn.GetFieldDefn(i).GetFieldType();

            switch (ft)
            {
                case (OgrFieldType.OFTDate):
                case (OgrFieldType.OFTTime):
                case (OgrFieldType.OFTDateTime):
                    return GetDateTime(i);

                case OgrFieldType.OFTInteger:
                    return _ogrFeature.GetFieldAsInteger(i);

                case OgrFieldType.OFTIntegerList:
                    Int32 count;
                    Int32[] iresult = _ogrFeature.GetFieldAsIntegerList(i, out count);
                    return iresult;

                case OgrFieldType.OFTReal:
                    return _ogrFeature.GetFieldAsDouble(i);

                case OgrFieldType.OFTRealList:
                    Double[] dresult = _ogrFeature.GetFieldAsDoubleList(i, out count);
                    return dresult;

                case OgrFieldType.OFTString:
                case OgrFieldType.OFTWideString:
                    return _ogrFeature.GetFieldAsString(i);

                case OgrFieldType.OFTStringList:
                case OgrFieldType.OFTWideStringList:
                    String[] sresults = _ogrFeature.GetFieldAsStringList(i);
                    return sresults;

                case OgrFieldType.OFTBinary:
                    //throw new OgrProviderException("Don't know how to get Field of type OFTBinary");
                    return null;
                default:
                    throw new OgrProviderException("Unknown OgrFieldType");
            }
        }

        /// <summary>
        /// Ruft alle Attributfelder in der Auflistung für den aktuellen Datensatz ab.
        /// </summary>
        /// <returns>
        /// Die Anzahl der Instanzen von <see cref="T:System.Object"/> im Array.
        /// </returns>
        /// <param name="values">Ein Array vom Typ <see cref="T:System.Object"/>, in das die Attributfelder kopiert werden sollen. 
        ///             </param><filterpriority>2</filterpriority>
        public int GetValues(object[] values)
        {
            //values = new object[_ogrFeatureDefn.GetFieldCount()+1];
            values[0] = _ogrFeature.GetFID();
            for (Int32 i = 1; i < _ogrFeature.GetFieldCount(); i++)
                values[i] = GetValue(i);
            return values.Length;
        }

        /// <summary>
        /// Zurückgeben des Indexes des benannten Felds.
        /// </summary>
        /// <returns>
        /// Der Index des benannten Felds.
        /// </returns>
        /// <param name="name">Der Name des gesuchten Felds. 
        ///             </param><filterpriority>2</filterpriority>
        public int GetOrdinal(string name)
        {
            return _ogrFeature.GetFieldIndex(name) + 1;
        }

        /// <summary>
        /// Ruft den Wert der angegebenen Spalte als booleschen Wert ab.
        /// </summary>
        /// <returns>
        /// Der Wert der Spalte.
        /// </returns>
        /// <param name="i">Die nullbasierte Ordnungszahl der Spalte. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft den Wert der angegebenen Spalte als 8-Bit-Ganzzahl ohne Vorzeichen ab.
        /// </summary>
        /// <returns>
        /// Der Wert der angegebenen Spalte als 8-Bit-Ganzzahl ohne Vorzeichen.
        /// </returns>
        /// <param name="i">Die nullbasierte Ordnungszahl der Spalte. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Liest beginnend am angegebenen Pufferoffset einen Stream von Bytes aus dem angegebenen Spaltenoffset als Array in den Puffer.
        /// </summary>
        /// <returns>
        /// Die tatsächlich gelesene Anzahl von Bytes.
        /// </returns>
        /// <param name="i">Die nullbasierte Ordnungszahl der Spalte. 
        ///             </param><param name="fieldOffset">Der Index im Feld, an dem der Lesevorgang beginnen soll. 
        ///             </param><param name="buffer">Der Puffer, in den der Bytestream gelesen werden soll. 
        ///             </param><param name="bufferoffset">Der Index für <paramref name="buffer"/>, an dem der Lesevorgang beginnen soll. 
        ///             </param><param name="length">Die Anzahl der zu lesenden Bytes. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft den Zeichenwert der angegebenen Spalte ab.
        /// </summary>
        /// <returns>
        /// Der Zeichenwert der angegebenen Spalte.
        /// </returns>
        /// <param name="i">Die nullbasierte Ordnungszahl der Spalte. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Liest beginnend am angegebenen Pufferoffset einen Stream von Zeichen aus dem angegebenen Spaltenoffset als Array in den Puffer.
        /// </summary>
        /// <returns>
        /// Die tatsächlich gelesene Anzahl von Zeichen.
        /// </returns>
        /// <param name="i">Die nullbasierte Ordnungszahl der Spalte. 
        ///             </param><param name="fieldoffset">Der Index in der Zeile, an dem der Lesevorgang beginnen soll. 
        ///             </param><param name="buffer">Der Puffer, in den der Bytestream gelesen werden soll. 
        ///             </param><param name="bufferoffset">Der Index für <paramref name="buffer"/>, an dem der Lesevorgang beginnen soll. 
        ///             </param><param name="length">Die Anzahl der zu lesenden Bytes. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gibt den GUID-Wert des angegebenen Felds zurück.
        /// </summary>
        /// <returns>
        /// Der GUID-Wert des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft den Wert des angegebenen Felds als 16-Bit-Ganzzahl mit Vorzeichen ab.
        /// </summary>
        /// <returns>
        /// Der Wert des angegebenen Felds als 16-Bit-Ganzzahl mit Vorzeichen.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft den Wert des angegebenen Felds als 32-Bit-Ganzzahl mit Vorzeichen ab.
        /// </summary>
        /// <returns>
        /// Der Wert des angegebenen Felds als 32-Bit-Ganzzahl mit Vorzeichen.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public int GetInt32(int i)
        {
            Int32 value = _ogrFeature.GetFieldAsInteger(i);
            return value;
        }

        /// <summary>
        /// Ruft den Wert des angegebenen Felds als 64-Bit-Ganzzahl mit Vorzeichen ab.
        /// </summary>
        /// <returns>
        /// Der Wert des angegebenen Felds als 64-Bit-Ganzzahl mit Vorzeichen.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public long GetInt64(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft die Gleitkommazahl mit einfacher Genauigkeit des angegebenen Felds ab.
        /// </summary>
        /// <returns>
        /// Die Gleitkommazahl mit einfacher Genauigkeit des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft die Gleitkommazahl mit doppelter Genauigkeit des angegebenen Felds ab.
        /// </summary>
        /// <returns>
        /// Die Gleitkommazahl mit doppelter Genauigkeit des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public double GetDouble(int i)
        {
            Double value = _ogrFeature.GetFieldAsDouble(i);
            return value;
        }

        /// <summary>
        /// Ruft den Zeichenfolgenwert des angegebenen Felds ab.
        /// </summary>
        /// <returns>
        /// Der Zeichenfolgenwert des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public string GetString(int i)
        {
            String value = _ogrFeature.GetFieldAsString(i);
            return value;
        }

        /// <summary>
        /// Ruft den numerischen Wert für die feste Position des angegebenen Felds ab.
        /// </summary>
        /// <returns>
        /// Der numerische Wert für die feste Position des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ruft den Datums- und Uhrzeitwert des angegebenen Felds ab.
        /// </summary>
        /// <returns>
        /// Der Datums- und Uhrzeitwert des angegebenen Felds.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public DateTime GetDateTime(int i)
        {
            Int32 dd, mm, yy, h, m, s, tz;
            _ogrFeature.GetFieldAsDateTime(i, out yy, out mm, out dd, out h, out m, out s, out tz);
            if (yy == 0) return DateTime.MinValue;
            return new DateTime(yy, mm, dd, h, m, s, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Gibt ein <see cref="T:System.Data.IDataReader"/> für die angegebene Ordnungszahl der Spalte.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Data.IDataReader"/>.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gibt zurück, ob das angegebene Feld auf NULL festgelegt ist.
        /// </summary>
        /// <returns>
        /// true, wenn das angegebene Feld auf NULL festgelegt ist, andernfalls false.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public bool IsDBNull(int i)
        {
            return _ogrFeature.IsFieldSet(i);
        }

        /// <summary>
        /// Ruft die Anzahl der Spalten in der aktuellen Zeile ab.
        /// </summary>
        /// <returns>
        /// Wenn die Position außerhalb eines gültigen Recordsets liegt, dann 0 (null), andernfalls die Anzahl der Spalten im aktuellen Datensatz. Der Standardwert ist -1.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int FieldCount
        {
            get { return _ogrFeatureDefn.GetFieldCount(); }
        }

        /// <summary>
        /// Ruft die Spalte ab, die sich am angegebenen Index befindet.
        /// </summary>
        /// <returns>
        /// Die Spalte am angegebenen Index als <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="i">Der nullbasierte Index der abzurufenden Spalte. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        /// <summary>
        /// Ruft die Spalte mit dem angegebenen Namen ab.
        /// </summary>
        /// <returns>
        /// Die Spalte mit dem angegebenen Namen als <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="name">Der Name der zu suchenden Spalte. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Es wurde keine Spalte mit dem angegebenen Namen gefunden. 
        ///             </exception><filterpriority>2</filterpriority>
        object IDataRecord.this[string name]
        {
            get
            {
                Int32 index = _ogrFeatureDefn.GetFieldIndex(name);
                return GetValue(index);
            }
        }

        #endregion

        #region Implementation of IFeatureDataRecord

        /// <summary>
        /// Gets the geometry for the current position in the reader.
        /// </summary>
        public IGeometry Geometry
        {
            get
            {
                OgrGeometry geom = _ogrFeature.GetGeometryRef();
                if (geom != null)
                {
                    geom.FlattenTo2D();
                    byte[] buffer = new byte[geom.WkbSize()];
                    geom.ExportToWkb(buffer);
                    return GetResultGeometry(_geometryFactory.WkbReader.Read(buffer));
                }
                return null;
            }
        }

        private IGeometry GetResultGeometry(IGeometry geom)
        {
            if (_coordinateTransformation == null || _coordinateTransformation.MathTransform.IsIdentity)
                return geom;

            return _coordinateTransformation.Transform(geom, _geometryFactory);
        }
        /// <summary>
        /// Gets the extents for the current position in the reader.
        /// </summary>
        public IExtents Extents
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the object ID for the record.
        /// </summary>
        /// <returns>
        /// The object ID for the record, or <see langword="null"/> 
        /// if <see cref="IFeatureDataRecord.HasOid"/> is <see langword="false"/>.
        /// </returns>
        public object GetOid()
        {
            return _ogrFeature.GetFID();
        }

        /// <summary>
        /// Gets a value indicating if the feature record
        /// has an object Identifier (OID).
        /// </summary>
        public bool HasOid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this feature record
        /// has been fully loaded from the data source.
        /// </summary>
        // eventually fully load a record, yet this won't be able to record it.
        public bool IsFullyLoaded
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the object ID.
        /// </summary>
        /// <remarks>
        /// OidType gets a <see cref="Type"/> which can be used
        /// to call GetOid with generic type parameters in order to avoid 
        /// boxing. If <see cref="IFeatureDataRecord.HasOid"/> returns false, <see cref="IFeatureDataRecord.OidType"/>
        /// returns <see langword="null"/>.
        /// </remarks>
        public Type OidType
        {
            get { return typeof(Int32); }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransformation; }
            set { _coordinateTransformation = value; }
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ogrFeature != null)
                {
                    _ogrFeature.Dispose();
                    //_ogrFeature = null;
                }

                //System.Diagnostics.Debug.WriteLine(string.Format("OgrFeatureDataRecord.Dispose._ogrFeatureDefn.GetReferenceCount() = {0}", _ogrFeatureDefn.GetReferenceCount()));
                //if (_ogrFeatureDefn != null && _ogrFeatureDefn.GetReferenceCount() == 0)
                //{
                //    _ogrFeatureDefn.Dispose();
                //}
            }
        }

        public GeometryStyle GeometryStyle
        {
            get
            {
                return ParseGeometryStyle(_ogrFeature.GetStyleString());
            }
        }

        private GeometryStyle ParseGeometryStyle(string ogrStyleString)
        {
            return null;
        }

        public LabelStyle LabelStyle
        {
            get
            {
                return ParseLabelString(_ogrFeature.GetStyleString());
            }
        }

        private LabelStyle ParseLabelString(string ogrStyleString)
        {
            throw new NotImplementedException();
        }
    }
}