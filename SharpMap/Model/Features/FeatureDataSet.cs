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
        private FeatureTableCollection _featureTables;

		/// <summary>
		/// Initializes a new instance of the FeatureDataSet class.
		/// </summary>
		public FeatureDataSet()
		{
			this.initClass();
			System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.schemaChanged);
			//this.Tables.CollectionChanged += schemaChangedHandler;
			this.Relations.CollectionChanged += schemaChangedHandler;
			this.initClass();
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
					this.Tables.Add(new FeatureDataTable(ds.Tables["FeatureTable"]));
				}

				this.DataSetName = ds.DataSetName;
				this.Prefix = ds.Prefix;
				this.Namespace = ds.Namespace;
				this.Locale = ds.Locale;
				this.CaseSensitive = ds.CaseSensitive;
				this.EnforceConstraints = ds.EnforceConstraints;
				this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
			}
			else
			{
				this.initClass();
			}

			this.GetSerializationData(info, context);
			CollectionChangeEventHandler schemaChangedHandler = new CollectionChangeEventHandler(this.schemaChanged);
			//this.Tables.CollectionChanged += schemaChangedHandler;
			this.Relations.CollectionChanged += schemaChangedHandler;
		}

		/// <summary>
		/// Gets the collection of tables contained in the FeatureDataSet
		/// </summary>
		public new FeatureTableCollection Tables
		{
			get { return _featureTables; }
		}

		/// <summary>
		/// Copies the structure of the FeatureDataSet, including all FeatureDataTable schemas, relations, and constraints. Does not copy any data. 
		/// </summary>
		/// <returns></returns>
		public new FeatureDataSet Clone()
		{
			FeatureDataSet cln = ((FeatureDataSet)(base.Clone()));
			return cln;
		}

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
			this.Reset();
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
			this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override System.Xml.Schema.XmlSchema GetSchemaSerializable()
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			this.WriteXmlSchema(new XmlTextWriter(stream, null));
			stream.Position = 0;
			return System.Xml.Schema.XmlSchema.Read(new XmlTextReader(stream), null);
		}


		private void initClass()
		{
			_featureTables = new FeatureTableCollection();
			//this.DataSetName = "FeatureDataSet";
			this.Prefix = "";
            this.Namespace = "http://www.codeplex.com/Wiki/View.aspx?ProjectName=SharpMap/FeatureDataSet.xsd";
			this.Locale = new System.Globalization.CultureInfo("en-US");
			this.CaseSensitive = false;
			this.EnforceConstraints = true;
		}

		private bool shouldSerializeFeatureTable()
		{
			return false;
		}

		private void schemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
		{
			if ((e.Action == System.ComponentModel.CollectionChangeAction.Remove))
			{
				//this.InitVars();
			}
		}
	}
}

