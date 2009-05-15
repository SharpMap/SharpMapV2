// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RegexSource
{
    internal class RegexToUpperApp
    {
        public static void Run()
        {
            Console.WriteLine(
                @"This app will parse a directory of files and replace patterns of text with another pattern uppercased eg.
using search pattern (?<=public [A-Za-z<>]+ )([a-z])(?=[A-Za-z]+[\s]+\{)
replace pattern \0
the string ""public int address {"" is transformed to ""public int Address {""
");

            Console.WriteLine("Please Enter the Regex Search Pattern");
            string search = Console.ReadLine();

            Console.WriteLine("Please Enter the Regex Replace Pattern");

            string replace = Console.ReadLine();

            Regex r = new Regex(search);

            Console.WriteLine("Please enter the path to the directory.");
            string path = Console.ReadLine();

            Console.WriteLine("Please enter the file search pattern");
            string pattern = Console.ReadLine();

            DirectoryInfo inf = new DirectoryInfo(path);
            FileInfo[] files = inf.GetFiles(pattern);

            string exportPath = Path.Combine(path, "Regexed");
            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            Regex r2 = new Regex(replace);

            foreach (FileInfo info in files)
            {
                string txt = File.ReadAllText(info.FullName);


                string regexed = r.Replace(txt, 
                    a => 
                        r2.Replace(a.ToString(), replace).ToUpper());

                File.WriteAllText(Path.Combine(exportPath, info.Name), regexed);
            }
        }
    }
}