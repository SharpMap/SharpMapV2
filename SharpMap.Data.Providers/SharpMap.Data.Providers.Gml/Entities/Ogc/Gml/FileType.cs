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
    [Serializable, XmlType(TypeName = "FileType", Namespace = Declarations.SchemaVersion)]
    public class FileType
    {
        [XmlIgnore] private string _compression;
        [XmlIgnore] private string _fileName;
        [XmlIgnore] private string _fileReference;
        [XmlIgnore] private CodeType _fileStructure;
        [XmlIgnore] private string _mimeType;
        [XmlIgnore] private RangeParameters _rangeParameters;

        public FileType()
        {
            FileName = string.Empty;
            FileReference = string.Empty;
        }

        [XmlElement(ElementName = "compression", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI"
            , Namespace = Declarations.SchemaVersion)]
        public string Compression
        {
            get { return _compression; }
            set { _compression = value; }
        }

        [XmlElement(ElementName = "fileName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [XmlElement(ElementName = "fileReference", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "anyURI", Namespace = Declarations.SchemaVersion)]
        public string FileReference
        {
            get { return _fileReference; }
            set { _fileReference = value; }
        }

        [XmlElement(Type = typeof (CodeType), ElementName = "fileStructure", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CodeType FileStructure
        {
            get { return _fileStructure; }
            set { _fileStructure = value; }
        }

        [XmlElement(ElementName = "mimeType", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }

        [XmlElement(Type = typeof (RangeParameters), ElementName = "rangeParameters", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public RangeParameters RangeParameters
        {
            get { return _rangeParameters; }
            set { _rangeParameters = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            RangeParameters.MakeSchemaCompliant();
            FileStructure.MakeSchemaCompliant();
        }
    }
}