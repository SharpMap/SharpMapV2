/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SharpLayers.Layers;

namespace SharpLayers.Controls
{
    public class Map : Panel, IScriptControl, ISerializeToOLJson, IOLClass
    {
        private readonly List<IControl> _controls = new List<IControl>();
        private readonly List<ILayer> _layers = new List<ILayer>();

        public ICollection<ILayer> Layers
        {
            get { return _layers; }
        }


        public ICollection<IControl> OLControls
        {
            get { return _controls; }
        }

        [OLJsonSerialization(SerializedName = "baseLayer", SerializationFlags = OLSerializationFlags.FindOLComponent)]
        public string BaseLayerId { get; set; }

        [OLJsonSerialization(SerializedName = "div")] //, SerializationFlags = OLSerializationFlags.GetElement
            public override string ClientID
        {
            get { return base.ClientID; }
        }

        [OLJsonSerialization(SerializedName = "options")]
        public OLMapOptions Options { get; set; }

        #region IOLClass Members

        public string JsClass
        {
            get { return "OpenLayers.Map"; }
        }

        #endregion

        #region IScriptControl Members

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var layerIds = new List<string>();
            foreach (ILayer l in Layers)
            {
                foreach (ScriptDescriptor d in l.GetScriptDescriptors())
                    yield return d;

                layerIds.Add(l.Id);
            }


            var cntrlIds = new List<string>();
            foreach (IControl c in OLControls)
            {
                foreach (ScriptDescriptor d in c.GetScriptDescriptors())
                    yield return d;
                cntrlIds.Add(c.Id);
            }

            var desc = new ScriptComponentDescriptor(JsClass);

            desc.AddScriptProperty("aaaServerInit", OLJsonSerializer.Serialize(this));

            var sb = new StringBuilder();
            sb.Append("[");
            if (layerIds.Count > 0)
            {
                foreach (string l in layerIds)
                    sb.AppendFormat("$find(\"{0}\"),", l);
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");


            desc.AddScriptProperty("serverLayers", sb.ToString());


            sb = new StringBuilder();
            sb.Append("[");
            if (OLControls.Count > 0)
            {
                foreach (string id in cntrlIds)
                {
                    sb.AppendFormat("$find(\"{0}\"),", id);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            desc.AddScriptProperty("serverControls", sb.ToString());


            yield return desc;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            foreach (ScriptReference r in OpenLayersScripts.GetScriptReferences())
                yield return r;
        }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager manager = ScriptManager.GetCurrent(Page);

            if (manager != null)
                manager.RegisterScriptControl(this);
            else
                throw new ApplicationException("No script manager found");

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
                ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(this);

            base.Render(writer);
        }
    }
}