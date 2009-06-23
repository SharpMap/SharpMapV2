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
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class Size : IClientClass
    {
        [ExtenderControlProperty]
        [ClientPropertyName("w")]
        public double? Width { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("h")]
        public double? Height { get; set; }

        #region IClientClass Members

        [ExtenderControlProperty]
        [ClientPropertyName("typeToBuild")]
        public string ClientClassName
        {
            get { return "OpenLayers.Size"; }
        }

        public bool NotSet
        {
            get { return !(Width.HasValue || Height.HasValue); }
        }

        #endregion
    }
}