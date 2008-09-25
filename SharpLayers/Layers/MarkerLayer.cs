/*
 *	This file is part of SharpLayers
 *  SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace SharpLayers.Layers
{
    public class MarkerLayer : LayerBase<MarkerLayerOptions>
    {
        public override string JsClass
        {
            get { return "OpenLayers.Layer.Markers"; }
        }

        public IEnumerable<Marker> Markers { get; set; }


        public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            foreach (Marker m in Markers)
                foreach (ScriptDescriptor s in m.GetScriptDescriptors())
                    yield return s;


            var desc = new ScriptComponentDescriptor(JsClass);

            desc.AddScriptProperty("aaaServerInit", OLJsonSerializer.Serialize(this));

            var sb = new StringBuilder();
            sb.Append("[");
            if (Markers.Count() > 0)
            {
                foreach (Marker m in Markers)
                    sb.AppendFormat("$find(\"{0}\"),", m.Id);
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");


            desc.AddScriptProperty("serverMarkers", sb.ToString());
            yield return desc;
        }
    }
}