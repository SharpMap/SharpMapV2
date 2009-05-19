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
using SharpMap.Entities.Iso.Gsr;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "AbstractRS_referenceSystem_Type", Namespace = "http://www.isotc211.org/2005/gmd")
    ]
    public abstract class AbstractRS_referenceSystem_Type : AbstractObjectType
    {
        [XmlIgnore] private List<EX_extent_PropertyType> _domainOfValidity;
        [XmlIgnore] private RS_identifier_PropertyType _name;

        [XmlElement(Type = typeof (EX_extent_PropertyType), ElementName = "domainOfValidity", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_extent_PropertyType> DomainOfValidity
        {
            get
            {
                if (_domainOfValidity == null)
                {
                    _domainOfValidity = new List<EX_extent_PropertyType>();
                }
                return _domainOfValidity;
            }
            set { _domainOfValidity = value; }
        }

        [XmlElement(Type = typeof (RS_identifier_PropertyType), ElementName = "name", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RS_identifier_PropertyType Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Name.MakeSchemaCompliant();
        }
    }
}