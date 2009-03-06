namespace MapViewer.DataSource
{
    partial class PostGis
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
            this.cbDataBases = new System.Windows.Forms.ComboBox();
            this.grpLogin = new System.Windows.Forms.GroupBox();
            this.chkUsername = new System.Windows.Forms.CheckBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.tbUName = new System.Windows.Forms.TextBox();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.lbServer = new System.Windows.Forms.Label();
            this.lbDataBase = new System.Windows.Forms.Label();
            this.lbTable = new System.Windows.Forms.Label();
            this.cbTables = new System.Windows.Forms.ComboBox();
            this.dgvColumns = new System.Windows.Forms.DataGridView();
            this.lbColumns = new System.Windows.Forms.Label();
            this.lbDefinitionQuery = new System.Windows.Forms.Label();
            this.tbDefinitionQuery = new System.Windows.Forms.TextBox();
            this.lbSRID = new System.Windows.Forms.Label();
            this.grpLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).BeginInit();
            this.SuspendLayout();
            // 
            // cbDataBases
            // 
            this.cbDataBases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDataBases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataBases.FormattingEnabled = true;
            this.cbDataBases.Location = new System.Drawing.Point(134, 113);
            this.cbDataBases.Name = "cbDataBases";
            this.cbDataBases.Size = new System.Drawing.Size(224, 21);
            this.cbDataBases.TabIndex = 4;
            this.cbDataBases.DataSourceChanged += new System.EventHandler(this.cbDataBases_DataSourceChanged);
            this.cbDataBases.SelectedIndexChanged += new System.EventHandler(this.cbDataBases_SelectedIndexChanged);
            // 
            // grpLogin
            // 
            this.grpLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLogin.Controls.Add(this.chkUsername);
            this.grpLogin.Controls.Add(this.tbPassword);
            this.grpLogin.Controls.Add(this.lbPassword);
            this.grpLogin.Controls.Add(this.tbUName);
            this.grpLogin.Location = new System.Drawing.Point(14, 29);
            this.grpLogin.Name = "grpLogin";
            this.grpLogin.Size = new System.Drawing.Size(344, 78);
            this.grpLogin.TabIndex = 2;
            this.grpLogin.TabStop = false;
            this.grpLogin.Text = "Login Details on Postgres Database Server";
            // 
            // chkUsername
            // 
            this.chkUsername.AutoSize = true;
            this.chkUsername.Checked = true;
            this.chkUsername.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUsername.Location = new System.Drawing.Point(9, 19);
            this.chkUsername.Name = "chkUsername";
            this.chkUsername.Size = new System.Drawing.Size(74, 17);
            this.chkUsername.TabIndex = 0;
            this.chkUsername.Text = "Username";
            this.chkUsername.UseVisualStyleBackColor = true;
            this.chkUsername.CheckedChanged += new System.EventHandler(this.chkUsername_CheckedChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPassword.Location = new System.Drawing.Point(120, 42);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(218, 20);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(6, 45);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(53, 13);
            this.lbPassword.TabIndex = 2;
            this.lbPassword.Text = "Password";
            // 
            // tbUName
            // 
            this.tbUName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUName.Location = new System.Drawing.Point(120, 17);
            this.tbUName.Name = "tbUName";
            this.tbUName.Size = new System.Drawing.Size(218, 20);
            this.tbUName.TabIndex = 1;
            // 
            // tbServer
            // 
            this.tbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServer.Location = new System.Drawing.Point(134, 3);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(224, 20);
            this.tbServer.TabIndex = 1;
            this.tbServer.Text = "server=localhost;port=5432";
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(11, 6);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(38, 13);
            this.lbServer.TabIndex = 0;
            this.lbServer.Text = "Server";
            // 
            // lbDataBase
            // 
            this.lbDataBase.AutoSize = true;
            this.lbDataBase.Location = new System.Drawing.Point(11, 116);
            this.lbDataBase.Name = "lbDataBase";
            this.lbDataBase.Size = new System.Drawing.Size(53, 13);
            this.lbDataBase.TabIndex = 3;
            this.lbDataBase.Text = "Database";
            // 
            // lbTable
            // 
            this.lbTable.AutoSize = true;
            this.lbTable.Location = new System.Drawing.Point(11, 141);
            this.lbTable.Name = "lbTable";
            this.lbTable.Size = new System.Drawing.Size(34, 13);
            this.lbTable.TabIndex = 5;
            this.lbTable.Text = "Table";
            // 
            // cbTables
            // 
            this.cbTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTables.Enabled = false;
            this.cbTables.FormattingEnabled = true;
            this.cbTables.Location = new System.Drawing.Point(134, 138);
            this.cbTables.Name = "cbTables";
            this.cbTables.Size = new System.Drawing.Size(224, 21);
            this.cbTables.TabIndex = 6;
            this.cbTables.DataSourceChanged += new System.EventHandler(this.cbTables_DataSourceChanged);
            this.cbTables.SelectedIndexChanged += new System.EventHandler(this.cbTables_SelectedIndexChanged);
            // 
            // dgvColumns
            // 
            this.dgvColumns.AllowUserToAddRows = false;
            this.dgvColumns.AllowUserToDeleteRows = false;
            this.dgvColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumns.Enabled = false;
            this.dgvColumns.Location = new System.Drawing.Point(14, 211);
            this.dgvColumns.Name = "dgvColumns";
            this.dgvColumns.Size = new System.Drawing.Size(344, 97);
            this.dgvColumns.TabIndex = 9;
            this.dgvColumns.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvColumns_CellDoubleClick);
            // 
            // lbColumns
            // 
            this.lbColumns.AutoSize = true;
            this.lbColumns.Location = new System.Drawing.Point(11, 195);
            this.lbColumns.Name = "lbColumns";
            this.lbColumns.Size = new System.Drawing.Size(47, 13);
            this.lbColumns.TabIndex = 8;
            this.lbColumns.Text = "Columns";
            // 
            // lbDefinitionQuery
            // 
            this.lbDefinitionQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbDefinitionQuery.AutoSize = true;
            this.lbDefinitionQuery.Location = new System.Drawing.Point(11, 312);
            this.lbDefinitionQuery.Name = "lbDefinitionQuery";
            this.lbDefinitionQuery.Size = new System.Drawing.Size(82, 13);
            this.lbDefinitionQuery.TabIndex = 10;
            this.lbDefinitionQuery.Text = "Definition Query";
            // 
            // tbDefinitionQuery
            // 
            this.tbDefinitionQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDefinitionQuery.Enabled = false;
            this.tbDefinitionQuery.Location = new System.Drawing.Point(14, 327);
            this.tbDefinitionQuery.Multiline = true;
            this.tbDefinitionQuery.Name = "tbDefinitionQuery";
            this.tbDefinitionQuery.Size = new System.Drawing.Size(344, 59);
            this.tbDefinitionQuery.TabIndex = 11;
            // 
            // lbSRID
            // 
            this.lbSRID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSRID.AutoSize = true;
            this.lbSRID.Location = new System.Drawing.Point(11, 165);
            this.lbSRID.Name = "lbSRID";
            this.lbSRID.Size = new System.Drawing.Size(39, 13);
            this.lbSRID.TabIndex = 7;
            this.lbSRID.Text = "SRID: ";
            // 
            // PostGis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbSRID);
            this.Controls.Add(this.tbDefinitionQuery);
            this.Controls.Add(this.lbDefinitionQuery);
            this.Controls.Add(this.lbColumns);
            this.Controls.Add(this.dgvColumns);
            this.Controls.Add(this.cbTables);
            this.Controls.Add(this.lbTable);
            this.Controls.Add(this.lbDataBase);
            this.Controls.Add(this.tbServer);
            this.Controls.Add(this.lbServer);
            this.Controls.Add(this.grpLogin);
            this.Controls.Add(this.cbDataBases);
            this.Name = "PostGis";
            this.Size = new System.Drawing.Size(373, 389);
            this.grpLogin.ResumeLayout(false);
            this.grpLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDataBases;
        private System.Windows.Forms.GroupBox grpLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.TextBox tbUName;
        private System.Windows.Forms.CheckBox chkUsername;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label lbServer;
        private System.Windows.Forms.Label lbDataBase;
        private System.Windows.Forms.Label lbTable;
        private System.Windows.Forms.ComboBox cbTables;
        private System.Windows.Forms.DataGridView dgvColumns;
        private System.Windows.Forms.Label lbColumns;
        private System.Windows.Forms.Label lbDefinitionQuery;
        private System.Windows.Forms.TextBox tbDefinitionQuery;
        private System.Windows.Forms.Label lbSRID;
    }
}
