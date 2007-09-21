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

namespace SharpMap.Data
{
    /// <summary>
    /// Specifies how to merge schema when FeatureDataTable instances or
    /// FeatureDataSet instances are merged.
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
        /// Adds schema from the source to the target if it is missing.
        /// </summary>
		Add = 1,

        /// <summary>
        /// Adds schema from the source to the target if it is missing, and
        /// adds any key information from the source to the target if it is missing.
        /// </summary>
        AddWithKey = 5,

        /// <summary>
        /// Replaces the schema in the target by converting columns to a type
        /// which supports both source data and target data, if possible.
        /// </summary>
        Replace = 2,

        /// <summary>
        /// Adds any key information from the source to the target if it is missing.
        /// </summary>
		Key = 4,

        /// <summary>
        /// Matches key columns if their type matches, regardless of the column names.
        /// </summary>
		KeyByType = 32,

        /// <summary>
        /// Matches columns by their names in a case-insensitive manner.
        /// </summary>
        CaseInsensitive = 8,

        /// <summary>
        /// Converts the source column to the target column type if a conversion exists.
        /// </summary>
        ConvertTypes = 16
	}
}
