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
    [Serializable, XmlType(TypeName = "DMSAngleType", Namespace = Declarations.SchemaVersion)]
    public class DMSAngleType
    {
        [XmlIgnore] private decimal _decimalMinutes;
        [XmlIgnore] private Degrees _degrees;
        [XmlIgnore] private string _minutes;
        [XmlIgnore] private decimal _seconds;
        [XmlIgnore] public bool DecimalMinutesSpecified = true;
        [XmlIgnore] public bool SecondsSpecified;

        public DMSAngleType()
        {
            Minutes = string.Empty;
        }

        [XmlElement(ElementName = "decimalMinutes", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "decimal", Namespace = Declarations.SchemaVersion)]
        public decimal DecimalMinutes
        {
            get { return _decimalMinutes; }
            set
            {
                _decimalMinutes = value;
                DecimalMinutesSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Degrees), ElementName = "degrees", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Degrees Degrees
        {
            get { return _degrees; }
            set { _degrees = value; }
        }

        [XmlElement(ElementName = "minutes", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "nonNegativeInteger", Namespace = Declarations.SchemaVersion)]
        public string Minutes
        {
            get { return _minutes; }
            set { _minutes = value; }
        }

        [XmlElement(ElementName = "seconds", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "decimal",
            Namespace = Declarations.SchemaVersion)]
        public decimal Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                SecondsSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
            Degrees.MakeSchemaCompliant();
        }
    }
}