namespace MapViewer.DataSource
{
    partial class OgrMask
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.cboOgrDatasourceTypes = new System.Windows.Forms.ComboBox();
            this.lblOgrDataSourcType = new System.Windows.Forms.Label();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.lblDriver = new System.Windows.Forms.Label();
            this.cboDriver = new System.Windows.Forms.ComboBox();
            this.lstLayers = new System.Windows.Forms.ListView();
            this.lblLayers = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(13, 106);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(88, 13);
            this.lblConnectionString.TabIndex = 4;
            this.lblConnectionString.Text = "ConnectionString";
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConnectionString.Location = new System.Drawing.Point(16, 123);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(257, 20);
            this.tbConnectionString.TabIndex = 5;
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(279, 120);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(28, 23);
            this.bBrowse.TabIndex = 6;
            this.bBrowse.Text = "...";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // ofd
            // 
            this.ofd.Filter = "shapefiles|*.shp";
            // 
            // cboOgrDatasourceTypes
            // 
            this.cboOgrDatasourceTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOgrDatasourceTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOgrDatasourceTypes.FormattingEnabled = true;
            this.cboOgrDatasourceTypes.Location = new System.Drawing.Point(16, 33);
            this.cboOgrDatasourceTypes.Name = "cboOgrDatasourceTypes";
            this.cboOgrDatasourceTypes.Size = new System.Drawing.Size(291, 21);
            this.cboOgrDatasourceTypes.TabIndex = 1;
            this.cboOgrDatasourceTypes.SelectedIndexChanged += new System.EventHandler(this.cboOgrDatasourceTypes_SelectedIndexChanged);
            // 
            // lblOgrDataSourcType
            // 
            this.lblOgrDataSourcType.AutoSize = true;
            this.lblOgrDataSourcType.Location = new System.Drawing.Point(13, 17);
            this.lblOgrDataSourcType.Name = "lblOgrDataSourcType";
            this.lblOgrDataSourcType.Size = new System.Drawing.Size(99, 13);
            this.lblOgrDataSourcType.TabIndex = 0;
            this.lblOgrDataSourcType.Text = "Type of datasource";
            // 
            // lblDriver
            // 
            this.lblDriver.AutoSize = true;
            this.lblDriver.Location = new System.Drawing.Point(13, 57);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(84, 13);
            this.lblDriver.TabIndex = 2;
            this.lblDriver.Text = "Available drivers";
            // 
            // cboDriver
            // 
            this.cboDriver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDriver.FormattingEnabled = true;
            this.cboDriver.Location = new System.Drawing.Point(16, 73);
            this.cboDriver.Name = "cboDriver";
            this.cboDriver.Size = new System.Drawing.Size(291, 21);
            this.cboDriver.TabIndex = 3;
            // 
            // lstLayers
            // 
            this.lstLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLayers.FullRowSelect = true;
            this.lstLayers.HideSelection = false;
            this.lstLayers.Location = new System.Drawing.Point(16, 171);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(291, 146);
            this.lstLayers.TabIndex = 7;
            this.lstLayers.UseCompatibleStateImageBehavior = false;
            this.lstLayers.View = System.Windows.Forms.View.Details;
            // 
            // lblLayers
            // 
            this.lblLayers.AutoSize = true;
            this.lblLayers.Location = new System.Drawing.Point(13, 155);
            this.lblLayers.Name = "lblLayers";
            this.lblLayers.Size = new System.Drawing.Size(84, 13);
            this.lblLayers.TabIndex = 8;
            this.lblLayers.Text = "Available Layers";
            // 
            // OgrMask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLayers);
            this.Controls.Add(this.lstLayers);
            this.Controls.Add(this.lblDriver);
            this.Controls.Add(this.cboDriver);
            this.Controls.Add(this.lblOgrDataSourcType);
            this.Controls.Add(this.cboOgrDatasourceTypes);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.tbConnectionString);
            this.Controls.Add(this.lblConnectionString);
            this.Name = "OgrMask";
            this.Size = new System.Drawing.Size(321, 329);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.ComboBox cboOgrDatasourceTypes;
        private System.Windows.Forms.Label lblOgrDataSourcType;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.Label lblDriver;
        private System.Windows.Forms.ComboBox cboDriver;
        private System.Windows.Forms.ListView lstLayers;
        private System.Windows.Forms.Label lblLayers;
    }
}
