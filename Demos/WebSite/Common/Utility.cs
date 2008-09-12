/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using System.Globalization;
using GeoAPI.Geometries;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public static class Utility
    {
        public static IExtents2D ParseExtents(IGeometryFactory geomFactory, string extents)
        {
            string[] strVals = extents.Split(new[] {','});
            if (strVals.Length != 4)
                return null;
            double minx = 0;
            double miny = 0;
            double maxx = 0;
            double maxy = 0;
            if (!double.TryParse(strVals[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out minx))
                return null;
            if (!double.TryParse(strVals[2], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out maxx))
                return null;
            if (maxx < minx)
                return null;

            if (!double.TryParse(strVals[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out miny))
                return null;
            if (!double.TryParse(strVals[3], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out maxy))
                return null;
            if (maxy < miny)
                return null;

            return geomFactory.CreateExtents2D(minx, miny, maxx, maxy);
        }
    }
}