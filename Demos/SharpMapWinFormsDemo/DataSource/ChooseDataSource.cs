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
using System.Windows.Forms;
using SharpMap.Data;

namespace MapViewer.DataSource
{
    public partial class ChooseDataSource : Form, ICreateDataProvider
    {
        public ChooseDataSource()
        {
            InitializeComponent();
        }

        private ICreateDataProvider Builder
        {
            get
            {
                if (pContainer.Controls.Count == 0) return null;

                return pContainer.Controls[0] as ICreateDataProvider;
            }
        }

        public IFeatureProvider Provider { get; protected set; }

        #region ICreateDataProvider Members

        public IFeatureProvider GetProvider()
        {
            if (Builder != null)
                return Builder.GetProvider();
            return null;
        }

        #endregion

        private void cbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((string) cbDataSource.SelectedItem).ToLower())
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
            }
        }

        private void LoadPostGisBuilder()
        {
            LoadBuilder(new PostGis());
        }

        private void LoadBuilder(ICreateDataProvider builder)
        {
            if (pContainer.Controls.Count != 0)
            {
                Control c = pContainer.Controls[0];
                pContainer.Controls.Remove(c);
                c.Dispose();
            }

            pContainer.Controls.Add((Control) builder);
            ((Control) builder).Dock = DockStyle.Fill;
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
                Close();
            }
        }
    }
}