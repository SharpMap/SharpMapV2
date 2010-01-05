/*
 *	This file is part of SharpMapMapViewer
 *  SharpMapMapViewer is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpMap.Data;

namespace MapViewer.DataSource
{
    public partial class ChooseDataSource : Form, ICreateDataProvider, ICreateRasterProvider
    {
        private static Int32 _lastProviderPick;

        public ChooseDataSource()
        {
            InitializeComponent();
            cbDataSource.SelectedIndex = _lastProviderPick;
        }

        private ICreateDataProvider DataBuilder
        {
            get
            {
                if (pContainer.Controls.Count == 0) 
                    return null;

                ICreateDataProvider cdp = pContainer.Controls[0] as ICreateDataProvider;
                if(cdp == null)
                    return null;

                return cdp;
            }
        }

        private ICreateRasterProvider RasterBuilder
        {
            get
            {
                if ( pContainer.Controls.Count == 0)
                    return null;

                ICreateRasterProvider crp = pContainer.Controls[0] as ICreateRasterProvider;
                if ( crp == null )
                    return null;

                return crp;
            }
        }

        public IFeatureProvider Provider { get; protected set; }
        public IEnumerable<IRasterProvider> RasterProviders { get; protected set; }

        #region ICreateDataProvider Members

        public IFeatureProvider GetProvider()
        {
            if (DataBuilder != null)
                return DataBuilder.GetProvider();
            return null;
        }

        #endregion

        private void cbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((string)cbDataSource.SelectedItem).ToLower())
            {
                case "shapefile":
                    LoadShapefileBuilder();
                    return;
                case "mssqlspatial":
                    LoadMsSqlSpatialBuilder();
                    return;
                case "mssqlserver2008":
                    LoadMsSqlServerBuilder();
                    return;
                case "spatiallite":
                    LoadSpatialLiteBuilder();
                    return;
                case "postgis":
                    LoadPostGisBuilder();
                    return;
                case "ibm db2 spatialextender":
                    LoadDB2SpatialExtender();
                    return;
                case "gdal for sharpmap":
                    LoadGdalForSharpMap();
                    return;
            }
        }

        private void LoadGdalForSharpMap()
        {
            LoadBuilder(new GdalMask());
        }

        private void LoadDB2SpatialExtender()
        {
            LoadBuilder(new DB2SpatialExtender());
        }

        private void LoadPostGisBuilder()
        {
            LoadBuilder(new PostGis());
        }

        private void LoadBuilder(Object builder)
        {
            if (!(builder is ICreateDataProvider ||
                 builder is ICreateRasterProvider))
                return;

            if (pContainer.Controls.Count != 0)
            {
                Control c = pContainer.Controls[0];
                pContainer.Controls.Remove(c);
                c.Dispose();
            }

            pContainer.Controls.Add((Control)builder);
            ((Control)builder).Dock = DockStyle.Fill;
        }

        private void LoadSpatialLiteBuilder()
        {
            LoadBuilder(new SpatialLite());
        }

        private void LoadMsSqlServerBuilder()
        {
            LoadBuilder(new SqlServer2008());
        }

        private void LoadMsSqlSpatialBuilder()
        {
            LoadBuilder(new MsSqlSpatial());
        }

        private void LoadShapefileBuilder()
        {
            LoadBuilder(new Shapefile());
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void bReturnDataSource_Click(object sender, EventArgs e)
        {
            IFeatureProvider prov = GetProvider();
            if (prov != null)
            {
                Provider = prov;
                DialogResult = DialogResult.OK;
                _lastProviderPick = cbDataSource.SelectedIndex;
                Close();
            }
            IEnumerable<IRasterProvider> providers = GetRasterProviders();
            if ( providers != null)
            {
                RasterProviders = providers;
                DialogResult = DialogResult.OK;
                _lastProviderPick = cbDataSource.SelectedIndex;
                Close();
            }
        }

        #region ICreateDataProvider Members

        public IEnumerable<IRasterProvider> GetRasterProviders()
        {
            if (RasterBuilder != null)
                return RasterBuilder.GetRasterProviders();
            return null;
        }

        public string ProviderName
        {
            get
            {
                if (DataBuilder != null)
                    return DataBuilder.ProviderName;

                return RasterBuilder.ProviderName;
            }
        }

        #endregion
    }
}
