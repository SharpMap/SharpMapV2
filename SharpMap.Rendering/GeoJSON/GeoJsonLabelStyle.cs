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
namespace SharpMap.Styles
{
    public class GeoJsonLabelStyle : LabelStyle, IGeometryPreProcessingStyle
    {
        /// <summary>
        /// Should we return the geometry data with the label?
        /// </summary>
        public bool IncludeGeometryData { get; set; }

        #region IGeometryPreProcessingStyle Members

        /// <summary>
        /// Should we process the geometry (e.g simplify) before rendering 
        /// </summary>
        public bool PreProcessGeometries { get; set; }

        /// <summary>
        /// The action to execute on the geometry if PreProcessGeometries == true (e.g simplify)
        /// </summary>
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