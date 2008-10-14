/*
 *  The attached / following is part of SharpMap.Data.Providers.PostGis
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
 *  - Npgsql - .Net Data Provider for Postgresql Provider by 
 *    Josh Cooley,Francisco Figueiredo jr. and others, 
 *    released under this license http://npgsql.projects.postgresql.org/license.html
 *    http://npgsql.projects.postgresql.org/
 *    
 */
using System;
using System.Runtime.Serialization;

namespace SharpMap.Data.Providers.PostGis
{
    /// <summary>
    /// Exception thrown during SpatiaLite2 operations
    /// </summary>
    public class PostGis_Exception : SharpMapDataException
    {
        public PostGis_Exception() : base() { }
        public PostGis_Exception(String message) : base(message) { }
        public PostGis_Exception(String message, Exception inner) : base(message, inner) { }
        public PostGis_Exception(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
