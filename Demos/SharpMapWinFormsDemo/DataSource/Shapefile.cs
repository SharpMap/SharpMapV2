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
using System.IO;
using System.Windows.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    public partial class Shapefile : UserControl, ICreateDataProvider
    {
        public Shapefile()
        {
            InitializeComponent();
        }

        #region ICreateDataProvider Members

        public IFeatureProvider GetProvider()
        {
            IGeometryServices svc = new GeometryServices();
            if (File.Exists(tbPath.Text))
                return new ShapeFileProvider(tbPath.Text, svc.DefaultGeometryFactory, svc.CoordinateSystemFactory, false)
                           {IsSpatiallyIndexed = false};
            return null;
        }

        #endregion

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                tbPath.Text = openFileDialog1.FileName;
        }

        #region ICreateDataProvider Members


        public string ProviderName
        {
            get { return Path.GetFileNameWithoutExtension(tbPath.Text); }
        }

        #endregion
    }
}