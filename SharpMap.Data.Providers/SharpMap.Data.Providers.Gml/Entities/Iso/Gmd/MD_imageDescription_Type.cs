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
    [Serializable, XmlType(TypeName = "MD_imageDescription_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_imageDescription_Type : MD_coverageDescription_Type
    {
        [XmlIgnore] private BooleanPropertyType _cameraCalibrationInformationAvailability;
        [XmlIgnore] private RealPropertyType _cloudCoverPercentage;
        [XmlIgnore] private IntegerPropertyType _compressionGenerationQuantity;
        [XmlIgnore] private BooleanPropertyType _filmDistortionInformationAvailability;
        [XmlIgnore] private RealPropertyType _illuminationAzimuthAngle;
        [XmlIgnore] private RealPropertyType _illuminationElevationAngle;
        [XmlIgnore] private MD_identifier_PropertyType _imageQualityCode;
        [XmlIgnore] private MD_imagingConditionCode_PropertyType _imagingCondition;
        [XmlIgnore] private BooleanPropertyType _lensDistortionInformationAvailability;
        [XmlIgnore] private MD_identifier_PropertyType _processingLevelCode;
        [XmlIgnore] private BooleanPropertyType _radiometricCalibrationDataAvailability;
        [XmlIgnore] private BooleanPropertyType _triangulationIndicator;

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "cameraCalibrationInformationAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType CameraCalibrationInformationAvailability
        {
            get { return _cameraCalibrationInformationAvailability; }
            set { _cameraCalibrationInformationAvailability = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "cloudCoverPercentage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType CloudCoverPercentage
        {
            get { return _cloudCoverPercentage; }
            set { _cloudCoverPercentage = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "compressionGenerationQuantity",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType CompressionGenerationQuantity
        {
            get { return _compressionGenerationQuantity; }
            set { _compressionGenerationQuantity = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "filmDistortionInformationAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType FilmDistortionInformationAvailability
        {
            get { return _filmDistortionInformationAvailability; }
            set { _filmDistortionInformationAvailability = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "illuminationAzimuthAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType IlluminationAzimuthAngle
        {
            get { return _illuminationAzimuthAngle; }
            set { _illuminationAzimuthAngle = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "illuminationElevationAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType IlluminationElevationAngle
        {
            get { return _illuminationElevationAngle; }
            set { _illuminationElevationAngle = value; }
        }

        [XmlElement(Type = typeof (MD_identifier_PropertyType), ElementName = "imageQualityCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_identifier_PropertyType ImageQualityCode
        {
            get { return _imageQualityCode; }
            set { _imageQualityCode = value; }
        }

        [XmlElement(Type = typeof (MD_imagingConditionCode_PropertyType), ElementName = "imagingCondition",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_imagingConditionCode_PropertyType ImagingCondition
        {
            get { return _imagingCondition; }
            set { _imagingCondition = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "lensDistortionInformationAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType LensDistortionInformationAvailability
        {
            get { return _lensDistortionInformationAvailability; }
            set { _lensDistortionInformationAvailability = value; }
        }

        [XmlElement(Type = typeof (MD_identifier_PropertyType), ElementName = "processingLevelCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_identifier_PropertyType ProcessingLevelCode
        {
            get { return _processingLevelCode; }
            set { _processingLevelCode = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "radiometricCalibrationDataAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType RadiometricCalibrationDataAvailability
        {
            get { return _radiometricCalibrationDataAvailability; }
            set { _radiometricCalibrationDataAvailability = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "triangulationIndicator", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType TriangulationIndicator
        {
            get { return _triangulationIndicator; }
            set { _triangulationIndicator = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}