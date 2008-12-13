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
using System.IO;
using SharpMap.Data;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Demo.FormatConverter.Common;
using SharpMap.Utilities;

namespace SharpMap.Demo.FormatConverter.ShapeFile
{
    [ConfigureProvider(typeof (ShapeFileProvider), "ShapeFile")]
    public class ConfigureShapeFileProvider : IConfigureFeatureSource, IConfigureFeatureTarget
    {
        #region IConfigureFeatureTarget Members

        public IWritableFeatureProvider<TOid> ConstructTargetProvider<TOid>(IGeometryServices geometryServices)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IConfigureFeatureSource Members

        public IFeatureProvider ConstructSourceProvider(IGeometryServices geometryServices)
        {
            string path;
            while (true)
            {
                Console.WriteLine("Please enter the path to the shapefile");
                path = Console.ReadLine();

                if (File.Exists(path))
                    break;
                Console.WriteLine("Invalid Path");
            }

            var p = new ShapeFileProvider(path, geometryServices.DefaultGeometryFactory,
                                          geometryServices.CoordinateSystemFactory) {IsSpatiallyIndexed = false};

            return p;
        }

        #endregion
    }
}