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
    [Serializable, XmlType(TypeName = "EllipsoidType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class EllipsoidType : IdentifiedObjectType
    {
        [XmlIgnore] private SecondDefiningParameter_E _secondDefiningParameter;
        [XmlIgnore] private SemiMajorAxis _semiMajorAxis;

        [XmlElement(Type = typeof (SecondDefiningParameter_E), ElementName = "secondDefiningParameter",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SecondDefiningParameter_E SecondDefiningParameter
        {
            get { return _secondDefiningParameter; }
            set { _secondDefiningParameter = value; }
        }

        [XmlElement(Type = typeof (SemiMajorAxis), ElementName = "semiMajorAxis", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SemiMajorAxis SemiMajorAxis
        {
            get { return _semiMajorAxis; }
            set { _semiMajorAxis = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            SemiMajorAxis.MakeSchemaCompliant();
            SecondDefiningParameter.MakeSchemaCompliant();
        }
    }
}