// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
    [XmlType(TypeName = "PostTown", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostTown
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<PostTownName> _postTownName;
        [XmlIgnore] private PostTownSuffix _postTownSuffix;
        [XmlIgnore] private string _type;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_addressLine == null) _addressLine = new List<AddressLine>();
                return _addressLine;
            }
            set { _addressLine = value; }
        }

        [XmlElement(Type = typeof (PostTownName), ElementName = "PostTownName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostTownName> PostTownName
        {
            get
            {
                if (_postTownName == null) _postTownName = new List<PostTownName>();
                return _postTownName;
            }
            set { _postTownName = value; }
        }

        [XmlElement(Type = typeof (PostTownSuffix), ElementName = "PostTownSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostTownSuffix PostTownSuffix
        {
            get { return _postTownSuffix; }
            set { _postTownSuffix = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}