/*
*  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
*  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
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
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Protocol
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent",
        "SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent.js")]
    [TargetControlType(typeof (Control))]
    public abstract class ProtocolComponent<TProtocolBuilderParams> : ComponentBase<TProtocolBuilderParams>,
                                                                      IProtocolComponent<TProtocolBuilderParams>
        where TProtocolBuilderParams : IProtocolBuilderParams
    {
        #region IProtocolComponent<TProtocolBuilderParams> Members

        IProtocolBuilderParams IProtocolComponent.BuilderParams
        {
            get { return BuilderParams; }
            set { BuilderParams = (TProtocolBuilderParams) value; }
        }

        #endregion

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            //do nothing
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            //do nothing
        }
    }
}