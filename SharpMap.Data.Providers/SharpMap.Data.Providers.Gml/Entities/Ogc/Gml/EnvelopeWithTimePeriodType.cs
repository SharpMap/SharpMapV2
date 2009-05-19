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
    [Serializable, XmlType(TypeName = "EnvelopeWithTimePeriodType", Namespace = Declarations.SchemaVersion)]
    public class EnvelopeWithTimePeriodType : EnvelopeType
    {
        [XmlIgnore] private TimePositionType _beginPosition;
        [XmlIgnore] private TimePositionType _endPosition;
        [XmlIgnore] private string _frame;

        public EnvelopeWithTimePeriodType()
        {
            Frame = "#ISO-8601";
        }

        [XmlElement(Type = typeof (TimePositionType), ElementName = "beginPosition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimePositionType BeginPosition
        {
            get { return _beginPosition; }
            set { _beginPosition = value; }
        }

        [XmlElement(Type = typeof (TimePositionType), ElementName = "endPosition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimePositionType EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }

        [XmlAttribute(AttributeName = "frame", DataType = "anyURI")]
        public string Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BeginPosition.MakeSchemaCompliant();
            EndPosition.MakeSchemaCompliant();
        }
    }
}