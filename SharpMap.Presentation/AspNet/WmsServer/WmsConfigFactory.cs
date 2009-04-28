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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using SharpMap.Presentation.AspNet.Utility;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Utilities;

namespace SharpMap.Presentation.AspNet.WmsServer
{
    public abstract class WmsRequestConfigFactoryBase
        : IMapRequestConfigFactory<WmsMapRequestConfig>
    {
        public abstract Capabilities.WmsServiceDescription Description { get; }

        #region IMapRequestConfigFactory<WmsMapRequestConfig> Members

        public virtual WmsMapRequestConfig CreateConfig(HttpContext context)
        {
            WmsMapRequestConfig config = new WmsMapRequestConfig();
            config.ServiceDescription = Description;

            config.CacheKey = CreateCacheKey(context);


            bool ignorecase = true;

            if (context.Request.Params["REQUEST"] == null)
                WmsException.ThrowWmsException("Required parameter REQUEST not specified");

            //Check if version is supported
            if (context.Request.Params["VERSION"] != null)
            {
                if (String.Compare(context.Request.Params["VERSION"], "1.3.0", ignorecase) != 0)
                    WmsException.ThrowWmsException("Only version 1.3.0 supported");
            }
            else
                //Version is mandatory if REQUEST!=GetCapabilities. Check if this is a capabilities request, since VERSION is null
            {
                if (String.Compare(context.Request.Params["REQUEST"], "GetCapabilities", ignorecase) != 0)
                    WmsException.ThrowWmsException("VERSION parameter not supplied");
            }


            if (String.Compare(context.Request.Params["REQUEST"], "GetCapabilities", ignorecase) == 0)
            {
                //Service parameter is mandatory for GetCapabilities request
                if (context.Request.Params["SERVICE"] == null)
                    WmsException.ThrowWmsException("Required parameter SERVICE not specified");

                if (String.Compare(context.Request.Params["SERVICE"], "WMS") != 0)
                    WmsException.ThrowWmsException(
                        "Invalid service for GetCapabilities Request. Service parameter must be 'WMS'");

                config.WmsMode = WmsMode.Capabilites;
                config.MimeType = "text/xml";
                return config;
            }

            if (String.Compare(context.Request.Params["REQUEST"], "GetMap", ignorecase) != 0)
                WmsException.ThrowWmsException("Invalid REQUEST parameter");

            config.WmsMode = WmsMode.Map;


            //Check for required parameters
            if (context.Request.Params["LAYERS"] == null)
                WmsException.ThrowWmsException("Required parameter LAYERS not specified");
            config.EnabledLayerNames = new List<string>(context.Request.Params["LAYERS"].Split(','));

            if (context.Request.Params["STYLES"] == null)
                WmsException.ThrowWmsException("Required parameter STYLES not specified");

            if (context.Request.Params["CRS"] == null)
                WmsException.ThrowWmsException("Required parameter CRS not specified");
            config.Crs = context.Request.Params["CRS"];
            config.BaseSrid = config.Crs;

            if (context.Request.Params["BBOX"] == null)
                WmsException.ThrowWmsException(WmsExceptionCode.InvalidDimensionValue,
                                               "Required parameter BBOX not specified");

            if (context.Request.Params["WIDTH"] == null)
                WmsException.ThrowWmsException(WmsExceptionCode.InvalidDimensionValue,
                                               "Required parameter WIDTH not specified");

            if (context.Request.Params["HEIGHT"] == null)
                WmsException.ThrowWmsException(WmsExceptionCode.InvalidDimensionValue,
                                               "Required parameter HEIGHT not specified");

            if (context.Request.Params["FORMAT"] == null)
                WmsException.ThrowWmsException("Required parameter FORMAT not specified");

            Color bkgnd = Color.White;

            //Set background color of map
            if (String.Compare(context.Request.Params["TRANSPARENT"], "TRUE", ignorecase) == 0)
                bkgnd = Color.Transparent;
            else if (context.Request.Params["BGCOLOR"] != null)
            {
                try
                {
                    bkgnd = ColorTranslator.FromHtml(context.Request.Params["BGCOLOR"]);
                }
                catch
                {
                    WmsException.ThrowWmsException("Invalid parameter BGCOLOR");
                }
                ;
            }


            config.BackgroundColor = ViewConverter.Convert(bkgnd);
            config.MimeType = context.Request.Params["FORMAT"];

            //Parse map size
            int width = 0;
            int height = 0;

            if (!int.TryParse(context.Request.Params["WIDTH"], out width))
                WmsException.ThrowWmsException(WmsExceptionCode.InvalidDimensionValue, "Invalid parameter WIDTH");
            else if (Description.MaxWidth > 0 && width > Description.MaxWidth)
                WmsException.ThrowWmsException(WmsExceptionCode.OperationNotSupported, "Parameter WIDTH too large");

            if (!int.TryParse(context.Request.Params["HEIGHT"], out height))
                WmsException.ThrowWmsException(WmsExceptionCode.InvalidDimensionValue, "Invalid parameter HEIGHT");
            else if (Description.MaxHeight > 0 && height > Description.MaxHeight)
                WmsException.ThrowWmsException(WmsExceptionCode.OperationNotSupported, "Parameter HEIGHT too large");


            config.OutputSize = new Size2D(width, height);

            if (context.Request.Params["BBOX"] == null)
                WmsException.ThrowWmsException("Invalid parameter BBOX");

            config.RealWorldBounds = UrlUtility.ParseExtents(new GeometryServices()[config.Crs],
                                                             context.Request.Params["BBOX"]);


            return config;
        }


        IMapRequestConfig IMapRequestConfigFactory.CreateConfig(HttpContext context)
        {
            return CreateConfig(context);
        }

        #endregion

        public virtual string CreateCacheKey(HttpContext context)
        {
            char[] key = context.Request.Url.PathAndQuery.ToLower().ToCharArray();
            Array.Reverse(key);
            return new string(key);
        }
    }
}