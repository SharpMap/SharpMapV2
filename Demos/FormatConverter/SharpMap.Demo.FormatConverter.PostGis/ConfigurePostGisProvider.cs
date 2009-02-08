/*
 *  This file is part of SharpMap.Demo.FormatConverter
 *  SharpMap.Demo.FormatConverter is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2009
 *          Ingenieurgruppe IVV GmbH & Co. KG
 *          http://www.ivv-aachen.de
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
using SharpMap.Data.Providers.PostGis;
using SharpMap.Demo.FormatConverter.Common;
using SharpMap.Expressions;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Demo.FormatConverter.PostGis
{
    [ConfigureProvider(typeof(PostGisProvider<>), "PostGis")]
    public class ConfigurePostGisProvider : IConfigureFeatureSource, IConfigureFeatureTarget
    {
        private string _oidColumn;
        private IFeatureProvider _sourceProvider;
        private Type _specializedType;
        private IWritableFeatureProvider _targetProvider;
        private bool disposed;
        private bool settingChanged = false;

        #region IConfigureFeatureSource Members

        private void header(string header)
        {
            Console.WriteLine("\n*");
            Console.WriteLine(string.Format("* {0}", header));
            Console.WriteLine("*\n");
        }

        private string GetValue( string question, string defaultAnswer )
        {
            Console.WriteLine( question );

            if ( !string.IsNullOrEmpty(defaultAnswer))
                Console.WriteLine( string.Format( "If you just press enter\n  '{0}'\nis used.", defaultAnswer ) );

            string answer = "";
            while(true)
            {
                answer = Console.ReadLine();
                if ( string.IsNullOrEmpty( answer ) )
                {
                    if ( !string.IsNullOrEmpty( defaultAnswer ) ) return defaultAnswer;
                }
                else
                {
                    return answer;
                }
                Console.WriteLine( question );
            }

        }

        public IFeatureProvider ConstructSourceProvider(IGeometryServices geometryServices)
        {
            header( "Construct PostGis source provider\n" +
                  "* Author Felix Obermaier 2009\n" +
                  "* Ingenieurgruppe IVV GmbH & Co. KG\n" +
                  "* http://www.ivv-aachen.de" );

            string connectionString = GetValue(
                "Please enter the connection string for the source database file.",
                Properties.Settings.Default.SourceConnectionString);

            string schema = GetValue( "Please enter the schema name",
                Properties.Settings.Default.SourceSchema );

            string tableName = GetValue( "Please enter the table name.", null );

            _oidColumn = GetValue( "Please enter the id column name.", null );

            string geometryColumn = GetValue("Please enter the geometry column name.", null);

            Type type;
            var dbUtility = new PostGisDbUtility();
            using (IDbConnection conn = dbUtility.CreateConnection(connectionString))
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT \"{0}\" FROM \"{1}\".\"{2}\" LIMIT 1;", _oidColumn, schema,
                                                    tableName);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    type = cmd.ExecuteScalar().GetType();
                }
            }

            Type t = typeof (PostGisProvider<>);
            Type specialized = t.MakeGenericType(type);

            _sourceProvider =
                (IFeatureProvider)
                Activator.CreateInstance(specialized, 
                                         geometryServices.DefaultGeometryFactory, 
                                         connectionString, schema, tableName,
                                         _oidColumn,
                                         geometryColumn,
                                         geometryServices.CoordinateTransformationFactory );
            _sourceProvider.Open();

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

            if ( oidType == typeof( UInt16 ) )
                oidType = typeof( Int16 );
            else if ( oidType == typeof( UInt32 ) )
                oidType = typeof( Int32 );
            else if ( oidType == typeof( UInt64 ) )
                oidType = typeof( Int64 );

            Type typ = typeof( PostGisProvider<> );
            _specializedType = typ.MakeGenericType( oidType );

            header( "Construct PostGis target provider\n" +
                  "* Author Felix Obermaier 2009\n" +
                  "* Ingenieurgruppe IVV GmbH & Co. KG\n" +
                  "* http://www.ivv-aachen.de" );

            string connectionString = GetValue(
                "Please enter the connection string for the source database file.",
                Properties.Settings.Default.SourceConnectionString );

            string schema = GetValue( "Please enter the schema name",
                Properties.Settings.Default.SourceSchema );

            string tableName = GetValue( "Please enter the table name.", null );

            //PostGisProviderStatic.CreateDataTable<oidType.GetType()>( schemaTable, schema, tableName, connectionString)
            Type pgpstatic = typeof(PostGisProviderStatic);
            MethodInfo miCreateDataTable = pgpstatic.GetMethod(
                "CreateDataTable", 
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof( FeatureDataTable ), typeof( string ), typeof( string ), typeof( string ), typeof( string ) },
                null);

            MethodInfo miCreateDataTableGeneric = miCreateDataTable.MakeGenericMethod(new[] {oidType});

            miCreateDataTableGeneric.Invoke(
                null, 
                new object[] {schemaTable, schema, tableName,connectionString, PostGisProviderStatic.DefaultGeometryColumnName});

            _targetProvider =
                (IWritableFeatureProvider)
                Activator.CreateInstance(_specializedType, 
                                         schemaTable.GeometryFactory, 
                                         connectionString, schema, tableName,
                                         schemaTable.Columns[0].ColumnName,
                                         PostGisProviderStatic.DefaultGeometryColumnName,
                                         new GeometryServices().CoordinateTransformationFactory);

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

        public void PostImport( )
        {
            if ( _sourceProvider != null )
            {
                if ( _sourceProvider.ConnectionId != Properties.Settings.Default.SourceConnectionString )
                {
                    Console.WriteLine( "\nDo you want to save the source provider connection string for future use (Y/any other key)?" );
                    if ( Console.ReadKey( true ).Key == ConsoleKey.Y )
                    {
                        Properties.Settings.Default.SourceConnectionString = _sourceProvider.ConnectionId;
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
            if ( _targetProvider != null )
            {
                if ( _targetProvider.ConnectionId != Properties.Settings.Default.TargetConnectionString )
                {
                    Console.WriteLine( "\nDo you want to save the target provider connection string for future use (Y/any other key)" );
                    if ( Console.ReadKey( true ).Key == ConsoleKey.Y )
                    {
                        Properties.Settings.Default.TargetConnectionString = _targetProvider.ConnectionId;
                        settingChanged = true;
                    }
                }
                //Console.WriteLine( "\nCleaning up: VACUUM" );
                //_targetProvider.Vacuum();
            }

            if ( settingChanged )
            {
                Properties.Settings.Default.Save();
                settingChanged = false;
                Console.WriteLine( "\nSettings saved\n\n" );
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

        ~ConfigurePostGisProvider( )
        {
            //Dispose(false)
        }

        //private static SpatiaLite2ShapeType GetShapeType()
        //{
        //    string[] names = Enum.GetNames(typeof(SpatiaLite2ShapeType));

        //    Console.WriteLine("SpatiaLite2 databases allow only homogenous geometry types. The following shape types are available.\n");
        //    for (int i = 1; i < names.Length; i++) // omit SpatiaLite2ShapeType._Undefined
        //    {
        //        Console.WriteLine(string.Format("\t{0} {1}", i, names[i]));
        //    }

        //    while (true)
        //    {
        //        Console.WriteLine("Please enter the id of the shape type to export.\nIt must match the geometry type in the input data set.");

        //        int i;

        //        if (int.TryParse(Console.ReadLine(), out i))
        //        {
        //            if (i > -1 && i < names.Length)
        //                return (SpatiaLite2ShapeType)Enum.Parse(typeof(SpatiaLite2ShapeType), names[i]);
        //        }

        //        Console.WriteLine("Invalid option.");
        //    }
        //}

        //private static SpatiaLite2IndexType GetSpatialIndex()
        //{
        //    string[] names = Enum.GetNames(typeof(SpatiaLite2IndexType));

        //    Console.WriteLine("SpatiaLite2 databases allow the following spatial indices.\n");
        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        Console.WriteLine(string.Format("\t{0} {1}", i, names[i]));
        //    }

        //    while (true)
        //    {
        //        Console.WriteLine("Please enter the id of the spatial index to apply.");

        //        int i;

        //        if (int.TryParse(Console.ReadLine(), out i))
        //        {
        //            if (i > -1 && i < names.Length)
        //                return (SpatiaLite2IndexType)Enum.Parse(typeof(SpatiaLite2IndexType), names[i]);
        //        }

        //        Console.WriteLine("Invalid option.");
        //    }
        //}

    }
}