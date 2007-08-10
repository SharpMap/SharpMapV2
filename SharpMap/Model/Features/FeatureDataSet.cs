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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Runtime.Serialization;
using System.ComponentModel;
using SharpMap.Indexing.RTree;
using System.Globalization;
using System.Xml.Schema;
using System.IO;
using SharpMap.Indexing;
using SharpMap.Geometries;
using System.Threading;
using System.Reflection.Emit;
using System.Reflection;

namespace SharpMap
{
	/// <summary>
	/// Represents an in-memory cache of spatial data. 
	/// </summary>
    /// <remarks>
    /// The FeatureDataSet is an extension of System.Data.DataSet.
    /// </remarks>
	[Serializable()]
	public class FeatureDataSet : DataSet
    {
        #region Nested Types
        private delegate void SetDefaultViewManagerDelegate(FeatureDataSet dataSet, FeatureDataViewManager viewManager);
        private delegate FeatureDataViewManager GetDefaultViewManagerDelegate(FeatureDataSet dataSet);
        #endregion

        #region Type Fields
        private static readonly SetDefaultViewManagerDelegate _setDefaultViewManager;
        private static readonly GetDefaultViewManagerDelegate _getDefaultViewManager;
        #endregion

        #region Static Constructor
        static FeatureDataSet()
        {
            // Create DefaultViewManager getter method
            DynamicMethod get_DefaultViewManagerMethod = new DynamicMethod("get_DefaultViewManager_DynamicMethod",
                typeof(FeatureDataViewManager), null, typeof(DataSet));

            ILGenerator il = get_DefaultViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeof(DataSet).GetField("defaultViewManager", BindingFlags.Instance | BindingFlags.NonPublic));
            il.Emit(OpCodes.Ret);

            _getDefaultViewManager = get_DefaultViewManagerMethod.CreateDelegate(typeof(GetDefaultViewManagerDelegate))
                as GetDefaultViewManagerDelegate;

            // Create DefaultViewManager setter method
            DynamicMethod set_DefaultViewManagerMethod = new DynamicMethod("set_DefaultViewManager_DynamicMethod",
                typeof(FeatureDataViewManager), null, typeof(DataSet));

            il = set_DefaultViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, typeof(DataSet).GetField("defaultViewManager", BindingFlags.Instance | BindingFlags.NonPublic));

            _setDefaultViewManager = set_DefaultViewManagerMethod.CreateDelegate(typeof(SetDefaultViewManagerDelegate))
                as SetDefaultViewManagerDelegate;
        }
        #endregion

        #region Object Fields
        private FeatureTableCollection _featureTables;
        private SelfOptimizingDynamicSpatialIndex<FeatureDataRow> _rTreeIndex;
        private BoundingBox _visibleRegion;
        private object _defaultViewManagerSync = new object();
        private int _defaultViewManagerInitialized = 0;
        #endregion

        #region Constructors
        /// <summary>
		/// Initializes a new instance of the FeatureDataSet class.
		/// </summary>
		public FeatureDataSet()
		{
			initClass();
			CollectionChangeEventHandler schemaChangedHandler = schemaChanged;
			//this.Tables.CollectionChanged += schemaChangedHandler;
			Relations.CollectionChanged += schemaChangedHandler;
			initClass();
		}

		/// <summary>
		/// Initializes a new instance of the FeatureDataSet class.
		/// </summary>
		/// <param name="info">Serialized info.</param>
		/// <param name="context">Remoting context.</param>
		protected FeatureDataSet(SerializationInfo info, StreamingContext context)
		{
			string schemaString = ((string)(info.GetValue("XmlSchema", typeof(string))));
			
            if ((schemaString != null))
			{
				DataSet ds = new DataSet();

				ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(schemaString)));
				
                if ((ds.Tables["FeatureTable"] != null))
				{
					Tables.Add(new FeatureDataTable(ds.Tables["FeatureTable"]));
				}

				DataSetName = ds.DataSetName;
				Prefix = ds.Prefix;
				Namespace = ds.Namespace;
				Locale = ds.Locale;
				CaseSensitive = ds.CaseSensitive;
				EnforceConstraints = ds.EnforceConstraints;
				Merge(ds, false, System.Data.MissingSchemaAction.Add);
			}
			else
			{
				initClass();
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

        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set { _visibleRegion = value; }
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
		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether Relations property should be persisted.
		/// </summary>
		/// <returns></returns>
		protected override bool ShouldSerializeRelations()
		{
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
			this.DataSetName = ds.DataSetName;
			this.Prefix = ds.Prefix;
			this.Namespace = ds.Namespace;
			this.Locale = ds.Locale;
			this.CaseSensitive = ds.CaseSensitive;
			this.EnforceConstraints = ds.EnforceConstraints;
			this.Merge(ds, false, MissingSchemaAction.Add);
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

        #region Private helper methods
        private void initClass()
		{
			_featureTables = new FeatureTableCollection(base.Tables);
			//this.DataSetName = "FeatureDataSet";
			Prefix = "";
            Namespace = "http://www.codeplex.com/Wiki/View.aspx?ProjectName=SharpMap/FeatureDataSet.xsd";
			Locale = new CultureInfo("en-US");
			CaseSensitive = false;
			EnforceConstraints = true;

            IIndexRestructureStrategy restructureStrategy = new NullRestructuringStrategy();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.Default, 4.0);
            IEntryInsertStrategy<RTreeIndexEntry<FeatureDataRow>> insertStrategy = new GuttmanQuadraticInsert<FeatureDataRow>();
            INodeSplitStrategy nodeSplitStrategy = new GuttmanQuadraticSplit<FeatureDataRow>();
            _rTreeIndex = new SelfOptimizingDynamicSpatialIndex<FeatureDataRow>(restructureStrategy,
                restructureHeuristic, insertStrategy, nodeSplitStrategy, new DynamicRTreeBalanceHeuristic()); 
		}

		private bool shouldSerializeFeatureTable()
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

