// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "PostalRouteType", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostalRouteType
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private List<PostalRouteName> __PostalRouteName;
        [XmlIgnore] private PostalRouteNumber __PostalRouteNumber;
        [XmlIgnore] private PostBox __PostBox;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (__AddressLine == null) __AddressLine = new List<AddressLine>();
                return __AddressLine;
            }
            set { __AddressLine = value; }
        }

        [XmlElement(Type = typeof (PostalRouteName), ElementName = "PostalRouteName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalRouteName> PostalRouteName
        {
            get
            {
                if (__PostalRouteName == null) __PostalRouteName = new List<PostalRouteName>();
                return __PostalRouteName;
            }
            set { __PostalRouteName = value; }
        }

        [XmlElement(Type = typeof (PostalRouteNumber), ElementName = "PostalRouteNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteNumber PostalRouteNumber
        {
            get
            {
                if (__PostalRouteNumber == null) __PostalRouteNumber = new PostalRouteNumber();
                return __PostalRouteNumber;
            }
            set { __PostalRouteNumber = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get
            {
                if (__PostBox == null) __PostBox = new PostBox();
                return __PostBox;
            }
            set { __PostBox = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (PostalRouteName _c in PostalRouteName) _c.MakeSchemaCompliant();
            PostalRouteNumber.MakeSchemaCompliant();
        }
    }
}