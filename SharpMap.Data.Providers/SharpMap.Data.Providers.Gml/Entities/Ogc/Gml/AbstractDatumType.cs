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
    [Serializable, XmlType(TypeName = "AbstractDatumType", Namespace = "http://www.opengis.net/gml/3.2")]
    public abstract class AbstractDatumType : IdentifiedObjectType
    {
        [XmlIgnore] private AnchorDefinition _anchorDefinition;
        [XmlIgnore] private DomainOfValidity _domainOfValidity;
        [XmlIgnore] private DateTime _realizationEpoch = DateTime.Now;
        [XmlIgnore] private List<string> _scope;
        [XmlIgnore] public bool RealizationEpochSpecified;

        [XmlElement(Type = typeof (AnchorDefinition), ElementName = "anchorDefinition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AnchorDefinition AnchorDefinition
        {
            get { return _anchorDefinition; }
            set { _anchorDefinition = value; }
        }

        [XmlElement(Type = typeof (DomainOfValidity), ElementName = "domainOfValidity", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DomainOfValidity DomainOfValidity
        {
            get { return _domainOfValidity; }
            set { _domainOfValidity = value; }
        }

        [XmlElement(ElementName = "realizationEpoch", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "date", Namespace = "http://www.opengis.net/gml/3.2")]
        public DateTime RealizationEpoch
        {
            get { return _realizationEpoch; }
            set
            {
                _realizationEpoch = value;
                RealizationEpochSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime RealizationEpochUtc
        {
            get { return _realizationEpoch.ToUniversalTime(); }
            set
            {
                _realizationEpoch = value.ToLocalTime();
                RealizationEpochSpecified = true;
            }
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

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}