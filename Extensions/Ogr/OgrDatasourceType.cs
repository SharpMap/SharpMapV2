using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using OgrOgr = OSGeo.OGR.Ogr;
using OgrDriver = OSGeo.OGR.Driver;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// An enumeration of the current datasources for OgrProvider
    /// </summary>
    public enum OgrDatasourceType
    {
        /// <summary>
        /// Value indicating that the data is stored in a file
        /// </summary>
        File,
        /// <summary>
        /// Value indicating that the data is stored in a folder
        /// </summary>
        Folder,
        /// <summary>
        /// Value indicating that the data is stored in a database
        /// </summary>
        Database,
        /// <summary>
        /// Value indicating that the data is fetched from a protocol
        /// </summary>
        Protocol
    }

    public class OgrDatasourceTypeDefinition
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Key;
        public readonly string Filter;
        public readonly string Prefix;

        internal OgrDatasourceTypeDefinition(int id,string name, string key, string filter)
        {
            Id = id;
            Name = name;
            Key = key;
            Filter = filter;
            Prefix = "";
        }

        internal OgrDatasourceTypeDefinition(int id, string name, string key, string filter, string prefix)
        {
            Id = id;
            Name = name;
            Key = key;
            Filter = filter;
            Prefix = prefix;
        }
        public string ToFilterString()
        {
            return string.Format("[OGR] {0}|{1}", Name, Filter);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Key);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class OgrDatasourceTypeStore
    {
        private readonly static IDictionary<string, OgrDatasourceTypeDefinition>[] Stores = new IDictionary<string, OgrDatasourceTypeDefinition>[4];

        static OgrDatasourceTypeStore()
        {
            OgrForSharpMap.Configure();
            //setup 
            for (int i = 0; i < 4; i++)
                Stores[i] = new Dictionary<string, OgrDatasourceTypeDefinition>();

            //Get all registered drivers
            SortedList<string, string> driverNames = GetDriverNames();

            //Add configurations
            AddFromRessouceStream(driverNames);
            AddFromConfigFile(driverNames);
        }

        private static void AddFromConfigFile(SortedList<string, string> driverNames)
        {
            NameValueConfigurationCollection dstSection = (NameValueConfigurationCollection) ConfigurationManager.GetSection("datasourceType");
            if (dstSection == null) return;
            string[] additional = dstSection.AllKeys;
            foreach (var s in additional)
            {
                string value = dstSection[s].Value;
                IDictionary<string, OgrDatasourceTypeDefinition> store = Stores[int.Parse(value.Substring(0, 1))];
                value = value.Substring(2);
                AddToStore(driverNames, store, s, value.Split('|'));
            }
        }

        private static void AddFromRessouceStream(SortedList<string, string> driverNames)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream(@"SharpMap.Data.Providers.Ressourcen.OgrDatasourceTypes.tsv"))
            {
                if (s != null)
                    using (StreamReader sr = new StreamReader(s, Encoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            string config = sr.ReadLine();
                            if (string.IsNullOrEmpty(config)) continue;
                            {
                                string[] parts = config.Split('|');
                                string[] options = new string[parts.Length-2];
                                Array.Copy(parts, 2, options, 0, options.Length);
                                IDictionary<string, OgrDatasourceTypeDefinition> store = Stores[int.Parse(parts[1])];
                                AddToStore(driverNames, store, parts[0], options);
                            }
                        }
                    }
            }
        }

        private static void AddToStore(IDictionary<string, string> driverNames, IDictionary<string, OgrDatasourceTypeDefinition> store, string key, string[] options)
        {
            if (driverNames.ContainsKey(key))
            {
                if (!store.ContainsKey(key))
                {
                    OgrDatasourceTypeDefinition dstd = new OgrDatasourceTypeDefinition(
                        store.Count, options[0], key, options[1]);
                    store.Add(key, dstd);
                }
                else
                    System.Diagnostics.Trace.WriteLine(string.Format("Driver with key '{0}' already added!", options[0]));
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Could not add '{0}' driver with key '{1}'", options[0], key));
            }
        }

        private static SortedList<string, string> GetDriverNames()
        {
            SortedList<string, string> driverNames= new SortedList<string, string>();
            for (int i = 0; i < OgrOgr.GetDriverCount(); i++)
            {
                using (OgrDriver dr = OgrOgr.GetDriver(i))
                    driverNames.Add(dr.name, dr.name);
            }
            return driverNames;
        }

        /// <summary>
        /// Returns the ogr datasource list available for the given <see cref="OgrDatasourceType"/>
        /// </summary>
        /// <param name="datasourceType">the <see cref="OgrDatasourceType"/></param>
        /// <returns>List of all available providers</returns>
        public static IList GetDatasourceList(OgrDatasourceType datasourceType)
        {
            IDictionary<string, OgrDatasourceTypeDefinition> selected = Stores[(int)datasourceType];
            return new List<OgrDatasourceTypeDefinition>(selected.Values);
        }

        public static string GetOfdFilterString()
        {
            StringBuilder sb = new StringBuilder();
            foreach ( OgrDatasourceTypeDefinition dtd in Stores[0].Values)
            {
                sb.AppendFormat("[OGR] {0}|{1}|", dtd.Name, dtd.Filter);
            }
            sb.Append("All files|*.*");
            return sb.ToString();
        }
    }

    
}
