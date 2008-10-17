// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using System.Reflection.Emit;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.DataStructures;
using GeoAPI.Indexing;
using GeoAPI.Geometries;
using System.Reflection;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents a geographic feature, stored as 
    /// a row of data in a <see cref="FeatureDataTable"/>.
    /// </summary>
    [Serializable]
    public class FeatureDataRow : DataRow, IFeatureDataRecord, IBoundable<IExtents>
    {
        #region Nested types
        private delegate DataColumnCollection GetColumnsDelegate(DataRow row);
        #endregion

        #region Type fields
        private static readonly GetColumnsDelegate _getColumns;
        #endregion

        #region Static constructor
        static FeatureDataRow()
        {
            DynamicMethod getColumnsMethod = new DynamicMethod("FeatureDataRow_GetColumns",
                                                               MethodAttributes.Static | MethodAttributes.Public,
                                                               CallingConventions.Standard,
                                                               typeof(DataColumnCollection),		// return type
                                                               new Type[] { typeof(DataRow) },		// one parameter of type DataRow
                                                               typeof(DataRow),					    // owning type
                                                               false);								// don't skip JIT visibility checks

            ILGenerator il = getColumnsMethod.GetILGenerator();
            FieldInfo columnsField = typeof(DataRow).GetField("_columns", BindingFlags.NonPublic | BindingFlags.Instance);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, columnsField);
            il.Emit(OpCodes.Ret);

            _getColumns = (GetColumnsDelegate)getColumnsMethod.CreateDelegate(typeof(GetColumnsDelegate));
        }
        #endregion

        #region Instance fields
        // TODO: implement original and proposed geometry to match DataRow RowState model
        private IGeometry _originalGeometry;
        private IGeometry _currentGeometry;
        private IGeometry _proposedGeometry;
        private Boolean _isGeometryModified = false;
        private IExtents _extents;
        private Boolean _isFullyLoaded;
        private ICoordinateTransformation _coordinateTransform;
        #endregion

        #region Object constructor
        internal FeatureDataRow(DataRowBuilder rb)
            : base(rb)
        {
        }
        #endregion

        /// <summary>
        /// Accepts all pending values in the feature and makes them
        /// the current state.
        /// </summary>
        public new void AcceptChanges()
        {
            base.AcceptChanges();
            _isGeometryModified = false;
        }

        public String Evaluate(Expression expression)
        {
            PropertyNameExpression property = expression as PropertyNameExpression;
            if (property != null)
            {
                return this[property.PropertyName].ToString();
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets geographic extents for the feature.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="Geometry"/> is not null, since Extents
        /// just reflects the Geometry's extents, if set.
        /// </exception>
        public IExtents Extents
        {
            get
            {
                return Geometry != null
                           ? Geometry.Extents
                           : _extents;
            }
            set
            {
                if (Geometry != null)
                {
                    throw new InvalidOperationException("Geometry is not null - cannot set extents.");
                }

                _extents = value;
            }
        }

        /// <summary>
        /// The geometry of the feature.
        /// </summary>
        public IGeometry Geometry
        {
            get { return _currentGeometry; }
            set
            {
                if (ReferenceEquals(_currentGeometry, value))
                {
                    return;
                }

                IGeometry oldGeometry = _currentGeometry;
                _currentGeometry = value;

                if (RowState != DataRowState.Detached)
                {
                    _isGeometryModified = true;
                    Table.RowGeometryChanged(this, oldGeometry);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this feature record
        /// has been fully loaded from the data source.
        /// </summary>
        public Boolean IsFullyLoaded
        {
            get { return _isFullyLoaded; }
            internal set { _isFullyLoaded = value; }
        }

        /// <summary>
        /// Gets true if the <see cref="Geometry"/> value for the
        /// feature has been modified.
        /// </summary>
        public Boolean IsGeometryModified
        {
            get { return _isGeometryModified; }
        }

        /// <summary>
        /// Returns true of the geometry is null.
        /// </summary>
        /// <returns></returns>
        public Boolean IsGeometryNull()
        {
            return Geometry == null;
        }

        /// <summary>
        /// Gets the <see cref="FeatureDataTable"/> for which this
        /// row has schema.
        /// </summary>
        public new FeatureDataTable Table
        {
            get { return base.Table as FeatureDataTable; }
        }

        /// <summary>
        /// Sets the geometry column to null.
        /// </summary>
        public void SetGeometryNull()
        {
            Geometry = null;
        }

        #region IDataRecord Members

        public Int32 FieldCount
        {
            get { return InternalColumns.Count; }
        }

        public Boolean GetBoolean(Int32 i)
        {
            return Convert.ToBoolean(this[i]);
        }

        public Byte GetByte(Int32 i)
        {
            return Convert.ToByte(this[i]);
        }

        public Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public Char GetChar(Int32 i)
        {
            return Convert.ToChar(this[i]);
        }

        public Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(Int32 i)
        {
            throw new NotImplementedException();
        }

        public String GetDataTypeName(Int32 i)
        {
            return InternalColumns[i].DataType.ToString();
        }

        public DateTime GetDateTime(Int32 i)
        {
            return Convert.ToDateTime(this[i]);
        }

        public decimal GetDecimal(Int32 i)
        {
            return Convert.ToDecimal(this[i]);
        }

        public Double GetDouble(Int32 i)
        {
            return Convert.ToDouble(this[i]);
        }

        public Type GetFieldType(Int32 i)
        {
            return InternalColumns[i].DataType;
        }

        public Single GetFloat(Int32 i)
        {
            return Convert.ToSingle(this[i]);
        }

        public Guid GetGuid(Int32 i)
        {
            return (Guid)this[i];
        }

        public Int16 GetInt16(Int32 i)
        {
            return Convert.ToInt16(this[i]);
        }

        public Int32 GetInt32(Int32 i)
        {
            return Convert.ToInt32(this[i]);
        }

        public Int64 GetInt64(Int32 i)
        {
            return Convert.ToInt64(this[i]);
        }

        public String GetName(Int32 i)
        {
            return InternalColumns[i].ColumnName;
        }

        public Int32 GetOrdinal(String name)
        {
            return InternalColumns[name].Ordinal;
        }

        public String GetString(Int32 i)
        {
            return Convert.ToString(this[i]);
        }

        public Object GetValue(Int32 i)
        {
            return this[i];
        }

        public Int32 GetValues(Object[] values)
        {
            Object[] items = ItemArray;
            Int32 elementsCopied = Math.Max(values.Length, items.Length);
            Array.Copy(items, values, elementsCopied);
            return elementsCopied;
        }

        public Boolean IsDBNull(Int32 i)
        {
            return IsNull(i);
        }

        #endregion

        protected DataColumnCollection InternalColumns
        {
            get { return _getColumns(this); }
        }

        #region IFeatureDataRecord Members

        public virtual Object GetOid()
        {
            if (Table != null && HasOid)
            {
                return this[Table.PrimaryKey[0]];
            }

            return null;
        }

        public virtual TOid GetOid<TOid>()
        {
            throw new NotSupportedException("GetOid<TOid> is not supported " +
                                            "for weakly typed FeatureDataRow. " +
                                            "Use FeatureDataRow<TOid> instead.");
        }

        public virtual Boolean HasOid
        {
            get
            {
                return Table == null ? false : Table.PrimaryKey.Length == 1;
            }
        }

        public virtual Type OidType
        {
            get { return HasOid ? Table.PrimaryKey[0].DataType : null; }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransform; }
            set { _coordinateTransform = value; }
        }

        #endregion

        #region IBoundable<IExtents> Members

        IExtents IBoundable<IExtents>.Bounds
        {
            get { return Extents; }
        }

        Boolean IIntersectable<IExtents>.Intersects(IExtents bounds)
        {
            return Extents.Intersects(bounds);
        }

        #endregion
    }
}