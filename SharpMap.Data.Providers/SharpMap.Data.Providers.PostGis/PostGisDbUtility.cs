/*
 *  The attached / following is part of SharpMap.Data.Providers.PostGis
 *  SharpMap.Data.Providers.PostGis is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2008
 *  
 *  This work is based on SharpMap.Data.Providers.Db by John Diss for 
 *  Newgrove Consultants Ltd, www.newgrove.com
 *  
 *  Other than that, this spatial data provider requires:
 *  - SharpMap by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/SharpMap
 *    
 *  - GeoAPI.Net by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/GeoApi
 *    
 *  - Npgsql - .Net Data Provider for Postgresql Provider by 
 *    Josh Cooley,Francisco Figueiredo jr. and others, 
 *    released under this license http://npgsql.projects.postgresql.org/license.html
 *    http://npgsql.projects.postgresql.org/
 *    
 */
using System;
using System.Data;
using Npgsql;
using SharpMap.Data.Providers.Db;

namespace SharpMap.Data.Providers.PostGis
{
    public class PostGisDbUtility : IDbUtility<DbType>
    {
        #region IDbUtility<DbType> Members

        public IDbConnection CreateConnection(string connectionString)
        {
            String cns = connectionString.ToLower();
            if (cns.Contains("enlist=false")) cns = cns.Replace("enlist=false", "enlist=true");
            if (!cns.Contains("enlist=true")) cns += " enlist=true;";

            NpgsqlConnection cn = new NpgsqlConnection(connectionString);
            return cn;
        }

        public IDataParameter CreateParameter<TValue>(String parameterName, TValue paramValue,
                                                      ParameterDirection parameterDirection)
        {
            NpgsqlParameter p = Equals(null, paramValue)
                                    ? new NpgsqlParameter(
                                          ParameterNameForQueries(parameterName), DBNull.Value)
                                    : new NpgsqlParameter(
                                          ParameterNameForQueries(parameterName), paramValue);

            p.Direction = parameterDirection;
            return p;
        }

        public IDbCommand CreateCommand()
        {
            return new NpgsqlCommand();
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new NpgsqlDataAdapter((NpgsqlCommand) cmd);
        }

        public IDataParameter CreateParameter<TValue>(string parameterName, ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType<TValue>(), parameterDirection);
        }

        public IDataParameter CreateParameter(string parameterName, Type netType,
                                              ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType(netType), parameterDirection);
        }

        public IDataParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection parameterDirection)
        {
            IDataParameter p = new NpgsqlParameter(parameterName, dbType, GetDbSize(dbType))
                                   {Direction = parameterDirection};
            return p;
        }

        string IDbUtility<DbType>.GetTypeString(DbType dbType)
        {
            return GetTypeString(dbType);
        }

        string IDbUtility<DbType>.GetTypeString(Type netType)
        {
            return GetTypeString(netType);
        }

        DbType IDbUtility<DbType>.GetDbType<TValue>()
        {
            return GetDbType<TValue>();
        }

        DbType IDbUtility<DbType>.GetDbType(Type netType)
        {
            return GetDbType(netType);
        }

        int IDbUtility<DbType>.GetDbSize(DbType dbType)
        {
            return GetDbSize(dbType);
        }

        string IDbUtility<DbType>.GetFullTypeString(DbType dbType, int size)
        {
            return GetFullTypeString(dbType, size);
        }

        string IDbUtility<DbType>.GetFullTypeString(Type netType, int size)
        {
            return GetFullTypeString(netType, size);
        }

        string IDbUtility<DbType>.GetFullTypeString(DbType dbType)
        {
            return GetFullTypeString(dbType);
        }

        string IDbUtility<DbType>.GetFullTypeString(Type netType)
        {
            return GetFullTypeString(netType);
        }

        int IDbUtility<DbType>.GetDbSize(Type dbType)
        {
            return GetDbSize(dbType);
        }

        public IDataParameter CreateParameter(string parameterName, object value, ParameterDirection paramDirection)
        {
            return new NpgsqlParameter(parameterName.StartsWith(":") ? parameterName : ":" + parameterName, value)
                       {Direction = paramDirection};
        }

        #endregion

        public static string GetTypeString(DbType dbType)
        {
            throw new NotImplementedException();
        }

        public static string GetTypeString(Type netType)
        {
            switch (netType.FullName)
            {
                case "System.Byte":
                    return "int2";
                case "System.Boolean":
                    return "bool";
                case "System.Single":
                    return "float4";
                case "System.Double":
                    return "float8";
                case "System.Decimal":
                    return "numeric";
                case "System.Int16":
                    return "int2";
                case "System.Int32":
                    return "int4";
                case "System.Int64":
                    return "int8";
                case "System.DateTime":
                    return "time";
                case "System.TimeSpan":
                    return "interval";
                case "System.Guid":
                    return "uuid";
                case "System.Byte[]":
                    return "bytea";
                case "System.String":
                    return "text";
                case "System.Array":
                    return "array";
                case "GeoAPI.Geometries.IGeometry":
                    return "geometry";
                default:
                    throw (new NotSupportedException("Unsupported datatype '" + netType.Name + "' found in datasource"));
            }
        }

        public static DbType GetDbType<TValue>()
        {
            return GetDbType(typeof (TValue));
        }

        public static DbType GetDbType(Type netType)
        {
            switch (netType.ToString())
            {
                case "System.Byte":
                    return DbType.Int64;
                case "System.Boolean":
                    return DbType.Int64;
                case "System.Single":
                    return DbType.Double;
                case "System.Double":
                    return DbType.Double;
                case "System.Int16":
                    return DbType.Int64;
                case "System.Int32":
                    return DbType.Int64;
                case "System.Int64":
                    return DbType.Int64;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Byte[]":
                    return DbType.Binary;
                case "System.String":
                    return DbType.String;
                default:
                    throw (new NotSupportedException("Unsupported datatype '" + netType.Name + "' found in datasource"));
            }
        }

        public static int GetDbSize(DbType dbType)
        {
            return 0;
            //throw new NotImplementedException();
        }

        public string ParameterNameForQueries(string parameterName)
        {
            if (parameterName.StartsWith(":"))
                return parameterName;
            else
                return String.Format(":{0}", parameterName);
        }

        public static string GetFullTypeString(DbType dbType, int size)
        {
            throw new NotImplementedException();
        }

        public static string GetFullTypeString(Type netType, int size)
        {
            throw new NotImplementedException();
        }

        public static string GetFullTypeString(DbType dbType)
        {
            return GetFullTypeString(dbType, GetDbSize(dbType));
        }

        public static string GetFullTypeString(Type netType)
        {
            return GetFullTypeString(GetDbType(netType), GetDbSize(netType));
        }

        public static int GetDbSize(Type dbType)
        {
            return GetDbSize(GetDbType(dbType));
        }
    }
}