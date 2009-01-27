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
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.SpatiaLite2;
using SharpMap.Demo.FormatConverter.Common;
using SharpMap.Expressions;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Demo.FormatConverter.SpatiaLite2
{
    [ConfigureProvider(typeof(SpatiaLite2Provider), "SpatiaLite2")]
    public class ConfigureSpatiaLiteProvider : IConfigureFeatureSource, IConfigureFeatureTarget
    {
        private string _oidColumn;
        private SpatiaLite2Provider _sourceProvider;
        private SpatiaLite2Provider _targetProvider;
        private bool disposed;
        private bool settingChanged = false;

        #region IConfigureFeatureSource Members

        private void header(string header)
        {
            Console.WriteLine("\n*");
            Console.WriteLine(string.Format("* {0}", header));
            Console.WriteLine("*\n");
        }
        public IFeatureProvider ConstructSourceProvider(IGeometryServices geometryServices)
        {
            header("Construct source provider");

            Console.WriteLine("Please enter the connection string for the source database file. Press Enter to take the following:");
            Console.WriteLine(Properties.Settings.Default.SourceConnectionString);
            
            string connectionString = Console.ReadLine();
            if (connectionString == "")
                connectionString = Properties.Settings.Default.SourceConnectionString;

            Console.WriteLine("Please enter the table name.");
            string tableName = Console.ReadLine();
            Console.WriteLine("Please enter the id column name.");
            _oidColumn = Console.ReadLine();
            Console.WriteLine("Please enter the geometry column name.");
            string geometryColumn = Console.ReadLine();

            _sourceProvider = new SpatiaLite2Provider(geometryServices.DefaultGeometryFactory,
                connectionString, "main", tableName, _oidColumn, geometryColumn);

            Console.WriteLine(string.Format(
                "\nThe defined source provider serves shapes of the following type:\n{0}",
                _sourceProvider.ValidGeometryType.ToString()));

            _sourceProvider.CoordinateTransformation = 
                SetCoordinateTransfromation(_sourceProvider.OriginalSpatialReference);

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

            header("Construct target provider");

            Console.WriteLine("Please enter the connection string for the target database file.\nRemember 'Connection Timeout=0' for large datasets.\nPress Enter to take the following:");
            Console.WriteLine(Properties.Settings.Default.TargetConnectionString);
            
            string connectionString = Console.ReadLine();
            if (connectionString == "") 
                connectionString = Properties.Settings.Default.TargetConnectionString;

            Console.WriteLine("Please enter the table name.");
            string tableName = Console.ReadLine();

            SpatiaLite2ShapeType shapeType = GetShapeType();

            SpatiaLite2IndexType spatialIndex = GetSpatialIndex();

            SpatiaLite2Provider.CreateDataTable(schemaTable, tableName, 
                connectionString, SpatiaLite2Provider.DefaultGeometryColumnName,
                shapeType, spatialIndex);

            _targetProvider = new SpatiaLite2Provider(geometryFactory, connectionString, tableName );

            _targetProvider.Open();
            return _targetProvider;
        }

        private ICoordinateTransformation SetCoordinateTransfromation(ICoordinateSystem spatialReference)
        {
            if ( spatialReference != null )
            {
                string lastTargetCS = string.IsNullOrEmpty( Properties.Settings.Default.TargetSrid )
                    ?
                        "(e.g. 'EPSG:4326'). Otherwise just press Enter."
                    :
                        string.Format( "(Last time you used: '{0}'). Only press Enter to do again or \n'{1}' to omit coordinat transformation.", 
                            Properties.Settings.Default.TargetSrid, spatialReference.AuthorityCode );
                
                Console.WriteLine(string.Format(
                    "\nThe Srid Authority Code of the sources' coordinate system is '{0}'.\n" +
                    "If you wish to apply a coordinate transformation supply Srid Authority Code.\n" +
                    "{1}.",
                    spatialReference.AuthorityCode));

                string targetCS = Console.ReadLine();
                if ( string.IsNullOrEmpty( targetCS ) ) targetCS = Properties.Settings.Default.TargetSrid;
                if ( string.Equals( targetCS, spatialReference.AuthorityCode ) ) targetCS = "";

                if (!string.IsNullOrEmpty(targetCS))
                {
                    ICoordinateSystem csTo = SridMap.DefaultInstance.Process(targetCS, (ICoordinateSystem)null);
                    if ( csTo != null )
                    {
                        ICoordinateTransformation ct = new GeometryServices().CoordinateTransformationFactory
                            .CreateFromCoordinateSystems(spatialReference, csTo);

                        return ct;
                    }
                    else
                    {
                        Console.WriteLine( string.Format("Cannot access the following coordinate system: {0}", targetCS));
                    }
                }
            }
            return null;
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

        ~ConfigureSpatiaLiteProvider()
        {
            //Dispose(false);
        }

        private static SpatiaLite2ShapeType GetShapeType()
        {
            string[] names = Enum.GetNames(typeof(SpatiaLite2ShapeType));

            Console.WriteLine("SpatiaLite2 databases allow only homogenous geometry types. The following shape types are available.\n");
            for (int i = 1; i < names.Length; i++) // omit SpatiaLite2ShapeType._Undefined
            {
                Console.WriteLine(string.Format("\t{0} {1}", i, names[i]));
            }

            while (true)
            {
                Console.WriteLine("Please enter the id of the shape type to export.\nIt must match the geometry type in the input data set.");

                int i;

                if (int.TryParse(Console.ReadLine(), out i))
                {
                    if (i > -1 && i < names.Length)
                        return (SpatiaLite2ShapeType)Enum.Parse(typeof(SpatiaLite2ShapeType), names[i]);
                }

                Console.WriteLine("Invalid option.");
            }
        }

        private static SpatiaLite2IndexType GetSpatialIndex()
        {
            string[] names = Enum.GetNames(typeof(SpatiaLite2IndexType));

            Console.WriteLine("SpatiaLite2 databases allow the following spatial indices.\n");
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine(string.Format("\t{0} {1}", i, names[i]));
            }

            while (true)
            {
                Console.WriteLine("Please enter the id of the spatial index to apply.");

                int i;

                if (int.TryParse(Console.ReadLine(), out i))
                {
                    if (i > -1 && i < names.Length)
                        return (SpatiaLite2IndexType)Enum.Parse(typeof(SpatiaLite2IndexType), names[i]);
                }

                Console.WriteLine("Invalid option.");
            }
        }

        #region IConfigureFeatureTarget Members


        public void PostImport()
        {
            if (_sourceProvider != null)
            {
                if (_sourceProvider.ConnectionString != Properties.Settings.Default.SourceConnectionString)
                {
                    Console.WriteLine("\nDo you want to save the source provider connection string for future use (Y/any other key)?");
                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        Properties.Settings.Default.SourceConnectionString = _sourceProvider.ConnectionString;
                        settingChanged = true;
                    }
                }
                if ( _sourceProvider.OriginalSrid != _sourceProvider.Srid )
                {
                    Console.WriteLine( "\nDo you want to save the reprojection Srid for future use(Y/any other key)?" );
                    if ( Console.ReadKey( true ).Key == ConsoleKey.Y )
                    {
                        Properties.Settings.Default.TargetSrid = _sourceProvider.Srid;
                        settingChanged = true;
                    }
                }
            }
            if (_targetProvider != null)
            {
                if (_targetProvider.ConnectionString != Properties.Settings.Default.TargetConnectionString)
                {
                    Console.WriteLine("\nDo you want to save the target provider connection string for future use (Y/any other key)");
                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        Properties.Settings.Default.TargetConnectionString = _targetProvider.ConnectionString;
                        settingChanged = true;
                    }
                }
                Console.WriteLine("\nCleaning up: VACUUM");
                _targetProvider.Vacuum();
            }

            if (settingChanged)
            {
                Properties.Settings.Default.Save();
                settingChanged = false;
                Console.WriteLine("\nSettings saved\n\n");
            }
        }

        #endregion
    }
}