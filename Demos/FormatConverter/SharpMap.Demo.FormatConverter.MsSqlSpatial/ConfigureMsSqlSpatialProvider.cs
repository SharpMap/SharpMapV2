/*
 *	This file is part of SharpMap.Demo.FormatConverter
 *  SharpMap.Demo.FormatConverter is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Demo.FormatConverter.Common;
using SharpMap.Expressions;
using SharpMap.Utilities;

namespace SharpMap.Demo.FormatConverter.MsSqlSpatial
{
    [ConfigureProvider(typeof(MsSqlSpatialProvider), "MsSqlSpatial")]
    public class ConfigureMsSqlSpatialProvider : IConfigureFeatureSource, IConfigureFeatureTarget
    {
        private MsSqlSpatialProvider _sourceProvider;
        private bool disposed;

        #region IConfigureFeatureSource Members

        public IFeatureProvider ConstructSourceProvider(IGeometryServices geometryServices)
        {
            Console.WriteLine("Please enter the connection string for the source server.");
            string connectionString = Console.ReadLine();
            Console.WriteLine("Please enter the spatial schema");
            string sschema = Console.ReadLine();
            Console.WriteLine("Please enter the data tables' schema");
            string dtschema = Console.ReadLine();
            Console.WriteLine("Please enter the table name.");
            string tableName = Console.ReadLine();
            Console.WriteLine("Please enter the id column name.");
            string oidColumn = Console.ReadLine();
            Console.WriteLine("Please enter the geometry column name.");
            string geometryColumn = Console.ReadLine();
            Console.WriteLine("Please enter the SRID (e.g EPSG:4326)");
            string srid = Console.ReadLine();

            _sourceProvider = new MsSqlSpatialProvider(geometryServices[srid], connectionString, sschema, dtschema,
                                                       tableName, oidColumn, geometryColumn);
            return _sourceProvider;
        }

        public FeatureQueryExpression ConstructSourceQueryExpression()
        {
            return new FeatureQueryExpression(new AllAttributesExpression(), null, null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string OidColumnName
        {
            get { return _sourceProvider.OidColumn; }
        }

        #endregion

        #region IConfigureFeatureTarget Members

        public IWritableFeatureProvider ConstructTargetProvider(Type oidType, IGeometryFactory geometryFactory,
                                                                ICoordinateSystemFactory csFactory,
                                                                FeatureDataTable schemaTable)
        {
            throw new NotImplementedException();
        }

        #endregion

        ~ConfigureMsSqlSpatialProvider()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_sourceProvider != null)
                    _sourceProvider.Close();

                disposed = true;
            }
        }

        #region IConfigureFeatureTarget Members


        public void PostImport()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IConfigureFeatureTarget Members


        public Type OidType
        {
            get { return typeof(long); }
        }

        #endregion
    }
}