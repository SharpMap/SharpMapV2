namespace MapViewer.Controls
{
    partial class StylesControl
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
            this.stylesTree1 = new MapViewer.Controls.StylesTree();
            this.SuspendLayout();
            // 
            // stylesTree1
            // 
            this.stylesTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stylesTree1.Location = new System.Drawing.Point(0, 0);
            this.stylesTree1.Name = "stylesTree1";
            this.stylesTree1.Size = new System.Drawing.Size(150, 150);
            this.stylesTree1.TabIndex = 0;
            // 
            // StylesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stylesTree1);
            this.Name = "StylesControl";
            this.ResumeLayout(false);

        }

        #endregion

        private StylesTree stylesTree1;
    }
}
