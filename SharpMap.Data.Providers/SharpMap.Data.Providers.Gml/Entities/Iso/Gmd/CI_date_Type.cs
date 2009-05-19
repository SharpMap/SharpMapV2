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
    [Serializable, XmlType(TypeName = "CI_date_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_date_Type : AbstractObjectType
    {
        [XmlIgnore] private DatePropertyType _date;
        [XmlIgnore] private CI_dateTypeCode_PropertyType _dateType;

        [XmlElement(Type = typeof (DatePropertyType), ElementName = "date", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DatePropertyType Date
        {
            get { return _date; }
            set { _date = value; }
        }

        [XmlElement(Type = typeof (CI_dateTypeCode_PropertyType), ElementName = "dateType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_dateTypeCode_PropertyType DateType
        {
            get { return _dateType; }
            set { _dateType = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Date.MakeSchemaCompliant();
            DateType.MakeSchemaCompliant();
        }
    }
}