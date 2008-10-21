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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.ComponentBase.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [RequiredScript(typeof (OpenLayersExtender))]
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.ComponentBase",
        "SharpMap.Presentation.Web.SharpLayers.ComponentBase.js")]
    [TargetControlType(typeof (Control))]
    public abstract class ComponentBase : ScriptControl
    {
        private readonly Dictionary<string, Control> _findControlHelperCache = new Dictionary<string, Control>();
        protected abstract string ScriptComponentName { get; }


        protected override void OnPreRender(EventArgs e)
        {
            if (!ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                BuildOnClient();

            base.OnPreRender(e);
        }

        protected virtual void BuildOnClient()
        {
//            string script = string.Format(CultureInfo.InvariantCulture,
//                                          @"(function(){{
//    var fn = function(){{
//        SharpMap.Presentation.Web.SharpLayers.ComponentBase.buildViaServer('{0}');
//        Sys.Application.remove_load(fn);
//    }};
//    Sys.Application.add_load(fn);
//    }})();",
//                                          ClientID);

//            ScriptManager.RegisterStartupScript(this, typeof(HostBaseExtender<>), "buildOnClient" + ClientID, script,
//                                                true);
        }


        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            return ScriptObjectBuilder.GetScriptReferences(GetType());
        }


        protected Control FindControlHelper(string id)
        {
            Control c = null;
            if (_findControlHelperCache.ContainsKey(id))
            {
                c = _findControlHelperCache[id];
            }
            else
            {
                c = base.FindControl(id); // Use "base." to avoid calling self in an infinite loop
                Control nc = NamingContainer;
                while ((null == c) && (null != nc))
                {
                    c = nc.FindControl(id);
                    nc = nc.NamingContainer;
                }
                if (null == c)
                {
                    // Note: props MAY be null, but we're firing the event anyway to let the user
                    // do the best they can
                    var args = new ResolveControlEventArgs(id);

                    OnResolveControlID(args);
                    c = args.Control;
                }
                if (null != c)
                {
                    _findControlHelperCache[id] = c;
                }
            }
            return c;
        }

        public event ResolveControlEventHandler ResolveControlID;

        protected virtual void OnResolveControlID(ResolveControlEventArgs e)
        {
            if (ResolveControlID != null)
            {
                ResolveControlID(this, e);
            }
        }

        public override Control FindControl(string id)
        {
            // Use FindControlHelper so that more complete searching and OnResolveControlID will be used
            return FindControlHelper(id);
        }
    }

    [ParseChildren(DefaultProperty = "BuilderParams", ChildrenAsProperties = true)]
    public abstract class ComponentBase<TBuilderParams> : ComponentBase, IHaveBuilderParams<TBuilderParams>
        where TBuilderParams : IBuilderParams
    {
        #region IHaveBuilderParams<TBuilderParams> Members

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Category("Behavior"),
            Description("The Builder Options"),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public TBuilderParams BuilderParams { get; set; }

        #endregion

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptComponentDescriptor(ScriptComponentName);
            descriptor.ID = ClientID;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] {new BuilderParamsJavascriptConverter(FindControl)});
            descriptor.AddScriptProperty("builderParams", serializer.Serialize(BuilderParams));
            return new[] {descriptor};
        }
    }
}