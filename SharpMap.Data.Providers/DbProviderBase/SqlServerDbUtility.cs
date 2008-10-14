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
            return new SqlDataAdapter((SqlCommand) cmd);
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
                                     dbType, GetDbSize(dbType)) {Direction = parameterDirection};
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
            return GetDbType(typeof (TValue));
        }

        public static SqlDbType GetDbType(Type netType)
        {
            throw new NotImplementedException();
        }

        public static int GetDbSize(SqlDbType dbType)
        {
            throw new NotImplementedException();
        }

        public static string GetTypeString(SqlDbType dbType)
        {
            throw new NotImplementedException();
        }

        public static string GetTypeString(Type netType)
        {
            return GetTypeString(GetDbType(netType));
        }
    }
}