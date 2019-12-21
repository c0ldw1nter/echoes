using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Echoes
{
    public partial class Options : Form
    {
        OptionsHotkeyDialog optsDlg;
        List<string> colorSchemes;
        bool txtPercentageBarChanged = false;
        public Options()
        {
            ForeColor = Program.mainWindow.ForeColor;
            BackColor = Program.mainWindow.BackColor;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Program.mainWindow.ForeColor;
                c.BackColor = Program.mainWindow.BackColor;
                if (c is TabPage)
                {
                    foreach (Control cc in c.Controls)
                    {
                        cc.ForeColor = Program.mainWindow.ForeColor;
                        cc.BackColor = Program.mainWindow.BackColor;
                    }
                }
            }
            InitializeComponent();
            hotkeyDgv.ColumnHeadersDefaultCellStyle.BackColor = Program.mainWindow.controlBackColor.Lighten();
            hotkeyDgv.ColumnHeadersDefaultCellStyle.ForeColor = Program.mainWindow.controlForeColor;
            hotkeyDgv.DefaultCellStyle = Program.mainWindow.defaultCellStyle;
            hotkeyDgv.AlternatingRowsDefaultCellStyle = Program.mainWindow.alternatingCellStyle;
            hotkeyDgv.AutoGenerateColumns = false;
            LoadValues();
            LoadColorSchemes();
            RefreshMidiText();
            LoadHotkeys();
            LoadColors();
        }

        void LoadColorSchemes()
        {
            colorSchemes = new List<string>();
            XDocument xml;
            try
            {
                xml = XDocument.Load(Program.mainWindow.configXmlFileLocation);
            }
            catch (Exception)
            {
                return;
            }
            if (xml.Root.Element("colorSchemes") == null) return;
            foreach (XElement ele in xml.Root.Element("colorSchemes").Elements())
            {
                colorSchemes.Add(ele.Name.ToString());
            }
            colorSchemeSelectorCombo.DataSource = colorSchemes;
        }

        void LoadColors()
        {
            colorSeekBtn.BackColor = Program.mainWindow.seekBarForeColor;
            colorSeek2Btn.BackColor = Program.mainWindow.seekBarBackColor;
            colorVolBarBtn.BackColor = Program.mainWindow.volBarForeColor;
            colorVolBar2Btn.BackColor = Program.mainWindow.volBarBackColor;
            colorVisualsBtn.BackColor = Program.mainWindow.spectrumColor;
            colorTrackTitleBtn.BackColor = Program.mainWindow.trackTitleColor;
            colorTrackArtistBtn.BackColor = Program.mainWindow.trackArtistColor;
            colorTrackAlbumBtn.BackColor = Program.mainWindow.trackAlbumColor;
            colorControlBackBtn.BackColor = Program.mainWindow.controlBackColor;
            colorControlFrontBtn.BackColor = Program.mainWindow.controlForeColor;
            colorBackgroundBtn.BackColor = Program.mainWindow.backgroundColor;
            colorHighlightedBtn.BackColor = Program.mainWindow.highlightBackColor;
            colorHighlighted2Btn.BackColor = Program.mainWindow.highlightForeColor;
            foreach (Control ctrl in colorsPage.Controls)
            {
                if (ctrl is ModifiedControls.ModifiedButton)
                {
                    ((ModifiedControls.ModifiedButton)ctrl).AdjustTextColor();
                }
            }
        }

        void LoadValues()
        {
            //disconnect checkbox event
            showWaveformCheckbox.CheckedChanged -= showWaveformCheckbox_CheckedChanged;

            fpsBar.Value = Program.mainWindow.visualfps;
            fpsText.Text = "" + fpsBar.Value;
            popupCheckbox.Checked = Program.mainWindow.trackChangePopup;
            autoAdvanceCheckbox.Checked = Program.mainWindow.autoAdvance;
            autoShuffleCheckbox.Checked = Program.mainWindow.autoShuffle;
            normalizeCheckbox.Checked = Program.mainWindow.normalize;
            saveTransposeCheckbox.Checked = Program.mainWindow.saveTranspose;
            showWaveformCheckbox.Checked = Program.mainWindow.showWaveform;
            suppressHotkeyCheck.Checked = Program.mainWindow.suppressHotkeys;
            transposeIncrementChanger.Value = (decimal)Program.mainWindow.hotkeyTransposeIncrement;
            volIncrementChanger.Value = (decimal)Program.mainWindow.hotkeyVolumeIncrement * 100;
            fontPctTxt.Text = Program.mainWindow.fontSizePercentage + "%";
            reshuffleCheckbox.Checked = Program.mainWindow.reshuffleAfterListLoop;
            RefreshFontText();

            //reconnect event
            showWaveformCheckbox.CheckedChanged += showWaveformCheckbox_CheckedChanged;
        }

        void RefreshFontText()
        {
            fontTxt.Text = "Font 1: "+Program.mainWindow.font1.FontFamily.Name;
            font2Txt.Text = "Font 2: " + Program.mainWindow.font2.FontFamily.Name;
        }

        void RefreshMidiText()
        {
            midiTxt.Text = "Midi soundfont: ";
            if (String.IsNullOrWhiteSpace(Program.mainWindow.midiSfLocation)) midiTxt.Text += "None";
            midiTxt.Text += Path.GetFileName(Program.mainWindow.midiSfLocation);
        }

        void LoadHotkeys()
        {
            hotkeyDgv.Rows.Clear();
            foreach (HotkeyData hkd in Program.mainWindow.hotkeys)
            {
                hotkeyDgv.Rows.Add(new object[] {hkd.enabled, hkd.hotkey.ToString(), hkd.ModToString(), hkd.key.ToString()});
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Midi soundfont";
            dlg.Filter = "Soundfont (*.sf2)|*.sf2";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                Program.mainWindow.midiSfLocation = dlg.FileName.ToString();
                Program.mainWindow.InitMidi();
            }
            RefreshMidiText();
            dlg.Dispose();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This event is called once for each tab button in your tab control

            // First paint the background with a color based on the current tab

            // e.Index is the index of the tab in the TabPages collection.

            e.Graphics.FillRectangle(new SolidBrush(Program.mainWindow.controlBackColor), e.Bounds);

            // Then draw the current tab button text 
            Rectangle paddedBounds = e.Bounds;
            paddedBounds.Inflate(-2, -2);
            e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, this.Font, SystemBrushes.HighlightText, paddedBounds);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Program.mainWindow.visualfps = fpsBar.Value;
            Program.mainWindow.progressRefreshTimer.Interval = 1000/Program.mainWindow.visualfps;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            fpsText.Text = ""+fpsBar.Value;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.trackChangePopup = popupCheckbox.Checked;
        }

        private void hotkeyDgv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int rowNum=hotkeyDgv.HitTest(e.Location.X,e.Location.Y).RowIndex;
            if (rowNum < 0) return;
            if (optsDlg != null) optsDlg.Dispose();
            optsDlg = new OptionsHotkeyDialog(Program.mainWindow.hotkeys[rowNum].hotkey, Program.mainWindow.hotkeys[rowNum].ctrl, Program.mainWindow.hotkeys[rowNum].alt, Program.mainWindow.hotkeys[rowNum].shift, (int)Program.mainWindow.hotkeys[rowNum].key, Program.mainWindow.hotkeys[rowNum].enabled);
            if (optsDlg.ShowDialog(this) == DialogResult.OK)
            {
                Program.mainWindow.hotkeys[rowNum] = optsDlg.returnedData;
                LoadHotkeys();
            }
            optsDlg.Hide();
            optsDlg.Dispose();
        }

        private void hotkeyDgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                Program.mainWindow.hotkeys[e.RowIndex].enabled = (bool)(hotkeyDgv.Rows[e.RowIndex].Cells[0].Value);
                Program.mainWindow.SetHotkeySuppression();
            }
        }

        private void hotkeyDgv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ContextMenu cm = new ContextMenu();
            MenuItem miDefaultHotkeys = new MenuItem("Restore default hotkeys");
            miDefaultHotkeys.Click += (theSender, eventArgs) =>
            {
                Program.mainWindow.hotkeys = Program.defaultHotkeys.Copy();
                Program.mainWindow.SetHotkeySuppression();
                LoadHotkeys();
            };
            cm.MenuItems.Add(miDefaultHotkeys);
            cm.Show(hotkeyDgv, e.Location);
        }

        Color GetColorFromDialog(Color current)
        {
            ColorDialog cdlg = new ColorDialog();
            cdlg.Color = current;
            cdlg.FullOpen = true;
            if (cdlg.ShowDialog(this) == DialogResult.OK)
            {
                return cdlg.Color;
            }
            else
            {
                return current;
            }
        }

        private void colorSeekBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.seekBarForeColor = GetColorFromDialog(Program.mainWindow.seekBarForeColor);
            LoadColors();
        }

        private void colorVolBarBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.volBarForeColor = GetColorFromDialog(Program.mainWindow.volBarForeColor);
            Program.mainWindow.volumeBar.Refresh();
            LoadColors();
        }

        private void colorVisualsBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.spectrumColor = GetColorFromDialog(Program.mainWindow.spectrumColor);
            LoadColors();
        }

        private void colorsPage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ContextMenu cm = new ContextMenu();
            MenuItem miDefaultHotkeys = new MenuItem("Restore default colors");
            miDefaultHotkeys.Click += (theSender, eventArgs) =>
            {
                Program.mainWindow.SetDefaultColors();
                Program.mainWindow.SetColors();
                LoadColors();
            };
            cm.MenuItems.Add(miDefaultHotkeys);
            cm.Show(colorsPage, e.Location);
        }

        private void autoShuffleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.autoShuffle = autoShuffleCheckbox.Checked;
        }

        private void saveTransposeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.saveTranspose = saveTransposeCheckbox.Checked;
        }

        private void colorTrackTitleBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.trackTitleColor = GetColorFromDialog(Program.mainWindow.trackTitleColor);
            LoadColors();
        }

        private void colorTrackArtistBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.trackArtistColor = GetColorFromDialog(Program.mainWindow.trackArtistColor);
            LoadColors();
        }

        private void colorTrackAlbumBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.trackAlbumColor = GetColorFromDialog(Program.mainWindow.trackAlbumColor);
            LoadColors();
        }

        private void addHotkeyBtn_Click(object sender, EventArgs e)
        {
            if (optsDlg != null) optsDlg.Dispose();
            optsDlg = new OptionsHotkeyDialog(Hotkey.PLAYPAUSE, false, false, false, 0, true);
            if (optsDlg.ShowDialog(this) == DialogResult.OK)
            {
                Program.mainWindow.hotkeys.Add(optsDlg.returnedData);
                Program.mainWindow.SetHotkeySuppression();
                LoadHotkeys();
            }
            optsDlg.Hide();
            optsDlg.Dispose();
        }

        private void removeHotkeyBtn_Click(object sender, EventArgs e)
        {
            List<HotkeyData> toremove = new List<HotkeyData>();
            foreach (DataGridViewRow rw in hotkeyDgv.SelectedRows)
            {
                toremove.Add(Program.mainWindow.hotkeys[rw.Index]);
            }
            foreach (HotkeyData hk in toremove) Program.mainWindow.hotkeys.Remove(hk);
            Program.mainWindow.SetHotkeySuppression();
            LoadHotkeys();
        }

        private void hotkeyDgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) removeHotkeyBtn_Click(null, null);
        }

        private void volIncrementChanger_ValueChanged(object sender, EventArgs e)
        {
            Program.mainWindow.hotkeyVolumeIncrement = (float)volIncrementChanger.Value/100;
        }

        private void transposeIncrementChanger_ValueChanged(object sender, EventArgs e)
        {
            Program.mainWindow.hotkeyTransposeIncrement = (float)transposeIncrementChanger.Value;
        }

        private void rehookBtn_Click(object sender, EventArgs e)
        {
        }

        private void colorControlBackBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.controlBackColor = GetColorFromDialog(Program.mainWindow.controlBackColor);
            LoadColors();
            Program.mainWindow.SetColors();
        }

        private void colorControlFrontBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.controlForeColor = GetColorFromDialog(Program.mainWindow.controlForeColor);
            LoadColors();
            Program.mainWindow.SetColors();
        }

        private void colorBackgroundBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.backgroundColor = GetColorFromDialog(Program.mainWindow.backgroundColor);
            LoadColors();
            Program.mainWindow.SetColors();
        }

        private void colorSeek2Btn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.seekBarBackColor = GetColorFromDialog(Program.mainWindow.seekBarBackColor);
            LoadColors();
        }

        private void colorVolBar2Btn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.volBarBackColor = GetColorFromDialog(Program.mainWindow.volBarBackColor);
            LoadColors();
        }

        private void colorHighlightBtn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.highlightBackColor = GetColorFromDialog(Program.mainWindow.highlightBackColor);
            LoadColors();
            Program.mainWindow.SetColors();
        }

        private void colorHighlighted2Btn_Click(object sender, EventArgs e)
        {
            Program.mainWindow.highlightForeColor = GetColorFromDialog(Program.mainWindow.highlightForeColor);
            LoadColors();
            Program.mainWindow.SetColors();
        }

        private void colorSchemeSelectorCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            Program.mainWindow.LoadXmlColorScheme(colorSchemeSelectorCombo.SelectedValue.ToString());
            LoadColors();
        }

        private void addColorSchemeBtn_Click(object sender, EventArgs e)
        {
            string newName = "Color scheme";
            int tryInt=0;
            while (colorSchemes.Contains(newName))
            {
                tryInt++;
                newName = "Color scheme" + tryInt;
            }
            EnterTextForm form = new EnterTextForm("Enter name", newName);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                if (colorSchemes.Contains(form.enteredText.Text))
                {
                    if (MessageBox.Show("Such a preset already exists. Overwrite?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        Program.mainWindow.SaveXmlColorScheme(form.enteredText.Text);
                        LoadColorSchemes();
                    }
                }
                else
                {
                    Program.mainWindow.SaveXmlColorScheme(form.enteredText.Text);
                    LoadColorSchemes();
                }
            }
            form.Hide();
            form.Dispose();
        }

        private void fontChangeBtn_Click(object sender, EventArgs e)
        {
            FontDialog dlg = new FontDialog();
            dlg.Font = Program.mainWindow.font1;
            try {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Program.mainWindow.font1 = new Font(dlg.Font.FontFamily, Program.mainWindow.font1.Size, dlg.Font.Style);
                    RefreshFontText();
                    Program.mainWindow.SetFonts();
                }
                        }
            catch (ArgumentException) { MessageBox.Show("Font not supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            dlg.Dispose();
        }

        private void fontSizePercentTrack_ValueChanged(object sender, EventArgs e)
        {
            fontPctTxt.Text = "" + fontSizePercentTrack.Value+"%";
            txtPercentageBarChanged = true;
        }

        private void showWaveformCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (showWaveformCheckbox.Checked)
            {
                Program.mainWindow.waveform = null;
                Program.mainWindow.CreateWaveform();
            }
            Program.mainWindow.showWaveform = showWaveformCheckbox.Checked;
        }

        private void font2ChangeBtn_Click(object sender, EventArgs e)
        {
            FontDialog dlg = new FontDialog();
            dlg.Font = Program.mainWindow.font2;
            try
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Program.mainWindow.font2 = new Font(dlg.Font.FontFamily, Program.mainWindow.font2.Size, dlg.Font.Style);
                    RefreshFontText();
                    Program.mainWindow.SetFonts();
                }
            }
            catch (ArgumentException) { MessageBox.Show("Font not supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            dlg.Dispose();
        }

        private void fontSizePercentTrack_MouseUp(object sender, MouseEventArgs e)
        {
            if (txtPercentageBarChanged)
            {
                Program.mainWindow.fontSizePercentage = fontSizePercentTrack.Value;
                Program.mainWindow.SetFonts();
                txtPercentageBarChanged = false;
            }
        }

        private void setDefaultsBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Set default settings? This can't be undone.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Program.mainWindow.SetDefaultGeneral();
                LoadValues();
            }
        }

        private void suppressHotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.suppressHotkeys = suppressHotkeyCheck.Checked;
            Program.mainWindow.SetHotkeySuppression();
        }

        private void normalizeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.normalize = normalizeCheckbox.Checked;
            if (normalizeCheckbox.Checked) Program.mainWindow.Normalize();
        }

        private void reshuffleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Program.mainWindow.reshuffleAfterListLoop = reshuffleCheckbox.Checked;
        }
    }
}
