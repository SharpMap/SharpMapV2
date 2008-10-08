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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpMap.Data.Providers.SpatiaLite2
{
    internal class Proj4Reader
    {
        /// <summary>
        /// Content of PROJ4.csv file serves this structure
        /// </summary>
        public struct Proj4SpatialRefSys
        {
            //srid, auth_name, auth_srid, ref_sys_name, proj4text
            internal readonly Int64 Srid;
            internal readonly String AuthorityName;
            internal readonly Int64 AuthoritySrid;
            internal readonly String RefSysName;
            internal readonly String Proj4Text;

            public Proj4SpatialRefSys(Int64 srid, String authorityName, Int64 authoritySrid, String refSysName, String proj4Text)
            {
                Srid = srid;
                AuthorityName = authorityName;
                AuthoritySrid = authoritySrid;
                RefSysName = refSysName;
                Proj4Text = proj4Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>IEnumerable(Proj4SpatialRefSys)</returns>
        public static IEnumerable<Proj4SpatialRefSys> GetSRIDs()
        {
            using (TextReader sr = File.OpenText("PROJ4.CSV"))
            {
                while (!((StreamReader)sr).EndOfStream)
                {
                    String line = sr.ReadLine();
                    TextTokenizer tokenizer = new TextTokenizer(new StringReader(line), true);

                    tokenizer.Read();
                    Int64 srid = (Int64)tokenizer.CurrentTokenAsNumber;
                    tokenizer.Read();
                    String auth = ReadDoubleQuotedWord(tokenizer);
                    tokenizer.Read();
                    Int64 asrid = (Int64)tokenizer.CurrentTokenAsNumber;
                    tokenizer.Read();
                    String name = ReadDoubleQuotedWord(tokenizer);
                    tokenizer.Read();
                    String proj4 = ReadDoubleQuotedWord(tokenizer);

                    yield return new Proj4SpatialRefSys(srid, auth, asrid, name, proj4);

                }
            }
        }

        private static String ReadDoubleQuotedWord(TextTokenizer tokenizer)
        {
            TokenType tt = tokenizer.Read();
            if (tokenizer.CurrentToken == "\"") tokenizer.Read();
            StringBuilder sb = new StringBuilder();
            while (tokenizer.CurrentToken != "\"")
            {
                sb.Append(tokenizer.CurrentToken);
                tokenizer.Read(false);
            }
            tokenizer.Read();
            return sb.ToString();

        }
    }
}

