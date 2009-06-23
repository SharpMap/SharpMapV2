using System;
using System.Web.UI;

namespace SharpMap.Demo.SharpLayers
{
    public partial class Webform1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "zoomExtentOnLoad",
@"(function(){
    var f = function(){ $find('" + Panel1_MapHostExtender.ClientID + @"').get_hostedItem().zoomToMaxExtent();}
SharpMap.Presentation.Web.SharpLayers.InitSync.addPostLoad(f);
})();", true);
        }
    }
}