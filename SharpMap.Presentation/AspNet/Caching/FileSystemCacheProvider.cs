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
using System.Collections.Generic;
using System.IO;
using System.Web.Security;

namespace SharpMap.Presentation.AspNet.Caching
{
    public class FileSystemCacheProvider<TMapRequestConfig> : IMapCacheProvider<TMapRequestConfig>
        where TMapRequestConfig : IMapRequestConfig
    {
        public string BaseCacheDir { get; set; }

        #region IMapCacheProvider<TMapRequestConfig> Members

        public bool ExistsInCache(TMapRequestConfig config)
        {
            return File.Exists(GetCacheFilePath(config));
        }

        public void SaveToCache(TMapRequestConfig config, Stream data)
        {
            if (!ExistsInCache(config))
            {
                string path = GetCacheFilePath(config);

                lock (path)
                {
                    EnsurePath(Path.GetDirectoryName(path));

                    var buf = new byte[data.Length];
                    data.Read(buf, 0, buf.Length);
                    File.WriteAllBytes(path, buf);
                }
            }
        }

        public Stream RetrieveStream(TMapRequestConfig config)
        {
            string path = GetCacheFilePath(config);
            lock (path)
            {
                File.SetLastAccessTimeUtc(path, DateTime.UtcNow);
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8, true);
            }
        }

        public void RemoveFromCache(TMapRequestConfig config)
        {
            string path = GetCacheFilePath(config);
            lock (path)
                File.Delete(path);
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

        private void EnsurePath(string dirpath)
        {
            var di = new DirectoryInfo(dirpath);
            if (di.Exists)
                return;

            lock (dirpath)
            {
                var stk = new Stack<DirectoryInfo>();


                for (DirectoryInfo d = di; d != null && !d.Exists; d = d.Parent)
                {
                    stk.Push(d);
                }

                for (DirectoryInfo d = stk.Pop();; d = stk.Pop())
                {
                    if (!d.Exists)
                        d.Create();
                    if (stk.Count == 0)
                        break;
                }
            }
        }

        protected virtual string GetCacheFileName(TMapRequestConfig config)
        {
            return string.Format("{0}.sharpmapcache",
                                 FormsAuthentication.HashPasswordForStoringInConfigFile(config.CacheKey, "MD5"));
        }

        protected virtual string GetCacheFilePath(TMapRequestConfig config)
        {
            return Path.Combine(string.IsNullOrEmpty(BaseCacheDir) ? Path.GetTempPath() : BaseCacheDir,
                                GetCacheFileName(config));
        }
    }
}