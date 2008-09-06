/*
 *  The attached / following is part of SharpMap.Presentation.AspNet.WmsServer
 *  SharpMap.Presentation.AspNet.WmsServer is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.Text;

namespace SharpMap.Presentation.AspNet.WmsServer
{
    public class WmsException
        : XmlFormatableExceptionBase
    {
        private WmsExceptionCode _wmsExceptionCode;
        private string _xml;

        public WmsException(string message)
            : this(WmsExceptionCode.NotApplicable, message)
        {
        }

        public WmsException(WmsExceptionCode code, string message)
            : base(message)
        {
        }

        public WmsExceptionCode WmsExceptionCode
        {
            get { return _wmsExceptionCode; }
        }

        public override string XmlExceptionString
        {
            get
            {
                _xml = _xml ?? CreateXmlString();
                return _xml;
            }
        }


        internal static void ThrowWmsException(string Message)
        {
            ThrowWmsException(WmsExceptionCode.NotApplicable, Message);
        }

        internal static void ThrowWmsException(WmsExceptionCode code, string Message)
        {
            throw new WmsException(code, Message);
        }

        private string CreateXmlString()
        {
            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
            sb.Append(
                "<ServiceExceptionReport version=\"1.3.0\" xmlns=\"http://www.opengis.net/ogc\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opengis.net/ogc http://schemas.opengis.net/wms/1.3.0/exceptions_1_3_0.xsd\">\n");
            sb.Append("<ServiceException");
            if (WmsExceptionCode != WmsExceptionCode.NotApplicable)
                sb.Append(" code=\"" + WmsExceptionCode + "\"");
            sb.Append(">" + Message + "</ServiceException>\n");
            sb.Append("</ServiceExceptionReport>");
            return sb.ToString();
        }
    }

    /// <summary>
    /// WMS Exception codes
    /// </summary>
    public enum WmsExceptionCode
    {
        /// <summary>
        /// Request contains a Format not offered by the server.
        /// </summary>
        InvalidFormat,
        /// <summary>
        /// Request contains a CRS not offered by the server for one or more of the
        /// Layers in the request.
        /// </summary>
        InvalidCRS,
        /// <summary>
        /// GetMap request is for a Layer not offered by the server, or GetFeatureInfo
        /// request is for a Layer not shown on the map.
        /// </summary>
        LayerNotDefined,
        /// <summary>
        /// Request is for a Layer in a Style not offered by the server.
        /// </summary>
        StyleNotDefined,
        /// <summary>
        /// GetFeatureInfo request is applied to a Layer which is not declared queryable.
        /// </summary>
        LayerNotQueryable,
        /// <summary>
        /// GetFeatureInfo request contains invalid X or Y value.
        /// </summary>
        InvalidPoint,
        /// <summary>
        /// Value of (optional) UpdateSequence parameter in GetCapabilities request is
        /// equal to current value of service metadata update sequence number.
        /// </summary>
        CurrentUpdateSequence,
        /// <summary>
        /// Value of (optional) UpdateSequence parameter in GetCapabilities request is
        /// greater than current value of service metadata update sequence number.
        /// </summary>
        InvalidUpdateSequence,
        /// <summary>
        /// Request does not include a sample dimension value, and the server did not
        /// declare a default value for that dimension.
        /// </summary>
        MissingDimensionValue,
        /// <summary>
        /// Request contains an invalid sample dimension value.
        /// </summary>
        InvalidDimensionValue,
        /// <summary>
        /// Request is for an optional operation that is not supported by the server.
        /// </summary>
        OperationNotSupported,
        /// <summary>
        /// No error code
        /// </summary>
        NotApplicable
    }
}