/*
 *	This file is part of SharpMap.MapViewer
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
using System.ComponentModel;
using System.Windows.Forms;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using MapViewer.Commands;
using MapViewer.Controls;
using MapViewer.DataSource;
using MapViewer.MapActionHandler;
using MapViewer.SLD;
using SharpMap;
using SharpMap.Data;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else

#endif

namespace MapViewer
{
    using System.Configuration;
    using System.Diagnostics;
    using GeoAPI.CoordinateSystems;
    using GeoAPI.CoordinateSystems.Transformations;
    using SharpMap.Data.Providers;

    public partial class MapViewerForm : Form
    {
        private readonly CommandManager _commandManager = new CommandManager();

        private readonly BindingList<KeyValuePair<string, GeometryStyle>> _loadedStyles =
            new BindingList<KeyValuePair<string, GeometryStyle>>();

        private readonly IMapActionHandler AttributeQueryHandler = new MapActionHandler.MapActionHandler();
        private readonly IGeometryServices GeometryServices = new GeometryServices();
        private readonly WorkQueue workQueue;


        static MapViewerForm()
        {
            //jd: hopefully a temporary measure
            SridMap.DefaultInstance =
                new SridMap(new[] {new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});
        }

        public MapViewerForm()
        {
            InitializeComponent();
            workQueue = new WorkQueue(this);
            workQueue.Working += workQueue_Working;
            InitMap();
            InitCommands();
            AttributeQueryHandler.End += AttributeQueryHandler_End;
            AttributeQueryHandler.Begin += AttributeQueryHandler_Begin;
            layersView1.LayersContextMenu = layerContextMenu;
            layersView1.ContextMenuStrip = layersContextMenu;
            stylesControl1.Styles = _loadedStyles;
        }

        private IMapActionHandler MapActionHandler { get; set; }

        public CommandManager CommandManager
        {
            get { return _commandManager; }
        }

        public Map Map { get; set; }

        public ILayersView LayersView
        {
            get { return layersView1; }
        }

        public IMapView2D MapView
        {
            get { return mapViewControl1; }
        }


        private bool HasLayers
        {
            get { return Map.Layers.Count > 0; }
        }

        private void AttributeQueryHandler_Begin(object sender, MapActionHandlerEventArgs e)
        {
        }

        //Note: currently the selected features are one select behind.
        //e.g on drawing the first region all the layers features are returned
        // on drawing the second region all the features matching the first region are returned etc..
        private void AttributeQueryHandler_End(object sender, MapActionHandlerEventArgs e)
        {
            IFeatureLayer l =
                Enumerable.FirstOrDefault(
                    Caster.Cast<IFeatureLayer>(
                        Processor.Where(Map.SelectedLayers, delegate(ILayer o) { return o as IFeatureLayer != null; })));

            if (l != null)
            {
                FeatureDataView dv = new FeatureDataView(l.SelectedFeatures.Table);

                if (l.SelectedFeatures.AttributeFilter != null)
                    dv.AttributeFilter =
                        (AttributeBinaryExpression)
                        l.SelectedFeatures.AttributeFilter.Clone();

                if (l.SelectedFeatures.SpatialFilter != null)
                    dv.SpatialFilter =
                        (SpatialBinaryExpression)
                        l.SelectedFeatures.SpatialFilter.Clone();

                if (l.SelectedFeatures.OidFilter != null)
                    dv.OidFilter =
                        (OidCollectionExpression)
                        l.SelectedFeatures.OidFilter.Clone();

                if (l.SelectedFeatures.ViewDefinition != null)
                    dv.ViewDefinition =
                        (FeatureQueryExpression)
                        l.SelectedFeatures.ViewDefinition.Clone();


                QueryResultsTab tab = new QueryResultsTab(l.LayerName, dv);
                resultsTabControl.TabPages.Insert(0, tab);
                resultsTabControl.SelectedTab = tab;
            }
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
            Map = new Map(GeometryServices.DefaultGeometryFactory, GeometryServices.CoordinateTransformationFactory);
            mapViewControl1.ResumeLayout(false);
            MapView.BeginAction += MapView_BeginAction;
            MapView.EndAction += MapView_EndAction;
            MapView.Hover += MapView_Hover;
            Map.Layers.ListChanged += Layers_ListChanged;
            queryLayerComboBox.SelectedIndexChanged += queryLayerComboBox_SelectedIndexChanged;
        }

        private void queryLayerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Map.DeselectLayers(Map.SelectedLayers);

            if (queryLayerComboBox.SelectedIndex > -1)
                Map.SelectLayer((string) queryLayerComboBox.SelectedItem);
        }

        private void Layers_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                HandleMapLayerAdded(Map.Layers[e.NewIndex]);
            }
        }

        private void HandleMapLayerAdded(ILayer iLayer)
        {
            queryLayerComboBox.Items.Add(iLayer.LayerName);
        }


        private void MapView_Hover(object sender, MapActionEventArgs<Point2D> e)
        {
            if (MapActionHandler != null)
                MapActionHandler.HoverPoint = e.ActionPoint;
        }

        private void MapView_EndAction(object sender, MapActionEventArgs<Point2D> e)
        {
            if (MapActionHandler != null)
                MapActionHandler.EndPoint = e.ActionPoint;
        }

        private void MapView_BeginAction(object sender, MapActionEventArgs<Point2D> e)
        {
            if (MapActionHandler != null)
                MapActionHandler.BeginPoint = e.ActionPoint;
        }

        private void InitCommands()
        {
            #region "ApplicationExit Command"

            Command exitCommand = new Command(CommandNames.ApplicationExit);
            CommandManager.AddCommandSource(
                new ToolStripItemCommandSource<ToolStripItem>(
                    exitToolStripMenuItem, exitCommand));

            ExitCommandHandler exitHandler = new ExitCommandHandler(exitCommand);
            CommandManager.AddCommandHandler(exitHandler);

            #endregion

            #region"AddLayer Command"

            Command addLayerCommand = new Command(CommandNames.AddLayer);
            ActionCommandHandler addLayerHandler = new ActionCommandHandler(addLayerCommand, AddLayer);
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

            Command clearLayersCommand = new Command(CommandNames.ClearLayers);
            ActionCommandHandler clearLayersHandler = new ActionCommandHandler(clearLayersCommand, ClearLayers);
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

            #region mouse pan and zoom

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

            #endregion

            #region Query

            ICommand queryAdd = new Command(CommandNames.QueryAdd);

            CommandManager.AddCommand(queryAdd);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(queryAddButton,
                                                                                            queryAdd));
            CommandManager.AddCommandHandler(new ActionCommandHandler(queryAdd, EnableQueryAdd));

            ICommand queryRemove = new Command(CommandNames.QueryRemove);
            CommandManager.AddCommand(queryRemove);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripButton>(queryRemoveButton,
                                                                                            queryRemove));
            CommandManager.AddCommandHandler(new ActionCommandHandler(queryRemove, EnableQueryRemove));

            #endregion

            ICommand addStyle = new Command(CommandNames.AddStyle);
            CommandManager.AddCommand(addStyle);
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripMenuItem>(addStyleMenuStripItem,
                                                                                              addStyle));
            CommandManager.AddCommandSource(new ToolStripItemCommandSource<ToolStripMenuItem>(
                                                addStylesToolStripMenuItem,
                                                addStyle));
            CommandManager.AddCommandHandler(new ActionCommandHandler(addStyle, AddStyle));


            EnableDisableCommandsRequiringLayers();
        }

        private void AddStyle()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Styled Layer Descriptor Files|*.sld|Xml Documents|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    IDictionary<string, GeometryStyle> dict
                        = new SldConverter().ParseFeatureStyleFromFile(dlg.FileName);

                    foreach (KeyValuePair<string, GeometryStyle> pair in dict)
                        _loadedStyles.Add(pair);
                }
            }
        }


        private void EnableQueryRemove()
        {
            Map.ActiveTool = StandardMapView2DMapTools.QueryRemove;
            MapActionHandler = null;
        }

        private void EnableQueryAdd()
        {
            Map.ActiveTool = StandardMapView2DMapTools.Query;
            MapActionHandler = AttributeQueryHandler;
        }

        private void EnableZoomOutMouse()
        {
            Map.ActiveTool = StandardMapView2DMapTools.ZoomOut;
            MapActionHandler = null;
        }

        private void EnableZoomInMouse()
        {
            Map.ActiveTool = StandardMapView2DMapTools.ZoomIn;
            MapActionHandler = null;
        }

        private void EnablePan()
        {
            Map.ActiveTool = StandardMapView2DMapTools.Pan;
            MapActionHandler = null;
        }

        private void FixedZoomOut()
        {
            Zoom(1.2);
        }

        private void FixedZoomIn()
        {
            Zoom(1/1.2);
        }

        private void Zoom(double amount)
        {
            IExtents2D ext = (IExtents2D) MapView.ViewEnvelope.Clone();
            double dx, dy;
            dx = ext.Width*amount/2;
            dy = ext.Height*amount/2;
            ICoordinate2D c = (ICoordinate2D) ext.Center;

            MapView.ZoomToWorldBounds(ext.Factory.CreateExtents2D(c.X - dx, c.Y - dy, c.X + dx, c.Y + dy));
        }

        private void InvokeIfRequired(Delegate dlgt, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(dlgt, args);
                return;
            }
            dlgt.DynamicInvoke(args);
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
            CommandManager[CommandNames.QueryAdd].Enabled = enable;
            CommandManager[CommandNames.QueryRemove].Enabled = enable;
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
            while (Map.Layers.Count > 0)
            {
                ILayer l = Map.Layers[0];
                if (l.DataSource.IsOpen)
                    l.DataSource.Close();
                Map.Layers.Remove(l);
                l.DataSource.Dispose();
                l.Dispose();
            }
            Map.Layers.Clear();
        }


        private void AddLayer()
        {
            using (ChooseDataSource choose = new ChooseDataSource())
            {
                if (choose.ShowDialog() == DialogResult.OK)
                {
                    IFeatureProvider prov = choose.Provider;
                    string name = choose.ProviderName;
                    GeometryLayer layer = new GeometryLayer(name, prov);
                    AddLayer(layer);
                }
            }
        }

        private void AddLayer(IFeatureLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            this.workQueue.AddWorkItem(
                string.Format("Loading Datasource {0}", layer.LayerName),
                delegate
                {
                    layer.Features.IsSpatiallyIndexed = true;

                    layer.DataSource.Open();
                    this.InvokeIfRequired(new Action(delegate
                    {
                        if (this.Map.Layers.Count == 0)
                        {
                            if (layer.SpatialReference != null)
                                this.Map.SpatialReference = layer.SpatialReference;
                        }

                        this.Map.Layers.Insert(0, layer);

                        layer.Style = RandomStyle.RandomGeometryStyle();
                        
                        if (this.Map.Layers.Count != 1)
                            return;

                        this.mapViewControl1.Map = this.Map;
                        this.layersView1.Map = this.Map;
                        this.MapView.ZoomToExtents();
                    }));
                }, this.EnableDisableCommandsRequiringLayers,
                delegate(Exception ex)
                {
                    Trace.WriteLine(ex);
                    MessageBox.Show(string.Format("An error occured\n{0}\n{1}", ex.Message, ex.StackTrace));
                });
        }

        #region Nested type: CommandNames

        public static class CommandNames
        {
            public const string AddLayer = "AddLayer";
            public const string AddStyle = "AddStyle";
            public const string ApplicationExit = "AppExit";
            public const string ClearLayers = "ClearLayers";
            public const string EditLayerSymbology = "EditLayerSymbology";
            public const string EnablePan = "EnablePan";
            public const string FixedZoomIn = "FixedZoomIn";
            public const string FixedZoomOut = "FixedZoomOut";
            public const string MapLayersChanged = "MapLayersChanged";
            public const string QueryAdd = "QueryAdd";
            public const string QueryRemove = "QueryRemove";
            public const string RefreshMap = "RefreshMap";
            public const string ZoomFullExtent = "ZoomFullExtent";
            public const string ZoomInMouse = "ZoomInMouse";
            public const string ZoomLayerExtent = "ZoomLayerExtent";
            public const string ZoomOutMouse = "ZoomOutMouse";
        }

        #endregion

        private void SampleMapToolStripMenuItemClick(object sender, EventArgs e)
        {
            IEnumerable<IFeatureLayer> enumerable = MapHelper.CreateSqlLayers();
            foreach (IFeatureLayer layer in enumerable)
                this.AddLayer(layer);
        }

        private void PostGisMapToolStripMenuItemClick(object sender, EventArgs e)
        {
            IEnumerable<IFeatureLayer> enumerable = MapHelper.CreatePgisLayers();
            foreach (IFeatureLayer layer in enumerable)
                this.AddLayer(layer);
        }
    }
}