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
    [Serializable, XmlType(TypeName = "MD_aggregateInformation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_aggregateInformation_Type : AbstractObjectType
    {
        [XmlIgnore] private MD_identifier_PropertyType _aggregateDataSetIdentifier;
        [XmlIgnore] private CI_citation_PropertyType _aggregateDataSetName;
        [XmlIgnore] private DS_associationTypeCode_PropertyType _associationType;
        [XmlIgnore] private DS_initiativeTypeCode_PropertyType _initiativeType;

        [XmlElement(Type = typeof (MD_identifier_PropertyType), ElementName = "aggregateDataSetIdentifier",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_identifier_PropertyType AggregateDataSetIdentifier
        {
            get { return _aggregateDataSetIdentifier; }
            set { _aggregateDataSetIdentifier = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "aggregateDataSetName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType AggregateDataSetName
        {
            get { return _aggregateDataSetName; }
            set { _aggregateDataSetName = value; }
        }

        [XmlElement(Type = typeof (DS_associationTypeCode_PropertyType), ElementName = "associationType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DS_associationTypeCode_PropertyType AssociationType
        {
            get { return _associationType; }
            set { _associationType = value; }
        }

        [XmlElement(Type = typeof (DS_initiativeTypeCode_PropertyType), ElementName = "initiativeType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DS_initiativeTypeCode_PropertyType InitiativeType
        {
            get { return _initiativeType; }
            set { _initiativeType = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            AssociationType.MakeSchemaCompliant();
        }
    }
}