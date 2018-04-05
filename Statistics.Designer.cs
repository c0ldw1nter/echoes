namespace Echoes
{
    partial class Statistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Statistics));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.topTracksGrid = new System.Windows.Forms.DataGridView();
            this.num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listened = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.topArtistsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.topAlbumsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.searchBoxTrack = new System.Windows.Forms.TextBox();
            this.searchBoxArtist = new System.Windows.Forms.TextBox();
            this.searchBoxAlbum = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topTracksGrid)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topArtistsGrid)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topAlbumsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(619, 392);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.searchBoxTrack);
            this.tabPage1.Controls.Add(this.topTracksGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(611, 366);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Top tracks";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // topTracksGrid
            // 
            this.topTracksGrid.AllowUserToAddRows = false;
            this.topTracksGrid.AllowUserToDeleteRows = false;
            this.topTracksGrid.AllowUserToResizeRows = false;
            this.topTracksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.topTracksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.num,
            this.name,
            this.listened});
            this.topTracksGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.topTracksGrid.EnableHeadersVisualStyles = false;
            this.topTracksGrid.Location = new System.Drawing.Point(3, 29);
            this.topTracksGrid.Name = "topTracksGrid";
            this.topTracksGrid.RowHeadersVisible = false;
            this.topTracksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.topTracksGrid.Size = new System.Drawing.Size(605, 334);
            this.topTracksGrid.TabIndex = 0;
            this.topTracksGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.topArtistsGrid_CellFormatting);
            this.topTracksGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.topTracksGrid_CellMouseDoubleClick);
            // 
            // num
            // 
            this.num.DataPropertyName = "num";
            this.num.HeaderText = "#";
            this.num.Name = "num";
            this.num.ReadOnly = true;
            this.num.Width = 40;
            // 
            // name
            // 
            this.name.DataPropertyName = "name";
            this.name.HeaderText = "Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 300;
            // 
            // listened
            // 
            this.listened.DataPropertyName = "listened";
            this.listened.HeaderText = "Listened";
            this.listened.Name = "listened";
            this.listened.ReadOnly = true;
            this.listened.Width = 80;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.searchBoxArtist);
            this.tabPage2.Controls.Add(this.topArtistsGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(611, 366);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Top artists";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // topArtistsGrid
            // 
            this.topArtistsGrid.AllowUserToAddRows = false;
            this.topArtistsGrid.AllowUserToDeleteRows = false;
            this.topArtistsGrid.AllowUserToResizeRows = false;
            this.topArtistsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.topArtistsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.topArtistsGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.topArtistsGrid.EnableHeadersVisualStyles = false;
            this.topArtistsGrid.Location = new System.Drawing.Point(3, 29);
            this.topArtistsGrid.Name = "topArtistsGrid";
            this.topArtistsGrid.RowHeadersVisible = false;
            this.topArtistsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.topArtistsGrid.Size = new System.Drawing.Size(605, 334);
            this.topArtistsGrid.TabIndex = 1;
            this.topArtistsGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.topArtistsGrid_CellFormatting);
            this.topArtistsGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.topArtistsGrid_CellMouseDoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "num";
            this.dataGridViewTextBoxColumn1.HeaderText = "#";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "name";
            this.dataGridViewTextBoxColumn2.HeaderText = "Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 300;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "listened";
            this.dataGridViewTextBoxColumn3.HeaderText = "Listened";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.searchBoxAlbum);
            this.tabPage3.Controls.Add(this.topAlbumsGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(611, 366);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Top albums";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // topAlbumsGrid
            // 
            this.topAlbumsGrid.AllowUserToAddRows = false;
            this.topAlbumsGrid.AllowUserToDeleteRows = false;
            this.topAlbumsGrid.AllowUserToResizeRows = false;
            this.topAlbumsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.topAlbumsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.topAlbumsGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.topAlbumsGrid.EnableHeadersVisualStyles = false;
            this.topAlbumsGrid.Location = new System.Drawing.Point(3, 29);
            this.topAlbumsGrid.Name = "topAlbumsGrid";
            this.topAlbumsGrid.RowHeadersVisible = false;
            this.topAlbumsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.topAlbumsGrid.Size = new System.Drawing.Size(605, 334);
            this.topAlbumsGrid.TabIndex = 1;
            this.topAlbumsGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.topArtistsGrid_CellFormatting);
            this.topAlbumsGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.topAlbumsGrid_CellMouseDoubleClick);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "num";
            this.dataGridViewTextBoxColumn4.HeaderText = "#";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 40;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "name";
            this.dataGridViewTextBoxColumn5.HeaderText = "Name";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 300;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "listened";
            this.dataGridViewTextBoxColumn6.HeaderText = "Listened";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 80;
            // 
            // searchBoxTrack
            // 
            this.searchBoxTrack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBoxTrack.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchBoxTrack.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBoxTrack.Location = new System.Drawing.Point(3, 3);
            this.searchBoxTrack.Name = "searchBoxTrack";
            this.searchBoxTrack.Size = new System.Drawing.Size(605, 20);
            this.searchBoxTrack.TabIndex = 22;
            this.searchBoxTrack.TextChanged += new System.EventHandler(this.searchBoxTextChanged);
            // 
            // searchBoxArtist
            // 
            this.searchBoxArtist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBoxArtist.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchBoxArtist.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBoxArtist.Location = new System.Drawing.Point(3, 3);
            this.searchBoxArtist.Name = "searchBoxArtist";
            this.searchBoxArtist.Size = new System.Drawing.Size(605, 20);
            this.searchBoxArtist.TabIndex = 22;
            this.searchBoxArtist.TextChanged += new System.EventHandler(this.searchBoxTextChanged);
            // 
            // searchBoxAlbum
            // 
            this.searchBoxAlbum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBoxAlbum.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchBoxAlbum.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBoxAlbum.Location = new System.Drawing.Point(3, 3);
            this.searchBoxAlbum.Name = "searchBoxAlbum";
            this.searchBoxAlbum.Size = new System.Drawing.Size(605, 20);
            this.searchBoxAlbum.TabIndex = 23;
            this.searchBoxAlbum.TextChanged += new System.EventHandler(this.searchBoxTextChanged);
            // 
            // Statistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 392);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Statistics";
            this.Text = "Statistics";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topTracksGrid)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topArtistsGrid)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topAlbumsGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView topTracksGrid;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridViewTextBoxColumn num;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn listened;
        private System.Windows.Forms.DataGridView topArtistsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView topAlbumsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.TextBox searchBoxTrack;
        private System.Windows.Forms.TextBox searchBoxArtist;
        private System.Windows.Forms.TextBox searchBoxAlbum;
    }
}