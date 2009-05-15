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
        [XmlIgnore] private List<Change> _Change;
        [XmlIgnore] private List<Create> _Create;
        [XmlIgnore] private List<Delete> _Delete;
        [XmlIgnore] private string _targetHref;
        [XmlIgnore] private List<string> _UpdateExtensionGroup;
        [XmlIgnore] private List<string> _UpdateOpExtensionGroup;

        public UpdateType()
        {
            targetHref = string.Empty;
        }

        [XmlElement(ElementName = "targetHref", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string targetHref
        {
            get { return _targetHref; }
            set { _targetHref = value; }
        }

        [XmlElement(Type = typeof (Create), ElementName = "Create", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Create> Create
        {
            get
            {
                if (_Create == null) _Create = new List<Create>();
                return _Create;
            }
            set { _Create = value; }
        }

        [XmlElement(Type = typeof (Delete), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Delete> Delete
        {
            get
            {
                if (_Delete == null) _Delete = new List<Delete>();
                return _Delete;
            }
            set { _Delete = value; }
        }

        [XmlElement(Type = typeof (Change), ElementName = "Change", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Change> Change
        {
            get
            {
                if (_Change == null) _Change = new List<Change>();
                return _Change;
            }
            set { _Change = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "UpdateOpExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> UpdateOpExtensionGroup
        {
            get
            {
                if (_UpdateOpExtensionGroup == null) _UpdateOpExtensionGroup = new List<string>();
                return _UpdateOpExtensionGroup;
            }
            set { _UpdateOpExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "UpdateExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> UpdateExtensionGroup
        {
            get
            {
                if (_UpdateExtensionGroup == null) _UpdateExtensionGroup = new List<string>();
                return _UpdateExtensionGroup;
            }
            set { _UpdateExtensionGroup = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (Create _c in Create) _c.MakeSchemaCompliant();
            foreach (Delete _c in Delete) _c.MakeSchemaCompliant();
            foreach (Change _c in Change) _c.MakeSchemaCompliant();
        }
    }
}