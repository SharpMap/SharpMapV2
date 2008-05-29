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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers.PostGis
{
	public class PostGisFeatureDataReader : IFeatureDataReader
	{
		private PostGisProvider _provider;
		private bool _isDisposed;

		#region Object construction and disposal
		#region Constructor
		internal PostGisFeatureDataReader(PostGisProvider provider)
		{
			_provider = provider;
		}
		#endregion

		#region Dispose pattern

		~PostGisFeatureDataReader()
		{
			Dispose(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if(IsDisposed)
			{
				return;
			}

			if(disposing)
			{
				_provider = null;
			}
		}

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}
		#endregion
		#endregion

		#region IFeatureDataReader Members
        /// <summary>
        /// Gets the geometry for the current position in the reader.
        /// </summary>
        public Geometry Geometry
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the Object ID for the record.
        /// </summary>
        /// <returns>
        /// The Object ID for the record, or <see langword="null"/> 
        /// if <see cref="HasOid"/> is <see langword="false"/>.
        /// </returns>
        public object GetOid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating if the feature record
        /// has an Object Identifier (OID).
        /// </summary>
        public bool HasOid
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether this feature record
        /// has been fully loaded from the data source.
        /// </summary>
        // TODO: Reevaluate the IsFullyLoaded flag, since consecutive loads may 
        // eventually fully load a record, yet this won't be able to record it.
        public bool IsFullyLoaded
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the Object ID.
        /// </summary>
        /// <remarks>
        /// OidType gets a <see cref="Type"/> which can be used
        /// to call GetOid with generic type parameters in order to avoid 
        /// boxing. If <see cref="HasOid"/> returns false, <see cref="OidType"/>
        /// returns <see langword="null"/>.
        /// </remarks>
        public Type OidType
        {
            get { throw new NotImplementedException(); }
        }
		#endregion

		#region IDataReader Members

		public void Close()
		{
			throw new NotImplementedException();
		}

		public int Depth
		{
			get { throw new NotImplementedException(); }
		}

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		public bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public bool NextResult()
		{
			throw new NotImplementedException();
		}

		public bool Read()
		{
			throw new NotImplementedException();
		}

		public int RecordsAffected
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDataRecord Members

		public int FieldCount
		{
			get { throw new NotImplementedException(); }
		}

		public bool GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		public byte GetByte(int i)
		{
			throw new NotImplementedException();
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public char GetChar(int i)
		{
			throw new NotImplementedException();
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public System.Data.IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		public string GetDataTypeName(int i)
		{
			throw new NotImplementedException();
		}

		public DateTime GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		public decimal GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		public double GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		public Type GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		public Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		public short GetInt16(int i)
		{
			throw new NotImplementedException();
		}

		public int GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		public long GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		public string GetName(int i)
		{
			throw new NotImplementedException();
		}

		public int GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

		public string GetString(int i)
		{
			throw new NotImplementedException();
		}

		public object GetValue(int i)
		{
			throw new NotImplementedException();
		}

		public int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public bool IsDBNull(int i)
		{
			throw new NotImplementedException();
		}

		public object this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		public object this[int i]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

	    ///<summary>
	    ///Returns an enumerator that iterates through the collection.
	    ///</summary>
	    ///
	    ///<returns>
	    ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
	    ///</returns>
	    ///<filterpriority>1</filterpriority>
        public IEnumerator<IFeatureDataRecord> GetEnumerator()
	    {
	        throw new NotImplementedException();
	    }

	    ///<summary>
	    ///Returns an enumerator that iterates through a collection.
	    ///</summary>
	    ///
	    ///<returns>
	    ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
	    ///</returns>
	    ///<filterpriority>2</filterpriority>
	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }
	}
}
