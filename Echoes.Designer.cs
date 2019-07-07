using ModifiedControls;
using System.Windows.Forms;
namespace Echoes
{
    partial class Echoes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Echoes));
            this.trackGrid = new System.Windows.Forms.DataGridView();
            this.progressRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.trackText = new System.Windows.Forms.RichTextBox();
            this.tagsLoaderWorker = new System.ComponentModel.BackgroundWorker();
            this.gridSearchTimer = new System.Windows.Forms.Timer(this.components);
            this.searchBox = new System.Windows.Forms.TextBox();
            this.transposeChangerNum = new System.Windows.Forms.NumericUpDown();
            this.transposeTxt = new System.Windows.Forms.Label();
            this.fwdBtn = new System.Windows.Forms.PictureBox();
            this.rewBtn = new System.Windows.Forms.PictureBox();
            this.backBtn = new System.Windows.Forms.PictureBox();
            this.stopBtn = new System.Windows.Forms.PictureBox();
            this.playBtn = new System.Windows.Forms.PictureBox();
            this.visualsPicture = new System.Windows.Forms.PictureBox();
            this.echoesLogo = new System.Windows.Forms.PictureBox();
            this.openBtn = new System.Windows.Forms.PictureBox();
            this.exportBtn = new System.Windows.Forms.PictureBox();
            this.shuffleBtn = new System.Windows.Forms.PictureBox();
            this.optionsBtn = new System.Windows.Forms.PictureBox();
            this.repeatBtn = new System.Windows.Forms.PictureBox();
            this.normalizerWorker = new System.ComponentModel.BackgroundWorker();
            this.playlistInfoTxt = new System.Windows.Forms.Label();
            this.loadTrackWorker = new System.ComponentModel.BackgroundWorker();
            this.eqButton = new ModifiedControls.ModifiedButton();
            this.saveButton = new ModifiedControls.ModifiedButton();
            this.playlistSelectorCombo = new ModifiedControls.ModifiedComboBox();
            this.volumeBar = new ModifiedControls.ModifiedProgressBarVol();
            this.seekBar = new ModifiedControls.ModifiedProgressBarSeek();
            ((System.ComponentModel.ISupportInitialize)(this.trackGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transposeChangerNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fwdBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rewBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.visualsPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.echoesLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exportBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shuffleBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // trackGrid
            // 
            this.trackGrid.AllowUserToAddRows = false;
            this.trackGrid.AllowUserToDeleteRows = false;
            this.trackGrid.AllowUserToOrderColumns = true;
            this.trackGrid.AllowUserToResizeRows = false;
            this.trackGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.trackGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.trackGrid.EnableHeadersVisualStyles = false;
            this.trackGrid.Location = new System.Drawing.Point(10, 164);
            this.trackGrid.Name = "trackGrid";
            this.trackGrid.ReadOnly = true;
            this.trackGrid.RowHeadersVisible = false;
            this.trackGrid.Size = new System.Drawing.Size(1048, 394);
            this.trackGrid.TabIndex = 0;
            this.trackGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.trackGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.trackGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.trackGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.trackGrid.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView1_ColumnWidthChanged);
            this.trackGrid.Sorted += new System.EventHandler(this.dataGridView1_Sorted);
            this.trackGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this.trackGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
            this.trackGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // progressRefreshTimer
            // 
            this.progressRefreshTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // trackText
            // 
            this.trackText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.trackText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.trackText.DetectUrls = false;
            this.trackText.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.trackText.Location = new System.Drawing.Point(10, 75);
            this.trackText.Multiline = false;
            this.trackText.Name = "trackText";
            this.trackText.ReadOnly = true;
            this.trackText.Size = new System.Drawing.Size(1048, 18);
            this.trackText.TabIndex = 9;
            this.trackText.Text = "No track";
            this.trackText.FontChanged += new System.EventHandler(this.trackText_FontChanged);
            // 
            // tagsLoaderWorker
            // 
            this.tagsLoaderWorker.WorkerReportsProgress = true;
            this.tagsLoaderWorker.WorkerSupportsCancellation = true;
            this.tagsLoaderWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.tagsLoaderWorker_DoWork);
            this.tagsLoaderWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.tagsLoaderWorker_ProgressChanged);
            this.tagsLoaderWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.tagsLoaderWorker_RunWorkerCompleted);
            // 
            // gridSearchTimer
            // 
            this.gridSearchTimer.Interval = 50;
            this.gridSearchTimer.Tick += new System.EventHandler(this.gridSearchTimer_Tick);
            // 
            // searchBox
            // 
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.Location = new System.Drawing.Point(11, 139);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(178, 20);
            this.searchBox.TabIndex = 21;
            this.searchBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // transposeChangerNum
            // 
            this.transposeChangerNum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.transposeChangerNum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.transposeChangerNum.DecimalPlaces = 1;
            this.transposeChangerNum.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transposeChangerNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.transposeChangerNum.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.transposeChangerNum.Location = new System.Drawing.Point(539, 10);
            this.transposeChangerNum.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.transposeChangerNum.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            -2147483648});
            this.transposeChangerNum.Name = "transposeChangerNum";
            this.transposeChangerNum.Size = new System.Drawing.Size(50, 18);
            this.transposeChangerNum.TabIndex = 22;
            this.transposeChangerNum.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // transposeTxt
            // 
            this.transposeTxt.AutoSize = true;
            this.transposeTxt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transposeTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.transposeTxt.Location = new System.Drawing.Point(445, 10);
            this.transposeTxt.Name = "transposeTxt";
            this.transposeTxt.Size = new System.Drawing.Size(77, 15);
            this.transposeTxt.TabIndex = 23;
            this.transposeTxt.Text = "Transpose:";
            // 
            // fwdBtn
            // 
            this.fwdBtn.BackColor = System.Drawing.Color.Transparent;
            this.fwdBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.fwdBtn.Image = global::Echoes.Properties.Resources.forward1;
            this.fwdBtn.InitialImage = null;
            this.fwdBtn.Location = new System.Drawing.Point(241, 10);
            this.fwdBtn.Name = "fwdBtn";
            this.fwdBtn.Size = new System.Drawing.Size(60, 60);
            this.fwdBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fwdBtn.TabIndex = 37;
            this.fwdBtn.TabStop = false;
            this.fwdBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button5_Click);
            this.fwdBtn.MouseEnter += new System.EventHandler(this.button5_MouseEnter);
            this.fwdBtn.MouseLeave += new System.EventHandler(this.button5_MouseLeave);
            this.fwdBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button5_MouseUp);
            // 
            // rewBtn
            // 
            this.rewBtn.BackColor = System.Drawing.Color.Transparent;
            this.rewBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rewBtn.Image = global::Echoes.Properties.Resources.rewind1;
            this.rewBtn.InitialImage = null;
            this.rewBtn.Location = new System.Drawing.Point(208, 10);
            this.rewBtn.Name = "rewBtn";
            this.rewBtn.Size = new System.Drawing.Size(26, 60);
            this.rewBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.rewBtn.TabIndex = 36;
            this.rewBtn.TabStop = false;
            this.rewBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button7_Click);
            this.rewBtn.MouseEnter += new System.EventHandler(this.button7_MouseEnter);
            this.rewBtn.MouseLeave += new System.EventHandler(this.button7_MouseLeave);
            this.rewBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button7_MouseUp);
            // 
            // backBtn
            // 
            this.backBtn.BackColor = System.Drawing.Color.Transparent;
            this.backBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.backBtn.Image = global::Echoes.Properties.Resources.back1;
            this.backBtn.InitialImage = null;
            this.backBtn.Location = new System.Drawing.Point(142, 10);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(60, 60);
            this.backBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.backBtn.TabIndex = 35;
            this.backBtn.TabStop = false;
            this.backBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button6_Click);
            this.backBtn.MouseEnter += new System.EventHandler(this.button3_MouseEnter);
            this.backBtn.MouseLeave += new System.EventHandler(this.button3_MouseLeave);
            this.backBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button3_MouseUp);
            // 
            // stopBtn
            // 
            this.stopBtn.BackColor = System.Drawing.Color.Transparent;
            this.stopBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.stopBtn.Image = global::Echoes.Properties.Resources.stop1;
            this.stopBtn.InitialImage = null;
            this.stopBtn.Location = new System.Drawing.Point(76, 10);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(60, 60);
            this.stopBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.stopBtn.TabIndex = 34;
            this.stopBtn.TabStop = false;
            this.stopBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button2_Click);
            this.stopBtn.MouseEnter += new System.EventHandler(this.button2_MouseEnter);
            this.stopBtn.MouseLeave += new System.EventHandler(this.button2_MouseLeave);
            this.stopBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button2_MouseUp);
            // 
            // playBtn
            // 
            this.playBtn.BackColor = System.Drawing.Color.Transparent;
            this.playBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.playBtn.Image = global::Echoes.Properties.Resources.play1;
            this.playBtn.InitialImage = null;
            this.playBtn.Location = new System.Drawing.Point(10, 10);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(60, 60);
            this.playBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.playBtn.TabIndex = 33;
            this.playBtn.TabStop = false;
            this.playBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_Click);
            this.playBtn.MouseEnter += new System.EventHandler(this.button1_MouseEnter);
            this.playBtn.MouseLeave += new System.EventHandler(this.button1_MouseLeave);
            this.playBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
            // 
            // visualsPicture
            // 
            this.visualsPicture.Location = new System.Drawing.Point(595, 10);
            this.visualsPicture.Name = "visualsPicture";
            this.visualsPicture.Size = new System.Drawing.Size(397, 60);
            this.visualsPicture.TabIndex = 27;
            this.visualsPicture.TabStop = false;
            this.visualsPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseClick);
            // 
            // echoesLogo
            // 
            this.echoesLogo.BackgroundImage = global::Echoes.Properties.Resources.echoesLogoWhiteDim;
            this.echoesLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.echoesLogo.Location = new System.Drawing.Point(998, 10);
            this.echoesLogo.Name = "echoesLogo";
            this.echoesLogo.Size = new System.Drawing.Size(60, 60);
            this.echoesLogo.TabIndex = 14;
            this.echoesLogo.TabStop = false;
            this.echoesLogo.MouseEnter += new System.EventHandler(this.echoesLogo_MouseEnter);
            this.echoesLogo.MouseLeave += new System.EventHandler(this.echoesLogo_MouseLeave);
            this.echoesLogo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.echoesLogo_MouseUp);
            // 
            // openBtn
            // 
            this.openBtn.BackColor = System.Drawing.Color.Transparent;
            this.openBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.openBtn.Image = global::Echoes.Properties.Resources.import1;
            this.openBtn.InitialImage = null;
            this.openBtn.Location = new System.Drawing.Point(307, 10);
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(30, 30);
            this.openBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.openBtn.TabIndex = 38;
            this.openBtn.TabStop = false;
            this.openBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openBtn_MouseDown);
            this.openBtn.MouseEnter += new System.EventHandler(this.openBtn_MouseEnter);
            this.openBtn.MouseLeave += new System.EventHandler(this.openBtn_MouseLeave);
            this.openBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openBtn_MouseUp);
            // 
            // exportBtn
            // 
            this.exportBtn.BackColor = System.Drawing.Color.Transparent;
            this.exportBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.exportBtn.Image = global::Echoes.Properties.Resources.export1;
            this.exportBtn.InitialImage = null;
            this.exportBtn.Location = new System.Drawing.Point(307, 39);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(30, 30);
            this.exportBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.exportBtn.TabIndex = 39;
            this.exportBtn.TabStop = false;
            this.exportBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.exportBtn_MouseDown);
            this.exportBtn.MouseEnter += new System.EventHandler(this.exportBtn_MouseEnter);
            this.exportBtn.MouseLeave += new System.EventHandler(this.exportBtn_MouseLeave);
            this.exportBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.exportBtn_MouseUp);
            // 
            // shuffleBtn
            // 
            this.shuffleBtn.BackColor = System.Drawing.Color.Transparent;
            this.shuffleBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.shuffleBtn.Image = global::Echoes.Properties.Resources.shuffle1;
            this.shuffleBtn.InitialImage = null;
            this.shuffleBtn.Location = new System.Drawing.Point(343, 10);
            this.shuffleBtn.Name = "shuffleBtn";
            this.shuffleBtn.Size = new System.Drawing.Size(30, 30);
            this.shuffleBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.shuffleBtn.TabIndex = 40;
            this.shuffleBtn.TabStop = false;
            this.shuffleBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.shuffleBtn_MouseDown);
            this.shuffleBtn.MouseEnter += new System.EventHandler(this.shuffleBtn_MouseEnter);
            this.shuffleBtn.MouseLeave += new System.EventHandler(this.shuffleBtn_MouseLeave);
            this.shuffleBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.shuffleBtn_MouseUp);
            // 
            // optionsBtn
            // 
            this.optionsBtn.BackColor = System.Drawing.Color.Transparent;
            this.optionsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.optionsBtn.Image = global::Echoes.Properties.Resources.options1;
            this.optionsBtn.InitialImage = null;
            this.optionsBtn.Location = new System.Drawing.Point(343, 39);
            this.optionsBtn.Name = "optionsBtn";
            this.optionsBtn.Size = new System.Drawing.Size(30, 30);
            this.optionsBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.optionsBtn.TabIndex = 41;
            this.optionsBtn.TabStop = false;
            this.optionsBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.optionsBtn_MouseDown);
            this.optionsBtn.MouseEnter += new System.EventHandler(this.optionsBtn_MouseEnter);
            this.optionsBtn.MouseLeave += new System.EventHandler(this.optionsBtn_MouseLeave);
            this.optionsBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.optionsBtn_MouseUp);
            // 
            // repeatBtn
            // 
            this.repeatBtn.BackColor = System.Drawing.Color.Transparent;
            this.repeatBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.repeatBtn.Image = global::Echoes.Properties.Resources.repeatNone1;
            this.repeatBtn.InitialImage = null;
            this.repeatBtn.Location = new System.Drawing.Point(379, 9);
            this.repeatBtn.Name = "repeatBtn";
            this.repeatBtn.Size = new System.Drawing.Size(60, 60);
            this.repeatBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.repeatBtn.TabIndex = 42;
            this.repeatBtn.TabStop = false;
            this.repeatBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.repeatBtn_MouseDown);
            this.repeatBtn.MouseEnter += new System.EventHandler(this.repeatBtn_MouseEnter);
            this.repeatBtn.MouseLeave += new System.EventHandler(this.repeatBtn_MouseLeave);
            this.repeatBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.repeatBtn_MouseUp);
            // 
            // normalizerWorker
            // 
            this.normalizerWorker.WorkerReportsProgress = true;
            this.normalizerWorker.WorkerSupportsCancellation = true;
            this.normalizerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.normalizerWorker_DoWork);
            this.normalizerWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.normalizerWorker_RunWorkerCompleted);
            // 
            // playlistInfoTxt
            // 
            this.playlistInfoTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playlistInfoTxt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playlistInfoTxt.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playlistInfoTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.playlistInfoTxt.Location = new System.Drawing.Point(195, 141);
            this.playlistInfoTxt.Name = "playlistInfoTxt";
            this.playlistInfoTxt.Size = new System.Drawing.Size(544, 18);
            this.playlistInfoTxt.TabIndex = 43;
            this.playlistInfoTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadTrackWorker
            // 
            this.loadTrackWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadTrackWorker_DoWork);
            this.loadTrackWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadTrackWorker_RunWorkerCompleted);
            // 
            // eqButton
            // 
            this.eqButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.eqButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.eqButton.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eqButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.eqButton.Location = new System.Drawing.Point(460, 40);
            this.eqButton.Margin = new System.Windows.Forms.Padding(2);
            this.eqButton.Name = "eqButton";
            this.eqButton.Size = new System.Drawing.Size(38, 30);
            this.eqButton.TabIndex = 44;
            this.eqButton.Text = "EQ";
            this.eqButton.UseVisualStyleBackColor = false;
            this.eqButton.Click += new System.EventHandler(this.eqButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.saveButton.Location = new System.Drawing.Point(513, 40);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(76, 30);
            this.saveButton.TabIndex = 24;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.modifiedButton1_Click);
            // 
            // playlistSelectorCombo
            // 
            this.playlistSelectorCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.playlistSelectorCombo.CausesValidation = false;
            this.playlistSelectorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.playlistSelectorCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playlistSelectorCombo.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playlistSelectorCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.playlistSelectorCombo.FormattingEnabled = true;
            this.playlistSelectorCombo.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.playlistSelectorCombo.Location = new System.Drawing.Point(745, 138);
            this.playlistSelectorCombo.Name = "playlistSelectorCombo";
            this.playlistSelectorCombo.Size = new System.Drawing.Size(156, 21);
            this.playlistSelectorCombo.TabIndex = 15;
            this.playlistSelectorCombo.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            this.playlistSelectorCombo.DropDownClosed += new System.EventHandler(this.comboBox1_DropDownClosed);
            // 
            // volumeBar
            // 
            this.volumeBar.BackColor = System.Drawing.SystemColors.Control;
            this.volumeBar.Location = new System.Drawing.Point(907, 138);
            this.volumeBar.Maximum = 300;
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(150, 22);
            this.volumeBar.TabIndex = 7;
            this.volumeBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.progressBar2_MouseDown);
            this.volumeBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.volumeBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            // 
            // seekBar
            // 
            this.seekBar.Location = new System.Drawing.Point(10, 100);
            this.seekBar.Name = "seekBar";
            this.seekBar.Size = new System.Drawing.Size(1047, 30);
            this.seekBar.TabIndex = 5;
            this.seekBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseDown);
            this.seekBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.seekBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            // 
            // Echoes
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1070, 562);
            this.Controls.Add(this.eqButton);
            this.Controls.Add(this.playlistInfoTxt);
            this.Controls.Add(this.repeatBtn);
            this.Controls.Add(this.optionsBtn);
            this.Controls.Add(this.shuffleBtn);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.openBtn);
            this.Controls.Add(this.fwdBtn);
            this.Controls.Add(this.rewBtn);
            this.Controls.Add(this.backBtn);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.visualsPicture);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.transposeTxt);
            this.Controls.Add(this.transposeChangerNum);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.playlistSelectorCombo);
            this.Controls.Add(this.echoesLogo);
            this.Controls.Add(this.trackText);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.seekBar);
            this.Controls.Add(this.trackGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(640, 300);
            this.Name = "Echoes";
            this.Text = "Echoes";
            this.Activated += new System.EventHandler(this.Echoes_Enter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Echoes_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Echoes_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Echoes_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Echoes_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transposeChangerNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fwdBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rewBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.visualsPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.echoesLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exportBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shuffleBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repeatBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView trackGrid;
        private ModifiedProgressBarSeek seekBar;
        public System.Windows.Forms.Timer progressRefreshTimer;
        public ModifiedProgressBarVol volumeBar;
        private System.Windows.Forms.RichTextBox trackText;
        private System.Windows.Forms.PictureBox echoesLogo;
        private ModifiedComboBox playlistSelectorCombo;
        private System.ComponentModel.BackgroundWorker tagsLoaderWorker;
        private System.Windows.Forms.Timer gridSearchTimer;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.NumericUpDown transposeChangerNum;
        private System.Windows.Forms.Label transposeTxt;
        private ModifiedButton saveButton;
        private System.Windows.Forms.PictureBox visualsPicture;
        private System.Windows.Forms.PictureBox playBtn;
        private System.Windows.Forms.PictureBox stopBtn;
        private System.Windows.Forms.PictureBox backBtn;
        private System.Windows.Forms.PictureBox rewBtn;
        private System.Windows.Forms.PictureBox fwdBtn;
        private System.Windows.Forms.PictureBox openBtn;
        private System.Windows.Forms.PictureBox exportBtn;
        private System.Windows.Forms.PictureBox shuffleBtn;
        private System.Windows.Forms.PictureBox optionsBtn;
        private System.Windows.Forms.PictureBox repeatBtn;
        private ModifiedButton eqButton;
        public System.ComponentModel.BackgroundWorker normalizerWorker;
        private Label playlistInfoTxt;
        private System.ComponentModel.BackgroundWorker loadTrackWorker;
    }
}

