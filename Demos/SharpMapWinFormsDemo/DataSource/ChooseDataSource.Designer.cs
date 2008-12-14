namespace MapViewer.DataSource
{
    partial class ChooseDataSource
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbDataSource = new System.Windows.Forms.ComboBox();
            this.pContainer = new System.Windows.Forms.Panel();
            this.bReturnDataSource = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose Data Source Type";
            // 
            // cbDataSource
            // 
            this.cbDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataSource.FormattingEnabled = true;
            this.cbDataSource.Items.AddRange(new object[] {
            "Shapefile",
            "MsSqlSpatial",
            "MsSqlServer2008",
            "SpatialLite",
            "PostGIS",
            "IBM DB2 SpatialExtender"});
            this.cbDataSource.Location = new System.Drawing.Point(16, 30);
            this.cbDataSource.Name = "cbDataSource";
            this.cbDataSource.Size = new System.Drawing.Size(508, 21);
            this.cbDataSource.TabIndex = 1;
            this.cbDataSource.SelectedIndexChanged += new System.EventHandler(this.cbDataSource_SelectedIndexChanged);
            // 
            // pContainer
            // 
            this.pContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pContainer.AutoScroll = true;
            this.pContainer.Location = new System.Drawing.Point(16, 58);
            this.pContainer.Name = "pContainer";
            this.pContainer.Size = new System.Drawing.Size(508, 328);
            this.pContainer.TabIndex = 2;
            // 
            // bReturnDataSource
            // 
            this.bReturnDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bReturnDataSource.Location = new System.Drawing.Point(449, 392);
            this.bReturnDataSource.Name = "bReturnDataSource";
            this.bReturnDataSource.Size = new System.Drawing.Size(75, 23);
            this.bReturnDataSource.TabIndex = 3;
            this.bReturnDataSource.Text = "OK";
            this.bReturnDataSource.UseVisualStyleBackColor = true;
            this.bReturnDataSource.Click += new System.EventHandler(this.bReturnDataSource_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(16, 391);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // ChooseDataSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 425);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bReturnDataSource);
            this.Controls.Add(this.pContainer);
            this.Controls.Add(this.cbDataSource);
            this.Controls.Add(this.label1);
            this.Name = "ChooseDataSource";
            this.ShowInTaskbar = false;
            this.Text = "Choose Data Source";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDataSource;
        private System.Windows.Forms.Panel pContainer;
        private System.Windows.Forms.Button bReturnDataSource;
        private System.Windows.Forms.Button bCancel;
    }
}
