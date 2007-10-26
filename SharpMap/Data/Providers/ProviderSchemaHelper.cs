// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Data.Providers
{
    public static class ProviderSchemaHelper
    {
        public static readonly String ColumnNameColumn = "ColumnName";
        public static readonly String ColumnSizeColumn = "ColumnSize";
        public static readonly String ColumnOrdinalColumn = "ColumnOrdinal";
        public static readonly String NumericPrecisionColumn = "NumericPrecision";
        public static readonly String NumericScaleColumn = "NumericScale";
        public static readonly String DataTypeColumn = "DataType";
        public static readonly String AllowDBNullColumn = "AllowDBNull";
        public static readonly String IsReadOnlyColumn = "IsReadOnly";
        public static readonly String IsUniqueColumn = "IsUnique";
        public static readonly String IsRowVersionColumn = "IsRowVersion";
        public static readonly String IsKeyColumn = "IsKey";
        public static readonly String IsAutoIncrementColumn = "IsAutoIncrement";
        public static readonly String IsLongColumn = "IsLong";
        public static readonly String NumericPrecisionExtendedProperty = "Precision";
        public static readonly String NumericScaleExtendedProperty = "Scale";
        public static readonly String LengthExtendedProperty = "Length";

        /// <summary>
        /// Creates an empty schema table.
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <listheader>
        /// <term>Column</term>
        /// <description>
        /// Column in the schema table which describes the column in the data table.
        /// </description>
        /// </listheader>
        /// <item>
        /// <term>ColumnName</term>
        /// <description>
        /// The name of the column; this might not be unique. 
        /// If the column name cannot be determined, a null value is returned. 
        /// This name always reflects the most recent naming of the column 
        /// in the current view or command text.
        /// </description>
        /// </item>
        /// <item>
        /// <term>ColumnOrdinal</term>
        /// <description>
        /// The ordinal of the column. This is zero for the bookmark column of the row, if any. 
        /// Other columns are numbered starting with 1. This column cannot contain a null value.
        /// </description>
        /// </item>
        /// <item>
        /// <term>ColumnSize</term>
        /// <description>
        /// The maximum possible length of a value in the column. 
        /// For columns that use a fixed-length data type, this is the size of the data type.
        /// </description>
        /// </item>
        /// <item>
        /// <term>NumericPrecision</term>
        /// <description>
        /// If DbType is a numeric data type, this is the maximum precision of the column. 
        /// The precision depends on the definition of the column. 
        /// If DbType is not a numeric data type, this is a null value. If the underlying 
        /// table has a precision value for a non-numeric data type, 
        /// this value is used in the schema table.
        /// </description>
        /// </item>
        /// <item>
        /// <term>NumericScale</term>
        /// <description>
        /// If DbType is Decimal, the number of digits to the right of the decimal point. 
        /// Otherwise, this is a null value. If the underlying table returns a precision 
        /// value for a non-numeric data type, this value is used in the schema table.
        /// </description>
        /// </item>
        /// <item>
        /// <term>DataType</term>
        /// <description>
        /// Maps to the common language runtime type of DbType.
        /// </description>
        /// </item>
        /// <item>
        /// <term>ProviderType</term>
        /// <description>
        /// The underlying driver type.
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsLong</term>
        /// <description>
        /// <see langword="true"/> if the column holds a large object type, such as 
        /// a binary large object (BLOB) or character large object (CLOB); 
        /// <see langword="false"/> if it is a normal value column.
        /// </description>
        /// </item>
        /// <item>
        /// <term>AllowDBNull</term>
        /// <description>
        /// <see langword="true"/> if the consumer can set the column 
        /// to a null value or if the driver cannot determine whether the 
        /// consumer can set the column to a null value. Otherwise, <see langword="false"/>. 
        /// A column may contain null values, even if it cannot be set to a null value.
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsReadOnly</term>
        /// <description>
        /// <see langword="true"/> if the column cannot be modified; otherwise false.
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsRowVersion</term>
        /// <description>
        /// Set if the column contains a persistent row identifier 
        /// that cannot be written to, and has no meaningful value 
        /// except to identity the row.
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsUnique</term>
        /// <description>
        /// <para>
        /// <see langword="true"/>: No two rows in the base table (the table returned in BaseTableName) 
        /// can have the same value in this column. IsUnique is guaranteed to be true 
        /// if the column represents a key by itself or if there is a constraint of 
        /// type UNIQUE that applies only to this column.
        /// </para>
        /// <para>
        /// <see langword="false"/>: The column can contain duplicate values in the base table. 
        /// </para>
        /// <para>
        /// The default for this column is <see langword="false"/>.
        /// </para>
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsKey</term>
        /// <description>
        /// <para>
        /// <see langword="true"/>: The column is one of a set of columns in the rowset that, 
        /// taken together, uniquely identify the row. The set of columns with 
        /// IsKey set to true must uniquely identify a row in the rowset. 
        /// There is no requirement that this set of columns is a minimal set of columns. 
        /// This set of columns may be generated from a base table primary key, 
        /// a unique constraint, or a unique index.
        /// </para>
        /// <para>
        /// <see langword="true"/>: The column is not required to uniquely identify the row.
        /// </para>
        /// </description>
        /// </item>
        /// <item>
        /// <term>IsAutoIncrement</term>
        /// <description>
        /// <see langword="true"/> if the column assigns values to new rows in fixed increments; otherwise false. 
        /// The default for this column is <see langword="false"/>.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// A <see cref="DataTable"/> with one row per column in the target schema, 
        /// having a set of columns which describe the target column.
        /// </returns>
        internal static DataTable CreateSchemaTable()
        {
            DataTable schemaTable = new DataTable();
            // all of common, non "base-table" fields implemented
            schemaTable.Columns.Add(ColumnNameColumn, typeof(String));
            schemaTable.Columns.Add(ColumnSizeColumn, typeof(Int32));
            schemaTable.Columns.Add(ColumnOrdinalColumn, typeof(Int32));
            schemaTable.Columns.Add(NumericPrecisionColumn, typeof(Int16));
            schemaTable.Columns.Add(NumericScaleColumn, typeof(Int16));
            schemaTable.Columns.Add(DataTypeColumn, typeof(Type));
            schemaTable.Columns.Add(AllowDBNullColumn, typeof(bool));
            schemaTable.Columns.Add(IsReadOnlyColumn, typeof(bool));
            schemaTable.Columns.Add(IsUniqueColumn, typeof(bool));
            schemaTable.Columns.Add(IsRowVersionColumn, typeof(bool));
            schemaTable.Columns.Add(IsKeyColumn, typeof(bool));
            schemaTable.Columns.Add(IsAutoIncrementColumn, typeof(bool));
            schemaTable.Columns.Add(IsLongColumn, typeof(bool));

            return schemaTable;
        }


        public static DataTable DeriveSchemaTable(FeatureDataTable table,
            ColumnLengthComputationDelegate lengthComputationDelegate,
            ColumnPrecisionComputationDelegate precisionComputationDelegate,
            ColumnScaleComputationDelegate scaleComputationDelegate)
        {
            DataTable schema = CreateSchemaTable();
            foreach (DataColumn column in table.Columns)
            {
                DataColumn[] keyColumns = table.PrimaryKey ?? new DataColumn[] { };

                Int32 length = column.ExtendedProperties.ContainsKey(LengthExtendedProperty)
                    ? Convert.ToInt32(column.ExtendedProperties[LengthExtendedProperty])
                    : (lengthComputationDelegate == null) ? 0 : lengthComputationDelegate(column);

                short precision = column.ExtendedProperties.ContainsKey(NumericPrecisionExtendedProperty)
                    ? Convert.ToInt16(column.ExtendedProperties[NumericPrecisionExtendedProperty])
                    : (precisionComputationDelegate == null) ? (short)0 : precisionComputationDelegate(column);

                short scale = column.ExtendedProperties.ContainsKey(NumericScaleExtendedProperty)
                    ? Convert.ToInt16(column.ExtendedProperties[NumericScaleExtendedProperty])
                    : (scaleComputationDelegate == null) ? (short)0 : scaleComputationDelegate(column);

                schema.Rows.Add(
                    column.ColumnName,
                    length,
                    column.Ordinal,
                    precision,
                    scale,
                    column.DataType,
                    column.AllowDBNull,
                    column.ReadOnly,
                    column.Unique,
                    Array.Exists(keyColumns, delegate(DataColumn col) { return col == column; }),
                    column.AutoIncrement,
                    false);
            }

            return schema;
        }
    }
}
