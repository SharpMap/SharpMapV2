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
    [Serializable, XmlType(TypeName = "MD_geometricObjects_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_geometricObjects_Type : AbstractObjectType
    {
        [XmlIgnore] private IntegerPropertyType _geometricObjectCount;
        [XmlIgnore] private MD_geometricObjectTypeCode_PropertyType _geometricObjectType;

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "geometricObjectCount", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType GeometricObjectCount
        {
            get { return _geometricObjectCount; }
            set { _geometricObjectCount = value; }
        }

        [XmlElement(Type = typeof (MD_geometricObjectTypeCode_PropertyType), ElementName = "geometricObjectType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_geometricObjectTypeCode_PropertyType GeometricObjectType
        {
            get { return _geometricObjectType; }
            set { _geometricObjectType = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            GeometricObjectType.MakeSchemaCompliant();
        }
    }
}