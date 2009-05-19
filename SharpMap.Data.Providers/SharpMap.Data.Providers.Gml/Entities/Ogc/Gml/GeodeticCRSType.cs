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
    [Serializable, XmlType(TypeName = "GeodeticCRSType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class GeodeticCRSType : AbstractCRSType
    {
        [XmlIgnore] private CartesianCSProperty _cartesianCS;
        [XmlIgnore] private EllipsoidalCSProperty _ellipsoidalCS;
        [XmlIgnore] private GeodeticDatumProperty _geodeticDatum;
        [XmlIgnore] private SphericalCSProperty _sphericalCS;

        [XmlElement(Type = typeof (CartesianCSProperty), ElementName = "cartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CartesianCSProperty CartesianCS
        {
            get { return _cartesianCS; }
            set { _cartesianCS = value; }
        }

        [XmlElement(Type = typeof (EllipsoidalCSProperty), ElementName = "ellipsoidalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public EllipsoidalCSProperty EllipsoidalCS
        {
            get { return _ellipsoidalCS; }
            set { _ellipsoidalCS = value; }
        }

        [XmlElement(Type = typeof (GeodeticDatumProperty), ElementName = "geodeticDatum", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public GeodeticDatumProperty GeodeticDatum
        {
            get { return _geodeticDatum; }
            set { _geodeticDatum = value; }
        }

        [XmlElement(Type = typeof (SphericalCSProperty), ElementName = "sphericalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SphericalCSProperty SphericalCS
        {
            get { return _sphericalCS; }
            set { _sphericalCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            EllipsoidalCS.MakeSchemaCompliant();
            CartesianCS.MakeSchemaCompliant();
            SphericalCS.MakeSchemaCompliant();
            GeodeticDatum.MakeSchemaCompliant();
        }
    }
}