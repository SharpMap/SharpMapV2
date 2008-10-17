// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Data
{
    /// <summary>
    /// Specifies how to merge schema when <see cref="FeatureDataTable"/> instances or
    /// <see cref="FeatureDataSet"/> instances are merged.
    /// </summary>
    [Flags]
	public enum SchemaMergeAction
	{
        /// <summary>
        /// Specifies that schema information should not be merged. If schema
        /// differs and data doesn't match, an exception will probably be thrown.
        /// </summary>
        None = 0,

        /// <summary>
        /// Adds any non key columns (columns not in <see cref="DataTable.PrimaryKey"/>)
        ///  from the source to the target if missing.
        /// </summary>
		AddNonKey = 1,

        /// <summary>
        /// Adds any key columns (columns in <see cref="DataTable.PrimaryKey"/>)
        /// from the source to the target if missing.
        /// </summary>
        AddKey = 2,

        /// <summary>
        /// Adds all columns from the source to the target if missing.
        /// </summary>
        AddAll = 3,

        /// <summary>
        /// Adds all columns from the source to the target if missing, and
        /// adds any key information from the source to the target if missing.
        /// </summary>
        AddWithKey = AddAll | Key,

        /// <summary>
        /// Replaces the schema in the target by converting columns to a type
        /// which supports both source data and target data, if possible.
        /// </summary>
        Replace = 4,

        /// <summary>
        /// Merges the <see cref="DataTable.PrimaryKey"/> array.
        /// </summary>
		Key = 8,

        /// <summary>
        /// Matches <see cref="DataTable.PrimaryKey"/> columns if their type matches, 
        /// regardless of the column names.
        /// </summary>
		KeyByType = 32,

        /// <summary>
        /// Matches columns by their names in a case-insensitive manner.
        /// </summary>
        CaseInsensitive = 64,

        /// <summary>
        /// Allows columns to be matched if the names are the same and the type of 
        /// the source column has a conversion to the type of the target column, 
        /// otherwise column types need to be identical to match.
        /// </summary>
        MatchIfConvertible = 16
	}
}
