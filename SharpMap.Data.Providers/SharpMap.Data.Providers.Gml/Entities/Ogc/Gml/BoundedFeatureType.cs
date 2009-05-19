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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "BoundedFeatureType", Namespace = "http://www.opengis.net/gml/3.2")]
    public abstract class BoundedFeatureType : AbstractFeatureType
    {
        [XmlIgnore] private BoundedBy _boundedBy;
        [XmlIgnore] private Description _description;
        [XmlIgnore] private DescriptionReference _descriptionReference;
        [XmlIgnore] private Identifier _identifier;
        [XmlIgnore] private Location _location;
        [XmlIgnore] private List<MetaDataProperty> _metaDataProperty;
        [XmlIgnore] private List<Name> _name;

        [XmlElement(Type = typeof (BoundedBy), ElementName = "boundedBy", IsNullable = true,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public BoundedBy BoundedBy
        {
            get { return _boundedBy; }
            set { _boundedBy = value; }
        }

        [XmlElement(Type = typeof (Description), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (DescriptionReference), ElementName = "descriptionReference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DescriptionReference DescriptionReference
        {
            get { return _descriptionReference; }
            set { _descriptionReference = value; }
        }

        [XmlElement(Type = typeof (Identifier), ElementName = "identifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Identifier Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        [XmlElement(Type = typeof (Location), ElementName = "location", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        [XmlElement(Type = typeof (MetaDataProperty), ElementName = "metaDataProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<MetaDataProperty> MetaDataProperty
        {
            get
            {
                if (_metaDataProperty == null)
                {
                    _metaDataProperty = new List<MetaDataProperty>();
                }
                return _metaDataProperty;
            }
            set { _metaDataProperty = value; }
        }

        [XmlElement(Type = typeof (Name), ElementName = "name", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public List<Name> Name
        {
            get
            {
                if (_name == null)
                {
                    _name = new List<Name>();
                }
                return _name;
            }
            set { _name = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BoundedBy.MakeSchemaCompliant();
        }
    }
}