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
using SharpMap.Entities.Iso.Gsr;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "EX_verticalExtent_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class EX_verticalExtent_Type : AbstractObjectType
    {
        [XmlIgnore] private RealPropertyType _maximumValue;
        [XmlIgnore] private RealPropertyType _minimumValue;
        [XmlIgnore] private SC_cRS_PropertyType _verticalCRS;

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "maximumValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType MaximumValue
        {
            get { return _maximumValue; }
            set { _maximumValue = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "minimumValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType MinimumValue
        {
            get { return _minimumValue; }
            set { _minimumValue = value; }
        }

        [XmlElement(Type = typeof (SC_cRS_PropertyType), ElementName = "verticalCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public SC_cRS_PropertyType VerticalCRS
        {
            get { return _verticalCRS; }
            set { _verticalCRS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            MinimumValue.MakeSchemaCompliant();
            MaximumValue.MakeSchemaCompliant();
            VerticalCRS.MakeSchemaCompliant();
        }
    }
}