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

namespace SharpLayers.Layers
{
    public abstract class LayerBase<TOptions> : ILayer, IHaveLayerOptions<TOptions> where TOptions : ILayerOptions
    {
        private string _id;

        #region IHaveLayerOptions<TOptions> Members

        [OLJsonSerialization(SerializedName = "options")]
        public TOptions Options { get; set; }

        #endregion

        #region ILayer Members

        public abstract string JsClass { get; }

        [OLJsonSerialization(SerializedName = "id")]
        public string Id
        {
            get
            {
                EnsureId();
                return _id;
            }
            set { _id = value; }
        }


        [OLJsonSerialization(SerializedName = "name")]
        public string Name { get; set; }


        public virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptComponentDescriptor(JsClass) {ID = Id};
            descriptor.AddScriptProperty("aaaServerInit", OLJsonSerializer.Serialize(this));
            yield return descriptor;
        }

        public virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield break;
        }

        ILayerOptions IHaveLayerOptions.Options
        {
            get { return Options; }
            set { Options = (TOptions) value; }
        }

        #endregion

        private void EnsureId()
        {
            _id = _id ?? Guid.NewGuid().ToString();
        }
    }
}