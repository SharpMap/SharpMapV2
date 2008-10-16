namespace MapViewer
{
    partial class MapViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapViewer));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.bAddLayer = new System.Windows.Forms.ToolStripButton();
            this.clearLayersButton1 = new System.Windows.Forms.ToolStripButton();
            this.refreshMapToolBarButton = new System.Windows.Forms.ToolStripButton();
            this.zoomMapExtentsToolstripButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitVertical = new System.Windows.Forms.SplitContainer();
            this.LeftTabControl = new System.Windows.Forms.TabControl();
            this.layersTab = new System.Windows.Forms.TabPage();
            this.layersGrid = new System.Windows.Forms.DataGrid();
            this.layersContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLayersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.layerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomLayerExtentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSymbologyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageDataSource = new System.Windows.Forms.TabPage();
            this.stylesTab = new System.Windows.Forms.TabPage();
            this.splitHorizontal = new System.Windows.Forms.SplitContainer();
            this.resultsTabControl = new System.Windows.Forms.TabControl();
            this.mapViewControl1 = new SharpMap.Presentation.WinForms.MapViewControl();
            this.mapViewControlContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLayersToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullExtentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.customToolsToolstrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.standardToolsToolstrip = new System.Windows.Forms.ToolStrip();
            this.fixedZoomInButton = new System.Windows.Forms.ToolStripButton();
            this.fixedZoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.queryLayerComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.queryRectangleButton = new System.Windows.Forms.ToolStripButton();
            this.queryPolygonButton = new System.Windows.Forms.ToolStripButton();
            this.panButton = new System.Windows.Forms.ToolStripButton();
            this.zoomInButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.mainMenu.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitVertical.Panel1.SuspendLayout();
            this.splitVertical.Panel2.SuspendLayout();
            this.splitVertical.SuspendLayout();
            this.LeftTabControl.SuspendLayout();
            this.layersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layersGrid)).BeginInit();
            this.layersContextMenu.SuspendLayout();
            this.splitHorizontal.Panel1.SuspendLayout();
            this.splitHorizontal.Panel2.SuspendLayout();
            this.splitHorizontal.SuspendLayout();
            this.mapViewControlContextMenu.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.customToolsToolstrip.SuspendLayout();
            this.standardToolsToolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.toolStripMenuItem1});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1012, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem1,
            this.clearLayersToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(52, 20);
            this.toolStripMenuItem1.Text = "Layers";
            // 
            // addLayerToolStripMenuItem1
            // 
            this.addLayerToolStripMenuItem1.Name = "addLayerToolStripMenuItem1";
            this.addLayerToolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
            this.addLayerToolStripMenuItem1.Text = "Add Layer";
            // 
            // clearLayersToolStripMenuItem
            // 
            this.clearLayersToolStripMenuItem.Name = "clearLayersToolStripMenuItem";
            this.clearLayersToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.clearLayersToolStripMenuItem.Text = "Clear Layers";
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bAddLayer,
            this.clearLayersButton1,
            this.refreshMapToolBarButton,
            this.zoomMapExtentsToolstripButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(1012, 25);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // bAddLayer
            // 
            this.bAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bAddLayer.Image = ((System.Drawing.Image)(resources.GetObject("bAddLayer.Image")));
            this.bAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAddLayer.Name = "bAddLayer";
            this.bAddLayer.Size = new System.Drawing.Size(64, 22);
            this.bAddLayer.Text = "Add Layer";
            this.bAddLayer.ToolTipText = "Add Layer";
            // 
            // clearLayersButton1
            // 
            this.clearLayersButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.clearLayersButton1.Image = ((System.Drawing.Image)(resources.GetObject("clearLayersButton1.Image")));
            this.clearLayersButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearLayersButton1.Name = "clearLayersButton1";
            this.clearLayersButton1.Size = new System.Drawing.Size(74, 22);
            this.clearLayersButton1.Text = "Clear Layers";
            // 
            // refreshMapToolBarButton
            // 
            this.refreshMapToolBarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.refreshMapToolBarButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshMapToolBarButton.Image")));
            this.refreshMapToolBarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshMapToolBarButton.Name = "refreshMapToolBarButton";
            this.refreshMapToolBarButton.Size = new System.Drawing.Size(77, 22);
            this.refreshMapToolBarButton.Text = "Refresh Map";
            // 
            // zoomMapExtentsToolstripButton
            // 
            this.zoomMapExtentsToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomMapExtentsToolstripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomMapExtentsToolstripButton.Image")));
            this.zoomMapExtentsToolstripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomMapExtentsToolstripButton.Name = "zoomMapExtentsToolstripButton";
            this.zoomMapExtentsToolstripButton.Size = new System.Drawing.Size(100, 22);
            this.zoomMapExtentsToolstripButton.Text = "Zoom Full Extent";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 590);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1012, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // splitVertical
            // 
            this.splitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitVertical.Location = new System.Drawing.Point(0, 50);
            this.splitVertical.Name = "splitVertical";
            // 
            // splitVertical.Panel1
            // 
            this.splitVertical.Panel1.Controls.Add(this.LeftTabControl);
            // 
            // splitVertical.Panel2
            // 
            this.splitVertical.Panel2.Controls.Add(this.splitHorizontal);
            this.splitVertical.Size = new System.Drawing.Size(1012, 540);
            this.splitVertical.SplitterDistance = 337;
            this.splitVertical.TabIndex = 3;
            // 
            // LeftTabControl
            // 
            this.LeftTabControl.Controls.Add(this.layersTab);
            this.LeftTabControl.Controls.Add(this.tabPageDataSource);
            this.LeftTabControl.Controls.Add(this.stylesTab);
            this.LeftTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftTabControl.Location = new System.Drawing.Point(0, 0);
            this.LeftTabControl.Name = "LeftTabControl";
            this.LeftTabControl.SelectedIndex = 0;
            this.LeftTabControl.Size = new System.Drawing.Size(337, 540);
            this.LeftTabControl.TabIndex = 1;
            // 
            // layersTab
            // 
            this.layersTab.Controls.Add(this.layersGrid);
            this.layersTab.Location = new System.Drawing.Point(4, 22);
            this.layersTab.Name = "layersTab";
            this.layersTab.Padding = new System.Windows.Forms.Padding(3);
            this.layersTab.Size = new System.Drawing.Size(329, 514);
            this.layersTab.TabIndex = 0;
            this.layersTab.Text = "Layers";
            this.layersTab.UseVisualStyleBackColor = true;
            // 
            // layersGrid
            // 
            this.layersGrid.ContextMenuStrip = this.layersContextMenu;
            this.layersGrid.DataMember = "";
            this.layersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layersGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.layersGrid.Location = new System.Drawing.Point(3, 3);
            this.layersGrid.Name = "layersGrid";
            this.layersGrid.Size = new System.Drawing.Size(323, 508);
            this.layersGrid.TabIndex = 0;
            // 
            // layersContextMenu
            // 
            this.layersContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem,
            this.clearLayersToolStripMenuItem1,
            this.layerToolStripMenuItem});
            this.layersContextMenu.Name = "contextMenuStrip1";
            this.layersContextMenu.Size = new System.Drawing.Size(138, 70);
            // 
            // addLayerToolStripMenuItem
            // 
            this.addLayerToolStripMenuItem.Name = "addLayerToolStripMenuItem";
            this.addLayerToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.addLayerToolStripMenuItem.Text = "Add Layer";
            // 
            // clearLayersToolStripMenuItem1
            // 
            this.clearLayersToolStripMenuItem1.Name = "clearLayersToolStripMenuItem1";
            this.clearLayersToolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
            this.clearLayersToolStripMenuItem1.Text = "Clear Layers";
            // 
            // layerToolStripMenuItem
            // 
            this.layerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomLayerExtentToolStripMenuItem,
            this.editSymbologyToolStripMenuItem});
            this.layerToolStripMenuItem.Name = "layerToolStripMenuItem";
            this.layerToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.layerToolStripMenuItem.Text = "Layer";
            // 
            // zoomLayerExtentToolStripMenuItem
            // 
            this.zoomLayerExtentToolStripMenuItem.Name = "zoomLayerExtentToolStripMenuItem";
            this.zoomLayerExtentToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.zoomLayerExtentToolStripMenuItem.Text = "Zoom Layer Extent";
            // 
            // editSymbologyToolStripMenuItem
            // 
            this.editSymbologyToolStripMenuItem.Name = "editSymbologyToolStripMenuItem";
            this.editSymbologyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.editSymbologyToolStripMenuItem.Text = "Edit Symbology";
            // 
            // tabPageDataSource
            // 
            this.tabPageDataSource.Location = new System.Drawing.Point(4, 22);
            this.tabPageDataSource.Name = "tabPageDataSource";
            this.tabPageDataSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDataSource.Size = new System.Drawing.Size(305, 514);
            this.tabPageDataSource.TabIndex = 1;
            this.tabPageDataSource.Text = "Data Sources";
            this.tabPageDataSource.UseVisualStyleBackColor = true;
            // 
            // stylesTab
            // 
            this.stylesTab.Location = new System.Drawing.Point(4, 22);
            this.stylesTab.Name = "stylesTab";
            this.stylesTab.Padding = new System.Windows.Forms.Padding(3);
            this.stylesTab.Size = new System.Drawing.Size(305, 514);
            this.stylesTab.TabIndex = 2;
            this.stylesTab.Text = "Styles";
            this.stylesTab.UseVisualStyleBackColor = true;
            // 
            // splitHorizontal
            // 
            this.splitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitHorizontal.Name = "splitHorizontal";
            this.splitHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitHorizontal.Panel1
            // 
            this.splitHorizontal.Panel1.Controls.Add(this.resultsTabControl);
            // 
            // splitHorizontal.Panel2
            // 
            this.splitHorizontal.Panel2.Controls.Add(this.mapViewControl1);
            this.splitHorizontal.Size = new System.Drawing.Size(671, 540);
            this.splitHorizontal.SplitterDistance = 110;
            this.splitHorizontal.TabIndex = 1;
            // 
            // resultsTabControl
            // 
            this.resultsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsTabControl.Location = new System.Drawing.Point(0, 0);
            this.resultsTabControl.Name = "resultsTabControl";
            this.resultsTabControl.SelectedIndex = 0;
            this.resultsTabControl.Size = new System.Drawing.Size(671, 110);
            this.resultsTabControl.TabIndex = 0;
            // 
            // mapViewControl1
            // 
            this.mapViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapViewControl1.Location = new System.Drawing.Point(0, 0);
            this.mapViewControl1.Name = "mapViewControl1";
            this.mapViewControl1.Size = new System.Drawing.Size(671, 426);
            this.mapViewControl1.TabIndex = 0;
            this.mapViewControl1.Text = "mapViewControl1";
            this.mapViewControl1.Title = "mapViewControl1";
            // 
            // mapViewControlContextMenu
            // 
            this.mapViewControlContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layersToolStripMenuItem,
            this.refreshMapToolStripMenuItem,
            this.fullExtentToolStripMenuItem});
            this.mapViewControlContextMenu.Name = "mapViewControlContextMenu";
            this.mapViewControlContextMenu.Size = new System.Drawing.Size(141, 70);
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem2,
            this.clearLayersToolStripMenuItem2});
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.layersToolStripMenuItem.Text = "Layers";
            // 
            // addLayerToolStripMenuItem2
            // 
            this.addLayerToolStripMenuItem2.Name = "addLayerToolStripMenuItem2";
            this.addLayerToolStripMenuItem2.Size = new System.Drawing.Size(137, 22);
            this.addLayerToolStripMenuItem2.Text = "Add Layer";
            // 
            // clearLayersToolStripMenuItem2
            // 
            this.clearLayersToolStripMenuItem2.Name = "clearLayersToolStripMenuItem2";
            this.clearLayersToolStripMenuItem2.Size = new System.Drawing.Size(137, 22);
            this.clearLayersToolStripMenuItem2.Text = "Clear Layers";
            // 
            // refreshMapToolStripMenuItem
            // 
            this.refreshMapToolStripMenuItem.Name = "refreshMapToolStripMenuItem";
            this.refreshMapToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.refreshMapToolStripMenuItem.Text = "Refresh Map";
            // 
            // fullExtentToolStripMenuItem
            // 
            this.fullExtentToolStripMenuItem.Name = "fullExtentToolStripMenuItem";
            this.fullExtentToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.fullExtentToolStripMenuItem.Text = "Full Extent";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.standardToolsToolstrip);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.mainToolStrip);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.customToolsToolstrip);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1012, 26);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1012, 26);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // customToolsToolstrip
            // 
            this.customToolsToolstrip.Dock = System.Windows.Forms.DockStyle.None;
            this.customToolsToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2});
            this.customToolsToolstrip.Location = new System.Drawing.Point(812, 0);
            this.customToolsToolstrip.Name = "customToolsToolstrip";
            this.customToolsToolstrip.Size = new System.Drawing.Size(18, 25);
            this.customToolsToolstrip.TabIndex = 3;
            this.customToolsToolstrip.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // standardToolsToolstrip
            // 
            this.standardToolsToolstrip.Dock = System.Windows.Forms.DockStyle.None;
            this.standardToolsToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fixedZoomInButton,
            this.fixedZoomOutButton,
            this.toolStripSeparator1,
            this.queryLayerComboBox,
            this.queryRectangleButton,
            this.queryPolygonButton,
            this.panButton,
            this.zoomInButton,
            this.zoomOutButton});
            this.standardToolsToolstrip.Location = new System.Drawing.Point(321, 1);
            this.standardToolsToolstrip.Name = "standardToolsToolstrip";
            this.standardToolsToolstrip.Size = new System.Drawing.Size(631, 25);
            this.standardToolsToolstrip.TabIndex = 4;
            this.standardToolsToolstrip.Text = "toolStrip1";
            // 
            // fixedZoomInButton
            // 
            this.fixedZoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fixedZoomInButton.Image = ((System.Drawing.Image)(resources.GetObject("fixedZoomInButton.Image")));
            this.fixedZoomInButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fixedZoomInButton.Name = "fixedZoomInButton";
            this.fixedZoomInButton.Size = new System.Drawing.Size(86, 22);
            this.fixedZoomInButton.Text = "Fixed Zoom In";
            // 
            // fixedZoomOutButton
            // 
            this.fixedZoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fixedZoomOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fixedZoomOutButton.Name = "fixedZoomOutButton";
            this.fixedZoomOutButton.Size = new System.Drawing.Size(96, 22);
            this.fixedZoomOutButton.Text = "Fixed Zoom Out";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // queryLayerComboBox
            // 
            this.queryLayerComboBox.Name = "queryLayerComboBox";
            this.queryLayerComboBox.Size = new System.Drawing.Size(121, 25);
            // 
            // queryRectangleButton
            // 
            this.queryRectangleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.queryRectangleButton.Image = ((System.Drawing.Image)(resources.GetObject("queryRectangleButton.Image")));
            this.queryRectangleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.queryRectangleButton.Name = "queryRectangleButton";
            this.queryRectangleButton.Size = new System.Drawing.Size(65, 22);
            this.queryRectangleButton.Text = "Query Box";
            // 
            // queryPolygonButton
            // 
            this.queryPolygonButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.queryPolygonButton.Image = ((System.Drawing.Image)(resources.GetObject("queryPolygonButton.Image")));
            this.queryPolygonButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.queryPolygonButton.Name = "queryPolygonButton";
            this.queryPolygonButton.Size = new System.Drawing.Size(90, 22);
            this.queryPolygonButton.Text = "Query Polygon";
            // 
            // panButton
            // 
            this.panButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.panButton.Image = ((System.Drawing.Image)(resources.GetObject("panButton.Image")));
            this.panButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.panButton.Name = "panButton";
            this.panButton.Size = new System.Drawing.Size(31, 22);
            this.panButton.Text = "Pan";
            this.panButton.ToolTipText = "Pan";
            // 
            // zoomInButton
            // 
            this.zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomInButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInButton.Image")));
            this.zoomInButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(56, 22);
            this.zoomInButton.Text = "Zoom In";
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomOutButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutButton.Image")));
            this.zoomOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(66, 22);
            this.zoomOutButton.Text = "Zoom Out";
            // 
            // MapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 612);
            this.Controls.Add(this.splitVertical);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MapViewer";
            this.Text = "Map Viewer";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitVertical.Panel1.ResumeLayout(false);
            this.splitVertical.Panel2.ResumeLayout(false);
            this.splitVertical.ResumeLayout(false);
            this.LeftTabControl.ResumeLayout(false);
            this.layersTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layersGrid)).EndInit();
            this.layersContextMenu.ResumeLayout(false);
            this.splitHorizontal.Panel1.ResumeLayout(false);
            this.splitHorizontal.Panel2.ResumeLayout(false);
            this.splitHorizontal.ResumeLayout(false);
            this.mapViewControlContextMenu.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.customToolsToolstrip.ResumeLayout(false);
            this.customToolsToolstrip.PerformLayout();
            this.standardToolsToolstrip.ResumeLayout(false);
            this.standardToolsToolstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitVertical;
        private System.Windows.Forms.ToolStripButton bAddLayer;
        private SharpMap.Presentation.WinForms.MapViewControl mapViewControl1;
        private System.Windows.Forms.ContextMenuStrip layersContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clearLayersToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton clearLayersButton1;
        private System.Windows.Forms.ToolStripMenuItem clearLayersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripButton refreshMapToolBarButton;
        private System.Windows.Forms.ContextMenuStrip mapViewControlContextMenu;
        private System.Windows.Forms.ToolStripMenuItem layersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem clearLayersToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem refreshMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton zoomMapExtentsToolstripButton;
        private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomLayerExtentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSymbologyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullExtentToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip customToolsToolstrip;
        private System.Windows.Forms.SplitContainer splitHorizontal;
        private System.Windows.Forms.TabControl resultsTabControl;
        private System.Windows.Forms.TabControl LeftTabControl;
        private System.Windows.Forms.TabPage layersTab;
        private System.Windows.Forms.DataGrid layersGrid;
        private System.Windows.Forms.TabPage tabPageDataSource;
        private System.Windows.Forms.TabPage stylesTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStrip standardToolsToolstrip;
        private System.Windows.Forms.ToolStripButton fixedZoomInButton;
        private System.Windows.Forms.ToolStripButton fixedZoomOutButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox queryLayerComboBox;
        private System.Windows.Forms.ToolStripButton queryRectangleButton;
        private System.Windows.Forms.ToolStripButton queryPolygonButton;
        private System.Windows.Forms.ToolStripButton panButton;
        private System.Windows.Forms.ToolStripButton zoomInButton;
        private System.Windows.Forms.ToolStripButton zoomOutButton;

    }
}