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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMap.Entities.Atom;
using SharpMap.Entities.Ogc.Kml;
using SharpMap.Entities.xAL;

namespace SharpMap.Data.Providers.Kml.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class KmlTests
    {
        private const string RootPath =
            @"D:\Dev\Products\AcsV5.0\AcsV5.0\Externals\Sharpmap\SharpMap.Data.Providers\SharpMap.Data.Providers.Kml.Test\";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void ParseBasicKml()
        {
            string path = Path.Combine(RootPath, "basic.kml");
            KmlRoot r = Deserialize<KmlRoot>(path);
            Assert.IsNotNull(r);
        }

        private static T Deserialize<T>(string path) where T : class
        {
            XmlNameTable table = new NameTable();
            table.Add(" http://www.opengis.net/kml/2.2");
            table.Add("urn:oasis:names:tc:ciq:xsdschema:xAL:2.0");
            table.Add("http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T obj;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                obj = serializer.Deserialize(new XmlTextReader(fs, table)) as T;
            }
            return obj;
        }


        [TestMethod]
        public void WriteRoot()
        {
            KmlRoot root = new KmlRoot();
            Serialize(root, Path.Combine(RootPath, "root.kml"));
        }

        [TestMethod]
        public void WritePlacemark()
        {
            Placemark p = new Placemark();
            p.AddressDetails = new AddressDetails
                                   {
                                       Address = { Value = "12345 some street, somewhere , someplace" },
                                       AddressType = "Postal Address"
                                   };
            p.Author = new Author
                           {
                               Email =
                                   {
                                       "someguy@someplace.thing"
                                   },
                               Name =
                                   {
                                       "Some Guy"
                                   },
                               Uri =
                                   {
                                       "www.someplace.thing"
                                   }
                           };

            Serialize(p, Path.Combine(RootPath, "placemark.kml"));
        }

        private static void Serialize<T>(T obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, obj);
            }
        }
    }
}