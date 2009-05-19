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
    [Serializable, XmlType(TypeName = "LI_source_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class LI_source_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _description;
        [XmlIgnore] private MD_representativeFraction_PropertyType _scaleDenominator;
        [XmlIgnore] private CI_citation_PropertyType _sourceCitation;
        [XmlIgnore] private List<EX_extent_PropertyType> _sourceExtent;
        [XmlIgnore] private MD_referenceSystem_PropertyType _sourceReferenceSystem;
        [XmlIgnore] private List<LI_processStep_PropertyType> _sourceStep;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (MD_representativeFraction_PropertyType), ElementName = "scaleDenominator",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_representativeFraction_PropertyType ScaleDenominator
        {
            get { return _scaleDenominator; }
            set { _scaleDenominator = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "sourceCitation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType SourceCitation
        {
            get { return _sourceCitation; }
            set { _sourceCitation = value; }
        }

        [XmlElement(Type = typeof (EX_extent_PropertyType), ElementName = "sourceExtent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_extent_PropertyType> SourceExtent
        {
            get
            {
                if (_sourceExtent == null)
                {
                    _sourceExtent = new List<EX_extent_PropertyType>();
                }
                return _sourceExtent;
            }
            set { _sourceExtent = value; }
        }

        [XmlElement(Type = typeof (MD_referenceSystem_PropertyType), ElementName = "sourceReferenceSystem",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_referenceSystem_PropertyType SourceReferenceSystem
        {
            get { return _sourceReferenceSystem; }
            set { _sourceReferenceSystem = value; }
        }

        [XmlElement(Type = typeof (LI_processStep_PropertyType), ElementName = "sourceStep", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<LI_processStep_PropertyType> SourceStep
        {
            get
            {
                if (_sourceStep == null)
                {
                    _sourceStep = new List<LI_processStep_PropertyType>();
                }
                return _sourceStep;
            }
            set { _sourceStep = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}