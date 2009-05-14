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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "UpdateType", Namespace = Declarations.SchemaVersion), Serializable]
    public class UpdateType
    {
        [XmlIgnore] private List<Change> __Change;
        [XmlIgnore] private List<Create> __Create;
        [XmlIgnore] private List<Delete> __Delete;
        [XmlIgnore] private string __targetHref;
        [XmlIgnore] private List<string> __UpdateExtensionGroup;
        [XmlIgnore] private List<string> __UpdateOpExtensionGroup;

        public UpdateType()
        {
            targetHref = string.Empty;
        }

        [XmlElement(ElementName = "targetHref", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string targetHref
        {
            get { return __targetHref; }
            set { __targetHref = value; }
        }

        [XmlElement(Type = typeof (Create), ElementName = "Create", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Create> Create
        {
            get
            {
                if (__Create == null) __Create = new List<Create>();
                return __Create;
            }
            set { __Create = value; }
        }

        [XmlElement(Type = typeof (Delete), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Delete> Delete
        {
            get
            {
                if (__Delete == null) __Delete = new List<Delete>();
                return __Delete;
            }
            set { __Delete = value; }
        }

        [XmlElement(Type = typeof (Change), ElementName = "Change", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Change> Change
        {
            get
            {
                if (__Change == null) __Change = new List<Change>();
                return __Change;
            }
            set { __Change = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "UpdateOpExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> UpdateOpExtensionGroup
        {
            get
            {
                if (__UpdateOpExtensionGroup == null) __UpdateOpExtensionGroup = new List<string>();
                return __UpdateOpExtensionGroup;
            }
            set { __UpdateOpExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "UpdateExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> UpdateExtensionGroup
        {
            get
            {
                if (__UpdateExtensionGroup == null) __UpdateExtensionGroup = new List<string>();
                return __UpdateExtensionGroup;
            }
            set { __UpdateExtensionGroup = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (Create _c in Create) _c.MakeSchemaCompliant();
            foreach (Delete _c in Delete) _c.MakeSchemaCompliant();
            foreach (Change _c in Change) _c.MakeSchemaCompliant();
        }
    }
}