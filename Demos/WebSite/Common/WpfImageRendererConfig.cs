using System;
using SharpMap.Rendering.Web;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public class WpfImageRendererConfig
    {
        public class DefaultWpfImageRendererConfig
            : IMapRendererConfig<IMapRequestConfig, WpfImageRenderer>
        {
            #region IMapRendererConfig<IMapRequestConfig,WpfImageRenderer> Members

            public void ConfigureRenderer(IMapRequestConfig requestConfig, WpfImageRenderer renderer)
            {
                Type bitmapEncoderType = WpfImageRenderer.FindEncoder(requestConfig.MimeType);
                if (bitmapEncoderType == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("BitmapEncoderInfo for MimeType {0} not found. Using Defaults.",
                                      requestConfig.MimeType));
                    return;
                }

                renderer.ImageEncoder = bitmapEncoderType;

                //as more properties get added to the BasicMapRequestConfig we can start creating encoder params
                //to allow different image compression settings etc
            }

            public void ConfigureRenderer(IMapRequestConfig requestConfig, IWebMapRenderer renderer)
            {
                ConfigureRenderer(requestConfig, (WpfImageRenderer)renderer);
            }

            #endregion
        }
    }
}