using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SharpMap
{
    /// <summary>
    /// Represents one feature table of in-memory spatial data. 
    /// </summary>
#if !DEBUG_STEPINTO
	[System.Diagnostics.DebuggerStepThrough()]
#endif
    [Serializable()]
    public class FeatureDataTable : DataTable, IEnumerable<FeatureDataRow>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the FeatureDataTable class with no arguments.
        /// </summary>
        public FeatureDataTable()
            : base()
        {
            this.initClass();
        }

        /// <summary>
        /// Intitalizes a new instance of the FeatureDataTable class with the specified table name
        /// </summary>
        /// <param name="table"></param>
        public FeatureDataTable(DataTable table)
            : base(table.TableName)
        {
            //if (table.DataSet == null)
            //    throw new ArgumentException("Parameter 'table' must belong to a DataSet");

            if (table.DataSet == null || (table.CaseSensitive != table.DataSet.CaseSensitive))
            {
                this.CaseSensitive = table.CaseSensitive;
            }
            if (table.DataSet == null || (table.Locale.ToString() != table.DataSet.Locale.ToString()))
            {
                this.Locale = table.Locale;
            }
            if (table.DataSet == null || (table.Namespace != table.DataSet.Namespace))
            {
                this.Namespace = table.Namespace;
            }

            this.Prefix = table.Prefix;
            this.MinimumCapacity = table.MinimumCapacity;
            this.DisplayExpression = table.DisplayExpression;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after a FeatureDataRow has been changed successfully. 
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowChanged;

        /// <summary>
        /// Occurs when a FeatureDataRow is changing. 
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowChanging;

        /// <summary>
        /// Occurs after a row in the table has been deleted.
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowDeleted;

        /// <summary>
        /// Occurs before a row in the table is about to be deleted.
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowDeleting;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of rows that belong to this table.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if this property is set.</exception>
        public new DataRowCollection Rows
        {
            get { return base.Rows; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of rows in the table
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public int Count
        {
            get { return base.Rows.Count; }
        }

        /// <summary>
        /// Gets the feature data row at the specified index
        /// </summary>
        /// <param name="index">row index</param>
        /// <returns>FeatureDataRow</returns>
        public FeatureDataRow this[int index]
        {
            get { return (FeatureDataRow)base.Rows[index]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a row to the FeatureDataTable
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(FeatureDataRow row)
        {
            base.Rows.Add(row);
        }

        /// <summary>
        /// Returns an enumerator for enumering the rows of the FeatureDataTable
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<FeatureDataRow> GetEnumerator()
        {
            foreach (FeatureDataRow row in Rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// Clones the structure of the FeatureDataTable, including all FeatureDataTable schemas and constraints. 
        /// </summary>
        /// <returns></returns>
        public new FeatureDataTable Clone()
        {
            FeatureDataTable cln = ((FeatureDataTable)(base.Clone()));
            cln.InitVars();
            return cln;
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table.
        /// </summary>
        /// <returns></returns>
        public new FeatureDataRow NewRow()
        {
            return (FeatureDataRow)base.NewRow();
        }

        /// <summary>
        /// Removes the row from the table
        /// </summary>
        /// <param name="row">Row to remove</param>
        public void RemoveRow(FeatureDataRow row)
        {
            base.Rows.Remove(row);
        }
        #endregion

        #region Protected Methods and Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance()
        {
            return new FeatureDataTable();
        }

        protected void InitVars()
        {
            //this.columnFeatureGeometry = this.Columns["FeatureGeometry"];
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table, based on a datarow builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FeatureDataRow(builder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override System.Type GetRowType()
        {
            return typeof(FeatureDataRow);
        }
        #endregion

        #region Event Generators

        /// <summary>
        /// Raises the FeatureDataRowChanged event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            base.OnRowChanged(e);

            if ((this.FeatureDataRowChanged != null))
            {
                this.FeatureDataRowChanged(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        /// <summary>
        /// Raises the FeatureDataRowChanging event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);

            if ((this.FeatureDataRowChanging != null))
            {
                this.FeatureDataRowChanging(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        /// <summary>
        /// Raises the FeatureDataRowDeleted event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);

            if ((this.FeatureDataRowDeleted != null))
            {
                this.FeatureDataRowDeleted(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        /// <summary>
        /// Raises the FeatureDataRowDeleting event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            base.OnRowDeleting(e);
            if ((this.FeatureDataRowDeleting != null))
            {
                this.FeatureDataRowDeleting(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }
        #endregion

        #region Private Helper Methods

        private void initClass()
        {
            //this.columnFeatureGeometry = new DataColumn("FeatureGeometry", typeof(SharpMap.Geometries.Geometry), null, System.Data.MappingType.Element);
            //this.Columns.Add(this.columnFeatureGeometry);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
