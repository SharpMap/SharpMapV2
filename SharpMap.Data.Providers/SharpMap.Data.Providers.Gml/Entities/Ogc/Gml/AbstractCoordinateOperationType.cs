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
    [Serializable, XmlType(TypeName = "AbstractCoordinateOperationType", Namespace = "http://www.opengis.net/gml/3.2")]
    public abstract class AbstractCoordinateOperationType : IdentifiedObjectType
    {
        [XmlIgnore] private List<CoordinateOperationAccuracy> _coordinateOperationAccuracy;
        [XmlIgnore] private DomainOfValidity _domainOfValidity;
        [XmlIgnore] private string _operationVersion;
        [XmlIgnore] private List<string> _scope;
        [XmlIgnore] private SourceCRSProperty _sourceCRS;
        [XmlIgnore] private TargetCRSProperty _targetCrsProperty;

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

        [XmlElement(Type = typeof (DomainOfValidity), ElementName = "domainOfValidity", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DomainOfValidity DomainOfValidity
        {
            get { return _domainOfValidity; }
            set { _domainOfValidity = value; }
        }

        [XmlElement(ElementName = "operationVersion", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = "http://www.opengis.net/gml/3.2")]
        public string OperationVersion
        {
            get { return _operationVersion; }
            set { _operationVersion = value; }
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
        }
    }
}