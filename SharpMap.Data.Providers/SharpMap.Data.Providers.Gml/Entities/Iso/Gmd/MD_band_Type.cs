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
    [Serializable, XmlType(TypeName = "MD_band_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_band_Type : MD_rangeDimension_Type
    {
        [XmlIgnore] private IntegerPropertyType _bitsPerValue;
        [XmlIgnore] private RealPropertyType _maxValue;
        [XmlIgnore] private RealPropertyType _minValue;
        [XmlIgnore] private RealPropertyType _offset;
        [XmlIgnore] private RealPropertyType _peakResponse;
        [XmlIgnore] private RealPropertyType _scaleFactor;
        [XmlIgnore] private IntegerPropertyType _toneGradation;
        [XmlIgnore] private UomLengthPropertyType _units;

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "bitsPerValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType BitsPerValue
        {
            get { return _bitsPerValue; }
            set { _bitsPerValue = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "maxValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "minValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "offset", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "peakResponse", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType PeakResponse
        {
            get { return _peakResponse; }
            set { _peakResponse = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "scaleFactor", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "toneGradation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType ToneGradation
        {
            get { return _toneGradation; }
            set { _toneGradation = value; }
        }

        [XmlElement(Type = typeof (UomLengthPropertyType), ElementName = "units", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public UomLengthPropertyType Units
        {
            get { return _units; }
            set { _units = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}