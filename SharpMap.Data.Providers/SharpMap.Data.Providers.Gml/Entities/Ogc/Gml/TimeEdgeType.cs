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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TimeEdgeType", Namespace = Declarations.SchemaVersion)]
    public class TimeEdgeType : AbstractTimeTopologyPrimitiveType
    {
        [XmlIgnore] private TimeNodePropertyType _end;
        [XmlIgnore] private TimePeriodPropertyType _extent;
        [XmlIgnore] private TimeNodePropertyType _start;

        [XmlElement(Type = typeof (TimeNodePropertyType), ElementName = "end", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimeNodePropertyType End
        {
            get { return _end; }
            set { _end = value; }
        }

        [XmlElement(Type = typeof (TimePeriodPropertyType), ElementName = "extent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimePeriodPropertyType Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        [XmlElement(Type = typeof (TimeNodePropertyType), ElementName = "start", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimeNodePropertyType Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Start.MakeSchemaCompliant();
            End.MakeSchemaCompliant();
        }
    }
}