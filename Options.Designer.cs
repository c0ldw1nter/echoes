using ModifiedControls;
namespace Echoes
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.generalPage = new System.Windows.Forms.TabPage();
            this.normalizeCheckbox = new System.Windows.Forms.CheckBox();
            this.setDefaultsBtn = new ModifiedControls.ModifiedButton();
            this.font2ChangeBtn = new ModifiedControls.ModifiedButton();
            this.font2Txt = new System.Windows.Forms.Label();
            this.showWaveformCheckbox = new System.Windows.Forms.CheckBox();
            this.fontPctTxt = new System.Windows.Forms.TextBox();
            this.fontSizePercentTxt = new System.Windows.Forms.Label();
            this.fontSizePercentTrack = new System.Windows.Forms.TrackBar();
            this.fontChangeBtn = new ModifiedControls.ModifiedButton();
            this.fontTxt = new System.Windows.Forms.Label();
            this.autoShuffleCheckbox = new System.Windows.Forms.CheckBox();
            this.autoAdvanceCheckbox = new System.Windows.Forms.CheckBox();
            this.saveTransposeCheckbox = new System.Windows.Forms.CheckBox();
            this.fpsText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fpsBar = new System.Windows.Forms.TrackBar();
            this.button1 = new ModifiedControls.ModifiedButton();
            this.midiTxt = new System.Windows.Forms.Label();
            this.popupCheckbox = new System.Windows.Forms.CheckBox();
            this.hotkeysPage = new System.Windows.Forms.TabPage();
            this.suppressHotkeyCheck = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.transposeIncrementChanger = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.volIncrementChanger = new System.Windows.Forms.NumericUpDown();
            this.hotkeyDgv = new System.Windows.Forms.DataGridView();
            this.Enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.removeHotkeyBtn = new ModifiedControls.ModifiedButton();
            this.addHotkeyBtn = new ModifiedControls.ModifiedButton();
            this.colorsPage = new System.Windows.Forms.TabPage();
            this.addColorSchemeBtn = new ModifiedControls.ModifiedButton();
            this.colorSchemeSelectorCombo = new ModifiedControls.ModifiedComboBox();
            this.colorHighlighted2Btn = new ModifiedControls.ModifiedButton();
            this.colorHighlightedBtn = new ModifiedControls.ModifiedButton();
            this.colorSeek2Btn = new ModifiedControls.ModifiedButton();
            this.colorVolBar2Btn = new ModifiedControls.ModifiedButton();
            this.colorBackgroundBtn = new ModifiedControls.ModifiedButton();
            this.colorControlFrontBtn = new ModifiedControls.ModifiedButton();
            this.colorControlBackBtn = new ModifiedControls.ModifiedButton();
            this.colorTrackAlbumBtn = new ModifiedControls.ModifiedButton();
            this.colorTrackArtistBtn = new ModifiedControls.ModifiedButton();
            this.colorTrackTitleBtn = new ModifiedControls.ModifiedButton();
            this.colorVisualsBtn = new ModifiedControls.ModifiedButton();
            this.colorVolBarBtn = new ModifiedControls.ModifiedButton();
            this.colorSeekBtn = new ModifiedControls.ModifiedButton();
            this.reshuffleCheckbox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.generalPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fontSizePercentTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpsBar)).BeginInit();
            this.hotkeysPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transposeIncrementChanger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volIncrementChanger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hotkeyDgv)).BeginInit();
            this.colorsPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalPage);
            this.tabControl1.Controls.Add(this.hotkeysPage);
            this.tabControl1.Controls.Add(this.colorsPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(442, 262);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            // 
            // generalPage
            // 
            this.generalPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.generalPage.Controls.Add(this.reshuffleCheckbox);
            this.generalPage.Controls.Add(this.normalizeCheckbox);
            this.generalPage.Controls.Add(this.setDefaultsBtn);
            this.generalPage.Controls.Add(this.font2ChangeBtn);
            this.generalPage.Controls.Add(this.font2Txt);
            this.generalPage.Controls.Add(this.showWaveformCheckbox);
            this.generalPage.Controls.Add(this.fontPctTxt);
            this.generalPage.Controls.Add(this.fontSizePercentTxt);
            this.generalPage.Controls.Add(this.fontSizePercentTrack);
            this.generalPage.Controls.Add(this.fontChangeBtn);
            this.generalPage.Controls.Add(this.fontTxt);
            this.generalPage.Controls.Add(this.autoShuffleCheckbox);
            this.generalPage.Controls.Add(this.autoAdvanceCheckbox);
            this.generalPage.Controls.Add(this.saveTransposeCheckbox);
            this.generalPage.Controls.Add(this.fpsText);
            this.generalPage.Controls.Add(this.label2);
            this.generalPage.Controls.Add(this.fpsBar);
            this.generalPage.Controls.Add(this.button1);
            this.generalPage.Controls.Add(this.midiTxt);
            this.generalPage.Controls.Add(this.popupCheckbox);
            this.generalPage.Location = new System.Drawing.Point(4, 22);
            this.generalPage.Name = "generalPage";
            this.generalPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalPage.Size = new System.Drawing.Size(434, 236);
            this.generalPage.TabIndex = 0;
            this.generalPage.Text = "General";
            // 
            // normalizeCheckbox
            // 
            this.normalizeCheckbox.AutoSize = true;
            this.normalizeCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.normalizeCheckbox.Location = new System.Drawing.Point(5, 52);
            this.normalizeCheckbox.Name = "normalizeCheckbox";
            this.normalizeCheckbox.Size = new System.Drawing.Size(114, 17);
            this.normalizeCheckbox.TabIndex = 35;
            this.normalizeCheckbox.Text = "Normalize loudness";
            this.normalizeCheckbox.UseVisualStyleBackColor = true;
            this.normalizeCheckbox.CheckedChanged += new System.EventHandler(this.normalizeCheckbox_CheckedChanged);
            // 
            // setDefaultsBtn
            // 
            this.setDefaultsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.setDefaultsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setDefaultsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.setDefaultsBtn.Location = new System.Drawing.Point(367, 205);
            this.setDefaultsBtn.Name = "setDefaultsBtn";
            this.setDefaultsBtn.Size = new System.Drawing.Size(59, 23);
            this.setDefaultsBtn.TabIndex = 34;
            this.setDefaultsBtn.Text = "Defaults";
            this.setDefaultsBtn.UseVisualStyleBackColor = true;
            this.setDefaultsBtn.Click += new System.EventHandler(this.setDefaultsBtn_Click);
            // 
            // font2ChangeBtn
            // 
            this.font2ChangeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.font2ChangeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.font2ChangeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.font2ChangeBtn.Location = new System.Drawing.Point(213, 142);
            this.font2ChangeBtn.Name = "font2ChangeBtn";
            this.font2ChangeBtn.Size = new System.Drawing.Size(59, 23);
            this.font2ChangeBtn.TabIndex = 33;
            this.font2ChangeBtn.Text = "Change";
            this.font2ChangeBtn.UseVisualStyleBackColor = true;
            this.font2ChangeBtn.Click += new System.EventHandler(this.font2ChangeBtn_Click);
            // 
            // font2Txt
            // 
            this.font2Txt.AutoSize = true;
            this.font2Txt.Location = new System.Drawing.Point(214, 126);
            this.font2Txt.Name = "font2Txt";
            this.font2Txt.Size = new System.Drawing.Size(35, 13);
            this.font2Txt.TabIndex = 32;
            this.font2Txt.Text = "label5";
            // 
            // showWaveformCheckbox
            // 
            this.showWaveformCheckbox.AutoSize = true;
            this.showWaveformCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showWaveformCheckbox.Location = new System.Drawing.Point(5, 75);
            this.showWaveformCheckbox.Name = "showWaveformCheckbox";
            this.showWaveformCheckbox.Size = new System.Drawing.Size(99, 17);
            this.showWaveformCheckbox.TabIndex = 31;
            this.showWaveformCheckbox.Text = "Show waveform";
            this.showWaveformCheckbox.UseVisualStyleBackColor = true;
            this.showWaveformCheckbox.CheckedChanged += new System.EventHandler(this.showWaveformCheckbox_CheckedChanged);
            // 
            // fontPctTxt
            // 
            this.fontPctTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.fontPctTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fontPctTxt.Enabled = false;
            this.fontPctTxt.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontPctTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.fontPctTxt.Location = new System.Drawing.Point(214, 89);
            this.fontPctTxt.Name = "fontPctTxt";
            this.fontPctTxt.ReadOnly = true;
            this.fontPctTxt.Size = new System.Drawing.Size(212, 20);
            this.fontPctTxt.TabIndex = 30;
            this.fontPctTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fontSizePercentTxt
            // 
            this.fontSizePercentTxt.AutoSize = true;
            this.fontSizePercentTxt.Location = new System.Drawing.Point(214, 48);
            this.fontSizePercentTxt.Name = "fontSizePercentTxt";
            this.fontSizePercentTxt.Size = new System.Drawing.Size(106, 13);
            this.fontSizePercentTxt.TabIndex = 29;
            this.fontSizePercentTxt.Text = "Font size percentage";
            // 
            // fontSizePercentTrack
            // 
            this.fontSizePercentTrack.Location = new System.Drawing.Point(213, 64);
            this.fontSizePercentTrack.Maximum = 200;
            this.fontSizePercentTrack.Minimum = 50;
            this.fontSizePercentTrack.Name = "fontSizePercentTrack";
            this.fontSizePercentTrack.Size = new System.Drawing.Size(213, 45);
            this.fontSizePercentTrack.TabIndex = 28;
            this.fontSizePercentTrack.TickFrequency = 50;
            this.fontSizePercentTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this.fontSizePercentTrack.Value = 100;
            this.fontSizePercentTrack.ValueChanged += new System.EventHandler(this.fontSizePercentTrack_ValueChanged);
            this.fontSizePercentTrack.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fontSizePercentTrack_MouseUp);
            // 
            // fontChangeBtn
            // 
            this.fontChangeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.fontChangeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontChangeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fontChangeBtn.Location = new System.Drawing.Point(214, 22);
            this.fontChangeBtn.Name = "fontChangeBtn";
            this.fontChangeBtn.Size = new System.Drawing.Size(59, 23);
            this.fontChangeBtn.TabIndex = 27;
            this.fontChangeBtn.Text = "Change";
            this.fontChangeBtn.UseVisualStyleBackColor = true;
            this.fontChangeBtn.Click += new System.EventHandler(this.fontChangeBtn_Click);
            // 
            // fontTxt
            // 
            this.fontTxt.AutoSize = true;
            this.fontTxt.Location = new System.Drawing.Point(214, 6);
            this.fontTxt.Name = "fontTxt";
            this.fontTxt.Size = new System.Drawing.Size(35, 13);
            this.fontTxt.TabIndex = 26;
            this.fontTxt.Text = "label5";
            // 
            // autoShuffleCheckbox
            // 
            this.autoShuffleCheckbox.AutoSize = true;
            this.autoShuffleCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autoShuffleCheckbox.Location = new System.Drawing.Point(5, 29);
            this.autoShuffleCheckbox.Name = "autoShuffleCheckbox";
            this.autoShuffleCheckbox.Size = new System.Drawing.Size(79, 17);
            this.autoShuffleCheckbox.TabIndex = 25;
            this.autoShuffleCheckbox.Text = "Auto shuffle";
            this.autoShuffleCheckbox.UseVisualStyleBackColor = true;
            this.autoShuffleCheckbox.CheckedChanged += new System.EventHandler(this.autoShuffleCheckbox_CheckedChanged);
            // 
            // autoAdvanceCheckbox
            // 
            this.autoAdvanceCheckbox.AutoSize = true;
            this.autoAdvanceCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autoAdvanceCheckbox.Location = new System.Drawing.Point(5, 6);
            this.autoAdvanceCheckbox.Name = "autoAdvanceCheckbox";
            this.autoAdvanceCheckbox.Size = new System.Drawing.Size(90, 17);
            this.autoAdvanceCheckbox.TabIndex = 24;
            this.autoAdvanceCheckbox.Text = "Auto advance";
            this.autoAdvanceCheckbox.UseVisualStyleBackColor = true;
            // 
            // saveTransposeCheckbox
            // 
            this.saveTransposeCheckbox.AutoSize = true;
            this.saveTransposeCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveTransposeCheckbox.Location = new System.Drawing.Point(5, 98);
            this.saveTransposeCheckbox.Name = "saveTransposeCheckbox";
            this.saveTransposeCheckbox.Size = new System.Drawing.Size(202, 17);
            this.saveTransposeCheckbox.TabIndex = 23;
            this.saveTransposeCheckbox.Text = "Save transpose value between tracks";
            this.saveTransposeCheckbox.UseVisualStyleBackColor = true;
            this.saveTransposeCheckbox.CheckedChanged += new System.EventHandler(this.saveTransposeCheckbox_CheckedChanged);
            // 
            // fpsText
            // 
            this.fpsText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.fpsText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fpsText.Enabled = false;
            this.fpsText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fpsText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.fpsText.Location = new System.Drawing.Point(208, 210);
            this.fpsText.Name = "fpsText";
            this.fpsText.ReadOnly = true;
            this.fpsText.Size = new System.Drawing.Size(124, 20);
            this.fpsText.TabIndex = 22;
            this.fpsText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Visuals refresh rate (FPS)";
            // 
            // fpsBar
            // 
            this.fpsBar.Location = new System.Drawing.Point(211, 190);
            this.fpsBar.Maximum = 60;
            this.fpsBar.Minimum = 1;
            this.fpsBar.Name = "fpsBar";
            this.fpsBar.Size = new System.Drawing.Size(121, 45);
            this.fpsBar.TabIndex = 6;
            this.fpsBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.fpsBar.Value = 30;
            this.fpsBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.fpsBar.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.button1.Location = new System.Drawing.Point(5, 206);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Change";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // midiTxt
            // 
            this.midiTxt.AutoSize = true;
            this.midiTxt.Location = new System.Drawing.Point(5, 190);
            this.midiTxt.Name = "midiTxt";
            this.midiTxt.Size = new System.Drawing.Size(35, 13);
            this.midiTxt.TabIndex = 4;
            this.midiTxt.Text = "label1";
            // 
            // popupCheckbox
            // 
            this.popupCheckbox.AutoSize = true;
            this.popupCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.popupCheckbox.Location = new System.Drawing.Point(5, 121);
            this.popupCheckbox.Name = "popupCheckbox";
            this.popupCheckbox.Size = new System.Drawing.Size(164, 17);
            this.popupCheckbox.TabIndex = 1;
            this.popupCheckbox.Text = "Show popup on track change";
            this.popupCheckbox.UseVisualStyleBackColor = true;
            this.popupCheckbox.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // hotkeysPage
            // 
            this.hotkeysPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.hotkeysPage.Controls.Add(this.suppressHotkeyCheck);
            this.hotkeysPage.Controls.Add(this.label4);
            this.hotkeysPage.Controls.Add(this.transposeIncrementChanger);
            this.hotkeysPage.Controls.Add(this.label3);
            this.hotkeysPage.Controls.Add(this.volIncrementChanger);
            this.hotkeysPage.Controls.Add(this.hotkeyDgv);
            this.hotkeysPage.Controls.Add(this.removeHotkeyBtn);
            this.hotkeysPage.Controls.Add(this.addHotkeyBtn);
            this.hotkeysPage.Location = new System.Drawing.Point(4, 22);
            this.hotkeysPage.Name = "hotkeysPage";
            this.hotkeysPage.Padding = new System.Windows.Forms.Padding(3);
            this.hotkeysPage.Size = new System.Drawing.Size(434, 236);
            this.hotkeysPage.TabIndex = 1;
            this.hotkeysPage.Text = "Hotkeys";
            // 
            // suppressHotkeyCheck
            // 
            this.suppressHotkeyCheck.AutoSize = true;
            this.suppressHotkeyCheck.Location = new System.Drawing.Point(103, 6);
            this.suppressHotkeyCheck.Name = "suppressHotkeyCheck";
            this.suppressHotkeyCheck.Size = new System.Drawing.Size(95, 17);
            this.suppressHotkeyCheck.TabIndex = 32;
            this.suppressHotkeyCheck.Text = "Suppress keys";
            this.suppressHotkeyCheck.UseVisualStyleBackColor = true;
            this.suppressHotkeyCheck.CheckedChanged += new System.EventHandler(this.suppressHotkeyCheck_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Transpose increments";
            // 
            // transposeIncrementChanger
            // 
            this.transposeIncrementChanger.DecimalPlaces = 1;
            this.transposeIncrementChanger.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.transposeIncrementChanger.Location = new System.Drawing.Point(259, 33);
            this.transposeIncrementChanger.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.transposeIncrementChanger.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.transposeIncrementChanger.Name = "transposeIncrementChanger";
            this.transposeIncrementChanger.Size = new System.Drawing.Size(41, 20);
            this.transposeIncrementChanger.TabIndex = 30;
            this.transposeIncrementChanger.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.transposeIncrementChanger.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.transposeIncrementChanger.ValueChanged += new System.EventHandler(this.transposeIncrementChanger_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Volume increments";
            // 
            // volIncrementChanger
            // 
            this.volIncrementChanger.Location = new System.Drawing.Point(103, 34);
            this.volIncrementChanger.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.volIncrementChanger.Name = "volIncrementChanger";
            this.volIncrementChanger.Size = new System.Drawing.Size(39, 20);
            this.volIncrementChanger.TabIndex = 28;
            this.volIncrementChanger.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.volIncrementChanger.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.volIncrementChanger.ValueChanged += new System.EventHandler(this.volIncrementChanger_ValueChanged);
            // 
            // hotkeyDgv
            // 
            this.hotkeyDgv.AllowUserToAddRows = false;
            this.hotkeyDgv.AllowUserToDeleteRows = false;
            this.hotkeyDgv.AllowUserToResizeColumns = false;
            this.hotkeyDgv.AllowUserToResizeRows = false;
            this.hotkeyDgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.hotkeyDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.hotkeyDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Enabled,
            this.Function,
            this.Mod,
            this.Key});
            this.hotkeyDgv.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hotkeyDgv.EnableHeadersVisualStyles = false;
            this.hotkeyDgv.Location = new System.Drawing.Point(3, 60);
            this.hotkeyDgv.Name = "hotkeyDgv";
            this.hotkeyDgv.RowHeadersVisible = false;
            this.hotkeyDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.hotkeyDgv.Size = new System.Drawing.Size(428, 173);
            this.hotkeyDgv.TabIndex = 0;
            this.hotkeyDgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.hotkeyDgv_CellContentClick);
            this.hotkeyDgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hotkeyDgv_KeyDown);
            this.hotkeyDgv.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.hotkeyDgv_MouseDoubleClick);
            this.hotkeyDgv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.hotkeyDgv_MouseDown);
            // 
            // Enabled
            // 
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.Name = "Enabled";
            this.Enabled.Width = 60;
            // 
            // Function
            // 
            this.Function.DataPropertyName = "enabled";
            this.Function.HeaderText = "Function";
            this.Function.Name = "Function";
            this.Function.ReadOnly = true;
            this.Function.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Function.Width = 130;
            // 
            // Mod
            // 
            this.Mod.DataPropertyName = "mod";
            this.Mod.HeaderText = "Mod";
            this.Mod.Name = "Mod";
            this.Mod.ReadOnly = true;
            this.Mod.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Key
            // 
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            this.Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // removeHotkeyBtn
            // 
            this.removeHotkeyBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.removeHotkeyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeHotkeyBtn.Font = new System.Drawing.Font("White Rabbit", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeHotkeyBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.removeHotkeyBtn.Location = new System.Drawing.Point(33, 2);
            this.removeHotkeyBtn.Margin = new System.Windows.Forms.Padding(2);
            this.removeHotkeyBtn.Name = "removeHotkeyBtn";
            this.removeHotkeyBtn.Size = new System.Drawing.Size(24, 23);
            this.removeHotkeyBtn.TabIndex = 27;
            this.removeHotkeyBtn.Text = "-";
            this.removeHotkeyBtn.UseVisualStyleBackColor = false;
            this.removeHotkeyBtn.Click += new System.EventHandler(this.removeHotkeyBtn_Click);
            // 
            // addHotkeyBtn
            // 
            this.addHotkeyBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.addHotkeyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addHotkeyBtn.Font = new System.Drawing.Font("White Rabbit", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addHotkeyBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.addHotkeyBtn.Location = new System.Drawing.Point(5, 2);
            this.addHotkeyBtn.Margin = new System.Windows.Forms.Padding(2);
            this.addHotkeyBtn.Name = "addHotkeyBtn";
            this.addHotkeyBtn.Size = new System.Drawing.Size(24, 23);
            this.addHotkeyBtn.TabIndex = 26;
            this.addHotkeyBtn.Text = "+";
            this.addHotkeyBtn.UseVisualStyleBackColor = false;
            this.addHotkeyBtn.Click += new System.EventHandler(this.addHotkeyBtn_Click);
            // 
            // colorsPage
            // 
            this.colorsPage.Controls.Add(this.addColorSchemeBtn);
            this.colorsPage.Controls.Add(this.colorSchemeSelectorCombo);
            this.colorsPage.Controls.Add(this.colorHighlighted2Btn);
            this.colorsPage.Controls.Add(this.colorHighlightedBtn);
            this.colorsPage.Controls.Add(this.colorSeek2Btn);
            this.colorsPage.Controls.Add(this.colorVolBar2Btn);
            this.colorsPage.Controls.Add(this.colorBackgroundBtn);
            this.colorsPage.Controls.Add(this.colorControlFrontBtn);
            this.colorsPage.Controls.Add(this.colorControlBackBtn);
            this.colorsPage.Controls.Add(this.colorTrackAlbumBtn);
            this.colorsPage.Controls.Add(this.colorTrackArtistBtn);
            this.colorsPage.Controls.Add(this.colorTrackTitleBtn);
            this.colorsPage.Controls.Add(this.colorVisualsBtn);
            this.colorsPage.Controls.Add(this.colorVolBarBtn);
            this.colorsPage.Controls.Add(this.colorSeekBtn);
            this.colorsPage.Location = new System.Drawing.Point(4, 22);
            this.colorsPage.Name = "colorsPage";
            this.colorsPage.Padding = new System.Windows.Forms.Padding(3);
            this.colorsPage.Size = new System.Drawing.Size(434, 236);
            this.colorsPage.TabIndex = 2;
            this.colorsPage.Text = "Colors";
            this.colorsPage.UseVisualStyleBackColor = true;
            this.colorsPage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.colorsPage_MouseDown);
            // 
            // addColorSchemeBtn
            // 
            this.addColorSchemeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.addColorSchemeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addColorSchemeBtn.Font = new System.Drawing.Font("White Rabbit", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addColorSchemeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.addColorSchemeBtn.Location = new System.Drawing.Point(330, 206);
            this.addColorSchemeBtn.Margin = new System.Windows.Forms.Padding(2);
            this.addColorSchemeBtn.Name = "addColorSchemeBtn";
            this.addColorSchemeBtn.Size = new System.Drawing.Size(97, 23);
            this.addColorSchemeBtn.TabIndex = 28;
            this.addColorSchemeBtn.Text = "Save preset";
            this.addColorSchemeBtn.UseVisualStyleBackColor = false;
            this.addColorSchemeBtn.Click += new System.EventHandler(this.addColorSchemeBtn_Click);
            // 
            // colorSchemeSelectorCombo
            // 
            this.colorSchemeSelectorCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.colorSchemeSelectorCombo.CausesValidation = false;
            this.colorSchemeSelectorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorSchemeSelectorCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorSchemeSelectorCombo.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorSchemeSelectorCombo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.colorSchemeSelectorCombo.FormattingEnabled = true;
            this.colorSchemeSelectorCombo.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.colorSchemeSelectorCombo.Location = new System.Drawing.Point(6, 10);
            this.colorSchemeSelectorCombo.Name = "colorSchemeSelectorCombo";
            this.colorSchemeSelectorCombo.Size = new System.Drawing.Size(156, 21);
            this.colorSchemeSelectorCombo.TabIndex = 16;
            this.colorSchemeSelectorCombo.SelectionChangeCommitted += new System.EventHandler(this.colorSchemeSelectorCombo_SelectedValueChanged);
            // 
            // colorHighlighted2Btn
            // 
            this.colorHighlighted2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorHighlighted2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorHighlighted2Btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorHighlighted2Btn.Location = new System.Drawing.Point(162, 130);
            this.colorHighlighted2Btn.Name = "colorHighlighted2Btn";
            this.colorHighlighted2Btn.Size = new System.Drawing.Size(50, 25);
            this.colorHighlighted2Btn.TabIndex = 12;
            this.colorHighlighted2Btn.Text = "track";
            this.colorHighlighted2Btn.UseVisualStyleBackColor = true;
            this.colorHighlighted2Btn.Click += new System.EventHandler(this.colorHighlighted2Btn_Click);
            // 
            // colorHighlightedBtn
            // 
            this.colorHighlightedBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorHighlightedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorHighlightedBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorHighlightedBtn.Location = new System.Drawing.Point(112, 130);
            this.colorHighlightedBtn.Name = "colorHighlightedBtn";
            this.colorHighlightedBtn.Size = new System.Drawing.Size(51, 25);
            this.colorHighlightedBtn.TabIndex = 11;
            this.colorHighlightedBtn.Text = "Playing";
            this.colorHighlightedBtn.UseVisualStyleBackColor = true;
            this.colorHighlightedBtn.Click += new System.EventHandler(this.colorHighlightBtn_Click);
            // 
            // colorSeek2Btn
            // 
            this.colorSeek2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorSeek2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorSeek2Btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorSeek2Btn.Location = new System.Drawing.Point(56, 37);
            this.colorSeek2Btn.Name = "colorSeek2Btn";
            this.colorSeek2Btn.Size = new System.Drawing.Size(50, 25);
            this.colorSeek2Btn.TabIndex = 10;
            this.colorSeek2Btn.Text = "bar";
            this.colorSeek2Btn.UseVisualStyleBackColor = true;
            this.colorSeek2Btn.Click += new System.EventHandler(this.colorSeek2Btn_Click);
            // 
            // colorVolBar2Btn
            // 
            this.colorVolBar2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorVolBar2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorVolBar2Btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorVolBar2Btn.Location = new System.Drawing.Point(56, 68);
            this.colorVolBar2Btn.Name = "colorVolBar2Btn";
            this.colorVolBar2Btn.Size = new System.Drawing.Size(50, 25);
            this.colorVolBar2Btn.TabIndex = 9;
            this.colorVolBar2Btn.Text = "bar";
            this.colorVolBar2Btn.UseVisualStyleBackColor = true;
            this.colorVolBar2Btn.Click += new System.EventHandler(this.colorVolBar2Btn_Click);
            // 
            // colorBackgroundBtn
            // 
            this.colorBackgroundBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorBackgroundBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorBackgroundBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorBackgroundBtn.Location = new System.Drawing.Point(112, 37);
            this.colorBackgroundBtn.Name = "colorBackgroundBtn";
            this.colorBackgroundBtn.Size = new System.Drawing.Size(100, 25);
            this.colorBackgroundBtn.TabIndex = 8;
            this.colorBackgroundBtn.Text = "Background";
            this.colorBackgroundBtn.UseVisualStyleBackColor = true;
            this.colorBackgroundBtn.Click += new System.EventHandler(this.colorBackgroundBtn_Click);
            // 
            // colorControlFrontBtn
            // 
            this.colorControlFrontBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorControlFrontBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorControlFrontBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorControlFrontBtn.Location = new System.Drawing.Point(112, 99);
            this.colorControlFrontBtn.Name = "colorControlFrontBtn";
            this.colorControlFrontBtn.Size = new System.Drawing.Size(100, 25);
            this.colorControlFrontBtn.TabIndex = 7;
            this.colorControlFrontBtn.Text = "Controls Front";
            this.colorControlFrontBtn.UseVisualStyleBackColor = true;
            this.colorControlFrontBtn.Click += new System.EventHandler(this.colorControlFrontBtn_Click);
            // 
            // colorControlBackBtn
            // 
            this.colorControlBackBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorControlBackBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorControlBackBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorControlBackBtn.Location = new System.Drawing.Point(112, 68);
            this.colorControlBackBtn.Name = "colorControlBackBtn";
            this.colorControlBackBtn.Size = new System.Drawing.Size(100, 25);
            this.colorControlBackBtn.TabIndex = 6;
            this.colorControlBackBtn.Text = "Controls Back";
            this.colorControlBackBtn.UseVisualStyleBackColor = true;
            this.colorControlBackBtn.Click += new System.EventHandler(this.colorControlBackBtn_Click);
            // 
            // colorTrackAlbumBtn
            // 
            this.colorTrackAlbumBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorTrackAlbumBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorTrackAlbumBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorTrackAlbumBtn.Location = new System.Drawing.Point(6, 192);
            this.colorTrackAlbumBtn.Name = "colorTrackAlbumBtn";
            this.colorTrackAlbumBtn.Size = new System.Drawing.Size(100, 25);
            this.colorTrackAlbumBtn.TabIndex = 5;
            this.colorTrackAlbumBtn.Text = "Track Album";
            this.colorTrackAlbumBtn.UseVisualStyleBackColor = true;
            this.colorTrackAlbumBtn.Click += new System.EventHandler(this.colorTrackAlbumBtn_Click);
            // 
            // colorTrackArtistBtn
            // 
            this.colorTrackArtistBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorTrackArtistBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorTrackArtistBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorTrackArtistBtn.Location = new System.Drawing.Point(6, 161);
            this.colorTrackArtistBtn.Name = "colorTrackArtistBtn";
            this.colorTrackArtistBtn.Size = new System.Drawing.Size(100, 25);
            this.colorTrackArtistBtn.TabIndex = 4;
            this.colorTrackArtistBtn.Text = "Track Artist";
            this.colorTrackArtistBtn.UseVisualStyleBackColor = true;
            this.colorTrackArtistBtn.Click += new System.EventHandler(this.colorTrackArtistBtn_Click);
            // 
            // colorTrackTitleBtn
            // 
            this.colorTrackTitleBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorTrackTitleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorTrackTitleBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorTrackTitleBtn.Location = new System.Drawing.Point(6, 130);
            this.colorTrackTitleBtn.Name = "colorTrackTitleBtn";
            this.colorTrackTitleBtn.Size = new System.Drawing.Size(100, 25);
            this.colorTrackTitleBtn.TabIndex = 3;
            this.colorTrackTitleBtn.Text = "Track Title";
            this.colorTrackTitleBtn.UseVisualStyleBackColor = true;
            this.colorTrackTitleBtn.Click += new System.EventHandler(this.colorTrackTitleBtn_Click);
            // 
            // colorVisualsBtn
            // 
            this.colorVisualsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorVisualsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorVisualsBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorVisualsBtn.Location = new System.Drawing.Point(6, 99);
            this.colorVisualsBtn.Name = "colorVisualsBtn";
            this.colorVisualsBtn.Size = new System.Drawing.Size(100, 25);
            this.colorVisualsBtn.TabIndex = 2;
            this.colorVisualsBtn.Text = "Visualisation";
            this.colorVisualsBtn.UseVisualStyleBackColor = true;
            this.colorVisualsBtn.Click += new System.EventHandler(this.colorVisualsBtn_Click);
            // 
            // colorVolBarBtn
            // 
            this.colorVolBarBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorVolBarBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorVolBarBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorVolBarBtn.Location = new System.Drawing.Point(6, 68);
            this.colorVolBarBtn.Name = "colorVolBarBtn";
            this.colorVolBarBtn.Size = new System.Drawing.Size(51, 25);
            this.colorVolBarBtn.TabIndex = 1;
            this.colorVolBarBtn.Text = "Vol";
            this.colorVolBarBtn.UseVisualStyleBackColor = true;
            this.colorVolBarBtn.Click += new System.EventHandler(this.colorVolBarBtn_Click);
            // 
            // colorSeekBtn
            // 
            this.colorSeekBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.colorSeekBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorSeekBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorSeekBtn.Location = new System.Drawing.Point(6, 37);
            this.colorSeekBtn.Name = "colorSeekBtn";
            this.colorSeekBtn.Size = new System.Drawing.Size(51, 25);
            this.colorSeekBtn.TabIndex = 0;
            this.colorSeekBtn.Text = "Seek";
            this.colorSeekBtn.UseVisualStyleBackColor = true;
            this.colorSeekBtn.Click += new System.EventHandler(this.colorSeekBtn_Click);
            // 
            // reshuffleCheckbox
            // 
            this.reshuffleCheckbox.AutoSize = true;
            this.reshuffleCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reshuffleCheckbox.Location = new System.Drawing.Point(5, 144);
            this.reshuffleCheckbox.Name = "reshuffleCheckbox";
            this.reshuffleCheckbox.Size = new System.Drawing.Size(118, 17);
            this.reshuffleCheckbox.TabIndex = 36;
            this.reshuffleCheckbox.Text = "Shuffle after list loop";
            this.reshuffleCheckbox.UseVisualStyleBackColor = true;
            this.reshuffleCheckbox.CheckedChanged += new System.EventHandler(this.reshuffleCheckbox_CheckedChanged);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 262);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Options";
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.generalPage.ResumeLayout(false);
            this.generalPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fontSizePercentTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpsBar)).EndInit();
            this.hotkeysPage.ResumeLayout(false);
            this.hotkeysPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transposeIncrementChanger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volIncrementChanger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hotkeyDgv)).EndInit();
            this.colorsPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage generalPage;
        private System.Windows.Forms.TabPage hotkeysPage;
        private System.Windows.Forms.CheckBox popupCheckbox;
        private System.Windows.Forms.TabPage colorsPage;
        private System.Windows.Forms.Label midiTxt;
        private ModifiedButton button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar fpsBar;
        private System.Windows.Forms.TextBox fpsText;
        private System.Windows.Forms.DataGridView hotkeyDgv;
        private ModifiedButton colorSeekBtn;
        private ModifiedButton colorVisualsBtn;
        private ModifiedButton colorVolBarBtn;
        private System.Windows.Forms.CheckBox autoShuffleCheckbox;
        private System.Windows.Forms.CheckBox autoAdvanceCheckbox;
        private System.Windows.Forms.CheckBox saveTransposeCheckbox;
        private ModifiedButton colorTrackAlbumBtn;
        private ModifiedButton colorTrackArtistBtn;
        private ModifiedButton colorTrackTitleBtn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Function;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mod;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private ModifiedButton removeHotkeyBtn;
        private ModifiedButton addHotkeyBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown transposeIncrementChanger;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown volIncrementChanger;
        private ModifiedButton colorControlFrontBtn;
        private ModifiedButton colorControlBackBtn;
        private ModifiedButton colorBackgroundBtn;
        private ModifiedButton colorVolBar2Btn;
        private ModifiedButton colorSeek2Btn;
        private ModifiedButton colorHighlighted2Btn;
        private ModifiedButton colorHighlightedBtn;
        private ModifiedComboBox colorSchemeSelectorCombo;
        private ModifiedButton addColorSchemeBtn;
        private System.Windows.Forms.Label fontTxt;
        private ModifiedButton fontChangeBtn;
        private System.Windows.Forms.Label fontSizePercentTxt;
        private System.Windows.Forms.TrackBar fontSizePercentTrack;
        private System.Windows.Forms.TextBox fontPctTxt;
        private System.Windows.Forms.CheckBox showWaveformCheckbox;
        private ModifiedButton font2ChangeBtn;
        private System.Windows.Forms.Label font2Txt;
        private ModifiedButton setDefaultsBtn;
        private System.Windows.Forms.CheckBox suppressHotkeyCheck;
        private System.Windows.Forms.CheckBox normalizeCheckbox;
        private System.Windows.Forms.CheckBox reshuffleCheckbox;
    }
}