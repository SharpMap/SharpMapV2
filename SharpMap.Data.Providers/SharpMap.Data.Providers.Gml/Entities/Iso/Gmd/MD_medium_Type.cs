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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_medium_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_medium_Type : AbstractObjectType
    {
        [XmlIgnore] private List<RealPropertyType> _density;
        [XmlIgnore] private CharacterStringPropertyType _densityUnits;
        [XmlIgnore] private List<MD_mediumFormatCode_PropertyType> _mediumFormat;
        [XmlIgnore] private CharacterStringPropertyType _mediumNote;
        [XmlIgnore] private MD_mediumNameCode_PropertyType _name;
        [XmlIgnore] private IntegerPropertyType _volumes;

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "density", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<RealPropertyType> Density
        {
            get
            {
                if (_density == null)
                {
                    _density = new List<RealPropertyType>();
                }
                return _density;
            }
            set { _density = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "densityUnits", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType DensityUnits
        {
            get { return _densityUnits; }
            set { _densityUnits = value; }
        }

        [XmlElement(Type = typeof (MD_mediumFormatCode_PropertyType), ElementName = "mediumFormat", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_mediumFormatCode_PropertyType> MediumFormat
        {
            get
            {
                if (_mediumFormat == null)
                {
                    _mediumFormat = new List<MD_mediumFormatCode_PropertyType>();
                }
                return _mediumFormat;
            }
            set { _mediumFormat = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "mediumNote", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType MediumNote
        {
            get { return _mediumNote; }
            set { _mediumNote = value; }
        }

        [XmlElement(Type = typeof (MD_mediumNameCode_PropertyType), ElementName = "name", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_mediumNameCode_PropertyType Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "volumes", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType Volumes
        {
            get { return _volumes; }
            set { _volumes = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}