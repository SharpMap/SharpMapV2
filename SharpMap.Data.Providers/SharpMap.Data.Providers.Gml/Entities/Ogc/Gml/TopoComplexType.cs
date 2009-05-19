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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TopoComplexType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TopoComplexType : AbstractTopologyType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private bool _isMaximal;
        [XmlIgnore] private MaximalComplex _maximalComplex;
        [XmlIgnore] private List<SubComplex> _subComplex;
        [XmlIgnore] private List<SuperComplex> _superComplex;
        [XmlIgnore] private List<TopoPrimitiveMember> _topoPrimitiveMember;
        [XmlIgnore] private TopoPrimitiveMembers _topoPrimitiveMembers;
        [XmlIgnore] public bool AggregationTypeSpecified;
        [XmlIgnore] public bool IsMaximalSpecified;

        public TopoComplexType()
        {
            IsMaximal = false;
        }

        [XmlAttribute(AttributeName = "aggregationType")]
        public AggregationType AggregationType
        {
            get { return _aggregationType; }
            set
            {
                _aggregationType = value;
                AggregationTypeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "isMaximal", DataType = "boolean")]
        public bool IsMaximal
        {
            get { return _isMaximal; }
            set
            {
                _isMaximal = value;
                IsMaximalSpecified = true;
            }
        }

        [XmlElement(Type = typeof (MaximalComplex), ElementName = "maximalComplex", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MaximalComplex MaximalComplex
        {
            get { return _maximalComplex; }
            set { _maximalComplex = value; }
        }

        [XmlElement(Type = typeof (SubComplex), ElementName = "subComplex", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<SubComplex> SubComplex
        {
            get
            {
                if (_subComplex == null)
                {
                    _subComplex = new List<SubComplex>();
                }
                return _subComplex;
            }
            set { _subComplex = value; }
        }

        [XmlElement(Type = typeof (SuperComplex), ElementName = "superComplex", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<SuperComplex> SuperComplex
        {
            get
            {
                if (_superComplex == null)
                {
                    _superComplex = new List<SuperComplex>();
                }
                return _superComplex;
            }
            set { _superComplex = value; }
        }

        [XmlElement(Type = typeof (TopoPrimitiveMember), ElementName = "topoPrimitiveMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<TopoPrimitiveMember> TopoPrimitiveMember
        {
            get
            {
                if (_topoPrimitiveMember == null)
                {
                    _topoPrimitiveMember = new List<TopoPrimitiveMember>();
                }
                return _topoPrimitiveMember;
            }
            set { _topoPrimitiveMember = value; }
        }

        [XmlElement(Type = typeof (TopoPrimitiveMembers), ElementName = "topoPrimitiveMembers", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TopoPrimitiveMembers TopoPrimitiveMembers
        {
            get { return _topoPrimitiveMembers; }
            set { _topoPrimitiveMembers = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            MaximalComplex.MakeSchemaCompliant();
        }
    }
}