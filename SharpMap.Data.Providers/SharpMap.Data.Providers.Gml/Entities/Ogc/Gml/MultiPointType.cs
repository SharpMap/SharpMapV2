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
    [Serializable, XmlType(TypeName = "MultiPointType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class MultiPointType : AbstractGeometricAggregateType
    {
        [XmlIgnore] private List<PointMember> _pointMember;
        [XmlIgnore] private PointMembers _pointMembers;

        [XmlElement(Type = typeof (PointMember), ElementName = "pointMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<PointMember> PointMember
        {
            get
            {
                if (_pointMember == null)
                {
                    _pointMember = new List<PointMember>();
                }
                return _pointMember;
            }
            set { _pointMember = value; }
        }

        [XmlElement(Type = typeof (PointMembers), ElementName = "pointMembers", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public PointMembers PointMembers
        {
            get { return _pointMembers; }
            set { _pointMembers = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}