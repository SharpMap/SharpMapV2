using System;
using System.Drawing;
using System.Text;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ToolboxData("<{0}:SharpLayersConfigControl runat=server></{0}:SharpLayersConfigControl>")]
    public class SharpLayersConfigControl : Control
    {
        [UrlProperty]
        public String ImagesPath { get; set; }

        //[UrlProperty]
        //public String ScriptPath
        //{
        //    get;
        //    set;
        //}

        public Color? ImageLoadErrorBackgroundColor { get; set; }

        public Int32? DPI { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            //base.Render(writer);
            if (Page.IsPostBack)
                return;

            string script = BuildScript();
            Page.ClientScript.RegisterStartupScript(GetType(), "config", script, true);
        }

        private string BuildScript()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nSys.Application.add_init(function(){");

            if (!string.IsNullOrEmpty(ImagesPath))
            {
                string url = Page.ResolveClientUrl(ImagesPath);
                if (!url.EndsWith("/")) url += "/";
                sb.AppendFormat("OpenLayers.ImgPath = '{0}';\n", url);
            }

            if (ImageLoadErrorBackgroundColor.HasValue)
                sb.AppendFormat("OpenLayers.Util.onImageLoadErrorColor = '{0}';\n",
                                ColorTranslator.ToHtml(ImageLoadErrorBackgroundColor.Value));

            if (DPI.HasValue)
                sb.AppendFormat("OpenLayers.DOTS_PER_INCH = {0};\n", DPI.Value);

            sb.Append("})\n");
            return sb.ToString();
        }
    }
}