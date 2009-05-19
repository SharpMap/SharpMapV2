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
    [Serializable, XmlType(TypeName = "MD_browseGraphic_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_browseGraphic_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _fileDescription;
        [XmlIgnore] private CharacterStringPropertyType _fileName;
        [XmlIgnore] private CharacterStringPropertyType _fileType;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "fileDescription", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType FileDescription
        {
            get { return _fileDescription; }
            set { _fileDescription = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "fileName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "fileType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            FileName.MakeSchemaCompliant();
        }
    }
}