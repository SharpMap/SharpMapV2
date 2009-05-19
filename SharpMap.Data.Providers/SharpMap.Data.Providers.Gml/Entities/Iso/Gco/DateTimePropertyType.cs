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

namespace SharpMap.Entities.Iso.Gco
{
    [Serializable, XmlType(TypeName = "DateTime_propertyType", Namespace = "http://www.isotc211.org/2005/gco")]
    public class DateTimePropertyType
    {
        [XmlIgnore] private DateTime _dateTime;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] public bool DateTimeSpecified;

        public DateTimePropertyType()
        {
            DateTime = DateTime.Now;
        }

        [XmlElement(ElementName = "DateTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = "http://www.isotc211.org/2005/gco")]
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                DateTimeSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime DateTimeUtc
        {
            get { return _dateTime.ToUniversalTime(); }
            set
            {
                _dateTime = value.ToLocalTime();
                DateTimeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}