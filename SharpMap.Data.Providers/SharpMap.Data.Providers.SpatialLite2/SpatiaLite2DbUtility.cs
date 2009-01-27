/*
 *  The attached / following is part of SharpMap.Data.Providers.SpatiaLite2
 *  SharpMap.Data.Providers.SpatiaLite2 is free software � 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
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
 *  - SqLite, dedicated to public domain
 *    http://www.sqlite.org
 *  - SpatiaLite-2.2 by Alessandro Furieri released under a disjunctive tri-license:
 *    - 'Mozilla Public License, version 1.1 or later' OR
 *    - 'GNU General Public License, version 2.0 or later' 
 *    - 'GNU Lesser General Public License, version 2.1 or later' <--- this is the one
 *    http://www.gaia-gis.it/spatialite-2.2/index.html
 *    
 *  - SQLite ADO.NET 2.0 Provider by Robert Simpson, dedicated to public domain
 *    http://sourceforge.net/projects/sqlite-dotnet2
 *    
 */
using System;
using System.Data;
using System.Data.SQLite;
using SharpMap.Data.Providers.Db;

namespace SharpMap.Data.Providers.SpatiaLite2
{
    public class SpatiaLite2DbUtility : IDbUtility<DbType>
    {
        #region IDbUtility<DbType> Members

        public IDbConnection CreateConnection(string connectionString)
        {
            //load spatialite-extension
            var cn = new SQLiteConnection(connectionString);
            cn.Open();
            object ret = new SQLiteCommand("SELECT load_extension('libspatialite-2.dll');", cn).ExecuteScalar();
            //cn.Close();

            return cn;
        }

        public IDataParameter CreateParameter<TValue>(String parameterName, TValue parameterValue,
                                                      ParameterDirection paramDirection)
        {
            SQLiteParameter p = Equals(null, parameterValue)
                                    ? new SQLiteParameter(ParameterNameForQueries(parameterName), DBNull.Value)
                                    : new SQLiteParameter(ParameterNameForQueries(parameterName), parameterValue);
            p.Direction = paramDirection;
            return p;
        }

        public IDbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new SQLiteDataAdapter((SQLiteCommand)cmd);
        }

        public IDataParameter CreateParameter(string parameterName, Type netType,
                                              ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType(netType), parameterDirection);
        }


        public IDataParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection parameterDirection)
        {
            var p = new SQLiteParameter(ParameterNameForQueries(parameterName), dbType, GetDbSize(dbType)) { Direction = parameterDirection };
            return p;
        }

        public IDataParameter CreateParameter<TValue>(string parameterName, ParameterDirection paramDirection)
        {
            return CreateParameter(parameterName, GetDbType<TValue>(), paramDirection);
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

        #endregion

        public static DbType GetDbType<TValue>()
        {
            return GetDbType(typeof(TValue));
        }

        public static DbType GetDbType(Type netType)
        {
					switch ( netType.FullName )
					{
						case "System.Byte":
						case "System.SByte":
						case "System.Boolean":
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.UInt16":
						case "System.UInt32":
						case "System.UInt64":
							return DbType.Int64;
						case "System.Single":
						case "System.Double":
							return DbType.Double;
						case "System.DateTime":
							return DbType.DateTime;
						case "System.Byte[]":
							return DbType.Binary;
						case "System.String":
							return DbType.String;
						default:
							throw ( new NotSupportedException( "Unsupported datatype '" + netType.Name + "' found in datasource" ) );
					}
        }

        public String ParameterNameForQueries(String parameterName)
        {
            return parameterName.StartsWith("@")
                       ?
                           parameterName
                       :
                           String.Format("@{0}", parameterName);
        }

        public static int GetDbSize(DbType dbType)
        {
            return 0;
        }

        public static string GetTypeString(DbType dbType)
        {
            throw new NotImplementedException();
        }

        public static string GetTypeString(Type netType)
        {
            switch (netType.FullName)
            {
                case "System.Byte":
								case "System.SByte":
								case "System.Boolean":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    return "INTEGER";
                case "System.Single":
                case "System.Double":
                    return "REAL";
                case "System.DateTime":
                    return "DATETIME";
                case "System.Byte[]":
                    return "BLOB";
                case "System.String":
                    return "TEXT";
                case "GeoAPI.Geometries.IGeometry":
                    return "BLOB";
                default:
                    throw (new NotSupportedException("Unsupported datatype '" + netType.Name + "' found in datasource"));
            }
        }

        #region IDbUtility<DbType> Members


        public static string GetFullTypeString(DbType dbType, int size)
        {
            throw new NotImplementedException();
        }

        public static string GetFullTypeString(Type netType, int size)
        {
            return GetFullTypeString(GetDbType(netType), size);
        }

        public static string GetFullTypeString(DbType dbType)
        {
            return GetFullTypeString(dbType, GetDbSize(dbType));
        }

        public static string GetFullTypeString(Type netType)
        {
            return GetFullTypeString(GetDbType(netType));
        }

        public static int GetDbSize(Type dbType)
        {
            return GetDbSize(GetDbType(dbType));
        }

        #endregion

        #region IDbUtility<DbType> Members


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

        #endregion

        #region IDbUtility Members


        public IDataParameter CreateParameter(string parameterName, object value, ParameterDirection paramDirection)
        {
            return new SQLiteParameter(parameterName.StartsWith("@") ? parameterName : "@" + parameterName, value) { Direction = paramDirection };
        }

        #endregion
    }
}
