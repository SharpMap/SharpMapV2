using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;
using OgrLayer = OSGeo.OGR.Layer;
using OgrFeature = OSGeo.OGR.Feature;
using OgrFeatureDefn = OSGeo.OGR.FeatureDefn;

namespace SharpMap.Data.Providers
{
    public class OgrFeatureDataReader : IFeatureDataReader
    {

        private readonly IGeometryFactory _geometryFactory;
        private ICoordinateTransformation _coordinateTransformation;
        private readonly OgrProvider _ogrProvider;
        private readonly OgrLayer _ogrLayer;
        private readonly OgrFeatureDefn _ogrFeatureDefn;
        private OgrFeatureDataRecord _current;

        public OgrFeatureDataReader(OgrProvider provider, FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            _geometryFactory = provider.GeometryFactory;
            _ogrProvider = provider;
            //We don't care for options
            //options
            Console.WriteLine(provider.OgrLayer.GetRefCount());
            _ogrLayer = provider.OgrLayer;
            Console.WriteLine(provider.OgrLayer.GetRefCount());
            FeatureQueryExpressionToOgr(_ogrLayer, query);
            _ogrFeatureDefn = provider.OgrFeatureDefn;
        }

        private static void FeatureQueryExpressionToOgr(OgrLayer layer, FeatureQueryExpression query)
        {
            if (query.IsSpatialPredicateNonEmpty)
                SpatialBinaryExpressionToOgr(layer, query.SpatialPredicate);

            List<String> whereClauses = new List<string>();
            if (query.IsOidPredicateNonEmpty)
                whereClauses.Add(OidCollectionExpressionToOgr(query.OidPredicate));
            if (query.IsSingleAttributePredicateNonEmpty)
                whereClauses.Add(AttributeBinaryExpressionToOgrSql(query.SingleAttributePredicate));
            if (query.IsMultiAttributePredicateNonEmpty)
                whereClauses.Add(AttributesPredicateExpressionToOgr(query.MultiAttributePredicate));

            layer.SetAttributeFilter(string.Join(" AND ", whereClauses.ToArray()));
            //this is done automatically
            //_ogrLayer.ResetReading();
        }

        private static string AttributesPredicateExpressionToOgr(AttributesPredicateExpression multiAttributePredicate)
        {
            //I don't know what this is supposed to be
            return "";

        }

        private static string AttributeBinaryExpressionToOgrSql(AttributeBinaryExpression singleAttributePredicate)
        {
            if (singleAttributePredicate.HasCollectionValueExpression)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", ((PropertyNameExpression) singleAttributePredicate.Left).PropertyName);

            BinaryOperatorToOgrSql(sb, singleAttributePredicate.Op, (LiteralExpression)singleAttributePredicate.Right);
            return sb.ToString();
        }


        private static void BinaryOperatorToOgrSql(StringBuilder sb, BinaryOperator op, LiteralExpression ex)
        {
            string format = "";
            switch (op)
            {
                case BinaryOperator.And:
                    format = "AND {0}";
                    break;
                case BinaryOperator.Equals:
                    format = " = {0}";
                    break;
                case BinaryOperator.GreaterThan:
                    format = "> {0}";
                    break;
                case BinaryOperator.GreaterThanOrEqualTo:
                    format = ">= {0}";
                    break;
                case BinaryOperator.LessThan:
                    format = "< {0}";
                    break;
                case BinaryOperator.LessThanOrEqualTo:
                    format = "<= {0}";
                    break;
                case BinaryOperator.Like:
                    format = "LIKE {0}";
                    break;
                case BinaryOperator.NotEquals:
                    format = "!= {0}";
                    break;
                default:
                    throw new ArgumentException("op");
            }
            sb.AppendFormat(format, LiteralExpressionToOgrSql(ex));
        }

        private static string LiteralExpressionToOgrSql(LiteralExpression literalExpression)
        {
            string format = literalExpression.Value.GetType() == typeof (String)
                                ? "'{0}'"
                                : "{0}";
            return string.Format(format, literalExpression.Value);
        }

