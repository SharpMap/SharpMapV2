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
using SharpMap.Features;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal static class DbaseSchema
    {
        internal static readonly string OidColumnName = "OID";

        internal static DbaseField[] GetFields(DataTable schema)
        {
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}

            List<DbaseField> fields = new List<DbaseField>();

            foreach (DataRow row in schema.Rows)
            {
				if (String.Compare(row[ProviderSchemaHelper.ColumnNameColumn] as string, OidColumnName, 
					StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					continue;
				}

                DbaseField field = new DbaseField();
                field.ColumnName = row[ProviderSchemaHelper.ColumnNameColumn] as string;
                field.DataType = (Type)row[ProviderSchemaHelper.DataTypeColumn];
                field.Length = Convert.ToInt16(row[ProviderSchemaHelper.ColumnSizeColumn]);
                field.Decimals = Convert.ToByte(row[ProviderSchemaHelper.NumericPrecisionColumn]);
                fields.Add(field);
            }

            return fields.ToArray();
        }

        internal static FeatureDataTable<uint> GetFeatureTableForFields(DbaseField[] _dbaseColumns)
        {
            FeatureDataTable<uint> table = new FeatureDataTable<uint>(OidColumnName);

            foreach (DbaseField dbf in _dbaseColumns)
            {
                DataColumn col = table.Columns.Add(dbf.ColumnName, dbf.DataType);

				if (dbf.DataType == typeof(string))
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

		private static int getLengthByHeuristic(DataColumn column)
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
