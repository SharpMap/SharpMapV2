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
    public class SqlServerDbUtility : IDbUtility
    {
        #region IDbUtility Members

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public IDataParameter CreateParameter(string parameterName,
                                              object parameterValue,
                                              ParameterDirection paramDirection)
        {
            var p = new SqlParameter(parameterName.StartsWith("@") ? parameterName : "@" + parameterName, parameterValue);
            p.Direction = paramDirection;
            return p;
        }

        public IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public string FormatValue<T>(T value)
        {
            return string.Format(GetSqlFormatString(typeof (T)), value);
        }

        public string FormatValue(object value)
        {
            return string.Format(GetSqlFormatString(value.GetType()), value);
        }

        public string GetSqlFormatString(Type t)
        {
            if (t == typeof (string))
                return "'{0}'";

            return "{0}";
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand) cmd);
        }

        #endregion
    }
}