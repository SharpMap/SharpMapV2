/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
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
using System.IO;

namespace SharpMap.Presentation.AspNet.Caching
{
    /// <summary>
    /// A caching provider that does nothing!
    /// use it when you dont want caching
    /// </summary>
    public class NoCacheProvider
        : IMapCacheProvider
    {
        #region IMapCacheProvider Members

        public bool ExistsInCache(IMapRequestConfig config)
        {
            return false;
        }

        public void SaveToCache(IMapRequestConfig config, Stream data)
        {
            //do nothing
        }

        public Stream RetrieveStream(IMapRequestConfig config)
        {
            ///we should be testing ExistsInCache before calling this method.
            throw new InvalidOperationException();
        }

        public void RemoveFromCache(IMapRequestConfig config)
        {
            //do nothing
        }

        #endregion
    }
}