// Copyright 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DemoWinForm.Properties;
using SharpMap;
using SharpMap.Layers;
using SharpMap.Tools;

namespace DemoWinForm
{
    public partial class MainForm : Form
    {
        private readonly Dictionary<string, ILayerFactory> _layerFactoryCatalog
            = new Dictionary<string, ILayerFactory>();
        private readonly Map _map = new Map();

        public MainForm()
        {
            InitializeComponent();
            registerLayerFactories();
            mapViewControl1.Map = _map;
        }

        private void registerLayerFactories()
        {
            //ConfigurationManager.GetSection("LayerFactories");
            _layerFactoryCatalog[".shp"] = new ShapeFileLayerFactory();
            _layerFactoryCatalog["random"] = new RandomFeatureLayerFactory();
        }

        private void addLayer(ILayer layer)
        {
            _map.AddLayer(layer);
            //LayersDataGridView.Rows.Insert(0, true, getLayerTypeIcon(layer.GetType()), layer.LayerName);
        }

        private void loadLayer()
        {
            DialogResult result = AddLayerDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                foreach (string fileName in AddLayerDialog.FileNames)
                {
                    string extension = Path.GetExtension(fileName).ToLower();
                    ILayerFactory layerFactory;

                    if (!_layerFactoryCatalog.TryGetValue(extension, out layerFactory))
                    {
                        continue;
                    }

                    string layerName = Path.GetFileNameWithoutExtension(fileName);
                    ILayer layer = layerFactory.Create(layerName, fileName);
                    addLayer(layer);
                }

                changeUIOnLayerSelectionChange();
            }
        }

        private void removeLayer()
        {
            if (LayersDataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            string layerName = LayersDataGridView.SelectedRows[0].Cells[2].Value as string;

            ILayer layerToRemove = null;

            foreach (ILayer layer in _map.Layers)
            {
                if (layer.LayerName == layerName)
                {
                    layerToRemove = layer;
                    break;
                }
            }

            _map.Layers.Remove(layerToRemove);
            LayersDataGridView.Rows.Remove(LayersDataGridView.SelectedRows[0]);
        }

        private void changeMode(MapTool tool)
        {
            _map.ActiveTool = tool;

            ZoomInModeToolStripButton.Checked = (tool == StandardMapTools2D.ZoomIn);
            ZoomOutModeToolStripButton.Checked = (tool == StandardMapTools2D.ZoomOut);
            PanToolStripButton.Checked = (tool == StandardMapTools2D.Pan);
            QueryModeToolStripButton.Checked = (tool == StandardMapTools2D.Query);
        }

        private static object getLayerTypeIcon(Type type)
        {
            if (type == typeof(GeometryLayer))
            {
                return Resources.polygon;
            }

            return Resources.Raster;
        }

        private void changeUIOnLayerSelectionChange()
        {
            bool isLayerSelected = false;
            int layerIndex = -1;

            if (LayersDataGridView.SelectedRows.Count > 0)
            {
                isLayerSelected = true;
                layerIndex = LayersDataGridView.SelectedRows[0].Index;
            }

            RemoveLayerToolStripButton.Enabled = isLayerSelected;
            RemoveLayerToolStripMenuItem.Enabled = isLayerSelected;

            if (layerIndex < 0)
            {
                MoveUpToolStripMenuItem.Visible = false;
                MoveDownToolStripMenuItem.Visible = false;
                LayerContextMenuSeparator.Visible = false;
                return;
            }
            else
            {
                MoveUpToolStripMenuItem.Visible = true;
                MoveDownToolStripMenuItem.Visible = true;
                LayerContextMenuSeparator.Visible = true;
            }

            if (layerIndex == 0)
            {
                MoveUpToolStripMenuItem.Enabled = false;
            }
            else
            {
                MoveUpToolStripMenuItem.Enabled = true;
            }

            if (layerIndex == LayersDataGridView.Rows.Count - 1)
            {
                MoveDownToolStripMenuItem.Enabled = false;
            }
            else
            {
                MoveDownToolStripMenuItem.Enabled = true;
            }
        }

        private void AddLayerToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { loadLayer(); });
        }

        private void RemoveLayerToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { removeLayer(); });
        }

        private void AddLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { loadLayer(); });
        }

        private void RemoveLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { removeLayer(); });
        }

        private void ZoomToExtentsToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { zoomToExtents(); });
        }

        private void PanToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { changeMode(StandardMapTools2D.Pan); });
        }

        private void QueryModeToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { changeMode(StandardMapTools2D.Query); });
        }

        private void ZoomInModeToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { changeMode(StandardMapTools2D.ZoomIn); });
        }

        private void ZoomOutModeToolStripButton_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate() { changeMode(StandardMapTools2D.ZoomOut); });
        }

        private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LayersDataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            if (LayersDataGridView.SelectedRows[0].Index == 0)
            {
                return;
            }

            int rowIndex = LayersDataGridView.SelectedRows[0].Index;
            DataGridViewRow row = LayersDataGridView.Rows[rowIndex];
            LayersDataGridView.Rows.RemoveAt(rowIndex);
            LayersDataGridView.Rows.Insert(rowIndex - 1, row);

            int layerIndex = _map.Layers.Count - rowIndex - 1;
            ILayer layer = _map.Layers[layerIndex];
            _map.Layers.RemoveAt(layerIndex);
            _map.Layers.Insert(layerIndex + 1, layer);
        }

        private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LayersDataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            if (LayersDataGridView.SelectedRows[0].Index == LayersDataGridView.Rows.Count - 1)
            {
                return;
            }

            int rowIndex = LayersDataGridView.SelectedRows[0].Index;
            DataGridViewRow row = LayersDataGridView.Rows[rowIndex];
            LayersDataGridView.Rows.RemoveAt(rowIndex);
            LayersDataGridView.Rows.Insert(rowIndex + 1, row);

            int layerIndex = _map.Layers.Count - rowIndex - 1;
            ILayer layer = _map.Layers[layerIndex];
            _map.Layers.RemoveAt(layerIndex);
            _map.Layers.Insert(layerIndex - 1, layer);
        }

        private void LayersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate()
                                        {
                                            changeUIOnLayerSelectionChange();

                                            if (LayersDataGridView.SelectedRows.Count > 0)
                                            {
                                                _map.SelectLayer(LayersDataGridView.SelectedRows[0].Index);
                                            }
                                        });
        }

        //private void MainMapImage_MouseMove(GeoPoint WorldPos, MouseEventArgs ImagePos)
        //{
        //    CoordinatesLabel.Text = String.Format("Coordinates: {0:N5}, {1:N5}", WorldPos.X, WorldPos.Y);
        //}

        private void AddNewRandomGeometryLayer_Click(object sender, EventArgs e)
        {
            // TODO: implement random layer generation
            //BeginInvoke((MethodInvoker)delegate()
            //{
            //    string layerName;
            //    string connectionInfo;
            //    RandomFeatureLayerFactory.GetLayerNameAndInfo(out layerName, out connectionInfo);
            //    addLayer(_layerFactoryCatalog["random"].Create(layerName, connectionInfo));
            //});
        }

        private void zoomToExtents()
        {
            mapViewControl1.ZoomToExtents();
        }
    }
}