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
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace SharpLayers
{
    public class Marker : ISerializeToOLJson, IOLClass, IScriptControl
    {
        private string id;

        [OLJsonSerialization(SerializedName = "icon")]
        public OLIcon Icon { get; set; }

        [OLJsonSerialization(SerializedName = "id")]
        public string Id
        {
            get
            {
                EnsureId();
                return id;
            }
            set { id = value; }
        }

        [OLJsonSerialization(SerializedName = "lonlat")]
        public OLLonLat LonLat { get; set; }

        #region IOLClass Members

        public string JsClass
        {
            get { return "OpenLayers.Marker"; }
        }

        #endregion

        #region IScriptControl Members

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var desc = new ScriptComponentDescriptor(JsClass);
            desc.ID = Id;
            desc.AddScriptProperty("aaaServerInit", OLJsonSerializer.Serialize(this));
            yield return desc;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield break;
        }

        #endregion

        private void EnsureId()
        {
            if (!string.IsNullOrEmpty(id))
                return;
            id = Guid.NewGuid().ToString();
        }
    }
}