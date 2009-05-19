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
    [Serializable,
     XmlRoot(ElementName = "SecondDefiningParameter", Namespace = "http://www.opengis.net/gml/3.2", IsNullable = false)]
    public class SecondDefiningParameter
    {
        [XmlIgnore] private MeasureType _inverseFlattening;
        [XmlIgnore] private bool _isSphere;
        [XmlIgnore] private LengthType _semiMinorAxis;
        [XmlIgnore] public bool IsSphereSpecified;

        public SecondDefiningParameter()
        {
            IsSphere = true;
        }

        [XmlElement(Type = typeof (MeasureType), ElementName = "inverseFlattening", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MeasureType InverseFlattening
        {
            get { return _inverseFlattening; }
            set { _inverseFlattening = value; }
        }

        [XmlElement(ElementName = "isSphere", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public bool IsSphere
        {
            get { return _isSphere; }
            set
            {
                _isSphere = value;
                IsSphereSpecified = true;
            }
        }

        [XmlElement(Type = typeof (LengthType), ElementName = "semiMinorAxis", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LengthType SemiMinorAxis
        {
            get { return _semiMinorAxis; }
            set { _semiMinorAxis = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            InverseFlattening.MakeSchemaCompliant();
            SemiMinorAxis.MakeSchemaCompliant();
        }
    }
}