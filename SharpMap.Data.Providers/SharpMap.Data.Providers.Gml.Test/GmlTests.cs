using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMap.Entities.Ogc.Gml;
using System.Xml.Serialization;
using File = SharpMap.Entities.Ogc.Gml.File;

namespace SharpMap.Data.Providers.Gml.Test
{
    /// <summary>
    /// Summary description for GmlTests
    /// </summary>
    [TestClass]
    public class GmlTests
    {
        public GmlTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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


        private List<Type> _exportedTypes;

        [TestMethod]
        public void TestGmlSerialization()
        {
            foreach (FileInfo fi in new DirectoryInfo(RootPath).GetFiles())
                fi.Delete();



            Type t = typeof(AbstractFeature);
            Assembly a = t.Assembly;
            _exportedTypes = a.GetExportedTypes().Where(o => !o.IsAbstract).ToList();

            Dictionary<Type, Exception> exceptions = new Dictionary<Type, Exception>();

            foreach (Type e in _exportedTypes)
            {
                try
                {
                    SerializeType(e);

                }
                catch (Exception ex)
                {
                    exceptions.Add(e, ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregatedExceptions(exceptions);


        }


        private void SerializeType(Type e)
        {
            XmlSerializer serializer = new XmlSerializer(e);

            string path = Path.Combine(RootPath, e.Name + ".gml");

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, Mock(e, new List<Type>()));
            }
        }

        private object Mock(Type type, List<Type> circleRefTracker)
        {
            if (type.IsAbstract)
            {
                IList<XmlIncludeAttribute> attrs =
                    TypeDescriptor.GetAttributes(type).OfType<XmlIncludeAttribute>().ToList();

                if (attrs.Count() > 0)
                    return Mock(attrs[_rnd.Next(0, attrs.Count)].Type, circleRefTracker);

                foreach (Type t in _exportedTypes)
                    if (type.IsAssignableFrom(t))
                        return Mock(t, circleRefTracker);

                return null;
            }

            if (circleRefTracker.Contains(type))
                return null;

            object o = Activator.CreateInstance(type);
            circleRefTracker.Add(type);

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor desc in props)
            {

                if (typeof(IEnumerable).IsAssignableFrom(desc.PropertyType) && desc.PropertyType != typeof(string))
                {
                    Type[] typeparams = desc.PropertyType.GetGenericArguments();
                    if (typeparams.Length > 0)
                    {
                        IEnumerable enumerable = (IEnumerable)desc.GetValue(o);
                        Type enumerableType = typeparams[0];
                        for (int i = 0; i < 5; i++)
                            (enumerable as IList).Add(GetMockValue(enumerableType, circleRefTracker));
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                    desc.SetValue(o, GetMockValue(desc.PropertyType, circleRefTracker));
            }


            return o;

        }

        private Random _rnd = new Random(DateTime.Now.Millisecond);

        private object GetMockValue(Type type, List<Type> tracker)
        {
            if (type == typeof(String))
                return RandomString();
            if (type == typeof(Int32))
                return _rnd.Next();
            if (type == typeof(double))
                return _rnd.NextDouble();
            if (type == typeof(float))
                return (float)_rnd.NextDouble();
            if (type == typeof(bool))
                return _rnd.Next(0, 2) == 1;
            if (type == typeof(Int64))
                return (long)_rnd.Next();
            if (type == typeof(DateTime))
                return DateTime.Now.AddHours(-1 * _rnd.NextDouble() * 360.0 * 24.0);
            if (type == typeof(TimeSpan))
                return new TimeSpan(_rnd.Next(500), _rnd.Next(24), _rnd.Next(60), _rnd.Next(60));
            if (type == typeof(Decimal))
                return (Decimal)_rnd.NextDouble();
            if (type.IsEnum)
            {
                System.Array vals = Enum.GetValues(type);
                return vals.GetValue(_rnd.Next(0, vals.Length));
            }
            if (type.IsPrimitive || type.IsValueType)
                throw new NotImplementedException();

            return Mock(type, tracker);

        }

        private object RandomString()
        {
            return "Random String";
        }

        protected string RootPath
        {
            get { return @"d:\temp\gmlserialization\"; }
        }
    }


    public class AggregatedExceptions : Exception
    {
        private Dictionary<Type, Exception> _exceptions;
        public AggregatedExceptions(Dictionary<Type, Exception> dictionary)
        {
            _exceptions = dictionary;
        }

        private string _message;
        public override string Message
        {
            get
            {
                _message = _message ?? BuildMessage();
                return _message;
            }
        }

        private string BuildMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Exceptions.Count.ToString() + " errors");
            foreach (KeyValuePair<Type, Exception> kvp in Exceptions)
            {
                sb.AppendLine("Error Serializing Type: " + kvp.Key.Name);
                sb.AppendLine(BuildMessage(kvp));
                sb.AppendLine("=================================================");
            }
            return sb.ToString();
        }

        private string BuildMessage(KeyValuePair<Type, Exception> kvp)
        {
            StringBuilder sb = new StringBuilder();
            for (Exception curr = kvp.Value; curr != null; curr = curr.InnerException)
            {
                sb.AppendLine(curr.Message);
                if (curr.InnerException == null)
                {
                    sb.AppendLine(curr.StackTrace);
                    sb.AppendLine(curr.HelpLink);
                }
            }

            return sb.ToString();
        }

        public Dictionary<Type, Exception> Exceptions
        {
            get { return _exceptions; }
        }
    }
}
