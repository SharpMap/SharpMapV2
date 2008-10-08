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
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace SharpMap.Presentation.AspNet
{
    public interface IGeometryServices
    {
        ICoordinateFactory CoordinateFactory { get; }
        ICoordinateSystemFactory CoordinateSystemFactory { get; }
        ICoordinateTransformationFactory CoordinateTransformationFactory { get; }
        IGeometryFactory DefaultGeometryFactory { get; }
        IGeometryFactory this[int? srid] { get; }
    }
}