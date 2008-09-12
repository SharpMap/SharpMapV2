/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
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
using System.Diagnostics;
using System.Drawing.Imaging;
using SharpMap.Rendering.Web;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public class DefaultImageRendererConfig
        : IMapRendererConfig<IMapRequestConfig, GdiImageRenderer>
    {
        #region IMapRendererConfig<IMapRequestConfig,GdiImageRenderer> Members

        public void ConfigureRenderer(IMapRequestConfig requestConfig, GdiImageRenderer renderer)
        {
            ImageCodecInfo codecInfo = GdiImageRenderer.FindCodec(requestConfig.MimeType);
            if (codecInfo == null)
            {
                Trace.WriteLine(string.Format("ImageCodecInfo for MimeType {0} not found. Using Defaults.",
                                              requestConfig.MimeType));
                return;
            }

            renderer.ImageCodec = codecInfo;

            ///as more properties get added to the BasicMapRequestConfig we can start creating encoder params
            ///to allow different image compression settings etc
        }

        public void ConfigureRenderer(IMapRequestConfig requestConfig, IWebMapRenderer renderer)
        {
            ConfigureRenderer(requestConfig, (GdiImageRenderer) renderer);
        }

        #endregion
    }
}