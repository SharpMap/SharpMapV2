/*
 *  The attached / following is part of SharpMap.Data.Providers.SpatiaLite2
 *  SharpMap.Data.Providers.SpatiaLite2 is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2008
 *  
 *  This work is based on SharpMap.Data.Providers.Db by John Diss for 
 *  Newgrove Consultants Ltd, www.newgrove.com
 *  
 *  Other than that, this spatial data provider requires:
 *  - SharpMap by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/SharpMap
 *    
 *  - GeoAPI.Net by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/GeoApi
 *    
 *  - SqLite, dedicated to public domain
 *    http://www.sqlite.org
 *  - SpatiaLite-2.2 by Alessandro Furieri released under a disjunctive tri-license:
 *    - 'Mozilla Public License, version 1.1 or later' OR
 *    - 'GNU General Public License, version 2.0 or later' 
 *    - 'GNU Lesser General Public License, version 2.1 or later' <--- this is the one
 *    http://www.gaia-gis.it/spatialite-2.2/index.html
 *    
 *  - SQLite ADO.NET 2.0 Provider by Robert Simpson, dedicated to public domain
 *    http://sourceforge.net/projects/sqlite-dotnet2
 *    
 */

/*
 * 
 * 
 * Futher Modified by john diss
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Proj4Utility
{
    public class Proj4Reader
    {
        /// <summary>
        /// Content of PROJ4.csv file serves this structure
        /// </summary>
        public struct Proj4SpatialRefSys
        {
            //srid, auth_name, auth_srid, ref_sys_name, proj4text
            private readonly Int64 srid;
            private readonly String authorityName;
            private readonly Int64 authoritySrid;
            private readonly String refSysName;
            private readonly String proj4Text;
            private readonly String srText;

            public Proj4SpatialRefSys(Int64 srid, String authorityName, Int64 authoritySrid, String refSysName, String proj4Text, String srText)
            {
                this.srid = srid;
                this.authorityName = authorityName;
                this.authoritySrid = authoritySrid;
                this.refSysName = refSysName;
                this.proj4Text = proj4Text;
                this.srText = srText;
            }

            public long Srid
            {
                get { return srid; }
            }

            public string AuthorityName
            {
                get { return authorityName; }
            }

            public long AuthoritySrid
            {
                get { return authoritySrid; }
            }

            public string RefSysName
            {
                get { return refSysName; }
            }

            public string Proj4Text
            {
                get { return proj4Text; }
            }

            public string SrText
            {
                get { return srText; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>IEnumerable(Proj4SpatialRefSys)</returns>
        public static IEnumerable<Proj4SpatialRefSys> GetSRIDs(TextReader sr)
        {
            using (sr)
            {
                while (!((StreamReader)sr).EndOfStream)
                {
                    String line = sr.ReadLine();

                    //jd: original didnt seem to work.. the csv was not a csv and the tokenizer was splitting incorrectly - probably due to double double quoting.
                    //so modified the file removed double double quotes and split on tab;


                    string[] parts = line.Split('\t');


                    Int64 srid = long.Parse(parts[0]);

                    String auth = parts[1];

                    Int64 asrid = long.Parse(parts[2]);

                    String name = parts[3];

                    String proj4 = parts[4];

                    string srText = parts.Length > 5 ? parts[5] : "";


                    yield return new Proj4SpatialRefSys(srid, auth, asrid, name, proj4, srText);

                }
            }
        }


        public static IEnumerable<Proj4SpatialRefSys> GetSRIDs(string proj4FilePath)
        {
            return GetSRIDs(File.OpenText(proj4FilePath));
        }

        public static IEnumerable<Proj4SpatialRefSys> GetSRIDs()
        {
            return GetSRIDs(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Proj4Utility.PROJ4.tsv")));
        }

    }
}