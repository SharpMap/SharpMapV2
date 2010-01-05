#region License

/*
 *  The attached / following is part of MapViewer.
 *  
 *  MapViewer is free software © 2009 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2009
 *  
 */

#endregion

using System.Collections.Generic;
using SharpMap.Data;

namespace MapViewer.DataSource
{
    ///<summary>
    ///</summary>
    internal interface ICreateRasterProvider
    {
        ///<summary>
        /// 
        ///</summary>
        ///<returns>An enumeration of IRasterProviders</returns>
        IEnumerable<IRasterProvider> GetRasterProviders();
        /// <summary>
        /// Name of the Raster Provider class
        /// </summary>
        string ProviderName { get; }
    }
}