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

namespace SharpMap.Presentation.AspNet
{
    public interface IMapCacheProvider
    {
        bool ExistsInCache(IMapRequestConfig config);
        void SaveToCache(IMapRequestConfig config, Stream data);
        Stream RetrieveStream(IMapRequestConfig config);
        void RemoveFromCache(IMapRequestConfig config);
    }

    public interface IMapCacheProvider<TMapRequestConfig, TOutput>
        : IMapCacheProvider<TMapRequestConfig>
        where TMapRequestConfig : IMapRequestConfig
    {
        Func<TOutput, Stream> ConvertToStreamDelegate { get; }
        Func<Stream, TOutput> ConvertToObjectDelegate { get; }
        TOutput RetrieveObject(IMapRequestConfig config);
        void SaveToCache(IMapRequestConfig config, TOutput data);
    }

    public interface IMapCacheProvider<TMapRequestConfig> : IMapCacheProvider
        where TMapRequestConfig : IMapRequestConfig
    {
        bool ExistsInCache(TMapRequestConfig config);
        void SaveToCache(TMapRequestConfig config, Stream data);
        Stream RetrieveStream(TMapRequestConfig config);
        void RemoveFromCache(TMapRequestConfig config);
    }
}