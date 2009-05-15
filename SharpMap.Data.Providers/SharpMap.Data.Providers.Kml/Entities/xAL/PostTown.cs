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
    [XmlType(TypeName = "PostTown", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostTown
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<PostTownName> _PostTownName;
        [XmlIgnore] private PostTownSuffix _PostTownSuffix;
        [XmlIgnore] private string _Type;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_AddressLine == null) _AddressLine = new List<AddressLine>();
                return _AddressLine;
            }
            set { _AddressLine = value; }
        }

        [XmlElement(Type = typeof (PostTownName), ElementName = "PostTownName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostTownName> PostTownName
        {
            get
            {
                if (_PostTownName == null) _PostTownName = new List<PostTownName>();
                return _PostTownName;
            }
            set { _PostTownName = value; }
        }

        [XmlElement(Type = typeof (PostTownSuffix), ElementName = "PostTownSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostTownSuffix PostTownSuffix
        {
            get { return _PostTownSuffix; }
            set { _PostTownSuffix = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}