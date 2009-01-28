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
using System.Data;
using System.Reflection;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db;
using SharpMap.Demo.FormatConverter.Common;
using SharpMap.Expressions;
using SharpMap.Utilities;

namespace SharpMap.Demo.FormatConverter.SqlServer2008
{
    [ConfigureProvider(typeof (MsSqlServer2008Provider<>), "Sql Server 2008")]
    public class ConfigureSqlServer2008Provider : IConfigureFeatureSource, IConfigureFeatureTarget
    {
        private string _oidColumn;
        private IFeatureProvider _sourceProvider;
        private Type _specializedType;
        private IWritableFeatureProvider _targetProvider;
        private bool disposed;

        #region IConfigureFeatureSource Members

        public IFeatureProvider ConstructSourceProvider(IGeometryServices geometryServices)
        {
            Console.WriteLine("Please enter the connection string for the source server.");
            string connectionString = Console.ReadLine();
            Console.WriteLine("Please enter the data tables' schema");
            string dtschema = Console.ReadLine();
            Console.WriteLine("Please enter the table name.");
            string tableName = Console.ReadLine();
            Console.WriteLine("Please enter the id column name.");
            _oidColumn = Console.ReadLine();
            Console.WriteLine("Please enter the geometry column name.");
            string geometryColumn = Console.ReadLine();
            Console.WriteLine("Please enter the SRID (e.g EPSG:4326)");
            string srid = Console.ReadLine();

            Type type;
            var dbUtility = new SqlServerDbUtility();
            using (IDbConnection conn = dbUtility.CreateConnection(connectionString))
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT TOP 1 [{0}] FROM [{1}].[{2}] ", _oidColumn, dtschema,
                                                    tableName);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    type = cmd.ExecuteScalar().GetType();
                }
            }

            Type t = typeof (MsSqlServer2008Provider<>);
            Type specialized = t.MakeGenericType(type);

            _sourceProvider =
                (IFeatureProvider)
                Activator.CreateInstance(specialized, geometryServices[srid], connectionString, dtschema, tableName,
                                         _oidColumn, geometryColumn);
            _sourceProvider.Open();

            return _sourceProvider;
        }

        public FeatureQueryExpression ConstructSourceQueryExpression()
        {
            return new FeatureQueryExpression(new AllAttributesExpression(), null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string OidColumnName
        {
            get { return _oidColumn; }
        }

        #endregion

        #region IConfigureFeatureTarget Members

        public IWritableFeatureProvider ConstructTargetProvider(Type oidType, IGeometryFactory geometryFactory,
                                                                ICoordinateSystemFactory csFactory,
                                                                FeatureDataTable schemaTable)
        {
            if (oidType == typeof (UInt16))
                oidType = typeof (Int16);
            else if (oidType == typeof (UInt32))
                oidType = typeof (Int32);
            else if (oidType == typeof (UInt64))
                oidType = typeof (Int64);

            Type typ = typeof (MsSqlServer2008Provider<>);
            _specializedType = typ.MakeGenericType(oidType);

            Console.WriteLine(
                "Please enter the connection string for the target database server. Remember 'Connection Timeout=0' for large datasets.");
            string connectionString = Console.ReadLine();

            Console.WriteLine("Please enter the schema for the table.");
            string schemaName = Console.ReadLine();

            Console.WriteLine("Please enter the table name.");
            string tableName = Console.ReadLine();

            _targetProvider = (IWritableFeatureProvider) _specializedType.GetMethod(
                                                             "Create",
                                                             BindingFlags.Public | BindingFlags.Static,
                                                             null,
                                                             CallingConventions.Standard,
                                                             new[]
                                                                 {
                                                                     typeof (string), typeof (IGeometryFactory),
                                                                     typeof (string), typeof (string),
                                                                     typeof (FeatureDataTable)
                                                                 }, null)
                                                             .Invoke(null,
                                                                     new object[]
                                                                         {
                                                                             connectionString, geometryFactory,
                                                                             schemaName,
                                                                             tableName, schemaTable
                                                                         });

            _targetProvider.Open();
            return _targetProvider;
        }

        public void PostImport()
        {
            try
            {
                _specializedType.GetMethod("FixGeometries", BindingFlags.Public | BindingFlags.Instance, null,
                                           CallingConventions.HasThis, Type.EmptyTypes, null).Invoke(_targetProvider,
                                                                                                     new object[] {});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: an error occured while attempting to fix geometries. " + ex.Message);
            }

            try
            {
                Console.WriteLine("Create Envelope Columns? Y to create otherwise any other key.");
                if (string.Compare(Console.ReadLine(), "Y", StringComparison.CurrentCultureIgnoreCase) == 0)
                    _specializedType.GetMethod("CreateEnvelopeColumns", BindingFlags.Public | BindingFlags.Instance,
                                               null,
                                               CallingConventions.HasThis, Type.EmptyTypes, null).Invoke(
                        _targetProvider, new object[] {});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: an error occured while attempting to create envelope columns. " + ex.Message);
            }

            try
            {
                Console.WriteLine("Creating Spatial Index");
                _specializedType.GetMethod("RebuildSpatialIndex", BindingFlags.Public | BindingFlags.Instance, null,
                                           CallingConventions.HasThis, Type.EmptyTypes, null).Invoke(_targetProvider,
                                                                                                     new object[] {});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: an error occured while attempting to create spatial index. " + ex.Message);
            }

            try
            {
                Console.WriteLine(
                    "Register in Geometry_Columns Table? Y to register otherwise any other key. (Table will be created if it doesn't exist)");
                if (string.Compare(Console.ReadLine(), "Y", StringComparison.CurrentCultureIgnoreCase) == 0)
                    _specializedType.GetMethod("RegisterInGeometryColumnsTable",
                                               BindingFlags.Public | BindingFlags.Instance, null,
                                               CallingConventions.HasThis, Type.EmptyTypes, null)
                        .Invoke(_targetProvider, new object[] {});
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "ERROR: an error occured while attempting to register in the geometry columns table. " + ex.Message);
            }

            try
            {
                Console.WriteLine("Add check constraint on Srid? Y to add constraint otherwise any other key.");
                if (string.Compare(Console.ReadLine(), "Y", StringComparison.CurrentCultureIgnoreCase) == 0)
                    _specializedType.GetMethod("CreateSridConstraint", BindingFlags.Public | BindingFlags.Instance, null,
                                               CallingConventions.HasThis, Type.EmptyTypes, null)
                        .Invoke(_targetProvider, new object[] {});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: an error occured while attempting to add check constraint. " + ex.Message);
            }
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_sourceProvider != null)
                    _sourceProvider.Close();

                if (_targetProvider != null)
                    _targetProvider.Close();

                disposed = true;
            }
        }

        ~ConfigureSqlServer2008Provider()
        {
            Dispose(false);
        }
    }
}