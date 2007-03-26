using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SharpMap.Data.Providers
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
