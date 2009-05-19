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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "GeocentricCRSType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class GeocentricCRSType : AbstractCRSType
    {
        [XmlIgnore] private UsesCartesianCS _usesCartesianCS;
        [XmlIgnore] private UsesGeodeticDatum _usesGeodeticDatum;
        [XmlIgnore] private UsesSphericalCS _usesSphericalCS;

        [XmlElement(Type = typeof (UsesCartesianCS), ElementName = "usesCartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public UsesCartesianCS UsesCartesianCS
        {
            get { return _usesCartesianCS; }
            set { _usesCartesianCS = value; }
        }

        [XmlElement(Type = typeof (UsesGeodeticDatum), ElementName = "usesGeodeticDatum", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public UsesGeodeticDatum UsesGeodeticDatum
        {
            get { return _usesGeodeticDatum; }
            set { _usesGeodeticDatum = value; }
        }

        [XmlElement(Type = typeof (UsesSphericalCS), ElementName = "usesSphericalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public UsesSphericalCS UsesSphericalCS
        {
            get { return _usesSphericalCS; }
            set { _usesSphericalCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            UsesCartesianCS.MakeSchemaCompliant();
            UsesSphericalCS.MakeSchemaCompliant();
            UsesGeodeticDatum.MakeSchemaCompliant();
        }
    }
}