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
using System.Collections.Generic;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.SpatiaLite2
{
    internal class SpatiaLite2ExpressionTreeToSqlCompiler
        : ExpressionTreeToSqlCompilerBase<long>
    {
        public SpatiaLite2ExpressionTreeToSqlCompiler(SpatialDbProviderBase<long> provider,
                                                       Expression query,
                                                       SpatiaLite2IndexType indexType)

            : base(provider, query)
        {
        }
        
        protected override void WriteSpatialGeometryExpressionSqlInternal(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom)
        {

            if (op != SpatialOperation.None)
            {
                builder.Append(string.Format(" {0}( GeomFromWKB({1}), {2} )",
                  Enum.GetName(typeof(SpatialOperation), op),
                  CreateParameter(geom).ParameterName,
                  CreateParameter(Provider.GeometryColumn).ParameterName));

                builder.Append(" AND");
                WriteSpatialExtentsExpressionSqlInternal(builder, op, geom.Extents);
            }

        }

        protected override void WriteSpatialExtentsExpressionSqlInternal(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            IExtents2D exts = (IExtents2D)ext;

            builder.Append( string.Format( " Mbr{0}( BuildMbr({1}, {2}, {3}, {4}),{5} )",
                spatialOperation.ToString(),
                CreateParameter<double>( exts.XMin ).ParameterName,
                CreateParameter<double>( exts.YMin ).ParameterName,
                CreateParameter<double>( exts.XMax + 1.0e-6 ).ParameterName,
                CreateParameter<double>( exts.YMax + 1.0e-6 ).ParameterName,
                Provider.GeometryColumn ) );

        }
    }

}