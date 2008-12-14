/*
 *  The attached / following is part of SharpMap.Data.Providers.DB2_SpatialExtender
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
 *  - DB2 .NET Data Provider from IBM
 *    http://www-01.ibm.com/software/data/db2/windows/dotnet.html
 *    
 */
using System;
using System.Data;
using IBM.Data.DB2;
using IBM.Data.DB2Types;
using SharpMap.Data.Providers.Db;

namespace SharpMap.Data.Providers.DB2_SpatialExtender
{
    public class DB2_SpatialExtender_Utility : IDbUtility<DbType>
    {
        #region IDbUtility<DbType> Members

        public IDbConnection CreateConnection(string connectionString)
        {
            String cns = connectionString.ToLower();
            var cn = new DB2Connection(connectionString);
            cn.CacheData = true;
#if DEBUG
            cn.DebugOn = true;
#endif
            return cn;
        }

        public IDataParameter CreateParameter<TValue>(String parameterName, TValue paramValue,
                                                      ParameterDirection parameterDirection)
        {
            DB2Parameter p = Equals(null, paramValue)
                                    ? new DB2Parameter(
                                          ParameterNameForQueries(parameterName), DBNull.Value)
                                    : new DB2Parameter(
                                          ParameterNameForQueries(parameterName), paramValue);

            p.Direction = parameterDirection;
            return p;
        }

        public IDbCommand CreateCommand()
        {
            return new DB2Command();
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new DB2DataAdapter((DB2Command)cmd);
        }

        public IDataParameter CreateParameter<TValue>(string parameterName, ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDB2Type(GetDbType<TValue>()), parameterDirection);
        }

        public IDataParameter CreateParameter(string parameterName, Type netType,
                                              ParameterDirection parameterDirection)
        {
            return CreateParameter(parameterName, GetDbType(netType), parameterDirection);
        }

        public IDataParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection parameterDirection)
        {
            IDataParameter p= new DB2Parameter(parameterName, GetDB2Type(dbType), GetDbSize(dbType))
                                  {Direction = parameterDirection};
            return p;
        }

        public static string GetTypeString(DbType dbType)
        {
            throw new NotImplementedException();
        }

        public static string GetTypeString(Type netType)
        {
            switch (netType.FullName)
            {
                case "System.Single": return "REAL";
                case "System.Double": return "FLOAT";
                case "System.Decimal": return "DECIMAL";
                case "System.Byte":
                case "System.Boolean":
                case "System.Int16": return "SMALLINT";
                case "System.Int32": return "INT";
                case "System.Int64": return "BIGINT";
                case "System.DateTime": return "TIMESTAMP";
                case "System.TimeSpan": return "TIME";
                //case "System.Guid": return "uuid";
                case "System.Byte[]": return "BLOB";
                case "System.String": return "VARCHAR(250)";
                case "GeoAPI.Geometries.IGeometry": return "BLOB";
                default:
                    throw (new NotSupportedException("Unsupported datatype '" + netType.Name + "' found in datasource"));
            }
        }



        #endregion

        public static DbType GetDbType<TValue>()
        {
            return GetDbType(typeof(TValue));
        }


        public static DB2Type GetDB2Type(DbType dbType)
        {
            switch (dbType.ToString())
            { 
                case "Binary":
                    return DB2Type.Binary;
                case "Int64":
                    return DB2Type.BigInt;
                case "Int32":
                    return DB2Type.Integer;
                case "Boolean":
                case "Byte":
                case "Int16":
                    return DB2Type.SmallInt;
                case "String":
                    return DB2Type.VarChar;
                case "DateTime":
                    return DB2Type.Timestamp;
                case "Time":
                    return DB2Type.Time;
                case "Date":
                    return DB2Type.Date;
                case "Single":
                    return DB2Type.Real;
                case "Double":
                    return DB2Type.Float;
                case "Decimal":
                case "Currency":
                    return DB2Type.Decimal;
                default:
                    throw new NotSupportedException("Unsupported datatype '" + dbType.ToString() + "' found in datasource");
            }
        }
        public static DbType GetDbType(Type netType)
        {
            switch (netType.FullName)
            {
                case "System.Byte":
                    return DbType.Byte;
                case "System.Boolean":
                    return DbType.Boolean;
                case "System.Single":
                    return DbType.Single;
                case "System.Double":
                    return DbType.Double;
                case "System.Int16":
                    return DbType.Int16;
                case "System.Int32":
                    return DbType.Int32;
                case "System.Int64":
                    return DbType.Int64;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Byte[]":
                    return DbType.Binary;
                case "System.String":
                    return DbType.String;
                case "GeoAPI.Geometries.IGeometry":
                    return DbType.Binary;
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
            if (parameterName.StartsWith("@"))
                return parameterName;
            else
                return String.Format("@{0}", parameterName);
        }

        #region IDbUtility<DbType> Members

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


    }
}
