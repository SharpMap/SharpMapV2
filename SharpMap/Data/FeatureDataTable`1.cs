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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using GeoAPI.Geometries;
using SharpMap.Data;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents one feature table of in-memory spatial data, 
    /// with an object identifier (OID) type of <typeparamref name="TOid"/>.
    /// </summary>
    /// <typeparam name="TOid">Type of the object id.</typeparam>
    public class FeatureDataTable<TOid> : FeatureDataTable, IEnumerable<FeatureDataRow<TOid>>
    {
        private DataColumn _idColumn;

        #region Static Factory Methods
        /// <summary>
        /// Creates a new, empty <see cref="FeatureDataTable{TOid}"/>.
        /// </summary>
        /// <param name="idColumnName">The name of the id column in the feature table.</param>
        /// <returns>A new, empty FeatureDataTable{TOid} with a single column for the id.</returns>
        public static FeatureDataTable<TOid> CreateEmpty(String idColumnName, IGeometryFactory factory)
        {
            return CreateTableWithId(new FeatureDataTable(factory), idColumnName, factory);
        }

        /// <summary>
        /// Creates a new <see cref="FeatureDataTable{TOid}"/> for the given <paramref name="table"/>,
        /// copying both schema and data.
        /// </summary>
        /// <param name="table">The table to copy.</param>
        /// <param name="columnName">The name of the id column.</param>
        /// <returns>A FeatureDataTable{TOid} instance which is a copy of <paramref name="table"/> 
        /// and with id column <paramref name="columnName"/>.</returns>
        public static FeatureDataTable<TOid> CreateTableWithId(FeatureDataTable table, String columnName, IGeometryFactory factory)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            //table = table.Copy() as FeatureDataTable;

            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName, typeof(TOid));
            }

            return internalCreateTableWithId(table, table.Columns[columnName], factory);
        }

        /// <summary>
        /// Creates a new <see cref="FeatureDataTable{TOid}"/> for the given <paramref name="table"/>,
        /// copying both schema and data.
        /// </summary>
        /// <param name="table">The table to copy.</param>
        /// <param name="column">The id column.</param>
        /// <returns>A FeatureDataTable{TOid} instance which is a copy of <paramref name="table"/> 
        /// and with id column <paramref name="column"/>.</returns>
        public static FeatureDataTable<TOid> CreateTableWithId(FeatureDataTable table, DataColumn column, IGeometryFactory factory)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            return internalCreateTableWithId(table.Copy() as FeatureDataTable, column, factory);
        }

        private static FeatureDataTable<TOid> internalCreateTableWithId(FeatureDataTable tableCopy, DataColumn objectIdColumn, IGeometryFactory factory)
        {
            FeatureDataTable<TOid> tableWithId = new FeatureDataTable<TOid>(tableCopy, objectIdColumn.ColumnName, factory);

            // TODO: shouldn't this be in the base class? Need to check if changing base behavior will break stuff.
            foreach (DataColumn col in tableCopy.Columns)
            {
                if (String.Compare(col.ColumnName, objectIdColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    continue;
                }

                DataColumn colCopy = new DataColumn(col.ColumnName, col.DataType);
                colCopy.AllowDBNull = col.AllowDBNull;
                colCopy.AutoIncrement = col.AutoIncrement;
                colCopy.AutoIncrementSeed = col.AutoIncrementSeed;
                colCopy.AutoIncrementStep = col.AutoIncrementStep;
                colCopy.DateTimeMode = col.DateTimeMode;
                colCopy.DefaultValue = col.DefaultValue;

                foreach (DictionaryEntry entry in col.ExtendedProperties)
                {
                    colCopy.ExtendedProperties[entry.Key] = entry.Value;
                }

                colCopy.MaxLength = col.MaxLength;
                colCopy.ReadOnly = col.ReadOnly;
                colCopy.Unique = col.Unique;
                tableWithId.Columns.Add(colCopy);
            }

            foreach (DataRow row in tableCopy)
            {
                FeatureDataRow<TOid> newRow = tableWithId.NewRow() as FeatureDataRow<TOid>;
                Debug.Assert(newRow != null);
                Int32 itemCount = newRow.ItemArray.Length;
                newRow.ItemArray = new Object[itemCount];
                //Array.Copy(row.ItemArray, newRow.ItemArray, itemCount);
                newRow.ItemArray = row.ItemArray;
                tableWithId.AddRow(newRow);
            }

            return tableWithId;
        }
        #endregion

        #region Constructors

        private FeatureDataTable(IGeometryFactory factory)
            : base(factory)
        { }

        //public FeatureDataTable(String idColumnName)
        //{
        //    setIdColumn(idColumnName);
        //}

        public FeatureDataTable(String idColumnName, IGeometryFactory factory)
            : base(factory)
        {
            setIdColumn(idColumnName);
        }

        //public FeatureDataTable(String tableName, String idColumnName)
        //    : base(tableName)
        //{
        //    setIdColumn(idColumnName);
        //}

        public FeatureDataTable(String tableName, String idColumnName, IGeometryFactory factory)
            : base(tableName, factory)
        {
            setIdColumn(idColumnName);
        }

        /// <summary>
        /// Creates a new <see cref="FeatureDataTable{TOid}"/> from the given
        /// DataTable and with the <see cref="IdColumn"/> chosen the name
        /// given with <paramref name="idColumnName"/>.
        /// </summary>
        /// <param name="table">The table to copy.</param>
        /// <param name="idColumnName">The name of the OID column.</param>
        public FeatureDataTable(DataTable table, String idColumnName, IGeometryFactory factory)
            : base(table, factory)
        {
            setIdColumn(idColumnName);
        }

        protected FeatureDataTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Put extra stuff to serialize here...
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="DataColumn"/> which represents the id column 
        /// for the feature table.
        /// </summary>
        public DataColumn IdColumn
        {
            get { return _idColumn; }
            private set
            {
                _idColumn = value;
                PrimaryKey = new DataColumn[] { _idColumn };
            }
        }

        /// <summary>
        /// Gets or set an array of columns that serve as primary keys 
        /// for the feature data table.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when key array length is greater than 1.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the key array length is zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the column in the key array is not 
        /// equal to <see cref="IdColumn"/>.
        /// </exception>
        public new DataColumn[] PrimaryKey
        {
            get { return base.PrimaryKey; }
            set
            {
                if (value.Length > 1)
                {
                    throw new NotSupportedException(
                        "Compound key not supported on FeatureDataTable.");
                }

                if (value.Length == 0)
                {
                    throw new InvalidOperationException(
                        "FeatureDataTable`1 must have an id column.");
                }

                if (value[0] != IdColumn)
                {
                    throw new InvalidOperationException(
                        "The primary key must be the same as the IdColumn.");
                }

                base.PrimaryKey = value;
            }
        }

        /// <summary>
        /// Gets the feature data row at the specified index
        /// </summary>
        /// <param name="index">row index</param>
        /// <returns>FeatureDataRow</returns>
        public new FeatureDataRow<TOid> this[Int32 index]
        {
            get { return (FeatureDataRow<TOid>)base.Rows[index]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a feature row to the table.
        /// </summary>
        /// <param name="row">The feature row to add.</param>
        public void AddRow(FeatureDataRow<TOid> row)
        {
            Rows.Add(row);
        }

        /// <summary>
        /// Clones the structure of the FeatureDataTable, including all 
        /// <see cref="FeatureDataTable"/> schemas and constraints. 
        /// </summary>
        /// <returns></returns>
        public new FeatureDataTable<TOid> Clone()
        {
            FeatureDataTable<TOid> clone = ((FeatureDataTable<TOid>)(base.Clone()));
            clone.IdColumn = clone.Columns[IdColumn.ColumnName];
            return clone;
        }

        /// <summary>
        /// Finds a feature given the feature's <abbr title="Object identifier">OID</abbr>.
        /// </summary>
        /// <param name="key">
        /// The unique key value (the OID) to use to find the feature.
        /// </param>
        /// <returns>
        /// The feature row with the given <paramref name="key"/>, or <see langword="null"/>
        /// if a feature with the key wasn't found.
        /// </returns>
        public FeatureDataRow<TOid> Find(TOid key)
        {
            return base.Find(key) as FeatureDataRow<TOid>;
        }

        /// <summary>
        /// Returns an enumerator for enumering the rows of the 
        /// <see cref="FeatureDataTable{TOid}">table</see> instance.
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<FeatureDataRow<TOid>> GetEnumerator()
        {
            foreach (FeatureDataRow<TOid> row in Rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table.
        /// </summary>
        /// <param name="id">The feature id for the feature row.</param>
        /// <returns>
        /// A new <see cref="FeatureDataRow"/> with the same schema as the table,
        /// with the given <paramref name="id"/>.
        /// </returns>
        public FeatureDataRow<TOid> NewRow(TOid id)
        {
            FeatureDataRow<TOid> row = NewRow() as FeatureDataRow<TOid>;
            Debug.Assert(row != null);
            row[IdColumn] = id;
            return row;
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table.
        /// </summary>
        /// <param name="id">The feature id for the feature row.</param>
        /// <param name="isFullyLoaded">
        /// A value indicating whether the row is fully loaded from the data source
        /// or not (lazy-loaded).
        /// </param>
        /// <returns>
        /// A new <see cref="FeatureDataRow"/> with the same schema as the table,
        /// with the given <paramref name="id"/>.
        /// </returns>
        public FeatureDataRow NewRow(TOid id, Boolean isFullyLoaded)
        {
            FeatureDataRow<TOid> row = NewRow(id);
            row.IsFullyLoaded = isFullyLoaded;
            return row;
        }

        /// <summary>
        /// Removes the feature row from the table.
        /// </summary>
        /// <param name="row">Row to remove.</param>
        public void RemoveRow(FeatureDataRow<TOid> row)
        {
            Rows.Remove(row);
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Factory method to create a new instance of <see cref="FeatureDataTable{TOid}"/>
        /// for parent class use.
        /// </summary>
        /// <returns>A new instance of a <see cref="FeatureDataTable{TOid}"/>.</returns>
        protected override DataTable CreateInstance()
        {
            return new FeatureDataTable<TOid>(GeometryFactory);
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table, 
        /// based on a datarow builder.
        /// </summary>
        /// <param name="builder">A DataRowBuilder instance to create the record with.</param>
        /// <returns>A new DataRow with the same schema as the table.</returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FeatureDataRow<TOid>(builder);
        }

        /// <summary>
        /// Gets the type of <see cref="DataRow"/> used by the 
        /// <see cref="FeatureDataTable{TOid}"/>.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="Type"/> object identifying <see cref="FeatureDataRow{TOid}"/>.
        /// </returns>
        protected override Type GetRowType()
        {
            return typeof(FeatureDataRow<TOid>);
        }

        #endregion

        #region Private Helper Methods
        private void setIdColumn(String idColumnName)
        {
            if (String.IsNullOrEmpty(idColumnName))
            {
                throw new ArgumentNullException("idColumnName");
            }

            if (Columns.Contains(idColumnName))
            {
                if (Columns[idColumnName].DataType != typeof(TOid))
                {
                    throw new InvalidOperationException("Column with name " + idColumnName + " exists, but has different type from type parameter type: " + typeof(TOid).Name);
                }

                IdColumn = Columns[idColumnName];
            }
            else
            {
                DataColumn newIdColumn = new DataColumn(idColumnName, typeof(TOid));
                Columns.Add(newIdColumn);
                IdColumn = newIdColumn;
            }
        }
        #endregion

    }
}