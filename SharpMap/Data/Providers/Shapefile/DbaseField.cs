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
using System.Text;
using System.Data;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal class DbaseField
    {
        private string _columnName;
        private Type _dataType;
        private int _address;
        private short _length;
        private byte _decimals;

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public Int16 Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public byte Decimals
        {
            get { return _decimals; }
            set { _decimals = value; }
        }

        public override string ToString()
        {
            return String.Format("[DbaseField] Name: {0}; Type: {1}; Length: {2}; Decimals: {3}", ColumnName, DataType, Length, Decimals);
        }
    }
}
