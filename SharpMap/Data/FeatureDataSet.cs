// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
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
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using GeoAPI.Geometries;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents an in-memory cache of spatial data. 
    /// </summary>
    /// <remarks>
    /// The FeatureDataSet is an extension of System.Data.DataSet, and can be used 
    /// in much the same way.
    /// </remarks>
    [Serializable()]
    public class FeatureDataSet : DataSet
    {
        #region Nested types
        private delegate void SetDefaultViewManagerDelegate(FeatureDataSet dataSet, FeatureDataViewManager viewManager);
        private delegate FeatureDataViewManager GetDefaultViewManagerDelegate(FeatureDataSet dataSet);
        #endregion

        #region Type fields
        private static readonly SetDefaultViewManagerDelegate _setDefaultViewManager;
        private static readonly GetDefaultViewManagerDelegate _getDefaultViewManager;
        private static Int32 _nameSeries = -1;
        #endregion

        #region Static constructor
        static FeatureDataSet()
        {
            // Create DefaultViewManager getter method
            _getDefaultViewManager = generateGetDefaultViewManagerDelegate();

            // Create DefaultViewManager setter method
            _setDefaultViewManager = generateSetDefaultViewManagerDelegate();
        }
        #endregion

        #region Instance fields
        private FeatureTableCollection _featureTables;
        private readonly object _defaultViewManagerSync = new object();
        private Int32 _defaultViewManagerInitialized = 0;
        private readonly IGeometryFactory _geoFactory;
        #endregion

        #region Object constructors

        /// <summary>
        /// Initializes a new instance of the FeatureDataSet class.
        /// </summary>
        public FeatureDataSet(IGeometryFactory geoFactory)
            : this(generateName(), geoFactory) { }

        /// <summary>
        /// Initializes a new instance of the FeatureDataSet class with the given name.
        /// </summary>
        public FeatureDataSet(String name, IGeometryFactory geoFactory)
        {
            _geoFactory = geoFactory;
            initClass(name);
            CollectionChangeEventHandler schemaChangedHandler = schemaChanged;
            //this.Tables.CollectionChanged += schemaChangedHandler;
            Relations.CollectionChanged += schemaChangedHandler;
        }

        /// <summary>
        /// Initializes a new instance of the FeatureDataSet class.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Remoting context.</param>
        protected FeatureDataSet(SerializationInfo info, StreamingContext context)
        {
            String schemaString = ((String) (info.GetValue("XmlSchema", typeof (String))));

            if ((schemaString != null))
            {
                DataSet ds = new DataSet();

                ds.ReadXmlSchema(new XmlTextReader(new StringReader(schemaString)));

                if ((ds.Tables["FeatureTable"] != null))
                {
                    Tables.Add(new FeatureDataTable(ds.Tables["FeatureTable"], _geoFactory));
                }

                DataSetName = ds.DataSetName;
                Prefix = ds.Prefix;
                Namespace = ds.Namespace;
                Locale = ds.Locale;
                CaseSensitive = ds.CaseSensitive;
                EnforceConstraints = ds.EnforceConstraints;
                Merge(ds, false, MissingSchemaAction.Add);
            }
            else
            {
                initClass("Unknown");
            }

            GetSerializationData(info, context);
            CollectionChangeEventHandler schemaChangedHandler = schemaChanged;
            //Tables.CollectionChanged += schemaChangedHandler;
            Relations.CollectionChanged += schemaChangedHandler;
        }

        #endregion

        public new FeatureDataViewManager DefaultViewManager
        {
            get
            {
                if (_defaultViewManagerInitialized == 0)
                {
                    lock (_defaultViewManagerSync)
                    {
                        if (_defaultViewManagerInitialized == 0)
                        {
                            Interlocked.Increment(ref _defaultViewManagerInitialized);

                            // Read value to initialize base storage field.
                            DataViewManager temp = base.DefaultViewManager;

                            // Replace base storage field with subclass instance
                            _setDefaultViewManager(this, new FeatureDataViewManager(this, true));

                            // Get rid of initial instance, since we don't need it
                            temp.Dispose();
                        }
                    }
                }

                return _getDefaultViewManager(this);
            }
        }

        public IExtents Extents
        {
            get
            {
                IExtents extents = null;

                foreach (FeatureDataTable table in _featureTables)
                {
                    if (extents == null)
	                {
	                    extents = table.Extents;
	                }
                    else
                    {
                        extents.ExpandToInclude(table.Extents);
                    }
                }

                return extents;
            }
        }

        /// <summary>
        /// Gets the collection of tables contained in the FeatureDataSet.
        /// </summary>
        public new FeatureTableCollection Tables
        {
            get { return _featureTables; }
        }

        /// <summary>
        /// Copies the structure of the FeatureDataSet, 
        /// including all FeatureDataTable schemas, relations, 
        /// and constraints. Does not copy any data. 
        /// </summary>
        /// <returns></returns>
        public new FeatureDataSet Clone()
        {
            FeatureDataSet copy = base.Clone() as FeatureDataSet;
            return copy;
        }

        #region Overrides

        /// <summary>
        /// Gets a value indicating whether Tables property should be persisted.
        /// </summary>
        /// <returns></returns>
        protected override Boolean ShouldSerializeTables()
        {
            // TODO: no clue what to do here...
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether Relations property should be persisted.
        /// </summary>
        /// <returns></returns>
        protected override Boolean ShouldSerializeRelations()
        {
            // TODO: no clue what to do here...
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlSerializable(XmlReader reader)
        {
            Reset();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);
            //if ((ds.Tables["FeatureTable"] != null))
            //{
            //    this.Tables.Add(new FeatureDataTable(ds.Tables["FeatureTable"]));
            //}
            DataSetName = ds.DataSetName;
            Prefix = ds.Prefix;
            Namespace = ds.Namespace;
            Locale = ds.Locale;
            CaseSensitive = ds.CaseSensitive;
            EnforceConstraints = ds.EnforceConstraints;
            Merge(ds, false, MissingSchemaAction.Add);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override XmlSchema GetSchemaSerializable()
        {
            MemoryStream stream = new MemoryStream();
            WriteXmlSchema(new XmlTextWriter(stream, null));
            stream.Position = 0;
            return XmlSchema.Read(new XmlTextReader(stream), null);
        }
        #endregion

        #region Private static helper methods

        private static String generateName()
        {
            String name = "FeatureDataSet";

            Interlocked.Increment(ref _nameSeries);

            if (_nameSeries > 0)
            {
                name += _nameSeries.ToString();
            }

            return name;
        }

        private static SetDefaultViewManagerDelegate generateSetDefaultViewManagerDelegate()
        {
            DynamicMethod set_DefaultViewManagerMethod = new DynamicMethod("set_DefaultViewManager_DynamicMethod",
                                                                           null,
                                                                           new Type[]
                                                                               {
                                                                                   typeof (FeatureDataSet),
                                                                                   typeof (FeatureDataViewManager)
                                                                               },
                                                                           typeof (DataSet));

            ILGenerator il = set_DefaultViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld,
                    typeof (DataSet).GetField("defaultViewManager", BindingFlags.Instance | BindingFlags.NonPublic));

            return set_DefaultViewManagerMethod.CreateDelegate(typeof (SetDefaultViewManagerDelegate))
                   as SetDefaultViewManagerDelegate;
        }

        private static GetDefaultViewManagerDelegate generateGetDefaultViewManagerDelegate()
        {
            DynamicMethod get_DefaultViewManagerMethod = new DynamicMethod("get_DefaultViewManager_DynamicMethod",
                                                                           typeof (FeatureDataViewManager),
                                                                           new Type[] {typeof (FeatureDataSet)},
                                                                           typeof (DataSet));

            ILGenerator il = get_DefaultViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld,
                    typeof (DataSet).GetField("defaultViewManager", BindingFlags.Instance | BindingFlags.NonPublic));
            il.Emit(OpCodes.Ret);

            return get_DefaultViewManagerMethod.CreateDelegate(typeof (GetDefaultViewManagerDelegate))
                   as GetDefaultViewManagerDelegate;
        }

        #endregion

        #region Private helper methods

        private void initClass(String name)
        {
            DataSetName = name;
            _featureTables = new FeatureTableCollection(base.Tables);
            Prefix = "";
            Namespace = "http://www.codeplex.com/SharpMap/Wiki/View.aspx?title=FeatureDataSet";
            Locale = new CultureInfo("en-US");
            CaseSensitive = false;
            EnforceConstraints = true;
        }

        private Boolean shouldSerializeFeatureTable()
        {
            return false;
        }

        private void schemaChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                //this.InitVars();
            }
        }

        #endregion
    }
}