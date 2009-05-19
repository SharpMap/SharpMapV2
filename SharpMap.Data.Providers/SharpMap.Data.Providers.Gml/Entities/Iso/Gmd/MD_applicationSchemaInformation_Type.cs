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
    [Serializable,
     XmlType(TypeName = "MD_applicationSchemaInformation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_applicationSchemaInformation_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _constraintLanguage;
        [XmlIgnore] private BinaryPropertyType _graphicsFile;
        [XmlIgnore] private CI_citation_PropertyType _name;
        [XmlIgnore] private CharacterStringPropertyType _schemaAscii;
        [XmlIgnore] private CharacterStringPropertyType _schemaLanguage;
        [XmlIgnore] private BinaryPropertyType _softwareDevelopmentFile;
        [XmlIgnore] private CharacterStringPropertyType _softwareDevelopmentFileFormat;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "constraintLanguage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ConstraintLanguage
        {
            get { return _constraintLanguage; }
            set { _constraintLanguage = value; }
        }

        [XmlElement(Type = typeof (BinaryPropertyType), ElementName = "graphicsFile", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BinaryPropertyType GraphicsFile
        {
            get { return _graphicsFile; }
            set { _graphicsFile = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "name", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "schemaAscii", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType SchemaAscii
        {
            get { return _schemaAscii; }
            set { _schemaAscii = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "schemaLanguage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType SchemaLanguage
        {
            get { return _schemaLanguage; }
            set { _schemaLanguage = value; }
        }

        [XmlElement(Type = typeof (BinaryPropertyType), ElementName = "softwareDevelopmentFile", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BinaryPropertyType SoftwareDevelopmentFile
        {
            get { return _softwareDevelopmentFile; }
            set { _softwareDevelopmentFile = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "softwareDevelopmentFileFormat",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType SoftwareDevelopmentFileFormat
        {
            get { return _softwareDevelopmentFileFormat; }
            set { _softwareDevelopmentFileFormat = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Name.MakeSchemaCompliant();
            SchemaLanguage.MakeSchemaCompliant();
            ConstraintLanguage.MakeSchemaCompliant();
        }
    }
}