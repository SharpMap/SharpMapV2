// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable]
    public enum MD_pixelOrientationCode_Type
    {
        [XmlEnum(Name = "center")] Center = 0,
        [XmlEnum(Name = "lowerLeft")] LowerLeft = 1,
        [XmlEnum(Name = "lowerRight")] LowerRight = 2,
        [XmlEnum(Name = "upperLeft")] UpperLeft = 4,
        [XmlEnum(Name = "upperRight")] UpperRight = 3
    }
}