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
    [Serializable, XmlType(TypeName = "ProjectedCRSType", Namespace = Declarations.SchemaVersion)]
    public class ProjectedCRSType : AbstractGeneralDerivedCRSType
    {
        [XmlIgnore] private BaseGeodeticCRSProperty _baseGeodeticCRS;
        [XmlIgnore] private BaseGeographicCRS _baseGeographicCRS;
        [XmlIgnore] private CartesianCSProperty _cartesianCS;

        [XmlElement(Type = typeof (BaseGeodeticCRSProperty), ElementName = "baseGeodeticCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BaseGeodeticCRSProperty BaseGeodeticCRS
        {
            get { return _baseGeodeticCRS; }
            set { _baseGeodeticCRS = value; }
        }

        [XmlElement(Type = typeof (BaseGeographicCRS), ElementName = "baseGeographicCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BaseGeographicCRS BaseGeographicCRS
        {
            get { return _baseGeographicCRS; }
            set { _baseGeographicCRS = value; }
        }

        [XmlElement(Type = typeof (CartesianCSProperty), ElementName = "cartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CartesianCSProperty CartesianCS
        {
            get { return _cartesianCS; }
            set { _cartesianCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BaseGeodeticCRS.MakeSchemaCompliant();
            BaseGeographicCRS.MakeSchemaCompliant();
            CartesianCS.MakeSchemaCompliant();
        }
    }
}