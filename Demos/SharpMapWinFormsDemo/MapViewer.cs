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
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using MapViewer.Commands;
using NetTopologySuite.Coordinates;
using ProjNet.CoordinateSystems;
using SharpMap;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Styles;

namespace MapViewer
{
    public partial class MapViewer : Form
    {
        private readonly CommandManager _commandManager = new CommandManager();
        private readonly OpenFileDialog openFileDlg = new OpenFileDialog();

        private Map _map;

        public MapViewer()
        {
            InitializeComponent();
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

        public IGeometryFactory GeometryFactory { get; set; }

        public ICoordinateFactory CoordinateFactory { get; set; }

        public ICoordinateSequenceFactory CoordinateSequenceFactory { get; set; }

        public ICoordinateSystemFactory CoordinateSystemFactory { get; set; }

        private bool HasLayers
        {
            get { return Map.Layers.Count > 0; }
        }


        private void InitMap()
        {
            CoordinateFactory = new BufferedCoordinateFactory();

            CoordinateSequenceFactory = new BufferedCoordinateSequenceFactory(
                (BufferedCoordinateFactory) CoordinateFactory);

            GeometryFactory = new GeometryFactory<BufferedCoordinate>(
                (ICoordinateSequenceFactory<BufferedCoordinate>) CoordinateSequenceFactory);


            CoordinateSystemFactory = new CoordinateSystemFactory<BufferedCoordinate>(
                (ICoordinateFactory<BufferedCoordinate>) CoordinateFactory,
                (IGeometryFactory<BufferedCoordinate>) GeometryFactory);


            mapViewControl1.SuspendLayout();
            mapViewControl1.Size = splitVertical.Panel2.ClientSize;
            Map = new Map(GeometryFactory);
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

            Map.Layers.ListChanged +=
                delegate { mapLayersChangedCommand.FireCommand(); };
            CommandManager.AddCommand(mapLayersChangedCommand);

            CommandManager.AddCommandHandler(
                new ActionCommandHandler(mapLayersChangedCommand, delegate { EnableDisableCommandsRequiringLayers(); }));

            EnableDisableCommandsRequiringLayers();
        }

        private void EnableDisableCommandsRequiringLayers()
        {
            bool enable = HasLayers;
            CommandManager[CommandNames.ZoomFullExtent].Enabled = enable;
            CommandManager[CommandNames.ZoomLayerExtent].Enabled = enable;
            CommandManager[CommandNames.EditLayerSymbology].Enabled = enable;
            CommandManager[CommandNames.RefreshMap].Enabled = enable;
            CommandManager[CommandNames.ClearLayers].Enabled = enable;
        }

        private void ZoomMapExtent()
        {
            if (Map.Layers.Count > 0)
                mapViewControl1.ZoomToExtents();
        }

        private void ZoomLayerExtent(CommandEventArgs<ILayer> layerArgs)
        {
            if (layerArgs.Value != null)
                MapView.ZoomToWorldBounds((IExtents2D) layerArgs.Value.Extents);
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
            if (OpenFileDialog("Shapefiles|*.shp") == DialogResult.OK)
            {
                var shp = new ShapeFileProvider(openFileDlg.FileName, GeometryFactory, CoordinateSystemFactory);

                var lyr = new GeometryLayer(Guid.NewGuid().ToString(), shp);
                shp.Open(false);

                Map.Layers.Add(lyr);
                lyr.Style = new GeometryStyle();

                if (Map.Layers.Count == 1)
                {
                    mapViewControl1.Map = Map;
                    MapView.ZoomToExtents();
                }
                EnableDisableCommandsRequiringLayers();
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
            public const string MapLayersChanged = "MapLayersChanged";
            public const string RefreshMap = "RefreshMap";
            public const string ZoomFullExtent = "ZoomFullExtent";
            public const string ZoomLayerExtent = "ZoomLayerExtent";
        }

        #endregion
    }
}