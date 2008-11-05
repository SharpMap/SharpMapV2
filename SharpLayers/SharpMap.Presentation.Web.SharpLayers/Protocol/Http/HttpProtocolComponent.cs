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
using System;
using System.Collections;
using System.Web.UI;
using AjaxControlToolkit;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Protocol.Http
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent",
        "SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.js")]
    [TargetControlType(typeof (Control))]
    public class HttpProtocolComponent : ProtocolComponent<HttpProtocolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent"; }
        }
    }

    public class HttpProtocolComponentBuilder : ControlBuilder
    {
        public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName,
                                  string id, IDictionary attribs)
        {
            var formatType = (string) attribs["FormatType"];
            Type t = Type.GetType(formatType);

            Type buildType = typeof (HttpProtocolComponent).MakeGenericType(t);

            base.Init(parser, parentBuilder, buildType, tagName, id, attribs);
        }
    }
}