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
using System.Data.Common;
using SharpMap.Data;

namespace SharpMap.Data
{
    internal sealed class LoadFeaturesAdapter : DataAdapter
    {
        public Int32 Fill(FeatureDataTable table, IFeatureDataReader dataReader)
        {
            return Fill(new FeatureDataTable[] { table }, dataReader);
        }

        public Int32 Fill(ICollection<FeatureDataTable> dataTables, IFeatureDataReader dataReader)
        {
            FeatureDataTable[] tables = new FeatureDataTable[dataTables.Count];
            dataTables.CopyTo(tables, 0);
            return Fill(tables, dataReader, 0, 0);
        }

        protected override Int32 Fill(DataTable[] dataTables, IDataReader dataReader, Int32 startRecord, Int32 maxRecords)
        {
            if (dataTables.Length == 0)
            {
                return 0;
            }

            Int32 tableIndex = 0;

            IFeatureDataReader featureReader = dataReader as IFeatureDataReader;

            if (featureReader == null)
            {
                throw new ArgumentException("Parameter 'dataReader' " +
                                            "must be a IFeatureDataReader instance.");
            }

            do
            {
                FeatureDataTable table = dataTables[tableIndex] as FeatureDataTable;

                if (table == null)
                {
                    throw new ArgumentException("Components of 'dataTables' must be " +
                                                "FeatureDataTable instances.");
                }

                if (table.Columns.Count == 0)
                {
                    FillSchema(table, SchemaType.Mapped, featureReader);
                }

                table.Merge((IEnumerable<IFeatureDataRecord>)featureReader);

                tableIndex++;
            } while (dataReader.NextResult());

            return dataTables[0].Rows.Count;
        }
    }
}