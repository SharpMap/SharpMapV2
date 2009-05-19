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
    [Serializable, XmlType(TypeName = "AbstractGeneralTransformationType", Namespace = "http://www.opengis.net/gml/3.2")
    ]
    public abstract class AbstractGeneralTransformationType : AbstractCoordinateOperationType
    {
        [XmlIgnore] private List<CoordinateOperationAccuracy> _coordinateOperationAccuracy;
        [XmlIgnore] private Description _description;
        [XmlIgnore] private DescriptionReference _descriptionReference;
        [XmlIgnore] private DomainOfValidity _domainOfValidity;
        [XmlIgnore] private string _id;
        [XmlIgnore] private Identifier _identifier;
        [XmlIgnore] private List<MetaDataProperty> _metaDataProperty;
        [XmlIgnore] private List<Name> _name;
        [XmlIgnore] private string _operationVersion;
        [XmlIgnore] private string _remarks;
        [XmlIgnore] private List<string> _scope;
        [XmlIgnore] private SourceCRSProperty _sourceCRS;
        [XmlIgnore] private TargetCRSProperty _targetCrsProperty;

        public AbstractGeneralTransformationType()
        {
            OperationVersion = string.Empty;
        }

        [XmlElement(Type = typeof (CoordinateOperationAccuracy), ElementName = "coordinateOperationAccuracy",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<CoordinateOperationAccuracy> CoordinateOperationAccuracy
        {
            get
            {
                if (_coordinateOperationAccuracy == null)
                {
                    _coordinateOperationAccuracy = new List<CoordinateOperationAccuracy>();
                }
                return _coordinateOperationAccuracy;
            }
            set { _coordinateOperationAccuracy = value; }
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

        [XmlElement(Type = typeof (DomainOfValidity), ElementName = "domainOfValidity", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DomainOfValidity DomainOfValidity
        {
            get { return _domainOfValidity; }
            set { _domainOfValidity = value; }
        }

        [XmlAttribute(AttributeName = "id", DataType = "ID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlElement(Type = typeof (Identifier), ElementName = "identifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Identifier Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
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

        [XmlElement(ElementName = "operationVersion", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = "http://www.opengis.net/gml/3.2")]
        public string OperationVersion
        {
            get { return _operationVersion; }
            set { _operationVersion = value; }
        }

        [XmlElement(ElementName = "remarks", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public string Remarks
        {
            get { return _remarks; }
            set { _remarks = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "scope", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = "http://www.opengis.net/gml/3.2")]
        public List<string> Scope
        {
            get
            {
                if (_scope == null)
                {
                    _scope = new List<string>();
                }
                return _scope;
            }
            set { _scope = value; }
        }

        [XmlElement(Type = typeof (SourceCRSProperty), ElementName = "sourceCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SourceCRSProperty SourceCRS
        {
            get { return _sourceCRS; }
            set { _sourceCRS = value; }
        }

        [XmlElement(Type = typeof (TargetCRSProperty), ElementName = "targetCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TargetCRSProperty TargetCRS
        {
            get { return _targetCrsProperty; }
            set { _targetCrsProperty = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Identifier.MakeSchemaCompliant();
            SourceCRS.MakeSchemaCompliant();
            TargetCRS.MakeSchemaCompliant();
        }
    }
}