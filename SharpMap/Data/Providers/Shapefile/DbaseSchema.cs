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
using System.Collections.Generic;
using System.Data;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal static class DbaseSchema
    {
        internal static readonly String OidColumnName = "OID";

        internal static ICollection<DbaseField> GetFields(DataTable schema, DbaseHeader header)
        {
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}

            List<DbaseField> fields = new List<DbaseField>();
			DataView schemaView = new DataView(schema, "", ProviderSchemaHelper.ColumnOrdinalColumn, DataViewRowState.CurrentRows);

        	Int32 offset = 1;
			foreach (DataRowView rowView in schemaView)
            {
				if (String.Compare(rowView[ProviderSchemaHelper.ColumnNameColumn] as String, OidColumnName, 
					StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					continue;
				}

				String colName = rowView[ProviderSchemaHelper.ColumnNameColumn] as String;
				Type dataType = (Type)rowView[ProviderSchemaHelper.DataTypeColumn];
				Int16 length = Convert.ToInt16(rowView[ProviderSchemaHelper.ColumnSizeColumn]);
				Byte decimals = Convert.ToByte(rowView[ProviderSchemaHelper.NumericPrecisionColumn]);
				Int32 ordinal = Convert.ToInt32(rowView[ProviderSchemaHelper.ColumnOrdinalColumn]);

				DbaseField field = new DbaseField(header, colName, dataType, length, decimals, ordinal, offset);

                fields.Add(field);

            	offset += field.Length;
            }

            return fields;
        }

        internal static FeatureDataTable<uint> GetFeatureTableForFields(IEnumerable<DbaseField> _dbaseColumns)
        {
            FeatureDataTable<uint> table = new FeatureDataTable<uint>(OidColumnName);

            foreach (DbaseField dbf in _dbaseColumns)
            {
                DataColumn col = table.Columns.Add(dbf.ColumnName, dbf.DataType);

				if (dbf.DataType == typeof(String))
				{
					col.MaxLength = dbf.Length;
				}
				else
				{
                    col.ExtendedProperties[ProviderSchemaHelper.LengthExtendedProperty] = dbf.Length;
				}

				if (dbf.Decimals > 0)
				{
                    col.ExtendedProperties[ProviderSchemaHelper.NumericPrecisionExtendedProperty] = dbf.Decimals;
				}
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

		private static Int32 getLengthByHeuristic(DataColumn column)
		{
			switch (Type.GetTypeCode(column.DataType))
			{
				case TypeCode.Boolean:
					return 1;
				case TypeCode.DateTime:
					return 8;
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return 18;
				case TypeCode.Decimal:
				case TypeCode.Single:
				case TypeCode.Double:
					return 20;
				case TypeCode.Char:
				case TypeCode.String:
					return 254;
				default:
					throw new NotSupportedException("Type is not supported");
			}
        }

        internal static DataTable DeriveSchemaTable(FeatureDataTable model)
        {
            // UNDONE: the precision computation delegate should not be null
            return ProviderSchemaHelper.DeriveSchemaTable(model, getLengthByHeuristic, null, null);
        }
    }
}
