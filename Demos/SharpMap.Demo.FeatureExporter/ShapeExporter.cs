/*
 *	This file is part of SharpMap.Demo.FeatureExporter
 *  SharpMap.Demo.FeatureExporter is free software © 2008 Newgrove Consultants Limited, 
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
using System.IO;
using SharpMap.Data;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Utilities;

namespace SharpMap.Demo.FeatureExporter
{
    public class ShapeExporter : IDisposable
    {
        private static readonly IGeometryServices _geometryServices = new GeometryServices();
        private readonly ShapeFileProvider _provider;

        public ShapeExporter(string shapefilepath)
        {
            _provider = new ShapeFileProvider(shapefilepath, _geometryServices.DefaultGeometryFactory,
                                              _geometryServices.CoordinateSystemFactory, false);

            _provider.IsSpatiallyIndexed = false;
            _provider.Open();
        }

        public ShapeFileProvider Provider
        {
            get { return _provider; }
        }

        private string ExportDirectory { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            _provider.Close();
        }

        #endregion

        public static bool Run()
        {
            Console.WriteLine("Please enter the path to the shapefile");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                using (var exporter = new ShapeExporter(path))
                {
                    FeatureDataTable tbl = exporter.Provider.CreateNewTable();
                    Console.WriteLine("The shapefile dbf contains the following columns:");
                    foreach (DataColumn c in tbl.Columns)
                        Console.WriteLine(c.ColumnName);

                    Console.WriteLine("Please enter the ColumnName to base the files on");

                    string colName = Console.ReadLine();

                    if (tbl.Columns[colName] == null)
                        Console.WriteLine("Invalid Column");
                    else
                        exporter.Export(colName);
                }
            }
            else
                Console.WriteLine("Invalid Path");

            Console.WriteLine("Run again? Y to run again. Any other key to quit");

            return Console.ReadLine() == "Y";
        }

        private void Export(string colName)
        {
            ExportDirectory = Path.Combine(Path.GetDirectoryName(Provider.Filename), Path.GetFileNameWithoutExtension(Provider.Filename) + "_Export");
            if (!Directory.Exists(ExportDirectory))
                Directory.CreateDirectory(ExportDirectory);

            FeatureDataTable fdt = Provider.CreateNewTable();

            IFeatureDataReader reader = Provider.GetReader();
            while (reader.Read())
            {
                string exportFilePath = GenerateUniqueName(reader[colName].ToString());
                using (
                    ShapeFileProvider export = ShapeFileProvider.Create(ExportDirectory, exportFilePath,
                                                                        Provider.ShapeType, Provider.CreateNewTable(),
                                                                        Provider.GeometryFactory))
                {
                    export.IsSpatiallyIndexed = false;
                    export.Open();


                    var fdr = (FeatureDataRow<uint>)fdt.NewRow();
                    var vals = new object[fdt.Columns.Count];

                    reader.GetValues(vals);

                    fdr.ItemArray = vals;
                    fdr.Geometry = reader.Geometry;
                    export.Insert(fdr);
                    export.Close();
                }
            }
        }

        private string GenerateUniqueName(string p)
        {
            string path = Path.Combine(ExportDirectory, string.Format("{0}.shp", p));

            if (!File.Exists(path))
                return p;

            int i = 0;
            while (true)
            {
                string newName = string.Format("{0}_{1}", p, i);

                path = Path.Combine(ExportDirectory, string.Format("{0}.shp", newName));
                if (!File.Exists(path))
                    return newName;
                i += 1;
            }
        }
    }
}