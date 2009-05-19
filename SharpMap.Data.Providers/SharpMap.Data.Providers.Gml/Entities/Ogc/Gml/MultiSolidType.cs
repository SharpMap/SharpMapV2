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
    [Serializable, XmlType(TypeName = "MultiSolidType", Namespace = Declarations.SchemaVersion)]
    public class MultiSolidType : AbstractGeometricAggregateType
    {
        [XmlIgnore] private List<SolidMember> _solidMember;
        [XmlIgnore] private SolidMembers _solidMembers;

        [XmlElement(Type = typeof (SolidMember), ElementName = "solidMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SolidMember> SolidMember
        {
            get
            {
                if (_solidMember == null)
                {
                    _solidMember = new List<SolidMember>();
                }
                return _solidMember;
            }
            set { _solidMember = value; }
        }

        [XmlElement(Type = typeof (SolidMembers), ElementName = "solidMembers", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SolidMembers SolidMembers
        {
            get { return _solidMembers; }
            set { _solidMembers = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}