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
using System.Runtime.Serialization;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Exception thrown during SpatiaLite2 operations
    /// </summary>
    public class DB2SpatialExtenderException : SharpMapDataException
    {
        public DB2SpatialExtenderException()
        {
        }

        public DB2SpatialExtenderException(String message) : base(message)
        {
        }

        public DB2SpatialExtenderException(String message, Exception inner) : base(message, inner)
        {
        }

        public DB2SpatialExtenderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}