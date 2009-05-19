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
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "EX_geographicBoundingBox_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class EX_geographicBoundingBox_Type : AbstractEX_geographicExtent_Type
    {
        [XmlIgnore] private DecimalPropertyType _eastBoundLongitude;
        [XmlIgnore] private DecimalPropertyType _northBoundLatitude;
        [XmlIgnore] private DecimalPropertyType _southBoundLatitude;
        [XmlIgnore] private DecimalPropertyType _westBoundLongitude;

        [XmlElement(Type = typeof (DecimalPropertyType), ElementName = "eastBoundLongitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DecimalPropertyType EastBoundLongitude
        {
            get { return _eastBoundLongitude; }
            set { _eastBoundLongitude = value; }
        }

        [XmlElement(Type = typeof (DecimalPropertyType), ElementName = "northBoundLatitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DecimalPropertyType NorthBoundLatitude
        {
            get { return _northBoundLatitude; }
            set { _northBoundLatitude = value; }
        }

        [XmlElement(Type = typeof (DecimalPropertyType), ElementName = "southBoundLatitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DecimalPropertyType SouthBoundLatitude
        {
            get { return _southBoundLatitude; }
            set { _southBoundLatitude = value; }
        }

        [XmlElement(Type = typeof (DecimalPropertyType), ElementName = "westBoundLongitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DecimalPropertyType WestBoundLongitude
        {
            get { return _westBoundLongitude; }
            set { _westBoundLongitude = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            WestBoundLongitude.MakeSchemaCompliant();
            EastBoundLongitude.MakeSchemaCompliant();
            SouthBoundLatitude.MakeSchemaCompliant();
            NorthBoundLatitude.MakeSchemaCompliant();
        }
    }
}