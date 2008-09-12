using System;
using System.IO;
using SharpMap.Presentation.AspNet.Caching;

namespace SharpMap.Presentation.AspNet.WmsServer.Caching
{
    public class WmsAwareFileSystemCacheProvider
        : FileSystemCacheProvider<WmsMapRequestConfig>
    {
        protected override string GetCacheFileName(WmsMapRequestConfig config)
        {
            if (config.WmsMode == WmsMode.Capabilites)
                return "Capabilities.xml.sharpmapcache";

            string mimeType = config.MimeType.Substring(config.MimeType.IndexOf("/") + 1);

            /* Name the tile after the MaxX and MaxY of the real world extents */
            return string.Format("{0}_{1}.{2}.sharpmapcache", config.RealWorldBounds.XMax, config.RealWorldBounds.YMax,
                                 mimeType);
        }

        protected override string GetCacheFilePath(WmsMapRequestConfig config)
        {
            if (config.WmsMode == WmsMode.Capabilites)
            {
                string caps = Path.Combine(BaseCacheDir, config.CacheKey);
                return Path.Combine(caps, GetCacheFileName(config));
            }

            var layerNames = new string[config.EnabledLayerNames.Count];
            config.EnabledLayerNames.CopyTo(layerNames, 0);

            Array.Sort(layerNames);

            int layerKey = layerNames[0].ToLower().GetHashCode();
            for (int i = 1; i < layerNames.Length; i++)
            {
                layerKey ^= layerNames[i].ToLower().GetHashCode();
            }

            string path = Path.Combine(BaseCacheDir, config.CacheKey);

            path = Path.Combine(path, layerKey.ToString());

            path = Path.Combine(path, config.Crs.Replace(":", "_"));

            path = Path.Combine(path, string.Format("{0}x{1}", config.OutputSize.Width, config.OutputSize.Height));

            /* Windows file sytems dont like to have too many files in any given directory so we will try and limit the size of any directory*/
            string xy = string.Format("{0}\\{1}\\{2}\\{3}",
                /* group by x in chunks of 10Km  (assuming meters) */ (int)Math.Floor(config.RealWorldBounds.XMin / 10000),
                /* then by the actual MinX coordinate of the tile */ config.RealWorldBounds.XMin,
                /* group by y in chunks of 10Km (assuming meters) */ (int)Math.Floor(config.RealWorldBounds.YMin / 10000),
                /* then by the actual MinY coordinate of the tile */ config.RealWorldBounds.YMin);

            path = Path.Combine(path, xy);

            path = Path.Combine(path, GetCacheFileName(config));

            return path;
        }
    }
}