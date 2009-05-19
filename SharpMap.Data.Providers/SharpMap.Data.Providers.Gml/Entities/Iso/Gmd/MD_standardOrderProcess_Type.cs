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
    [Serializable, XmlType(TypeName = "MD_standardOrderProcess_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_standardOrderProcess_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _fees;
        [XmlIgnore] private CharacterStringPropertyType _orderingInstructions;
        [XmlIgnore] private DateTimePropertyType _plannedAvailableDateTime;
        [XmlIgnore] private CharacterStringPropertyType _turnaround;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "fees", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Fees
        {
            get { return _fees; }
            set { _fees = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "orderingInstructions",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType OrderingInstructions
        {
            get { return _orderingInstructions; }
            set { _orderingInstructions = value; }
        }

        [XmlElement(Type = typeof (DateTimePropertyType), ElementName = "plannedAvailableDateTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DateTimePropertyType PlannedAvailableDateTime
        {
            get { return _plannedAvailableDateTime; }
            set { _plannedAvailableDateTime = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "turnaround", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Turnaround
        {
            get { return _turnaround; }
            set { _turnaround = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}