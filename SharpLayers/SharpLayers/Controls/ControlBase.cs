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

namespace SharpLayers.Controls
{
    public abstract class ControlBase<TOptions> : IControl, IHaveControlOptions<TOptions>
        where TOptions : IControlOptions
    {
        private string _id;

        #region IControl Members

        public string Id
        {
            get
            {
                EnsureId();
                return _id;
            }
            set { _id = value; }
        }

        public virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var desc = new ScriptComponentDescriptor(JsClass);
            desc.ID = Id;
            desc.AddScriptProperty("aaaServerInit", OLJsonSerializer.Serialize(this));


            yield return desc;
        }

        public virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield break;
        }

        public abstract string JsClass { get; }

        #endregion

        #region IHaveControlOptions<TOptions> Members

        [OLJsonSerialization(SerializedName = "options")]
        public TOptions Options { get; set; }

        IControlOptions IHaveControlOptions.Options
        {
            get { return Options; }
            set { Options = (TOptions) value; }
        }

        #endregion

        private void EnsureId()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }
    }
}