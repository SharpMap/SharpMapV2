using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SharpMap.Data.Providers
{
    internal static class DbaseSchema
    {
        internal static DbaseField[] GetFields(System.Data.DataTable schema)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            List<DbaseField> fields = new List<DbaseField>();
            foreach (DataRow row in schema.Rows)
            {
                if (row[DbaseSchema.ColumnNameColumn].Equals(DbaseSchema.OidColumnName))
                    continue;

                DbaseField field = new DbaseField();
                field.ColumnName = row[DbaseSchema.ColumnNameColumn] as string;
                field.DataType = (Type)row[DbaseSchema.DataTypeColumn];
                field.Length = Convert.ToInt16(row[DbaseSchema.ColumnSizeColumn]);
                field.Decimals = Convert.ToByte(row[DbaseSchema.NumericPrecisionColumn]);
                fields.Add(field);
            }

            return fields.ToArray();
        }

        internal static FeatureDataTable<uint> GetFeatureTableForFields(DbaseField[] _dbaseColumns)
        {
            SharpMap.Data.FeatureDataTable<uint> table = new SharpMap.Data.FeatureDataTable<uint>(DbaseSchema.OidColumnName);
            foreach (DbaseField dbf in _dbaseColumns)
            {
                DataColumn col = table.Columns.Add(dbf.ColumnName, dbf.DataType);
                if (dbf.DataType == typeof(string))
                    col.MaxLength = dbf.Length;
                else
                    col.ExtendedProperties[DbaseSchema.LengthExtendedProperty] = dbf.Length;
                if (dbf.Decimals > 0)
                    col.ExtendedProperties[DbaseSchema.NumericPrecisionExtendedProperty] = dbf.Decimals;
            }

            return table;
        }

        internal static char GetFieldTypeCode(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return 'L';
                case TypeCode.DateTime:
                    return 'D';
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 'N';
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                    return 'F';
                case TypeCode.Char:
                case TypeCode.String:
                    return 'C';
                default:
                    throw new NotSupportedException("Type is not supported");
            }
        }

        /// <summary>
        /// Creates an empty schema table
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <listheader>
        ///     <term>Column</term>
        ///     <description>Column in the schema table which describes the column in the data table</description>
        /// </listheader>
        /// <item>
        ///     <term>ColumnName</term>
        ///     <description>
        ///     The name of the column; this might not be unique. If the column name cannot be determined, a null value is returned. 
        ///     This name always reflects the most recent naming of the column in the current view or command text.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>ColumnOrdinal</term>
        ///     <description>
        ///     The ordinal of the column. This is zero for the bookmark column of the row, if any. 
        ///     Other columns are numbered starting with 1. This column cannot contain a null value.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>ColumnSize</term>
        ///     <description>
        ///     The maximum possible length of a value in the column. 
        ///     For columns that use a fixed-length data type, this is the size of the data type.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>NumericPrecision</term>
        ///     <description>
        ///     If DbType is a numeric data type, this is the maximum precision of the column. 
        ///     The precision depends on the definition of the column. If DbType is not a numeric data type, 
        ///     this is a null value. If the underlying xBase table has a precision value for a 
        ///     non-numeric data type, this value is used in the schema table.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>NumericScale</term>
        ///     <description>
        ///     If DbType is Decimal, the number of digits to the right of the decimal point. 
        ///     Otherwise, this is a null value. If the underlying xBase table returns a precision 
        ///     value for a non-numeric data type, this value is used in the schema table.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>DataType</term>
        ///     <description>
        ///     Maps to the common language runtime type of DbType.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>ProviderType</term>
        ///     <description>
        ///     The underlying driver type.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsLong</term>
        ///     <description>
        ///     Always false
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>AllowDBNull</term>
        ///     <description>
        ///     true if the consumer can set the column 
        ///     to a null value or if the driver cannot determine whether the 
        ///     consumer can set the column to a null value. Otherwise, false. 
        ///     A column may contain null values, even if it cannot be set to a null value.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsReadOnly</term>
        ///     <description>
        ///     true if the column cannot be modified; otherwise false.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsRowVersion</term>
        ///     <description>
        ///     Set if the column contains a persistent row identifier 
        ///     that cannot be written to, and has no meaningful value 
        ///     except to identity the row.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsUnique</term>
        ///     <description>
        ///     <para>
        ///     true: No two rows in the base table (the table returned in BaseTableName) 
        ///     can have the same value in this column. IsUnique is guaranteed to be true 
        ///     if the column represents a key by itself or if there is a constraint of 
        ///     type UNIQUE that applies only to this column.
        ///     </para>
        ///     <para>
        /// 	false: The column can contain duplicate values in the base table. The default for this column is false.
        ///     </para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsKey</term>
        ///     <description>
        ///     <para>
        /// 	true: The column is one of a set of columns in the rowset that, 
        ///     taken together, uniquely identify the row. The set of columns with 
        ///     IsKey set to true must uniquely identify a row in the rowset. 
        ///     There is no requirement that this set of columns is a minimal set of columns. 
        ///     This set of columns may be generated from a base table primary key, a unique constraint, or a unique index.
        ///     </para>
        ///     <para>
        /// 	false: The column is not required to uniquely identify the row.
        ///     </para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>IsAutoIncrement</term>
        ///     <description>
        ///     true if the column assigns values to new rows in fixed increments; otherwise false. 
        ///     The default for this column is false.
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>A <see cref="DataTable"/> with one row per column in the target schema, having a set of columns which describe the target column</returns>
        internal static DataTable CreateSchemaTable()
        {
            DataTable schemaTable = new DataTable();
            // all of common, non "base-table" fields implemented
            schemaTable.Columns.Add(ColumnNameColumn, typeof(System.String));
            schemaTable.Columns.Add(ColumnSizeColumn, typeof(Int32));
            schemaTable.Columns.Add(ColumnOrdinalColumn, typeof(Int32));
            schemaTable.Columns.Add(NumericPrecisionColumn, typeof(Int16));
            schemaTable.Columns.Add(NumericScaleColumn, typeof(Int16));
            schemaTable.Columns.Add(DataTypeColumn, typeof(System.Type));
            schemaTable.Columns.Add(AllowDBNullColumn, typeof(bool));
            schemaTable.Columns.Add(IsReadOnlyColumn, typeof(bool));
            schemaTable.Columns.Add(IsUniqueColumn, typeof(bool));
            schemaTable.Columns.Add(IsRowVersionColumn, typeof(bool));
            schemaTable.Columns.Add(IsKeyColumn, typeof(bool));
            schemaTable.Columns.Add(IsAutoIncrementColumn, typeof(bool));
            schemaTable.Columns.Add(IsLongColumn, typeof(bool));

            return schemaTable;
        }

        internal static DataTable DeriveSchemaTable(FeatureDataTable table)
        {
            DataTable schema = CreateSchemaTable();
            foreach (DataColumn column in table.Columns)
            {
                DataColumn[] keyColumns = table.PrimaryKey ?? new DataColumn[] { };
                int length = column.ExtendedProperties.ContainsKey(LengthExtendedProperty) ? Convert.ToInt32(column.ExtendedProperties[LengthExtendedProperty]) : column.MaxLength;
                short precision = column.ExtendedProperties.ContainsKey(NumericPrecisionExtendedProperty) ? Convert.ToInt16(column.ExtendedProperties[NumericPrecisionExtendedProperty]) : (short)0;
                schema.Rows.Add(
                    column.ColumnName,
                    length,
                    column.Ordinal,
                    precision,
                    0,
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

        internal static readonly string OidColumnName = "OID";
        internal static readonly string ColumnNameColumn = "ColumnName";
        internal static readonly string ColumnSizeColumn = "ColumnSize";
        internal static readonly string ColumnOrdinalColumn = "ColumnOrdinal";
        internal static readonly string NumericPrecisionColumn = "NumericPrecision";
        internal static readonly string NumericScaleColumn = "NumericScale";
        internal static readonly string DataTypeColumn = "DataType";
        internal static readonly string AllowDBNullColumn = "AllowDBNull";
        internal static readonly string IsReadOnlyColumn = "IsReadOnly";
        internal static readonly string IsUniqueColumn = "IsUnique";
        internal static readonly string IsRowVersionColumn = "IsRowVersion";
        internal static readonly string IsKeyColumn = "IsKey";
        internal static readonly string IsAutoIncrementColumn = "IsAutoIncrement";
        internal static readonly string IsLongColumn = "IsLong";
        internal static readonly string NumericPrecisionExtendedProperty = "Decimals";
        internal static readonly string LengthExtendedProperty = "Length";
    }
}
