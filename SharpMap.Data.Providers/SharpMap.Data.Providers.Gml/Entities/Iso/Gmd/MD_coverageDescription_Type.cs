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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_coverageDescription_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_coverageDescription_Type : AbstractMD_contentInformation_Type
    {
        [XmlIgnore] private RecordTypePropertyType _attributeDescription;
        [XmlIgnore] private MD_coverageContentTypeCode_PropertyType _contentType;
        [XmlIgnore] private List<MD_rangeDimension_PropertyType> _dimension;

        [XmlElement(Type = typeof (RecordTypePropertyType), ElementName = "attributeDescription", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RecordTypePropertyType AttributeDescription
        {
            get { return _attributeDescription; }
            set { _attributeDescription = value; }
        }

        [XmlElement(Type = typeof (MD_coverageContentTypeCode_PropertyType), ElementName = "contentType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_coverageContentTypeCode_PropertyType ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        [XmlElement(Type = typeof (MD_rangeDimension_PropertyType), ElementName = "dimension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_rangeDimension_PropertyType> Dimension
        {
            get
            {
                if (_dimension == null)
                {
                    _dimension = new List<MD_rangeDimension_PropertyType>();
                }
                return _dimension;
            }
            set { _dimension = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            AttributeDescription.MakeSchemaCompliant();
            ContentType.MakeSchemaCompliant();
        }
    }
}