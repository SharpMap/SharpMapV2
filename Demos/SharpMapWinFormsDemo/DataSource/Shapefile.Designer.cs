namespace MapViewer.DataSource
{
    partial class Shapefile
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.cbForceCoordinate = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbStrictness = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Shapefile path:";
            // 
            // tbPath
            // 
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.Location = new System.Drawing.Point(18, 33);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(219, 20);
            this.tbPath.TabIndex = 1;
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(243, 30);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(66, 23);
            this.bBrowse.TabIndex = 2;
            this.bBrowse.Text = "Browse";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "shapefiles|*.shp";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Force coordinate:";
            // 
            // cbForceCoordinate
            // 
            this.cbForceCoordinate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbForceCoordinate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbForceCoordinate.FormattingEnabled = true;
            this.cbForceCoordinate.Location = new System.Drawing.Point(18, 73);
            this.cbForceCoordinate.Name = "cbForceCoordinate";
            this.cbForceCoordinate.Size = new System.Drawing.Size(291, 21);
            this.cbForceCoordinate.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Strictness";
            // 
            // cbStrictness
            // 
            this.cbStrictness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStrictness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStrictness.FormattingEnabled = true;
            this.cbStrictness.Location = new System.Drawing.Point(18, 114);
            this.cbStrictness.Name = "cbStrictness";
            this.cbStrictness.Size = new System.Drawing.Size(291, 21);
            this.cbStrictness.TabIndex = 6;
            // 
            // Shapefile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbStrictness);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbForceCoordinate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.label1);
            this.Name = "Shapefile";
            this.Size = new System.Drawing.Size(321, 329);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbForceCoordinate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbStrictness;
    }
}
