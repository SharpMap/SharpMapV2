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
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using MapViewer.Commands;
using MapViewer.DataSource;
using SharpMap;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Tools;
using SharpMap.Utilities;

namespace MapViewer
{
    public partial class MapViewer : Form
    {
        private readonly CommandManager _commandManager = new CommandManager();
        private readonly IGeometryServices GeometryServices = new GeometryServices();
        private readonly OpenFileDialog openFileDlg = new OpenFileDialog();
        private readonly WorkQueue workQueue;

        private Map _map;

        public MapViewer()
        {
            InitializeComponent();
            workQueue = new WorkQueue(this);
            workQueue.Working += workQueue_Working;
            InitMap();
            InitCommands();
        }

        public CommandManager CommandManager
        {
            get { return _commandManager; }
        }

        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                layersGrid.DataSource = value.Layers;
            }
        }

        public IMapView2D MapView
        {
            get { return mapViewControl1; }
        }


        private bool HasLayers
        {
            get { return Map.Layers.Count > 0; }
        }

        private void workQueue_Working(object sender, WorkQueueProcessEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateStatus), e.Status);
            else
                UpdateStatus(e.Status);
        }

        private void UpdateStatus(string p)
        {
            toolStripStatusLabel1.Text = p;
        }

        private void InitMap()
        {
            mapViewControl1.SuspendLayout();
            mapViewControl1.Size = splitVertical.Panel2.ClientSize;
            Map = new Map(GeometryServices.DefaultGeometryFactory);
            mapViewControl1.ResumeLayout(false);
        }

        private void InitCommands()
        {
            #region "ApplicationExit Command"

            var exitCommand = new Command(CommandNames.ApplicationExit);
            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    exitToolStripMenuItem, exitCommand));

            var exitHandler = new ExitCommandHandler(exitCommand);
            CommandManager.AddCommandHandler(exitHandler);

            #endregion

            #region"AddLayer Command"

            var addLayerCommand = new Command(CommandNames.AddLayer);
            var addLayerHandler = new ActionCommandHandler(addLayerCommand, AddLayer);
            CommandManager.AddCommand(addLayerCommand);
            CommandManager.AddCommandHandler(addLayerHandler);
            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    addLayerToolStripMenuItem, addLayerCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    addLayerToolStripMenuItem1, addLayerCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    addLayerToolStripMenuItem2, addLayerCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    bAddLayer, addLayerCommand));

            #endregion

            #region "ClearLayers Command"

            var clearLayersCommand = new Command(CommandNames.ClearLayers);
            var clearLayersHandler = new ActionCommandHandler(clearLayersCommand, ClearLayers);
            CommandManager.AddCommandHandler(clearLayersHandler);

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    clearLayersToolStripMenuItem, clearLayersCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    clearLayersToolStripMenuItem1, clearLayersCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    clearLayersButton1, clearLayersCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    clearLayersToolStripMenuItem2, clearLayersCommand));

            #endregion

            #region "Refresh map command"

            ICommand refreshMapCommand = new Command(CommandNames.RefreshMap);
            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    refreshMapToolBarButton, refreshMapCommand));

            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    refreshMapToolStripMenuItem, refreshMapCommand));

            CommandManager.AddCommandHandler(
                new ActionCommandHandler(refreshMapCommand, delegate { mapViewControl1.Refresh(); }));

            #endregion

            #region "Zoom map extent command"

            ICommand zoomMapExtentCommand = new Command(CommandNames.ZoomFullExtent);
            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    zoomMapExtentsToolstripButton, zoomMapExtentCommand));

            CommandManager.AddCommandHandler(
                new ActionCommandHandler(zoomMapExtentCommand, ZoomMapExtent));

            #endregion

            #region "Zoom Layer Extent"

            ICommand<CommandEventArgs<ILayer>> zoomLayerCommand
                = new Command<CommandEventArgs<ILayer>>(CommandNames.ZoomLayerExtent);

            ICommandComponentSource<ToolStripItem, CommandEventArgs<ILayer>> zoomLayerExtentToolStripMenuItemSource
                = new ToolStripItemCommandSource<ToolStripItem, CommandEventArgs<ILayer>>(
                    zoomLayerExtentToolStripMenuItem, zoomLayerCommand);

            CommandManager.AddCommandSource(zoomLayerExtentToolStripMenuItemSource);
            ICommandHandler<CommandEventArgs<ILayer>> zoomLayerHandler
                = new ActionCommandHandler<CommandEventArgs<ILayer>>(
                    zoomLayerCommand, ZoomLayerExtent);
            CommandManager.AddCommandHandler(zoomLayerHandler);

            #endregion

            #region "Edit Layer Symbology"

            ICommand<CommandEventArgs<ILayer>> editLayerSymbology
                = new Command<CommandEventArgs<ILayer>>(CommandNames.EditLayerSymbology);

            ICommandComponentSource<ToolStripItem, CommandEventArgs<ILayer>> editLayerSymbologySource
                = new ToolStripItemCommandSource<ToolStripItem, CommandEventArgs<ILayer>>(
                    editSymbologyToolStripMenuItem, editLayerSymbology);
            CommandManager.AddCommandSource(editLayerSymbologySource);


            CommandManager.AddCommandHandler(new ActionCommandHandler<CommandEventArgs<ILayer>>(editLayerSymbology,
                                                                                                EditLayerSymbology));

            #endregion

            ICommand mapLayersChangedCommand = new Command(CommandNames.MapLayersChanged);


            Map.Layers.ListChanged += delegate { mapLayersChangedCommand.FireCommand(); };

            CommandManager.AddCommand(mapLayersChangedCommand);

            CommandManager.AddCommandHandler(
                new ActionCommandHandler(mapLayersChangedCommand, EnableDisableCommandsRequiringLayers));

            #region fixed zoom in /out

            ICommand fixedZoomInCommand = new Command(CommandNames.FixedZoomIn);
            CommandManager.AddCommand(fixedZoomInCommand);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(fixedZoomInButton,
                                                                                            fixedZoomInCommand));
            CommandManager.AddCommandHandler(new ActionCommandHandler(fixedZoomInCommand, FixedZoomIn));


            ICommand fixedZoomOutCommand = new Command(CommandNames.FixedZoomOut);
            CommandManager.AddCommand(fixedZoomInCommand);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(fixedZoomOutButton,
                                                                                            fixedZoomOutCommand));
            CommandManager.AddCommandHandler(new ActionCommandHandler(fixedZoomOutCommand, FixedZoomOut));

            #endregion

            ICommand enablePan = new Command(CommandNames.EnablePan);
            CommandManager.AddCommand(enablePan);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(panButton, enablePan));
            CommandManager.AddCommandHandler(new ActionCommandHandler(enablePan, EnablePan));

            ICommand zoomInMouse = new Command(CommandNames.ZoomInMouse);
            CommandManager.AddCommand(zoomInMouse);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(zoomInButton, zoomInMouse));
            CommandManager.AddCommandHandler(new ActionCommandHandler(zoomInMouse, EnableZoomInMouse));


            ICommand zoomOutMouse = new Command(CommandNames.ZoomOutMouse);
            CommandManager.AddCommand(zoomOutMouse);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(zoomOutButton, zoomOutMouse));
            CommandManager.AddCommandHandler(new ActionCommandHandler(zoomOutMouse, EnableZoomOutMouse));


            EnableDisableCommandsRequiringLayers();
        }

        private void EnableZoomOutMouse()
        {
            Map.ActiveTool = StandardMapView2DMapTools.ZoomOut;
        }

        private void EnableZoomInMouse()
        {
            Map.ActiveTool = StandardMapView2DMapTools.ZoomIn;
        }

        private void EnablePan()
        {
            Map.ActiveTool = StandardMapView2DMapTools.Pan;
        }

        private void FixedZoomOut()
        {
            Zoom(1.2);
        }

        private void FixedZoomIn()
        {
            Zoom(0.8);
        }

        private void Zoom(double amount)
        {
            var ext = (IExtents2D)MapView.ViewEnvelope.Clone();
            double dx, dy;
            dx = ext.Width * amount / 2;
            dy = ext.Height * amount / 2;
            var c = (ICoordinate2D)ext.Center;

            MapView.ZoomToWorldBounds(ext.Factory.CreateExtents2D(c.X - dx, c.Y - dy, c.X + dx, c.Y + dy));
        }

        private void InvokeIfRequired(Delegate dlgt, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(dlgt, args);
                return;
            }
            dlgt.DynamicInvoke();
        }

        private void EnableDisableCommandsRequiringLayers()
        {
            bool enable = HasLayers;
            CommandManager[CommandNames.ZoomFullExtent].Enabled = enable;
            CommandManager[CommandNames.ZoomLayerExtent].Enabled = enable;
            CommandManager[CommandNames.EditLayerSymbology].Enabled = enable;
            CommandManager[CommandNames.RefreshMap].Enabled = enable;
            CommandManager[CommandNames.ClearLayers].Enabled = enable;
            CommandManager[CommandNames.FixedZoomIn].Enabled = enable;
            CommandManager[CommandNames.FixedZoomOut].Enabled = enable;
            CommandManager[CommandNames.EnablePan].Enabled = enable;
            CommandManager[CommandNames.ZoomInMouse].Enabled = enable;
            CommandManager[CommandNames.ZoomOutMouse].Enabled = enable;
        }

        private void ZoomMapExtent()
        {
            if (Map.Layers.Count > 0)
                mapViewControl1.ZoomToExtents();
        }

        private void ZoomLayerExtent(CommandEventArgs<ILayer> layerArgs)
        {
            if (layerArgs.Value != null)
                MapView.ZoomToWorldBounds((IExtents2D)layerArgs.Value.Extents);
        }

        private void EditLayerSymbology(CommandEventArgs<ILayer> layerArgs)
        {
        }

        private void ClearLayers()
        {
            foreach (ILayer l in Map.Layers)
                if (l.DataSource.IsOpen)
                    l.DataSource.Close();
            Map.Layers.Clear();
        }


        private void AddLayer()
        {
            var choose = new ChooseDataSource();
            if (choose.ShowDialog() == DialogResult.OK)
            {
                IFeatureProvider prov = choose.Provider;


                workQueue.AddWorkItem(
                    string.Format("Loading Datasource {0}", prov),
                    delegate
                    {
                        var lyr =
                            new GeometryLayer(
                                prov.ToString(),
                                prov);

                        prov.Open();

                        InvokeIfRequired(new Action(delegate
                                                        {
                                                            Map.Layers.Insert(0, lyr);

                                                            lyr.Style =
                                                                RandomStyle.RandomGeometryStyle();

                                                            if (Map.Layers.Count == 1)
                                                            {
                                                                mapViewControl1.Map = Map;
                                                                MapView.ZoomToExtents();
                                                            }
                                                        }));
                    }, delegate { EnableDisableCommandsRequiringLayers(); });
            }
        }


        private DialogResult OpenFileDialog(string filter)
        {
            openFileDlg.Filter = filter;
            return openFileDlg.ShowDialog();
        }

        #region Nested type: CommandNames

        public static class CommandNames
        {
            public const string AddLayer = "AddLayer";
            public const string ApplicationExit = "AppExit";
            public const string ClearLayers = "ClearLayers";
            public const string EditLayerSymbology = "EditLayerSymbology";
            public const string EnablePan = "EnablePan";
            public const string FixedZoomIn = "FixedZoomIn";
            public const string FixedZoomOut = "FixedZoomOut";
            public const string MapLayersChanged = "MapLayersChanged";
            public const string RefreshMap = "RefreshMap";
            public const string ZoomFullExtent = "ZoomFullExtent";
            public const string ZoomInMouse = "ZoomInMouse";
            public const string ZoomLayerExtent = "ZoomLayerExtent";
            public const string ZoomOutMouse = "ZoomOutMouse";
        }

        #endregion
    }
}
