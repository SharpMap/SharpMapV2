// Copyright 2010 Felix Obermaier, www.ivv-aachen.de
// Port from FWToolsHelper:
//
// Copyright 2009 John Diss www.newgrove.com
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Gdal = OSGeo.GDAL.Gdal;

namespace SharpMap
{
    /// <summary>
    /// This library supplies the means to access GDAL library through SharpMap
    /// </summary>
    /// <remarks>
    /// In order to get this library to work properly you need to provide
    /// <list type="bullet">
    /// <item><description>the native GDAL binaries,</description></item>
    /// <item><description>the charp bindings for those binaries and</description></item>
    /// <item><description>the appropriate environment variables for GDAL.</description></item>
    /// </list>
    /// <para>
    /// In order to provide the gdal binaries and charp bindings you may 
    /// <list type="bullet">
    /// <item>compile gdal yourself (have fun)</item> or 
    /// <item>choose to download either</item>
    /// <list type="bullet">
    /// <item>FWTools (http://fwtools.maptools.org/),</item>
    /// <item>Plain GDAL binaries (http://vbkto.dyndns.org/sdk/) provided by Tamas Szekeres</item>
    /// </list>
    /// </list>
    /// </para>
    /// <para>
    /// Use <see cref="GdalForSharpMap"/> to configure the gdal environment.
    /// </para>
    /// <example>
    /// This snipplet shows a sample of the [app|web].config file entries:
    /// <code>
    ///<add key="GdalBinPath" value="D:\Gdal\Native\x86;E:\Gdal\Native\x86\gdal\csharp;E:\Gdal\Native\x86\gdal\apps" />
    ///<add key="GdalDataPath" value="D:\Gdal\Native\x86\gdal-data" />
    ///<add key="GdalDriverPath" value="D:\Gdal\Native\x86\gdal\plugins;" />
    ///<add key="ProjLibPath" value="D:\Gdal\Native\x86\proj\SHARE" />
    /// </code>
    /// </example>
    ///</remarks>
    static class GdalForSharpMap
    {
        static GdalForSharpMap()
        {
            string gdalPath = ConfigurationManager.AppSettings["GdalBinPath"];
            PrependToPathIfReqired(gdalPath);

            string gdalDataPath = ConfigurationManager.AppSettings["GdalDataPath"];
            SetEnvPath("GDAL_DATA", gdalDataPath, true);

            string gdalDriverPath = ConfigurationManager.AppSettings["GdalDriverPath"];
            SetEnvPath("GDAL_PLUGINS_DIR", gdalDriverPath, false);

            string projLibPath = ConfigurationManager.AppSettings["ProjLibPath"];
            SetEnvPath("PROJ_LIB", projLibPath, true);

            Gdal.AllRegister();
        }

        private static void SetEnvPath(string variable, string directoryPath, bool ifExists)
        {
            if (String.IsNullOrEmpty(directoryPath))
            {
                if (ifExists) return;
                throw new GdalPathException(variable, directoryPath);
            }
            string[] paths = directoryPath.Split(';', ',');
            foreach (string path in paths)
            {
                if (String.IsNullOrEmpty(path)) continue;
                if (!Directory.Exists(path))
                {
                    if (!ifExists)
                        throw new GdalPathException(variable, path);
                }
            }

            Environment.SetEnvironmentVariable(variable, directoryPath);
        }

        private static void PrependToPathIfReqired(String directoryPath)
        {

            if (String.IsNullOrEmpty(directoryPath))
                throw new GdalPathException("PATH", directoryPath);

            string path = Environment.GetEnvironmentVariable("PATH");
            List<string>  paths = new List<string>(path.ToUpper().Split(new[] {';', ','}));

            List<string> directoryPathItems = new List<string>(directoryPath.Split(',', ';'));
            foreach (string pathItem in directoryPathItems)
            {
                if (String.IsNullOrEmpty(pathItem)) continue;
                if (paths.Contains(pathItem.ToUpper())) continue;

                path = pathItem + ";" + path;
            }
            Environment.SetEnvironmentVariable("PATH", path);
        }

        public static string GdalVersion
        {
            get { return Gdal.VersionInfo("VERSION_NUM"); }
        }

        public static void Configure()
        {
            //does nothing but ensure that the Static initializer has been called.
        }

        #region Nested type: OsGeo4WPathException

        public class GdalPathException : Exception
        {
            public GdalPathException(string variable, string path)
                : base(
                    string.Format("Error setting environment variable {0}: cannot find '{1}' directory!",
                                  variable, path))
            {
            }
        }

        #endregion
    }

}
