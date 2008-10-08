/*
 *  The attached / following is part of SharpMap.Data.Providers.SpatiaLite2
 *  SharpMap.Data.Providers.SpatiaLite2 is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
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
  public class SpatiaLite2_Utility : IDbUtility
  {
    #region IDbUtility Members

    public IDbConnection CreateConnection(string connectionString)
    {
      //load spatialite-extension
      SQLiteConnection cn = new SQLiteConnection(connectionString);
      cn.Open();
      object ret = new SQLiteCommand("SELECT load_extension('libspatialite-2.dll');", cn).ExecuteScalar();
      //cn.Close();

      return cn;
    }

    public IDataParameter CreateParameter<TValue>(string parameterName, TValue parameterValue,
                                          ParameterDirection paramDirection)
    {
      SQLiteParameter p = new SQLiteParameter(string.Format("@{0}", parameterName), parameterValue);
      p.Direction = paramDirection;
      return p;
    }

    public IDataParameter CreateParameterByType(string parameterName, DbType parameterType,
                                      ParameterDirection paramDirection)
    {
      if (!parameterName.StartsWith("@"))
        parameterName = "@" + parameterName;
      SQLiteParameter p = new SQLiteParameter( parameterName, parameterType);
      p.Direction = paramDirection;
      return p;
    }

    public IDbCommand CreateCommand()
    {
      return new SQLiteCommand();
    }

    public string FormatValue<T>(T value)
    {
      return string.Format(GetSqlFormatString(typeof(T)), value);
    }

    public string FormatValue(object value)
    {
      return string.Format(GetSqlFormatString(value.GetType()), value);
    }

    public string GetSqlFormatString(Type t)
    {
      if (t == typeof(string))
        return "'{0}'";

      return "{0}";
    }

    public IDbDataAdapter CreateAdapter(IDbCommand cmd)
    {
      return new SQLiteDataAdapter((SQLiteCommand)cmd);
    }

    #endregion

  }
}