        private static string OidCollectionExpressionToOgr(OidCollectionExpression oidPredicate)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var oidValue in oidPredicate.OidValues)
                builder.AppendFormat("{0}, ", oidValue);
            builder.Remove(builder.Length - 3, 2);
            return string.Format("FID IN({0})", builder.ToString());
        }

        private static void SpatialBinaryExpressionToOgr(OgrLayer layer, SpatialBinaryExpression expression)
        {
            IExtents extents = expression.SpatialExpression.Extents;
            layer.SetSpatialFilterRect(extents.Min[Ordinates.X], extents.Min[Ordinates.Y],
                                       extents.Max[Ordinates.X], extents.Max[Ordinates.Y]);

        }
        #region Implementation of IDisposable

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_current != null)
                {
                    _current.Dispose();
                    _current = null;
                }
                //if (_ogrFeatureDefn.GetReferenceCount() == 0)
                //    _ogrFeatureDefn.Dispose();

                //if (_ogrLayer.GetRefCount() == 0)
                //    _ogrLayer.Dispose();

            }
        }

        #endregion

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
            return _current.GetName(i);
        }

        /// <summary>
        /// Ruft die Datentypinformationen für das angegebene Feld ab.
        /// </summary>
        /// <returns>
        /// Die Datentypinformationen für das angegebene Feld.
        /// </returns>
        /// <param name="i">Der Index des zu suchenden Felds. 
        ///             </param><exception cref="T:System.IndexOutOfRangeException">Der übergebene Index lag außerhalb des Bereichs von 0 (null) bis <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        ///             </exception><filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            return _current.GetDataTypeName(i);
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
            return _current.GetFieldType(i);
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
            return _current.GetValue(i);
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
            return _current.GetValues(values);
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
            return _current.GetOrdinal(name);
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
            return _current.GetBoolean(i);
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
            return _current.GetByte(i);
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
            return _current.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
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
            return _current.GetChar(i);
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
            return _current.GetChars(i, fieldoffset, buffer, bufferoffset,length);
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
            return _current.GetGuid(i);
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
            return _current.GetInt16(i);
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
            return _current.GetInt32(i);
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
            return _current.GetInt64(i);
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
            return _current.GetFloat(i);
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
            return _current.GetDouble(i);
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
            return _current.GetString(i);
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
            return _current.GetDecimal(i);
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
            return _current.GetDateTime(i);
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
            return _current.GetData(i);
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
            return _current.IsDBNull(i);
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
            get{return _current.FieldCount;}
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
            get { return ((IDataRecord)_current)[i]; }
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
            get { return ((IDataRecord)_current)[name]; }
        }

        #endregion

        #region Implementation of IDataReader

        /// <summary>
        /// Schließt das <see cref="T:System.Data.IDataReader"/>-Objekt.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Gibt eine <see cref="T:System.Data.DataTable"/> zurück, die die Spaltenmetadaten der <see cref="T:System.Data.IDataReader"/>-Klasse beschreibt.
        /// </summary>
        /// <returns>
        /// Eine <see cref="T:System.Data.DataTable"/>, die die Spaltenmetadaten beschreibt.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">Die <see cref="T:System.Data.IDataReader"/> ist geschlossen. 
        ///             </exception><filterpriority>2</filterpriority>
        public DataTable GetSchemaTable()
        {
            return _ogrProvider.GetSchemaTable();
        }

        /// <summary>
        /// Setzt den Datenreader beim Lesen der Ergebnisse von SQL-Batchanweisungen auf das nächste Ergebnis.
        /// </summary>
        /// <returns>
        /// true, wenn weitere Zeilen vorhanden sind, andernfalls false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool NextResult()
        {
            return false;
        }

        /// <summary>
        /// Setzt den <see cref="T:System.Data.IDataReader"/> auf den nächsten Datensatz.
        /// </summary>
        /// <returns>
        /// true, wenn weitere Zeilen vorhanden sind, andernfalls false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool Read()
        {
            OgrFeature feature = _ogrLayer.GetNextFeature();
            if (feature != null)
            {
                if (_current != null) _current.Dispose();
                _current = new OgrFeatureDataRecord(_geometryFactory, _coordinateTransformation, Encoding.Default, feature, _ogrFeatureDefn);

                return true;
            }
            return false;

        }

        /// <summary>
        /// Ruft einen Wert ab, der die Tiefe der Schachtelung für die aktuelle Zeile angibt.
        /// </summary>
        /// <returns>
        /// Die Ebene der Schachtelung.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Depth
        {
            get { return 0; }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt, ob der Datenreader geschlossen ist.
        /// </summary>
        /// <returns>
        /// true, wenn der Datenreader geschlossen ist, andernfalls false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsClosed
        {
            get { return false; }
        }

        /// <summary>
        /// Ruft die Anzahl der durch die Ausführung der SQL-Anweisung geänderten, eingefügten oder gelöschten Zeilen ab.
        /// </summary>
        /// <returns>
        /// Die Anzahl der geänderten, eingefügten oder gelöschten Zeilen. 0 (null), wenn keine Zeilen betroffen sind oder die Ausführung der Anweisung fehlgeschlagen ist, und -1 bei SELECT-Anweisungen.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int RecordsAffected
        {
            get { return _ogrLayer.GetFeatureCount(1); }
        }

        #endregion

        #region Implementation of IFeatureDataRecord

        /// <summary>
        /// Gets the geometry for the current position in the reader.
        /// </summary>
        public IGeometry Geometry
        {
            get { return _current.Geometry; }
        }

        /// <summary>
        /// Gets the extents for the current position in the reader.
        /// </summary>
        public IExtents Extents
        {
            get { return _current.Extents; }
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
            return _current.GetOid();
        }

        /// <summary>
        /// Gets a value indicating if the feature record
        /// has an object Identifier (OID).
        /// </summary>
        public bool HasOid
        {
            get { return _current.HasOid; }
        }

        /// <summary>
        /// Gets a value indicating whether this feature record
        /// has been fully loaded from the data source.
        /// </summary>
        // TODO: Reevaluate the IsFullyLoaded flag, since consecutive loads may 
        // eventually fully load a record, yet this won't be able to record it.
        public bool IsFullyLoaded
        {
            get { return _current.IsFullyLoaded; }
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
            get { return _current.OidType; }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransformation; }
            set { _coordinateTransformation = value; }
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// Gibt einen Enumerator zurück, der die Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.Generic.IEnumerator`1"/>, der zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            while (Read())
                yield return this;
            Close();
        }

        /// <summary>
        /// Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}