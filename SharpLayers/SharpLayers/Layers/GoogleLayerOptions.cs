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
using System.Linq;

namespace SharpLayers.Layers
{
    public class GoogleLayerOptions
        : LayerOptionsBase, IFixedResolutionLayerOptions
    {
        private ICollection<double> _resolutions =
            new List<double>(
                new[]
                    {
                        1.40625,
                        0.703125,
                        0.3515625,
                        0.17578125,
                        0.087890625,
                        0.0439453125,
                        0.02197265625,
                        0.010986328125,
                        0.0054931640625,
                        0.00274658203125,
                        0.001373291015625,
                        0.0006866455078125,
                        0.00034332275390625,
                        0.000171661376953125,
                        0.0000858306884765625,
                        0.00004291534423828125,
                        0.00002145767211914062,
                        0.00001072883605957031,
                        0.00000536441802978515,
                        0.00000268220901489257
                    });

        [OLJsonSerialization(SerializedName = "isBaseLayer")]
        public override bool IsBaseLayer
        {
            get { return true; }
            set { throw new InvalidOperationException("GoogleLayer is always a base layer"); }
        }

        #region IFixedResolutionLayerOptions Members

        [OLJsonSerialization(SerializedName = "resolutions")]
        public ICollection<double> Resolutions
        {
            get { return _resolutions; }
            set
            {
                _resolutions = (ICollection<double>) (from v in value
                                                      orderby v descending
                                                      select v);
            }
        }

        #endregion
    }
}