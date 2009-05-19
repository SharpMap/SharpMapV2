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
    [Serializable, XmlType(TypeName = "AbstractDQ_element_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public abstract class AbstractDQ_element_Type : AbstractObjectType
    {
        [XmlIgnore] private List<DateTimePropertyType> _dateTime;
        [XmlIgnore] private CharacterStringPropertyType _evaluationMethodDescription;
        [XmlIgnore] private DQ_evaluationMethodTypeCode_PropertyType _evaluationMethodType;
        [XmlIgnore] private CI_citation_PropertyType _evaluationProcedure;
        [XmlIgnore] private CharacterStringPropertyType _measureDescription;
        [XmlIgnore] private MD_identifier_PropertyType _measureIdentification;
        [XmlIgnore] private List<CharacterStringPropertyType> _nameOfMeasure;
        [XmlIgnore] private List<DQ_result_PropertyType> _result;

        [XmlElement(Type = typeof (DateTimePropertyType), ElementName = "dateTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DateTimePropertyType> DateTime
        {
            get
            {
                if (_dateTime == null)
                {
                    _dateTime = new List<DateTimePropertyType>();
                }
                return _dateTime;
            }
            set { _dateTime = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "evaluationMethodDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType EvaluationMethodDescription
        {
            get { return _evaluationMethodDescription; }
            set { _evaluationMethodDescription = value; }
        }

        [XmlElement(Type = typeof (DQ_evaluationMethodTypeCode_PropertyType), ElementName = "evaluationMethodType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DQ_evaluationMethodTypeCode_PropertyType EvaluationMethodType
        {
            get { return _evaluationMethodType; }
            set { _evaluationMethodType = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "evaluationProcedure", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType EvaluationProcedure
        {
            get { return _evaluationProcedure; }
            set { _evaluationProcedure = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "measureDescription", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType MeasureDescription
        {
            get { return _measureDescription; }
            set { _measureDescription = value; }
        }

        [XmlElement(Type = typeof (MD_identifier_PropertyType), ElementName = "measureIdentification",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_identifier_PropertyType MeasureIdentification
        {
            get { return _measureIdentification; }
            set { _measureIdentification = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "nameOfMeasure", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> NameOfMeasure
        {
            get
            {
                if (_nameOfMeasure == null)
                {
                    _nameOfMeasure = new List<CharacterStringPropertyType>();
                }
                return _nameOfMeasure;
            }
            set { _nameOfMeasure = value; }
        }

        [XmlElement(Type = typeof (DQ_result_PropertyType), ElementName = "result", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DQ_result_PropertyType> Result
        {
            get
            {
                if (_result == null)
                {
                    _result = new List<DQ_result_PropertyType>();
                }
                return _result;
            }
            set { _result = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DQ_result_PropertyType _c in Result)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}