// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "BoundaryType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("outerBoundaryIs", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Boundary
    {
        private KmlObjectBase[] boundaryObjectExtensionGroupField;
        private string[] boundarySimpleExtensionGroupField;
        private LinearRing linearRingField;

        /// <remarks/>
        public LinearRing LinearRing
        {
            get { return linearRingField; }
            set { linearRingField = value; }
        }

        /// <remarks/>
        [XmlElement("BoundarySimpleExtensionGroup")]
        public string[] BoundarySimpleExtensionGroup
        {
            get { return boundarySimpleExtensionGroupField; }
            set { boundarySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("BoundaryObjectExtensionGroup")]
        public KmlObjectBase[] BoundaryObjectExtensionGroup
        {
            get { return boundaryObjectExtensionGroupField; }
            set { boundaryObjectExtensionGroupField = value; }
        }
    }
}