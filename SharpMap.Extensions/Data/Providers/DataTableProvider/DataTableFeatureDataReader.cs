using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Data;
using SharpMap.Geometries;

namespace SharpMap.Extensions.Data.Providers.DataTableProvider
{
	public class DataTableFeatureDataReader : IFeatureDataReader
	{
		private DataTableProvider _provider;
		private readonly BoundingBox _bounds;
		private bool _isDisposed;

		#region Object construction and disposal
		#region Constructors
		internal DataTableFeatureDataReader(DataTableProvider provider, BoundingBox bounds)
		{
			_provider = provider;
			_bounds = bounds;
		}
		#endregion

		#region Disposers and finalizers

		/// <summary>
		/// Finalizer
		/// </summary>
		~DataTableFeatureDataReader()
		{
			Dispose(false);
		}

		#region IDisposable Members
		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}
		#endregion

		private void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
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

		public Geometry GetGeometry()
		{
			throw new NotImplementedException();
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

		public System.Data.DataTable GetSchemaTable()
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
	}
}
