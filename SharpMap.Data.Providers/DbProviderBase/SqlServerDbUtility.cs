/*
 *  The attached / following is part of SharpMap.Data.Providers.Db
 *  SharpMap.Data.Providers.Db is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Data;
using System.Data.SqlClient;

namespace SharpMap.Data.Providers.Db
{
    public class SqlServerDbUtility : IDbUtility<SqlDbType>
    {
        #region IDbUtility<SqlDbType> Members

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public IDataParameter CreateParameter<TValue>(string parameterName,
                                                      TValue parameterValue,
                                                      ParameterDirection paramDirection)
        {
            SqlParameter p = Equals(null, parameterValue)
                                 ? new SqlParameter(
                                       parameterName.StartsWith("@") ? parameterName : "@" + parameterName, DBNull.Value)
                                 : new SqlParameter(
                                       parameterName.StartsWith("@") ? parameterName : "@" + parameterName,
                                       parameterValue);
            p.Direction = paramDirection;
            return p;
        }


        public IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }

        public IDataParameter CreateParameter(string parameterName, Type netType,
                                              ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType(netType), parameterDirection);
        }

        public IDataParameter CreateParameter(string parameterName, SqlDbType dbType,
                                              ParameterDirection parameterDirection)
        {
            var p = new SqlParameter(parameterName.StartsWith("@") ? parameterName : "@" + parameterName,
                                     dbType, GetDbSize(dbType)) { Direction = parameterDirection };
            return p;
        }

        public IDataParameter CreateParameter<TValue>(string parameterName, ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType<TValue>(), parameterDirection);
        }

        string IDbUtility<SqlDbType>.GetTypeString(SqlDbType dbType)
        {
            return GetTypeString(dbType);
        }

        string IDbUtility<SqlDbType>.GetTypeString(Type netType)
        {
            return GetTypeString(netType);
        }

        SqlDbType IDbUtility<SqlDbType>.GetDbType<TValue>()
        {
            return GetDbType<TValue>();
        }

        SqlDbType IDbUtility<SqlDbType>.GetDbType(Type netType)
        {
            return GetDbType(netType);
        }

        int IDbUtility<SqlDbType>.GetDbSize(SqlDbType dbType)
        {
            return GetDbSize(dbType);
        }

        #endregion

        public static SqlDbType GetDbType<TValue>()
        {
            return GetDbType(typeof(TValue));
        }

        public static SqlDbType GetDbType(Type netType)
        {
            if (netType == typeof(Int16) || netType == typeof(UInt16))
                return SqlDbType.SmallInt;
            if (netType == typeof(Int32) || netType == typeof(UInt32))
                return SqlDbType.Int;
            if (netType == typeof(Int64) || netType == typeof(UInt64))
                return SqlDbType.BigInt;
            if (netType == typeof(string))
                return SqlDbType.NVarChar;
            if (netType == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            if (netType == typeof(Single))
                return SqlDbType.Real;
            if (netType == typeof(Double))
                return SqlDbType.Float;
            if (netType == typeof(Decimal))
                return SqlDbType.Decimal;
            if (netType == typeof(byte[]))
                return SqlDbType.VarBinary;
            if (netType == typeof(Boolean))
                return SqlDbType.Bit; 

            throw new NotImplementedException();
        }

        public static int GetDbSize(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return 8;
                case SqlDbType.Binary:
                    return 8000;
                case SqlDbType.Bit:
                    return 1;
                case SqlDbType.Char:
                    return 8000;
                case SqlDbType.Date:
                    return 3;
                case SqlDbType.DateTime:
                    return 8;
                case SqlDbType.DateTime2:
                    return 8;
                case SqlDbType.DateTimeOffset:
                    return 10;
                case SqlDbType.Decimal:
                    return 17;
                case SqlDbType.Float:
                    return 8;
                case SqlDbType.Image:
                    return 16;
                case SqlDbType.Int:
                    return 4;
                case SqlDbType.Money:
                    return 8;
                case SqlDbType.NChar:
                    return 8000;
                case SqlDbType.NText:
                    return 16;
                case SqlDbType.NVarChar:
                    return 8000;
                case SqlDbType.Real:
                    return 4;
                case SqlDbType.SmallDateTime:
                    return 4;
                case SqlDbType.SmallInt:
                    return 2;
                case SqlDbType.SmallMoney:
                    return 4;
                case SqlDbType.Structured:
                    return 8000;
                case SqlDbType.Text:
                    return 16;
                case SqlDbType.Time:
                    return 5;
                case SqlDbType.Timestamp:
                    return 8;
                case SqlDbType.TinyInt:
                    return 1;
                case SqlDbType.Udt:
                    return 8000;
                case SqlDbType.UniqueIdentifier:
                    return 16;
                case SqlDbType.VarBinary:
                    return 8000;
                case SqlDbType.VarChar:
                    return 8000;
                case SqlDbType.Variant:
                    return 8000;
                case SqlDbType.Xml:
                    return 8000;
                default:
                    throw new ArgumentException(dbType + " is unknown");
            }
        }

        public static string GetTypeString(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return "bigint";
                case SqlDbType.Binary:
                    return "binary";
                case SqlDbType.Bit:
                    return "bit";
                case SqlDbType.Char:
                    return "char";
                case SqlDbType.Date:
                    return "date";
                case SqlDbType.DateTime:
                    return "datetime";
                case SqlDbType.DateTime2:
                    return "datetime2";
                case SqlDbType.DateTimeOffset:
                    return "datetimeoffset";
                case SqlDbType.Decimal:
                    return "decimal";
                case SqlDbType.Float:
                    return "float";
                case SqlDbType.Image:
                    return "image";
                case SqlDbType.Int:
                    return "int";
                case SqlDbType.Money:
                    return "money";
                case SqlDbType.NChar:
                    return "nchar";
                case SqlDbType.NText:
                    return "ntext";
                case SqlDbType.NVarChar:
                    return "nvarchar";
                case SqlDbType.Real:
                    return "real";
                case SqlDbType.SmallDateTime:
                    return "smalldatetime";
                case SqlDbType.SmallInt:
                    return "smallint";
                case SqlDbType.SmallMoney:
                    return "smallmoney";
                case SqlDbType.Structured:
                    return "structured";
                case SqlDbType.Text:
                    return "text";
                case SqlDbType.Time:
                    return "time";
                case SqlDbType.Timestamp:
                    return "timestamp";
                case SqlDbType.TinyInt:
                    return "tinyint";
                case SqlDbType.Udt:
                    return "varbinary";
                case SqlDbType.UniqueIdentifier:
                    return "uniqueidentifier";
                case SqlDbType.VarBinary:
                    return "varbinary";
                case SqlDbType.VarChar:
                    return "varchar";
                case SqlDbType.Variant:
                    return "variant";
                case SqlDbType.Xml:
                    return "xml";
                default:
                    throw new ArgumentException(dbType + " is unknown");
            }
        }

        public static string GetTypeString(Type netType)
        {
            return GetTypeString(GetDbType(netType));
        }

        #region IDbUtility<SqlDbType> Members


        public static string GetFullTypeString(SqlDbType dbType, int size)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return "bigint";
                case SqlDbType.Binary:
                    return string.Format("binary({0})", size);
                case SqlDbType.Bit: return "bit";
                case SqlDbType.Char:
                    return string.Format("char({0})", size);
                case SqlDbType.Date:
                    return "date";
                case SqlDbType.DateTime:
                    return "datetime";
                case SqlDbType.DateTime2:
                    return "datetime2";
                case SqlDbType.DateTimeOffset:
                    return "datetimeoffset";
                case SqlDbType.Decimal:
                    return "decimal";
                case SqlDbType.Float:
                    return "float";
                case SqlDbType.Image:
                    return "image";
                case SqlDbType.Int:
                    return "int";
                case SqlDbType.Money:
                    return "money";
                case SqlDbType.NChar:
                    return string.Format("nchar({0})", size);
                case SqlDbType.NText:
                    return "ntext";
                case SqlDbType.NVarChar:
                    return string.Format("nvarchar({0})", size == 0 || size > 4000 ? "max" : size.ToString());
                case SqlDbType.Real:
                    return "real";
                case SqlDbType.SmallDateTime:
                    return "smalldatetime";
                case SqlDbType.SmallInt:
                    return "smallint";
                case SqlDbType.SmallMoney:
                    return "smallmoney";
                case SqlDbType.Structured:
                    return "structured";
                case SqlDbType.Text:
                    return "text";
                case SqlDbType.Time:
                    return "time";
                case SqlDbType.Timestamp:
                    return "timestamp";
                case SqlDbType.TinyInt:
                    return "tinyint";
                case SqlDbType.Udt:
                    return "varbinary(max)";
                case SqlDbType.UniqueIdentifier:
                    return "uniqueidentifier";
                case SqlDbType.VarBinary:
                    return string.Format("varbinary({0})", size == 0 || size > 8000 ? "max" : size.ToString());
                case SqlDbType.VarChar:
                    return string.Format("varchar({0})", size == 0 || size > 8000 ? "max" : size.ToString());
                case SqlDbType.Variant:
                    return "variant";
                case SqlDbType.Xml:
                    return "xml";
                default:
                    throw new ArgumentException(dbType + " is unknown");
            }
        }

        public static string GetFullTypeString(Type netType, int size)
        {
            return GetFullTypeString(GetDbType(netType), size);
        }

        public static string GetFullTypeString(SqlDbType dbType)
        {
            return GetFullTypeString(dbType, GetDbSize(dbType));
        }

        public static string GetFullTypeString(Type netType)
        {
            return GetFullTypeString(GetDbType(netType));
        }

        #endregion

        #region IDbUtility<SqlDbType> Members


        string IDbUtility<SqlDbType>.GetFullTypeString(SqlDbType dbType, int size)
        {
            return GetFullTypeString(dbType, size);
        }

        string IDbUtility<SqlDbType>.GetFullTypeString(Type netType, int size)
        {
            return GetFullTypeString(netType, size);
        }

        string IDbUtility<SqlDbType>.GetFullTypeString(SqlDbType dbType)
        {
            return GetFullTypeString(dbType);
        }

        string IDbUtility<SqlDbType>.GetFullTypeString(Type netType)
        {
            return GetFullTypeString(netType);
        }

        #endregion

        #region IDbUtility<SqlDbType> Members


        public static int GetDbSize(Type dbType)
        {
            return GetDbSize(GetDbType(dbType));
        }

        #endregion

        #region IDbUtility<SqlDbType> Members


        int IDbUtility<SqlDbType>.GetDbSize(Type dbType)
        {
            return GetDbSize(dbType);
        }



        #endregion

        #region IDbUtility Members


        public IDataParameter CreateParameter(string parameterName, object value, ParameterDirection paramDirection)
        {
            Type t = value.GetType();
            //pesky unsigned integers!
            if (t == typeof(UInt32))
                value = Convert.ToInt32(value);

            if (t == typeof(UInt64))
                value = Convert.ToInt64(value);

            if (t == typeof(Int16))
                value = Convert.ToInt16(value);

            return new SqlParameter(parameterName.StartsWith("@") ? parameterName : "@" + parameterName, value) { Direction = paramDirection };
        }

        #endregion
    }
}