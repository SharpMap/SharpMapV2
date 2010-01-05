namespace MapViewer.DataSource
{
    partial class GdalMask
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
            this.lblPath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboPreviewGenerator = new System.Windows.Forms.ComboBox();
            this.cd = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.chkTransparentColor = new System.Windows.Forms.CheckBox();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.bBrowse = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(15, 16);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(76, 13);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "Selected Files:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Preview Generator:";
            // 
            // cboPreviewGenerator
            // 
            this.cboPreviewGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPreviewGenerator.FormattingEnabled = true;
            this.cboPreviewGenerator.Location = new System.Drawing.Point(18, 266);
            this.cboPreviewGenerator.Name = "cboPreviewGenerator";
            this.cboPreviewGenerator.Size = new System.Drawing.Size(291, 21);
            this.cboPreviewGenerator.TabIndex = 4;
            // 
            // btnColor
            // 
            this.btnColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColor.BackColor = System.Drawing.Color.Black;
            this.btnColor.Location = new System.Drawing.Point(243, 293);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(66, 23);
            this.btnColor.TabIndex = 6;
            this.btnColor.UseVisualStyleBackColor = false;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // chkTransparentColor
            // 
            this.chkTransparentColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkTransparentColor.AutoSize = true;
            this.chkTransparentColor.Checked = true;
            this.chkTransparentColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTransparentColor.Location = new System.Drawing.Point(18, 297);
            this.chkTransparentColor.Name = "chkTransparentColor";
            this.chkTransparentColor.Size = new System.Drawing.Size(113, 17);
            this.chkTransparentColor.TabIndex = 7;
            this.chkTransparentColor.Text = "Transparent Color:";
            this.chkTransparentColor.UseVisualStyleBackColor = true;
            this.chkTransparentColor.CheckedChanged += new System.EventHandler(this.chkTransparentColor_CheckedChanged);
            // 
            // lvFiles
            // 
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.Location = new System.Drawing.Point(18, 32);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(291, 188);
            this.lvFiles.TabIndex = 8;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(243, 226);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(66, 23);
            this.bBrowse.TabIndex = 9;
            this.bBrowse.Text = "Browse";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // GdalMask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.chkTransparentColor);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.cboPreviewGenerator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPath);
            this.Name = "GdalMask";
            this.Size = new System.Drawing.Size(321, 329);
            this.Load += new System.EventHandler(this.GdalMask_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboPreviewGenerator;
        private System.Windows.Forms.ColorDialog cd;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.CheckBox chkTransparentColor;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}
