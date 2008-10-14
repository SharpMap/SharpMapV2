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

namespace SharpMap.Data.Providers.Db
{
    public interface IDbUtility
    {
        IDbConnection CreateConnection(string connectionString);

        IDataParameter CreateParameter<TValue>(string parameterName, TValue parameterValue,
                                               ParameterDirection paramDirection);

        IDbCommand CreateCommand();
        IDbDataAdapter CreateAdapter(IDbCommand cmd);


        IDataParameter CreateParameter<TValue>(string parameterName, ParameterDirection parameterDirection);
        IDataParameter CreateParameter(string parameterName, Type netType, ParameterDirection parameterDirection);
    }

    public interface IDbUtility<TDbType> : IDbUtility
    {
        string GetTypeString(TDbType dbType);
        string GetTypeString(Type netType);

        TDbType GetDbType<TValue>();
        TDbType GetDbType(Type netType);
        int GetDbSize(TDbType dbType);
        IDataParameter CreateParameter(string parameterName, TDbType dbType, ParameterDirection parameterDirection);
    }
}