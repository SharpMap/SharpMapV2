﻿// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
    /// This class is experimental and not used.
    /// </summary>
    public class FeatureUniqueConstraint : UniqueConstraint
    {
        public FeatureUniqueConstraint(DataColumn[] columns)
            : base(columns) {}

        public FeatureUniqueConstraint(DataColumn column) : base(column) {}
        public FeatureUniqueConstraint(DataColumn column, Boolean isPrimaryKey) : base(column, isPrimaryKey) {}
        public FeatureUniqueConstraint(String name, DataColumn[] columns) : base(name, columns) {}
        public FeatureUniqueConstraint(DataColumn[] columns, Boolean isPrimaryKey) : base(columns, isPrimaryKey) {}
        public FeatureUniqueConstraint(String name, DataColumn column) : base(name, column) {}

        public FeatureUniqueConstraint(String name, DataColumn column, Boolean isPrimaryKey)
            : base(name, column, isPrimaryKey) {}

        public FeatureUniqueConstraint(String name, DataColumn[] columns, Boolean isPrimaryKey)
            : base(name, columns, isPrimaryKey) {}

        public FeatureUniqueConstraint(String name, String[] columnNames, Boolean isPrimaryKey)
            : base(name, columnNames, isPrimaryKey) {}
    }
}