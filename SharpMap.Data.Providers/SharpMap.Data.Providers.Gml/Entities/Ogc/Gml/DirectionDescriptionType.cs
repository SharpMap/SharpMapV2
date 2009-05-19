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
    [Serializable, XmlType(TypeName = "DirectionDescriptionType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DirectionDescriptionType
    {
        [XmlIgnore] private CompassPointEnumeration _compassPoint;
        [XmlIgnore] private Description _description;
        [XmlIgnore] private CodeType _keyword;
        [XmlIgnore] private ReferenceType _reference;
        [XmlIgnore] public bool CompassPointSpecified = true;

        [XmlElement(ElementName = "compassPoint", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public CompassPointEnumeration CompassPoint
        {
            get { return _compassPoint; }
            set
            {
                _compassPoint = value;
                CompassPointSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Description), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (CodeType), ElementName = "keyword", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CodeType Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        [XmlElement(Type = typeof (ReferenceType), ElementName = "reference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public ReferenceType Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            Keyword.MakeSchemaCompliant();
            Description.MakeSchemaCompliant();
            Reference.MakeSchemaCompliant();
        }
    }
}