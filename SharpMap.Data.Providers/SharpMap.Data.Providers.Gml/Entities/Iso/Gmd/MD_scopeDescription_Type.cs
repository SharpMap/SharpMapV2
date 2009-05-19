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
    [Serializable, XmlType(TypeName = "MD_scopeDescription_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_scopeDescription_Type
    {
        [XmlIgnore] private List<ObjectReferencePropertyType> _attributeInstances;
        [XmlIgnore] private List<ObjectReferencePropertyType> _attributes;
        [XmlIgnore] private CharacterStringPropertyType _dataset;
        [XmlIgnore] private List<ObjectReferencePropertyType> _featureInstances;
        [XmlIgnore] private List<ObjectReferencePropertyType> _features;
        [XmlIgnore] private CharacterStringPropertyType _other;

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "attributeInstances", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> AttributeInstances
        {
            get
            {
                if (_attributeInstances == null)
                {
                    _attributeInstances = new List<ObjectReferencePropertyType>();
                }
                return _attributeInstances;
            }
            set { _attributeInstances = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "attributes", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<ObjectReferencePropertyType>();
                }
                return _attributes;
            }
            set { _attributes = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "dataset", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Dataset
        {
            get { return _dataset; }
            set { _dataset = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "featureInstances", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> FeatureInstances
        {
            get
            {
                if (_featureInstances == null)
                {
                    _featureInstances = new List<ObjectReferencePropertyType>();
                }
                return _featureInstances;
            }
            set { _featureInstances = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "features", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> Features
        {
            get
            {
                if (_features == null)
                {
                    _features = new List<ObjectReferencePropertyType>();
                }
                return _features;
            }
            set { _features = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "other", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Other
        {
            get { return _other; }
            set { _other = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (ObjectReferencePropertyType _c in Attributes)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (ObjectReferencePropertyType _c in Features)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (ObjectReferencePropertyType _c in FeatureInstances)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (ObjectReferencePropertyType _c in AttributeInstances)
            {
                _c.MakeSchemaCompliant();
            }
            Dataset.MakeSchemaCompliant();
            Other.MakeSchemaCompliant();
        }
    }
}