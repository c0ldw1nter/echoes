namespace Echoes
{
    partial class DupeFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DupeFinder));
            this.dupeFinderWorker = new System.ComponentModel.BackgroundWorker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonStart = new System.Windows.Forms.Button();
            this.radioHash = new System.Windows.Forms.RadioButton();
            this.radioTitle = new System.Windows.Forms.RadioButton();
            this.radioFilepath = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.modifiedProgressBarLoading1 = new ModifiedControls.ModifiedProgressBarLoading();
            this.checkboxes = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bitrate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listened = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dupeFinderWorker
            // 
            this.dupeFinderWorker.WorkerReportsProgress = true;
            this.dupeFinderWorker.WorkerSupportsCancellation = true;
            this.dupeFinderWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.dupeFinderWorker_DoWork);
            this.dupeFinderWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.dupeFinderWorker_ProgressChanged);
            this.dupeFinderWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.dupeFinderWorker_RunWorkerCompleted);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkboxes,
            this.num,
            this.title,
            this.artist,
            this.length,
            this.file,
            this.bitrate,
            this.size,
            this.listened});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(744, 339);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(763, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(87, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // radioHash
            // 
            this.radioHash.AutoSize = true;
            this.radioHash.Checked = true;
            this.radioHash.Location = new System.Drawing.Point(763, 69);
            this.radioHash.Name = "radioHash";
            this.radioHash.Size = new System.Drawing.Size(76, 17);
            this.radioHash.TabIndex = 2;
            this.radioHash.TabStop = true;
            this.radioHash.Text = "MD5 Hash";
            this.radioHash.UseVisualStyleBackColor = true;
            this.radioHash.CheckedChanged += new System.EventHandler(this.radioHash_CheckedChanged);
            // 
            // radioTitle
            // 
            this.radioTitle.AutoSize = true;
            this.radioTitle.Location = new System.Drawing.Point(763, 92);
            this.radioTitle.Name = "radioTitle";
            this.radioTitle.Size = new System.Drawing.Size(45, 17);
            this.radioTitle.TabIndex = 3;
            this.radioTitle.Text = "Title";
            this.radioTitle.UseVisualStyleBackColor = true;
            this.radioTitle.CheckedChanged += new System.EventHandler(this.radioTitle_CheckedChanged);
            // 
            // radioFilepath
            // 
            this.radioFilepath.AutoSize = true;
            this.radioFilepath.Location = new System.Drawing.Point(763, 115);
            this.radioFilepath.Name = "radioFilepath";
            this.radioFilepath.Size = new System.Drawing.Size(65, 17);
            this.radioFilepath.TabIndex = 5;
            this.radioFilepath.Text = "File path";
            this.radioFilepath.UseVisualStyleBackColor = true;
            this.radioFilepath.CheckedChanged += new System.EventHandler(this.radioFilepath_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(762, 151);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 38);
            this.button1.TabIndex = 6;
            this.button1.Text = "Show in Windows";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(762, 195);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 38);
            this.button2.TabIndex = 7;
            this.button2.Text = "Delete Files";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(762, 239);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(87, 38);
            this.button3.TabIndex = 8;
            this.button3.Text = "Uncheck All";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // modifiedProgressBarLoading1
            // 
            this.modifiedProgressBarLoading1.Location = new System.Drawing.Point(12, 357);
            this.modifiedProgressBarLoading1.Name = "modifiedProgressBarLoading1";
            this.modifiedProgressBarLoading1.Size = new System.Drawing.Size(744, 23);
            this.modifiedProgressBarLoading1.TabIndex = 4;
            // 
            // checkboxes
            // 
            this.checkboxes.FalseValue = "false";
            this.checkboxes.HeaderText = "";
            this.checkboxes.Name = "checkboxes";
            this.checkboxes.TrueValue = "true";
            this.checkboxes.Width = 20;
            // 
            // num
            // 
            this.num.DataPropertyName = "num";
            this.num.HeaderText = "#";
            this.num.Name = "num";
            this.num.ReadOnly = true;
            this.num.Width = 35;
            // 
            // title
            // 
            this.title.DataPropertyName = "title";
            this.title.HeaderText = "Title";
            this.title.Name = "title";
            this.title.ReadOnly = true;
            this.title.Width = 200;
            // 
            // artist
            // 
            this.artist.DataPropertyName = "artist";
            this.artist.HeaderText = "Artist";
            this.artist.Name = "artist";
            this.artist.ReadOnly = true;
            this.artist.Width = 120;
            // 
            // length
            // 
            this.length.DataPropertyName = "length";
            this.length.HeaderText = "Length";
            this.length.Name = "length";
            this.length.ReadOnly = true;
            this.length.Width = 60;
            // 
            // file
            // 
            this.file.DataPropertyName = "filename";
            this.file.HeaderText = "File";
            this.file.Name = "file";
            this.file.ReadOnly = true;
            // 
            // bitrate
            // 
            this.bitrate.DataPropertyName = "bitrate";
            this.bitrate.HeaderText = "Bitrate";
            this.bitrate.Name = "bitrate";
            this.bitrate.ReadOnly = true;
            this.bitrate.Width = 70;
            // 
            // size
            // 
            this.size.DataPropertyName = "size";
            this.size.HeaderText = "Size";
            this.size.Name = "size";
            this.size.ReadOnly = true;
            this.size.Width = 70;
            // 
            // listened
            // 
            this.listened.DataPropertyName = "listened";
            this.listened.HeaderText = "Listened";
            this.listened.Name = "listened";
            this.listened.ReadOnly = true;
            this.listened.Width = 50;
            // 
            // DupeFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 388);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radioFilepath);
            this.Controls.Add(this.modifiedProgressBarLoading1);
            this.Controls.Add(this.radioTitle);
            this.Controls.Add(this.radioHash);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DupeFinder";
            this.Text = "Duplicate finder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker dupeFinderWorker;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RadioButton radioHash;
        private System.Windows.Forms.RadioButton radioTitle;
        private ModifiedControls.ModifiedProgressBarLoading modifiedProgressBarLoading1;
        private System.Windows.Forms.RadioButton radioFilepath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkboxes;
        private System.Windows.Forms.DataGridViewTextBoxColumn num;
        private System.Windows.Forms.DataGridViewTextBoxColumn title;
        private System.Windows.Forms.DataGridViewTextBoxColumn artist;
        private System.Windows.Forms.DataGridViewTextBoxColumn length;
        private System.Windows.Forms.DataGridViewTextBoxColumn file;
        private System.Windows.Forms.DataGridViewTextBoxColumn bitrate;
        private System.Windows.Forms.DataGridViewTextBoxColumn size;
        private System.Windows.Forms.DataGridViewTextBoxColumn listened;
    }
}