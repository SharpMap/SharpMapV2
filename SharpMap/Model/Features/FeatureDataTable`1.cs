// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using System.Text;
using System.Collections;

namespace SharpMap
{
    public class FeatureDataTable<TOid> : FeatureDataTable, IEnumerable<FeatureDataRow<TOid>>
    {
        private DataColumn _idColumn;

        public static FeatureDataTable<TOid> CreateEmpty(string idColumnName)
        {
            return CreateTableWithId(new FeatureDataTable(), idColumnName);
        }

        private FeatureDataTable()
            : base()
        {
        }

        public FeatureDataTable(string idColumnName)
            : base()
        {
            setIdColumn(idColumnName);
        }

        public FeatureDataTable(DataTable table, string idColumnName)
            : base(table)
        {
            setIdColumn(idColumnName);
        }

        public static FeatureDataTable<TOid> CreateTableWithId(FeatureDataTable table, string columnName)
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

            return InternalCreateTableWithId(table, table.Columns[columnName]);
        }

        public static FeatureDataTable<TOid> CreateTableWithId(FeatureDataTable table, DataColumn column)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            return InternalCreateTableWithId(table.Copy() as FeatureDataTable, column);
        }

        private static FeatureDataTable<TOid> InternalCreateTableWithId(FeatureDataTable tableCopy, DataColumn objectIdColumn)
        {
            FeatureDataTable<TOid> tableWithId = new FeatureDataTable<TOid>(tableCopy, objectIdColumn.ColumnName);

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
                int itemCount = newRow.ItemArray.Length;
                newRow.ItemArray = new object[itemCount];
                //Array.Copy(row.ItemArray, newRow.ItemArray, itemCount);
                newRow.ItemArray = row.ItemArray;
                tableWithId.AddRow(newRow);
            }

            return tableWithId;
        }

        public DataColumn IdColumn
        {
            get { return _idColumn; }
            private set { _idColumn = value; }
        }

        /// <summary>
        /// Gets the feature data row at the specified index
        /// </summary>
        /// <param name="index">row index</param>
        /// <returns>FeatureDataRow</returns>
        public new FeatureDataRow<TOid> this[int index]
        {
            get { return (FeatureDataRow<TOid>)base.Rows[index]; }
        }

        public void AddRow(FeatureDataRow<TOid> row)
        {
            base.Rows.Add(row);
        }

        /// <summary>
        /// Returns an enumerator for enumering the rows of the <see cref="FeatureDataTable{TOid}">table</see> instance.
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
        /// Clones the structure of the FeatureDataTable, including all FeatureDataTable schemas and constraints. 
        /// </summary>
        /// <returns></returns>
        public new FeatureDataTable<TOid> Clone()
        {
            FeatureDataTable<TOid> clone = ((FeatureDataTable<TOid>)(base.Clone()));
            clone.IdColumn = clone.Columns[IdColumn.ColumnName];
            return clone;
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table.
        /// </summary>
        /// <returns></returns>
        public FeatureDataRow<TOid> NewRow(TOid id)
        {
            FeatureDataRow<TOid> row = base.NewRow() as FeatureDataRow<TOid>;
            row[IdColumn] = id;
            return row;
        }

        /// <summary>
        /// Removes the row from the table
        /// </summary>
        /// <param name="row">Row to remove</param>
        public void RemoveRow(FeatureDataRow<TOid> row)
        {
            base.Rows.Remove(row);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance()
        {
            return new FeatureDataTable<TOid>();
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table, based on a datarow builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FeatureDataRow<TOid>(builder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override System.Type GetRowType()
        {
            return typeof(FeatureDataRow<TOid>);
        }

        private void setIdColumn(string idColumnName)
        {
            if (String.IsNullOrEmpty(idColumnName))
                throw new ArgumentNullException("idColumnName");

            if (Columns.Contains(idColumnName))
            {
                if (Columns[idColumnName].DataType != typeof(TOid))
                    throw new InvalidOperationException("Column with name " + idColumnName + " exists, but has different type from type parameter type: " + typeof(TOid).Name);

                IdColumn = Columns[idColumnName];
            }
            else
            {
                IdColumn = new DataColumn(idColumnName, typeof(TOid));
                Columns.Add(IdColumn);
            }
        }
    }
}
