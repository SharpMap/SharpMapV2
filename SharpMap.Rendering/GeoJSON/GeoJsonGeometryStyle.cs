/*
 *	This file is part of SharpMap.Rendering.GeoJson
 *  SharpMap.Rendering.GeoJson is free software © 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using GeoAPI.Geometries;
using SharpMap.Data;

namespace SharpMap.Styles
{
    public delegate IGeometry GeometryPreProcessor(IGeometry inputgeom);

    public delegate IDictionary<string, object> AttributeExtractionDelegate(IFeatureDataRecord record);

    /// <remarks>
    /// could be a bit unwieldy, but i need to be able to get access to the underlying data. Ideally we would be able to drop all the unneccesary style properties..
    /// </remarks>
    public class GeoJsonGeometryStyle
        : GeometryStyle, IGeometryPreProcessingStyle, IGeoJsonGeometryStyle
    {
        public bool IncludeAttributes { get; set; }
        public AttributeExtractionDelegate AttributeExtractionDelegate { get; set; }

        public bool IncludeBBox { get; set; }

        #region IGeometryPreProcessingStyle Members

        public bool PreProcessGeometries { get; set; }
        public GeometryPreProcessor GeometryPreProcessor { get; set; }


        private string _coordinateNumberFormatString = "{0:E}";

        public string CoordinateNumberFormatString
        {
            get { return _coordinateNumberFormatString; }
            set { _coordinateNumberFormatString = value; }
        }

        #endregion
    }
}