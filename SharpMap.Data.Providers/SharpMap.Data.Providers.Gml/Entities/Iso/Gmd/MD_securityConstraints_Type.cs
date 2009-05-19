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
    [Serializable, XmlType(TypeName = "MD_securityConstraints_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_securityConstraints_Type : MD_constraints_Type
    {
        [XmlIgnore] private MD_classificationCode_PropertyType _classification;
        [XmlIgnore] private CharacterStringPropertyType _classificationSystem;
        [XmlIgnore] private CharacterStringPropertyType _handlingDescription;
        [XmlIgnore] private CharacterStringPropertyType _userNote;

        [XmlElement(Type = typeof (MD_classificationCode_PropertyType), ElementName = "classification",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_classificationCode_PropertyType Classification
        {
            get { return _classification; }
            set { _classification = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "classificationSystem",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ClassificationSystem
        {
            get { return _classificationSystem; }
            set { _classificationSystem = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "handlingDescription", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType HandlingDescription
        {
            get { return _handlingDescription; }
            set { _handlingDescription = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "userNote", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType UserNote
        {
            get { return _userNote; }
            set { _userNote = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Classification.MakeSchemaCompliant();
        }
    }
}