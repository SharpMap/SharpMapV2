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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "AbstractCurveSegmentType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractCurveSegmentType
    {
        [XmlIgnore] private string _numDerivativeInterior;
        [XmlIgnore] private string _numDerivativesAtEnd;
        [XmlIgnore] private string _numDerivativesAtStart;

        public AbstractCurveSegmentType()
        {
            NumDerivativesAtStart = "0";
            NumDerivativesAtEnd = "0";
            NumDerivativeInterior = "0";
        }

        [XmlAttribute(AttributeName = "numDerivativeInterior", DataType = "integer")]
        public string NumDerivativeInterior
        {
            get { return _numDerivativeInterior; }
            set { _numDerivativeInterior = value; }
        }

        [XmlAttribute(AttributeName = "numDerivativesAtEnd", DataType = "integer")]
        public string NumDerivativesAtEnd
        {
            get { return _numDerivativesAtEnd; }
            set { _numDerivativesAtEnd = value; }
        }

        [XmlAttribute(AttributeName = "numDerivativesAtStart", DataType = "integer")]
        public string NumDerivativesAtStart
        {
            get { return _numDerivativesAtStart; }
            set { _numDerivativesAtStart = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}