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
using System.Web;
using System.Web.Caching;

namespace SharpMap.Presentation.AspNet.Caching
{
    public class AspNetCacheProvider<TMapRequestConfig> : IMapCacheProvider<TMapRequestConfig>
        where TMapRequestConfig : IMapRequestConfig
    {
        private TimeSpan _cacheTime = TimeSpan.FromMinutes(5);

        protected Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

        public TimeSpan CacheTime
        {
            get { return _cacheTime; }
            set { _cacheTime = value; }
        }

        #region IMapCacheProvider<TMapRequestConfig> Members

        public bool ExistsInCache(TMapRequestConfig config)
        {
            return Cache[config.CacheKey] != null;
        }

        public Stream RetrieveStream(TMapRequestConfig config)
        {
            if (ExistsInCache(config))
            {
                byte[] bytes = (byte[]) Cache[config.CacheKey];
                MemoryStream ms = new MemoryStream(bytes);
                ms.Position = 0;
                return ms;
            }
            throw new NullReferenceException("Cache Miss");
        }

        public void SaveToCache(TMapRequestConfig config, Stream data)
        {
            data.Position = 0;
            byte[] b;
            BinaryReader br = new BinaryReader(data);
            b = br.ReadBytes((int) data.Length);
            data.Position = 0;

            Cache.Insert(config.CacheKey, b, null, Cache.NoAbsoluteExpiration, CacheTime);
        }

        public void RemoveFromCache(TMapRequestConfig config)
        {
            if (ExistsInCache(config))
                Cache.Remove(config.CacheKey);
        }

        public bool ExistsInCache(IMapRequestConfig config)
        {
            return ExistsInCache((TMapRequestConfig) config);
        }

        public void SaveToCache(IMapRequestConfig config, Stream data)
        {
            SaveToCache((TMapRequestConfig) config, data);
        }

        public Stream RetrieveStream(IMapRequestConfig config)
        {
            return RetrieveStream((TMapRequestConfig) config);
        }

        public void RemoveFromCache(IMapRequestConfig config)
        {
            RemoveFromCache((TMapRequestConfig) config);
        }

        #endregion
    }

    public class AspNetCacheProvider<TMapRequestConfig, TOutput>
        : AspNetCacheProvider<TMapRequestConfig>, IMapCacheProvider<TMapRequestConfig, TOutput>
        where TMapRequestConfig : IMapRequestConfig
    {
        private readonly CacheConversionMap _cacheConversionMap = new CacheConversionMap();

        #region IMapCacheProvider<TMapRequestConfig,TOutput> Members

        public Func<TOutput, Stream> ConvertToStreamDelegate
        {
            get { return _cacheConversionMap.GetStreamConverter<TOutput>(); }
        }

        public Func<Stream, TOutput> ConvertToObjectDelegate
        {
            get { return _cacheConversionMap.GetObjectConverter<TOutput>(); }
        }

        public TOutput RetrieveObject(IMapRequestConfig config)
        {
            return RetrieveObject((TMapRequestConfig) config);
        }

        public void SaveToCache(IMapRequestConfig config, TOutput data)
        {
            SaveToCache((TMapRequestConfig) config, data);
        }

        #endregion

        public void SaveToCache(TMapRequestConfig config, TOutput data)
        {
            using (Stream s = ConvertToStreamDelegate(data))
                SaveToCache(config, s);
        }

        public TOutput RetrieveObject(TMapRequestConfig config)
        {
            return ConvertToObjectDelegate(RetrieveStream(config));
        }
    }
}