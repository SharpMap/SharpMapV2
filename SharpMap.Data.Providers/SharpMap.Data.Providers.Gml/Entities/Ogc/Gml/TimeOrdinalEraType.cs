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
    [Serializable, XmlType(TypeName = "TimeOrdinalEraType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimeOrdinalEraType : DefinitionType
    {
        [XmlIgnore] private TimeNodePropertyType _end;
        [XmlIgnore] private TimePeriodPropertyType _extent;
        [XmlIgnore] private GroupProperty _group;
        [XmlIgnore] private List<Member> _member;
        [XmlIgnore] private List<RelatedTimeType> _relatedTime;
        [XmlIgnore] private TimeNodePropertyType _start;

        [XmlElement(Type = typeof (TimeNodePropertyType), ElementName = "end", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeNodePropertyType End
        {
            get { return _end; }
            set { _end = value; }
        }

        [XmlElement(Type = typeof (TimePeriodPropertyType), ElementName = "extent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimePeriodPropertyType Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        [XmlElement(Type = typeof (GroupProperty), ElementName = "group", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public GroupProperty Group
        {
            get { return _group; }
            set { _group = value; }
        }

        [XmlElement(Type = typeof (Member), ElementName = "member", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public List<Member> Member
        {
            get
            {
                if (_member == null)
                {
                    _member = new List<Member>();
                }
                return _member;
            }
            set { _member = value; }
        }

        [XmlElement(Type = typeof (RelatedTimeType), ElementName = "relatedTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<RelatedTimeType> RelatedTime
        {
            get
            {
                if (_relatedTime == null)
                {
                    _relatedTime = new List<RelatedTimeType>();
                }
                return _relatedTime;
            }
            set { _relatedTime = value; }
        }

        [XmlElement(Type = typeof (TimeNodePropertyType), ElementName = "start", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeNodePropertyType Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}