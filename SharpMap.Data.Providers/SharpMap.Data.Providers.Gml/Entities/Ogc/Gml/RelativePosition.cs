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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable]
    public enum RelativePosition
    {
        [XmlEnum(Name = "After")] After = 1,
        [XmlEnum(Name = "Before")] Before = 0,
        [XmlEnum(Name = "Begins")] Begins = 2,
        [XmlEnum(Name = "BegunBy")] BegunBy = 11,
        [XmlEnum(Name = "Contains")] Contains = 6,
        [XmlEnum(Name = "During")] During = 4,
        [XmlEnum(Name = "EndedBy")] EndedBy = 12,
        [XmlEnum(Name = "Ends")] Ends = 3,
        [XmlEnum(Name = "Equals")] Equals = 5,
        [XmlEnum(Name = "Meets")] Meets = 8,
        [XmlEnum(Name = "MetBy")] MetBy = 10,
        [XmlEnum(Name = "OverlappedBy")] OverlappedBy = 9,
        [XmlEnum(Name = "Overlaps")] Overlaps = 7
    }
}