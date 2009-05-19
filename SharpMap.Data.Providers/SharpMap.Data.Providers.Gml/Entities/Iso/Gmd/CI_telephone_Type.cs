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
    [Serializable, XmlType(TypeName = "CI_telephone_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_telephone_Type : AbstractObjectType
    {
        [XmlIgnore] private List<CharacterStringPropertyType> _facsimile;
        [XmlIgnore] private List<CharacterStringPropertyType> _voice;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "facsimile", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Facsimile
        {
            get
            {
                if (_facsimile == null)
                {
                    _facsimile = new List<CharacterStringPropertyType>();
                }
                return _facsimile;
            }
            set { _facsimile = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "voice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Voice
        {
            get
            {
                if (_voice == null)
                {
                    _voice = new List<CharacterStringPropertyType>();
                }
                return _voice;
            }
            set { _voice = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}