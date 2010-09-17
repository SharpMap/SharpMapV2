#region License

/*
 *  The attached / following is part of MapViewer.
 *  
 *  MapViewer is free software © 2010 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2010
 *  
 */

#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    public partial class OgrMask : UserControl, ICreateDataProvider
    {
        private string[] _settings = new string[4];
        private int[] _ogrDriver = new int[4];
        private int _lastOgrDatasourceType = -1;
        private int _lastOgrDriverType = -1;

        public OgrMask()
        {
            InitializeComponent();
            cboOgrDatasourceTypes.DataSource = Enum.GetValues(typeof (OgrDatasourceType));
            cboOgrDatasourceTypes.SelectedIndex = 0;
            ofd.Filter = OgrDatasourceTypeStore.GetOfdFilterString();
            cboDriver.ValueMember = "Id";
            cboDriver.DisplayMember = "Name";
            lstLayers.Columns.Add("Index");
            lstLayers.Columns.Add("Layer");
            lstLayers.Columns.Add("GeometryType");

        }

        #region ICreateDataProvider Members

        public IEnumerable<IFeatureProvider> GetProviders()
        {
            if (lstLayers.SelectedIndices.Count == 0)yield break;

            IGeometryServices svc = new GeometryServices();
            foreach (var var in lstLayers.SelectedItems)
            {
                string text = ((ListViewItem) var).Text;
                yield return
                    new OgrProvider(tbConnectionString.Text, int.Parse(text), svc.DefaultGeometryFactory,
                                    svc.CoordinateSystemFactory);
            }

        }

        public IEnumerable<string> ProviderNames
        {
            get
            {
                foreach (var var in lstLayers.SelectedItems)
                {
                    string text = ((ListViewItem)var).Text;
                    yield return tbConnectionString.Text + " - " + text;
                }
            }
        }


        #endregion

        private void bBrowse_Click(object sender, EventArgs e)
        {
            switch (cboOgrDatasourceTypes.SelectedIndex)
            {
                case 0:
                    ofd.FileName = tbConnectionString.Text;
                    if (ofd.ShowDialog() == DialogResult.OK)
                        tbConnectionString.Text = ofd.FileName;
                    _ogrDriver[0] = ofd.FilterIndex;
                    break;
                case 1:
                    fbd.SelectedPath = tbConnectionString.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                        tbConnectionString.Text = fbd.SelectedPath;
                    break;
                case 2:

                case 3:

                default:
                    return;
            }

            lstLayers.Items.Clear();
            foreach (var lyr in OgrProvider.GetLayers(tbConnectionString.Text))
                lstLayers.Items.Add(new ListViewItem( new string[] {lyr.LayerIndex.ToString(),lyr.LayerName, Enum.GetName(typeof(OgcGeometryType), lyr.GeometryTypes )}));
        }

        private static object[] ToArray<T>(IEnumerable<T> enumerable)
        {
            List<object> tmp = new List<object>();
            foreach (T var in enumerable)
                tmp.Add(var);
            return tmp.ToArray();
        }
        /*
        private static T[] ToArray<T>(IEnumerable<T> enumerable)
        {
            List<T> tmp = new List<T>(enumerable);
            return tmp.ToArray();
        }
        */
        private void cboOgrDatasourceTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox) sender;
            
            lblDriver.Enabled = cboDriver.Enabled = box.SelectedIndex > 0;
            //cboDriver.Items.Clear();

            StoreCurrentSetting();
            SetDriver();
            _lastOgrDatasourceType = box.SelectedIndex;
        }

        private void StoreCurrentSetting()
        {
            //
            if (_lastOgrDatasourceType < 0) return;
            _settings[_lastOgrDatasourceType] = tbConnectionString.Text;
            _ogrDriver[_lastOgrDatasourceType] = cboDriver.SelectedIndex;
            _lastOgrDatasourceType = cboOgrDatasourceTypes.SelectedIndex;
        }

        private void SetDriver()
        {
            int index = -1;
            if (cboOgrDatasourceTypes.SelectedIndex > 0) 
            {
                index = cboOgrDatasourceTypes.SelectedIndex;
                cboDriver.DataSource = OgrDatasourceTypeStore.GetDatasourceList((OgrDatasourceType) index);
                cboDriver.SelectedIndex = _ogrDriver[index];
            }
            if (cboOgrDatasourceTypes.SelectedIndex > 0) tbConnectionString.Text = _settings[index];
        }

    }
}