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
    [XmlRoot(ElementName = "PostOffice", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class PostOffice
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private PostalRouteType __PostalRoute;
        [XmlIgnore] private PostBox __PostBox;
        [XmlIgnore] private List<PostOfficeName> __PostOfficeName;
        [XmlIgnore] private PostOfficeNumber __PostOfficeNumber;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return __Indicator; }
            set { __Indicator = value; }
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

        [XmlElement(Type = typeof (PostOfficeName), ElementName = "PostOfficeName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostOfficeName> PostOfficeName
        {
            get
            {
                if (__PostOfficeName == null) __PostOfficeName = new List<PostOfficeName>();
                return __PostOfficeName;
            }
            set { __PostOfficeName = value; }
        }

        [XmlElement(Type = typeof (PostOfficeNumber), ElementName = "PostOfficeNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOfficeNumber PostOfficeNumber
        {
            get
            {
                
                return __PostOfficeNumber;
            }
            set { __PostOfficeNumber = value; }
        }

        [XmlElement(Type = typeof (PostalRouteType), ElementName = "PostalRoute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteType PostalRoute
        {
            get
            {
                
                return __PostalRoute;
            }
            set { __PostalRoute = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get
            {
                
                return __PostBox;
            }
            set { __PostBox = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get
            {
                
                return __PostalCode;
            }
            set { __PostalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}