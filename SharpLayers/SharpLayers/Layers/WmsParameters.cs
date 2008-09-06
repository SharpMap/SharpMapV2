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

namespace SharpLayers.Layers
{
    /// <summary>
    /// TODO:tidy this up..
    /// </summary>
    public class WmsParameters : IOlJsNewable
    {
        private ICollection<string> _layers = new List<string>();
        private string _wmsVersion = "1.3.0";

        [OLJsonSerialization(SerializedName = "version")]
        public string WmsVersion
        {
            get { return _wmsVersion; }
            set { _wmsVersion = value; }
        }

        [OLJsonSerialization(SerializedName = "url")]
        public ICollection<string> Urls { get; set; }

        [OLJsonSerialization(SerializedName = "layers")]
        public ICollection<string> Layers
        {
            get { return _layers; }
            set { _layers = value; }
        }

        [OLJsonSerialization(SerializedName = "crs")]
        public string Crs { get; set; }

        #region IOlJsNewable Members

        public string GetNewString()
        {
            return "{" +
                   string.Format(
                       "url:[{0}], layers:[{1}], version:\"{2}\",crs:\"{3}\",transparent:true, format:\"image/png\"",
                       string.Join(",", Urls.Select(o => "\"" + o + "\"").ToArray()),
                       string.Join(",", Layers.Select(o => "\"" + o + "\"").ToArray()), WmsVersion, Crs) + "}";
        }

        #endregion
    }
}