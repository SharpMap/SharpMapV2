namespace MapViewer.DataSource
{
    partial class MsSqlSpatial
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
            this.tbServer = new System.Windows.Forms.TextBox();
            this.grpAuth = new System.Windows.Forms.GroupBox();
            this.rbTrusted = new System.Windows.Forms.RadioButton();
            this.rbSqlServer = new System.Windows.Forms.RadioButton();
            this.grpLogin = new System.Windows.Forms.GroupBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTable = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbDataBases = new System.Windows.Forms.ComboBox();
            this.grpAuth.SuspendLayout();
            this.grpLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // tbServer
            // 
            this.tbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServer.Location = new System.Drawing.Point(134, 53);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(213, 20);
            this.tbServer.TabIndex = 1;
            this.tbServer.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // grpAuth
            // 
            this.grpAuth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAuth.Controls.Add(this.rbTrusted);
            this.grpAuth.Controls.Add(this.rbSqlServer);
            this.grpAuth.Location = new System.Drawing.Point(14, 79);
            this.grpAuth.Name = "grpAuth";
            this.grpAuth.Size = new System.Drawing.Size(333, 44);
            this.grpAuth.TabIndex = 5;
            this.grpAuth.TabStop = false;
            this.grpAuth.Text = "Authentication Mode";
            // 
            // rbTrusted
            // 
            this.rbTrusted.AutoSize = true;
            this.rbTrusted.Location = new System.Drawing.Point(120, 19);
            this.rbTrusted.Name = "rbTrusted";
            this.rbTrusted.Size = new System.Drawing.Size(113, 17);
            this.rbTrusted.TabIndex = 6;
            this.rbTrusted.Text = "trusted connection";
            this.rbTrusted.UseVisualStyleBackColor = true;
            this.rbTrusted.CheckedChanged += new System.EventHandler(this.rbTrusted_CheckedChanged);
            // 
            // rbSqlServer
            // 
            this.rbSqlServer.AutoSize = true;
            this.rbSqlServer.Checked = true;
            this.rbSqlServer.Location = new System.Drawing.Point(6, 19);
            this.rbSqlServer.Name = "rbSqlServer";
            this.rbSqlServer.Size = new System.Drawing.Size(71, 17);
            this.rbSqlServer.TabIndex = 5;
            this.rbSqlServer.TabStop = true;
            this.rbSqlServer.Text = "SqlServer";
            this.rbSqlServer.UseVisualStyleBackColor = true;
            this.rbSqlServer.CheckedChanged += new System.EventHandler(this.rbSqlServer_CheckedChanged);
            // 
            // grpLogin
            // 
            this.grpLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLogin.Controls.Add(this.tbPassword);
            this.grpLogin.Controls.Add(this.label3);
            this.grpLogin.Controls.Add(this.tbUName);
            this.grpLogin.Controls.Add(this.label2);
            this.grpLogin.Location = new System.Drawing.Point(14, 130);
            this.grpLogin.Name = "grpLogin";
            this.grpLogin.Size = new System.Drawing.Size(333, 67);
            this.grpLogin.TabIndex = 6;
            this.grpLogin.TabStop = false;
            this.grpLogin.Text = "Login Details";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 41);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(207, 20);
            this.tbPassword.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password";
            // 
            // tbUName
            // 
            this.tbUName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUName.Location = new System.Drawing.Point(120, 13);
            this.tbUName.Name = "tbUName";
            this.tbUName.Size = new System.Drawing.Size(207, 20);
            this.tbUName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "UName";
            // 
            // cbTable
            // 
            this.cbTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTable.FormattingEnabled = true;
            this.cbTable.Location = new System.Drawing.Point(14, 265);
            this.cbTable.Name = "cbTable";
            this.cbTable.Size = new System.Drawing.Size(327, 21);
            this.cbTable.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 249);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Table";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Database";
            // 
            // cbDataBases
            // 
            this.cbDataBases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDataBases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataBases.FormattingEnabled = true;
            this.cbDataBases.Location = new System.Drawing.Point(14, 216);
            this.cbDataBases.Name = "cbDataBases";
            this.cbDataBases.Size = new System.Drawing.Size(327, 21);
            this.cbDataBases.TabIndex = 7;
            this.cbDataBases.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // MsSqlSpatial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbDataBases);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbTable);
            this.Controls.Add(this.grpLogin);
            this.Controls.Add(this.grpAuth);
            this.Controls.Add(this.tbServer);
            this.Controls.Add(this.label1);
            this.Name = "MsSqlSpatial";
            this.Size = new System.Drawing.Size(362, 300);
            this.grpAuth.ResumeLayout(false);
            this.grpAuth.PerformLayout();
            this.grpLogin.ResumeLayout(false);
            this.grpLogin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.GroupBox grpAuth;
        private System.Windows.Forms.RadioButton rbTrusted;
        private System.Windows.Forms.RadioButton rbSqlServer;
        private System.Windows.Forms.GroupBox grpLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUName;
        private System.Windows.Forms.Label label2;
        protected System.Windows.Forms.ComboBox cbTable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        protected System.Windows.Forms.ComboBox cbDataBases;
    }
}
