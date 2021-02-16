using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using SuperM3U;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Midi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Wma;
using Un4seen.Bass.AddOn.Mix;

namespace Echoes
{
    /* add more formats, 
     * make background worker not overflow, 
     * add customizable columns, 
     * make options window, 
     * allow tag edit while playing, 
     * loopTrack on mods not working
     * stereo/mono thing not working, probably gotta use mixer
     * playlist doesnt renumber correctly after adding
     * tags wont load if nowplaying is the song (re-entering playlist)
     * make ui get smaller/lightweight
     * add choice to insta-save playlist after deletion/insertion or need to click save*/

    /*options: 
     * add new track to top/bottom, 
     * hardware mixing, 
     * different hotkeys with different params
     * load next playlist after this one finishes*/

    // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
    // Compute the addition of each combination of the keys you want to be pressed
    // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...

    #region Enums
    public enum Hotkey
    {
        ADVANCEPLAYER, PREVIOUSPLAYER, PLAYPAUSE, VOLUMEUP, VOLUMEDOWN, TRANSPOSEUP, TRANSPOSEDOWN, DELETE, GLOBAL_VOLUMEUP, GLOBAL_VOLUMEDOWN, NEXTLIST, PREVLIST, SHUFFLE, REWIND, SKIPFORWARD, SKIPBACKWARD
    }
    public enum ItemType
    {
        Playlist, Track
    }
    public enum Modifier
    {
        NONE, CTRL, ALT, SHIFT
    }
    #endregion

    public partial class Echoes : Form
    {
        /*#region DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion*/
        //public int[] modValues = { 0, 1, 2, 4, 8 };

        #region Settings values
        /*To add a new setting value:
         * 1. define here; 
         * 2. define a default in Program class; 
         * 3. set it to default in SetDefaults() and equivalent;
         * 4. load it in LoadXmlConfig();
         * 5. save it in SaveXmlConfig();
         * 6. add a control of it to Options form;
        */

        Settings settings;

        //colors
        public Color backgroundColor;
        public Color controlBackColor;
        public Color controlForeColor;
        public Color highlightBackColor;
        public Color highlightForeColor;
        public Color seekBarBackColor;
        public Color seekBarForeColor;
        public Color volBarBackColor;
        public Color volBarForeColor;
        public Color trackTitleColor;
        public Color trackArtistColor;
        public Color trackAlbumColor;
        public Color spectrumColor;

        //general settings
        public Font font1;
        public Font font2;
        public int fontSizePercentage = 100;
        public int visualisationStyle;
        public int visualfps;
        public int repeat;
        public int converterSelectedBitrateIndex;
        public float volume;
        public float hotkeyVolumeIncrement;
        public float hotkeyTransposeIncrement;
        public bool saveTranspose;
        public bool autoShuffle;
        public bool autoAdvance;
        public bool reshuffleAfterListLoop;
        public bool trackChangePopup;
        public bool showWaveform;
        public bool normalize;
        public bool suppressHotkeys;
        public List<ColumnInfo> currentColumnInfo;
        public string midiSfLocation;

        //hotkey settings
        public bool hotkeysAllowed;
        public List<HotkeyData> hotkeys;

        #endregion

        #region Vars
        public string playlistDbLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "known_playlists.dat");
        public string tagsCacheLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "track_cache.xml");
        public string configXmlFileLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "config.xml");
        public string lameExeLocation = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "lame.exe");
        public string lameExeDownloadSite = "http://www.rarewares.org/mp3-lame-bundle.php";

        public XmlCacher xmlCacher;

        public int stream;
        public int mixer;
        public bool confirmSaveFlag = false;
        public bool streamLoaded = false;
        bool reinitTagLoader = false;
        public SYNCPROC endSync, stallSync;
        public BASS_MIDI_FONT[] midiFonts;
        public Visuals vs = new Un4seen.Bass.Misc.Visuals();
        int[] _fxEQ = { 0, 0, 0 };
        bool playlistChanged = false;
        public bool eqEnabled = false;
        int peak = 0;
        public float normGain = 0;
        public float[] eqGains = { 8f, -8f, 14f };
        bool normalizerRestart = false;
        public DSP_Gain DSPGain;
        Track toNormalize;
        System.Timers.Timer normalizeSmoother;
        bool normalizeSmoothingStop = false;
        float normalizeSmoothingTickSize;

        public Track nowPlaying = null;
        private Track trackToLoad;
        Track trackToReload;
        public List<Track> playlist = new List<Track>();
        public List<string> supportedModuleTypes = new List<string>() { ".mo3", ".it", ".xm", ".s3m", ".mtm", ".mod", ".umx" };
        public List<string> supportedAudioTypes = new List<string>() { ".mp3", ".mp2", ".mp1", ".wav", ".ogg", ".wma", ".m4a", ".flac" };
        public List<string> supportedWaveformTypes = new List<String>() { ".mp3", ".mp2", ".mp1", ".wav" };
        public List<string> supportedMidiTypes = new List<string>() { ".midi", ".mid" };

        Converter theConverter;
        TagEditor te;
        DupeFinder df;
        Statistics stats;
        Options opts;
        Equalizer eqWindow;
        FloatingOSDWindow popupWindow = new FloatingOSDWindow();
        KeyboardHook kh;
        DataGridViewRow highlightedRow;
        DataGridViewColumn highlightedColumn;
        Stopwatch timeListenedTracker = new Stopwatch();
        public WaveForm wf;
        public Bitmap waveformImage;
        ItemType displayedItems;

        List<string> knownPlaylists = new List<string>();
        List<string> playlistNames = new List<string>();
        List<int> savedSelectionForSort;

        string currentPlaylist = null;
        string gridSearchWord = "";

        bool draggingSeek = false;
        bool draggingVolume = false;
        bool wasPlaying = false;
        bool advanceFlag = false;
        bool gridSearchFirstStroke = true;
        bool sortingAllowed = true;
        bool showingPlayIcon = true;
        bool reloadTagsFlag = false;

        public IntPtr theHandle;

        int gridSearchTimerRemaining = 0;
        int gridSearchReset = 30;
        public int waveformCompleted = 0;
        public short[] waveform;
        private AutoResetEvent ResetEvent = new AutoResetEvent(false);

        public DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
        public DataGridViewCellStyle alternatingCellStyle = new DataGridViewCellStyle();
        public DataGridViewCellStyle highlightedCellStyle = new DataGridViewCellStyle();

        //                                        0      1         2        3          4         5            6        7        8         9         10             11           12         13        14        15             16
        readonly string[] COLUMN_PROPERTIES = { "num", "title", "artist", "length", "album", "listened", "filename", "year", "genre", "comment", "bitrate", "trackNumber", "lastOpened", "size", "format", "trueBitrate", "timesListened" };
        readonly string[] COLUMN_HEADERS = { "#", "Title", "Artist", "Length", "Album", "Listened", "File", "Year", "Genre", "Comment", "Bitrate", "Track #", "Last opened", "Size", "Format", "True Bitrate", "Times listened" };
        readonly string[] COLUMN_HEADERS_ALLOWED_CUSTOM = { "Title", "Artist", "Length", "Album", "Year", "Genre", "Comment", "Bitrate", "Format" };
        #endregion

        public Echoes()
        {
            vs.MaxFrequencySpectrum = Utils.FFTFrequency2Index(19200, 2047, 44100);
            xmlCacher = new XmlCacher(tagsCacheLocation);
            /*List<Track> trx = xmlCacher.GetAllTracks();
            xmlCacher.AddOrUpdate(trx);*/

            InitializeComponent();

            kh = new KeyboardHook();
            kh.Install();
            kh.KeyDown += kh_KeyDown;

            new ToolTip().SetToolTip(openBtn, "Open file");
            new ToolTip().SetToolTip(exportBtn, "Export playlist");
            new ToolTip().SetToolTip(optionsBtn, "Options");
            new ToolTip().SetToolTip(repeatBtn, "Loop (none/list/track)");
            new ToolTip().SetToolTip(shuffleBtn, "Shuffle");

            theHandle = Handle;

            supportedAudioTypes.AddRange(supportedModuleTypes);
            supportedAudioTypes.AddRange(supportedMidiTypes);

            DoubleBuffered = true;

            seekBar.Minimum = 0;
            seekBar.Maximum = 400;

            trackGrid.AutoGenerateColumns = false;
            trackGrid.ScrollBars = ScrollBars.Both;
            trackGrid.Font = font1;
            trackGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            trackGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            trackGrid.RowTemplate.Height = 18;
            trackGrid.DoubleBuffered(true);

            Form1_Resize(null, null);
            LoadXmlConfig();

            if (visualfps < 1) visualfps = 1;
            progressRefreshTimer.Interval = 1000 / visualfps;
            progressRefreshTimer.Start();
            RepositionControls();

            SetColors();
            LoadPlaylistDb();
            StartupLoadProcedure();
        }

        void AddHeaderContextMenu()
        {
            ContextMenuStrip headerContextMenu = new ContextMenuStrip();
            for (int i = 0; i < COLUMN_HEADERS.Length; i++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(COLUMN_HEADERS[i]);
                if (currentColumnInfo.Where(x => x.id == i).Count() > 0) mi.Checked = true;
                mi.Name = i + "";
                mi.Click += (theSender, eventArgs) =>
                {
                    if (mi.Checked)
                    {
                        currentColumnInfo.Remove(currentColumnInfo.First(x => x.id == Int32.Parse(mi.Name)));
                    }
                    else
                    {
                        currentColumnInfo.Add(new ColumnInfo(Int32.Parse(mi.Name), 100));
                    }
                    RefreshPlaylistGrid();
                    mi.Checked = !mi.Checked;
                };
                headerContextMenu.Items.Add(mi);
            }
            ToolStripMenuItem mit = new ToolStripMenuItem("<Restore default>");
            mit.Click += (theSender, eventArgs) =>
            {
                currentColumnInfo = Program.defaultColumnInfo;
                RefreshPlaylistGrid();
            };
            headerContextMenu.Items.Add(mit);
            foreach (DataGridViewColumn clmn in trackGrid.Columns)
            {
                clmn.HeaderCell.ContextMenuStrip = headerContextMenu;
            }
        }

        public void RemoveEQ()
        {
            foreach (int i in _fxEQ)
            {
                Bass.BASS_ChannelRemoveFX(stream, i);
            }
        }

        public void DefineEQ()
        {
            if (!eqEnabled) return;
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            _fxEQ[0] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[1] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[2] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            eq.fBandwidth = 18f;
            eq.fCenter = 100f;
            eq.fGain = 0f;
            Bass.BASS_FXSetParameters(_fxEQ[0], eq);
            eq.fCenter = 1000f;
            Bass.BASS_FXSetParameters(_fxEQ[1], eq);
            eq.fCenter = 8000f;
            Bass.BASS_FXSetParameters(_fxEQ[2], eq);
        }

        public void ApplyEQ()
        {
            if (!eqEnabled) return;
            for (int i = 0; i < eqGains.Length; i++)
            {
                UpdateEQ(i, eqGains[i]);
            }
        }

        void UpdateEQ(int band, float gain)
        {
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            if (Bass.BASS_FXGetParameters(_fxEQ[band], eq))
            {
                eq.fGain = gain;
                Bass.BASS_FXSetParameters(_fxEQ[band], eq);
            }
        }

        void kh_KeyDown(KeyEventArgs e)
        {
            //System.Media.SystemSounds.Beep.Play();
            //if (e == null) return;
            foreach (HotkeyData hk in hotkeys)
            {
                if (hk.key == e.KeyCode && hk.ctrl == e.Control && hk.alt == e.Alt && hk.shift == e.Shift)
                {
                    DoHotkeyEvent(hk.hotkey);
                }
            }
        }

        void PointToMissingFile(Track t)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Audio files|";
            foreach (string s in supportedAudioTypes)
            {
                dlg.Filter += "*" + s + ";";
            }
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                if (supportedAudioTypes.Contains(Path.GetExtension(dlg.FileName).ToLower()))
                {
                    xmlCacher.ChangeFilename(t.filename, dlg.FileName);
                    t.filename = dlg.FileName;
                    t.GetTags();
                    if (MessageBox.Show("Do you want to look for other missing files in this directory?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FindMissingFilesIn(Path.GetDirectoryName(dlg.FileName), false);
                    }
                }
                else
                {
                    MessageBox.Show("File type not supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void FindMissingFilesIn(string path, bool inclSubdirs)
        {
            List<Track> missingTracks = playlist.Where(x => !File.Exists(x.filename)).ToList();
            SearchOption opt;
            if (inclSubdirs) opt = SearchOption.AllDirectories;
            else opt = SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles(path, "*", opt);
            int foundFiles = 0;
            foreach (Track t in missingTracks)
            {
                string match = files.FirstOrDefault(x => Path.GetFileName(x) == Path.GetFileName(t.filename));
                if (match != null)
                {
                    xmlCacher.ChangeFilename(t.filename, match);
                    t.filename = match;
                    t.GetTags();
                    foundFiles++;
                }
            }
            MessageBox.Show(foundFiles + "/" + missingTracks.Count + " missing tracks found." + Environment.NewLine + "Save the playlist.");
        }

        public void SetDefaultColors()
        {
            backgroundColor = Program.backgroundColorDefault.Copy();
            controlBackColor = Program.controlBackColorDefault.Copy();
            controlForeColor = Program.controlForeColorDefault.Copy();
            highlightBackColor = Program.highlightBackColorDefault.Copy();
            highlightForeColor = Program.highlightForeColorDefault.Copy();
            seekBarBackColor = Program.seekBarBackColorDefault.Copy();
            seekBarForeColor = Program.seekBarForeColorDefault.Copy();
            volBarBackColor = Program.volBarBackColorDefault.Copy();
            volBarForeColor = Program.volBarForeColorDefault.Copy();
            trackTitleColor = Program.trackTitleColorDefault.Copy();
            trackArtistColor = Program.trackArtistColorDefault.Copy();
            trackAlbumColor = Program.trackAlbumColorDefault.Copy();
            spectrumColor = Program.spectrumColorDefault.Copy();
            Refresh();
        }

        public void SetDefaultGeneral()
        {
            font1 = Program.font1Default.Copy();
            font2 = Program.font2Default.Copy();
            fontSizePercentage = 100;
            SetFonts();
            hotkeyVolumeIncrement = Program.hotkeyVolumeIncrementDefault;
            hotkeyTransposeIncrement = Program.hotkeyTransposeIncrementDefault;
            SetVolume(Program.volumeDefault);
            autoAdvance = Program.autoAdvanceDefault;
            autoShuffle = Program.autoShuffleDefault;
            trackChangePopup = Program.trackChangePopupDefault;
            currentColumnInfo = Program.defaultColumnInfo.Copy();
            saveTranspose = Program.saveTransposeDefault;
            repeat = Program.repeatDefault;
            normalize = Program.normalizeDefault;
            visualisationStyle = Program.visualisationStyleDefault;
            visualfps = Program.visualfpsDefault;
            midiSfLocation = Program.midiSfLocationDefault;
            reshuffleAfterListLoop = Program.reshuffleAfterListLoopDefault;
        }


        public void SetDefaults()
        {
            //colors
            SetDefaultColors();

            //general
            SetDefaultGeneral();

            //hotkey settings
            hotkeysAllowed = Program.hotkeysAllowedDefault;
            suppressHotkeys = Program.suppressHotkeysDefault;
            hotkeys = Program.defaultHotkeys.Copy();
            SetHotkeySuppression();
        }

        void DoHotkeyEvent(Hotkey k)
        {
            if (k == Hotkey.ADVANCEPLAYER) AdvancePlayer();
            else if (k == Hotkey.PREVIOUSPLAYER) PlayerPrevious();
            else if (k == Hotkey.PLAYPAUSE)
            {
                if (IsPlaying())
                {
                    PausePlayer();
                }
                else
                {
                    Play();
                }
            }
            else if (k == Hotkey.VOLUMEUP) SetVolume(volume + hotkeyVolumeIncrement);
            else if (k == Hotkey.VOLUMEDOWN) SetVolume(volume - hotkeyVolumeIncrement);
            else if (k == Hotkey.TRANSPOSEUP)
            {
                if (transposeChangerNum.Value <= 11)
                {
                    transposeChangerNum.Value += (decimal)hotkeyTransposeIncrement;
                }
            }
            else if (k == Hotkey.TRANSPOSEDOWN)
            {
                if (transposeChangerNum.Value >= -11)
                {
                    transposeChangerNum.Value -= (decimal)hotkeyTransposeIncrement;
                }
            }
            else if (k == Hotkey.DELETE)
            {
                if (nowPlaying != null)
                {
                    int currRow = FindTrackRow(nowPlaying).Index;
                    if (trackGrid.Rows[currRow] == null) return;
                    DeleteTrack(new List<int> { nowPlaying.num });
                    if (trackGrid.Rows.Count - 1 >= currRow)
                    {
                        Track t = (Track)trackGrid.Rows[currRow].DataBoundItem;
                        LoadAudio(t);
                    }
                }
            }
            else if (k == Hotkey.GLOBAL_VOLUMEUP) AdjustGlobalVolume(0.01f);
            else if (k == Hotkey.GLOBAL_VOLUMEDOWN) AdjustGlobalVolume(-0.01f);
            else if (k == Hotkey.NEXTLIST && !tagsLoaderWorker.IsBusy)
            {
                if (playlistSelectorCombo.SelectedIndex < playlistSelectorCombo.Items.Count - 1)
                {
                    playlistSelectorCombo.SelectedIndex++;
                }
                else
                {
                    playlistSelectorCombo.SelectedIndex = 0;
                }
                comboBox1_SelectionChangeCommitted(null, null);
                if (displayedItems != ItemType.Playlist) PlayFirst();
            }
            else if (k == Hotkey.PREVLIST && !tagsLoaderWorker.IsBusy)
            {
                if (playlistSelectorCombo.SelectedIndex > 0)
                {
                    playlistSelectorCombo.SelectedIndex--;
                }
                else
                {
                    playlistSelectorCombo.SelectedIndex = playlistSelectorCombo.Items.Count - 1;
                }
                comboBox1_SelectionChangeCommitted(null, null);
                if (displayedItems != ItemType.Playlist) PlayFirst();
            }
            else if (k == Hotkey.SHUFFLE) Shuffle();
            else if (k == Hotkey.REWIND) SetPosition(0);
            else if (k == Hotkey.SKIPFORWARD) SkipSeconds(5);
            else if (k == Hotkey.SKIPBACKWARD) SkipSeconds(-5);
        }

        void RepositionControls()
        {
            trackText.Location = new Point(trackText.Location.X, playBtn.Location.Y + playBtn.Height + 5);
            seekBar.Location = new Point(seekBar.Location.X, trackText.Location.Y + trackText.Height + 5);
            searchBox.Location = new Point(searchBox.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            playlistInfoTxt.Location = new Point(searchBox.Location.X + searchBox.Width + 20, seekBar.Location.Y + seekBar.Height + 5);
            playlistInfoTxt.Width = playlistSelectorCombo.Location.X - 10 - playlistInfoTxt.Location.X;
            playlistInfoTxt.Height = playlistSelectorCombo.Height;
            playlistSelectorCombo.Location = new Point(playlistSelectorCombo.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            volumeBar.Location = new Point(volumeBar.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            volumeBar.Height = playlistSelectorCombo.Height;
            trackGrid.Location = new Point(trackGrid.Location.X, searchBox.Location.Y + searchBox.Height + 5);
            //Form1_Resize(null, null);
            //Refresh();
        }

        public void SkipSeconds(double sekonds)
        {
            bool negative = false;
            if (sekonds < 0)
            {
                negative = true;
                sekonds = -sekonds;
            }
            if (!streamLoaded) return;
            long secsInBytes = Bass.BASS_ChannelSeconds2Bytes(stream, sekonds);
            if (negative) secsInBytes = -secsInBytes;
            if (GetPosition() + secsInBytes > GetLength())
            {
                SetPosition(GetLength());
            }
            else if (GetPosition() + secsInBytes < 0)
            {
                SetPosition(0);
            }
            else
            {
                SetPosition(GetPosition() + secsInBytes);
            }
        }

        public void SetFonts()
        {
            trackText.Font = new Font(font1.FontFamily, 12 * (float)fontSizePercentage / 100, font1.Style);
            searchBox.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            playlistInfoTxt.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            playlistSelectorCombo.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            //trackGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            trackGrid.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            trackGrid.RowTemplate.Height = (int)(trackGrid.Font.Height * 1.5);
            foreach (DataGridViewRow rw in trackGrid.Rows)
            {
                rw.Height = trackGrid.RowTemplate.Height;
            }
            trackGrid.Invalidate();
            //trackGrid.RowTemplate.Height = (int)(trackGrid.Font.Height*1.5f);
            //trackGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            /*foreach (Control ctrl in this.Controls)
            {
                if (!(ctrl is Button) && !(ctrl is NumericUpDown) && ctrl.Name != "transposeTxt")
                {
                    ctrl.Font = new Font(mainFont.FontFamily, ctrl.Font.Size*(float)fontSizePercentage/100, mainFont.Style);
                }
            }*/
            RepositionControls();
            DisplayTrackInfo(nowPlaying);
            this.Refresh();
        }

        public void SetColors()
        {
            this.BackColor = backgroundColor;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label || ctrl is PictureBox || ctrl is TextBox && ctrl.Name != "searchBox")
                {
                    ctrl.BackColor = backgroundColor;
                }
                else
                {
                    ctrl.BackColor = controlBackColor;
                }
                if (ctrl is ModifiedControls.ModifiedComboBox)
                {
                    ((ModifiedControls.ModifiedComboBox)ctrl).HighlightColor = seekBarForeColor;
                }
                ctrl.ForeColor = controlForeColor;
                ctrl.Refresh();
            }
            //dgv
            defaultCellStyle.BackColor = controlBackColor;
            defaultCellStyle.ForeColor = controlForeColor;
            defaultCellStyle.SelectionBackColor = controlForeColor;
            defaultCellStyle.SelectionForeColor = controlBackColor;

            alternatingCellStyle.BackColor = controlBackColor.Darken();
            alternatingCellStyle.ForeColor = controlForeColor.Darken();
            alternatingCellStyle.SelectionBackColor = controlForeColor.Darken();
            alternatingCellStyle.SelectionForeColor = controlBackColor.Darken();

            highlightedCellStyle.ForeColor = highlightForeColor;
            highlightedCellStyle.BackColor = highlightBackColor;
            highlightedCellStyle.SelectionBackColor = controlForeColor.Darken().Darken();
            highlightedCellStyle.SelectionForeColor = highlightForeColor;

            trackGrid.ColumnHeadersDefaultCellStyle.BackColor = controlBackColor.Lighten();
            trackGrid.ColumnHeadersDefaultCellStyle.ForeColor = controlForeColor;

            trackGrid.BackgroundColor = controlBackColor;
            trackGrid.DefaultCellStyle = defaultCellStyle;
            trackGrid.AlternatingRowsDefaultCellStyle = alternatingCellStyle;
            trackGrid.Refresh();
        }

        void WaveFormCallback(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished)
        {
            if (finished && wf.FileName == nowPlaying.filename)
            {
                Invoke((MethodInvoker)delegate
                {
                    wf.SyncPlayback(stream);
                    if (showWaveform)
                    {
                        waveformImage = wf.CreateBitmap(Screen.FromControl(this).WorkingArea.Width * 2, this.seekBar.Height, -1, -1, true);
                        waveformImage = waveformImage.SetOpacity(0.5d);
                    }

                    if (normalize) Normalize();
                    volumeBar.Refresh();
                });
                // if your playback stream uses a different resolution than the WF
                // use this to sync them
            }
            else
            {
                if (showWaveform)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        waveformImage = wf.CreateBitmap(Screen.FromControl(this).WorkingArea.Width * 2, this.seekBar.Height, -1, -1, true);
                        waveformImage = waveformImage.SetOpacity(0.5d);
                    });
                }
            }
        }
        public void CreateWaveform()
        {
            if (wf != null && wf.IsRenderingInProgress) wf.RenderStop();
            waveformImage = null;
            if (nowPlaying == null) { return; }
            wf = new Un4seen.Bass.Misc.WaveForm();
            wf.FileName = nowPlaying.filename;
            wf.CallbackFrequency = 1000;
            var handler = new WAVEFORMPROC(WaveFormCallback);
            wf.NotifyHandler = handler;
            wf.ColorBackground = Color.Transparent;
            wf.DrawWaveForm = WaveForm.WAVEFORMDRAWTYPE.Mono;
            wf.ColorLeft = wf.ColorLeft2 = wf.ColorLeftEnvelope = Color.Black;

            // it is important to use the same resolution flags as for the playing stream
            // e.g. if an already playing stream is 32-bit float, so this should also be
            // or use 'SyncPlayback' as shown below
            int strm;
            if (!LoadStream(wf.FileName, out strm, BASSFlag.BASS_MUSIC_STOPBACK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_DECODE)) return;
            wf.RenderStart(strm, true);
            volumeBar.Refresh();
        }

        public bool IsPlaying()
        {
            return Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        /*protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312) {
                Hotkey k=(Hotkey)m.WParam.ToInt32();
                DoHotkeyEvent(k);
            }
            base.WndProc(ref m);
        }*/

        int PitchFreq(int cents, int freq)
        {
            return (int)(Math.Pow(2, (double)cents / 1200d) * freq);
        }

        public void SetFrequency()
        {
            if (streamLoaded) Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, (float)(PitchFreq((int)(transposeChangerNum.Value * 100), Bass.BASS_ChannelGetInfo(stream).freq)));
            seekBar.Refresh();
        }

        public void SetChannels(bool stereo)
        {
            if (!streamLoaded) return;
            if (stereo) Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_MONO);
            else Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_SAMPLE_MONO, BASSFlag.BASS_SAMPLE_MONO);
        }

        public void UpdateCache(List<Track> trs)
        {
            xmlCacher.AddOrUpdate(trs);
        }

        public void UpdateCache()
        {
            UpdateCache(playlist);
        }

        void SetListenedTime(Track track, int time)
        {
            track.listened = time;
            xmlCacher.AddOrUpdate(new List<Track> { track });
        }

        void LoadFiles(List<string> files)
        {
            //tag loader stuff
            if (tagsLoaderWorker.IsBusy)
            {
                tagsLoaderWorker.CancelAsync();
                LoadFiles(files);
                return;
            }
            //

            var loadingPlaylist = new List<Track>();
            string lastList = null;
            foreach (string file in files)
            {
                if (supportedAudioTypes.Contains(Path.GetExtension(file).ToLower()))
                {
                    loadingPlaylist.Add(new Track(file, Path.GetFileName(file)));
                }
                else if (Path.GetExtension(file).ToLower() == ".m3u")
                {
                    AddKnownPlaylist(file);
                    lastList = file;
                }
            }
            if (loadingPlaylist.Count == 0)
            {
                //there were no valid track files, load m3u if any
                if (lastList != null) ImportM3u(lastList);
                return;
            }

            //there were track files
            if (displayedItems != ItemType.Track)
            {
                //in playlist list mode, create new playlist
                LoadCustomPlaylist(loadingPlaylist);
                displayedItems = ItemType.Track;
                tagsLoaderWorker.RunWorkerAsync();
            }
            else
            {
                //in a created playlist, add tracks to it
                loadingPlaylist.Reverse();
                bool addDupes = false;
                bool askToAddDupes = true;
                foreach (Track t in loadingPlaylist)
                {
                    if (!addDupes && playlist.Where(x => x.filename == t.filename).Count() > 0)
                    {
                        if (askToAddDupes)
                        {
                            addDupes = MessageBox.Show("There are duplicates. Add them?", "Dupes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
                            askToAddDupes = false;
                            if (addDupes)
                            {
                                playlist.Insert(0, t);
                            }
                        }
                    }
                    else playlist.Insert(0, t);
                }
                FixPlaylistNumbers(playlist);
                RefreshPlaylistGrid();
                if (tagsLoaderWorker.IsBusy)
                {
                    reinitTagLoader = true;
                    tagsLoaderWorker.CancelAsync();
                }
                else
                {
                    tagsLoaderWorker.RunWorkerAsync();
                }
                playlistChanged = true;
            }
            RefreshTotalTimeLabel();
        }

        bool AskToQuitWorker()
        {
            if (!tagsLoaderWorker.IsBusy) return true;
            if (MessageBox.Show("Tag loading in progress. Any loaded tags will not be saved if you quit. Do you wish to quit?", "Loading", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return false;
            }
            return true;
        }

        void AskToSavePlaylistChanges()
        {
            if (!playlistChanged || displayedItems != ItemType.Track) return;
            if (MessageBox.Show("Playlist has unsaved changes. Save now?", "Playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                SavePlaylist();
            }
        }

        void StartupLoadProcedure()
        {
            string lastList = null;
            playlist = new List<Track>();
            foreach (string file in Program.filesToOpen)
            {
                if (supportedAudioTypes.Contains(Path.GetExtension(file).ToLower()))
                {
                    playlist.Add(new Track(file, Path.GetFileName(file)));
                }
                else if (Path.GetExtension(file).ToLower() == ".m3u")
                {
                    AddKnownPlaylist(file);
                    lastList = file;
                }
            }
            PopulatePlaylistList();
            if (playlist.Count > 0)
            {
                LoadCustomPlaylist(playlist);
                displayedItems = ItemType.Track;
                tagsLoaderWorker.RunWorkerAsync();
                Play();
            }
            else if (lastList != null) ImportM3u(lastList);
            else DisplayPlaylistsInGrid();
        }

        public void OpenFile(Track t)
        {
            if (Path.GetExtension(t.filename).ToLower() == ".m3u")
            {
                ImportM3u(t.filename);
            }
            else
            {
                LoadAudio(t);
            }
        }

        void OpenFile(string filename)
        {
            var match = playlist.First(x => x.filename == filename);
            if (match == null) OpenFile(new Track(filename, filename));
            else OpenFile(match);
        }

        private void RefreshPlaylistGrid()
        {
            trackGrid.Columns.Clear();
            List<DataGridViewTextBoxColumn> columns = new List<DataGridViewTextBoxColumn>();
            for (int i = 0; i < currentColumnInfo.Count; i++)
            {
                columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = currentColumnInfo[i].width,
                    DataPropertyName = COLUMN_PROPERTIES[currentColumnInfo[i].id],
                    Name = COLUMN_PROPERTIES[currentColumnInfo[i].id],
                    HeaderText = COLUMN_HEADERS[currentColumnInfo[i].id],
                    Tag = currentColumnInfo[i].id
                });
            }
            foreach (DataGridViewTextBoxColumn clmn in columns)
            {
                trackGrid.Columns.Add(clmn);
            }

            //filter with searchbox
            SortableBindingList<Track> source;
            string searchWord = searchBox.Text;
            if (String.IsNullOrEmpty(searchWord))
            {
                source = new SortableBindingList<Track>(playlist);
            }
            else
            {
                List<Track> tracksWithDupes = new List<Track>();
                var searchOrKeywords = searchWord.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string or in searchOrKeywords)
                {
                    var sourceList = new List<Track>(playlist);
                    var searchKeywords = or.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in searchKeywords)
                    {
                        bool invertSearch = false;
                        string searchWrd = s.TrimStart();
                        if (searchWrd.StartsWith("!"))
                        {
                            searchWrd = searchWrd.Substring(1);
                            invertSearch = true;
                        }
                        searchWrd = searchWrd.Trim();

                        //process special keywords
                        switch (searchWrd)
                        {
                            case "%monthago":
                                sourceList = sourceList.Where(x => x.lastOpened < DateTime.Now.AddDays(-30)).ToList();
                                break;
                            case "%weekago":
                                sourceList = sourceList.Where(x => x.lastOpened < DateTime.Now.AddDays(-7)).ToList();
                                break;
                            case "%dayago":
                                sourceList = sourceList.Where(x => x.lastOpened < DateTime.Now.AddDays(-1)).ToList();
                                break;
                            case "%never":
                                sourceList = sourceList.Where(x => x.lastOpened == DateTime.MinValue).ToList();
                                break;
                            default:
                                //process keywords with =variable
                                if (searchWrd.StartsWith("%daysago="))
                                {
                                    searchWrd = searchWrd.Split('=').Last();
                                    try
                                    {
                                        int dayNumber = Int32.Parse(searchWrd);
                                        sourceList = sourceList.Where(x => x.lastOpened < DateTime.Now.AddDays(dayNumber * -1)).ToList();
                                    }
                                    catch (Exception) { }
                                    break;
                                }
                                else if (searchWord.StartsWith("%artist="))
                                {
                                    searchWrd = searchWrd.Split('=').Last().ToLower();
                                    sourceList = sourceList.Where(x => x.artist.ToLower() == searchWrd).ToList();
                                    break;
                                }
                                else if (searchWord.StartsWith("%title="))
                                {
                                    searchWrd = searchWrd.Split('=').Last().ToLower();
                                    sourceList = sourceList.Where(x => x.title.ToLower() == searchWrd).ToList();
                                    break;
                                }
                                else if (searchWord.StartsWith("%album="))
                                {
                                    searchWrd = searchWrd.Split('=').Last().ToLower();
                                    sourceList = sourceList.Where(x => x.album.ToLower().ToLower() == searchWrd).ToList();
                                    break;
                                }
                                else if (searchWord.StartsWith("%top="))
                                {
                                    searchWrd = searchWrd.Split('=').Last().ToLower();
                                    int searchInt;
                                    if (int.TryParse(searchWrd, out searchInt))
                                    {
                                        sourceList = sourceList.Where(x => x.num <= searchInt).ToList();
                                    }
                                    break;
                                }

                                //default search processor
                                if (invertSearch) sourceList = sourceList.Where(x => !x.title.ContainsIgnoreCase(searchWrd) && !x.album.ContainsIgnoreCase(searchWrd) && !x.artist.ContainsIgnoreCase(searchWrd)).ToList();
                                else sourceList = sourceList.Where(x => x.title.ContainsIgnoreCase(searchWrd) || x.album.ContainsIgnoreCase(searchWrd) || x.artist.ContainsIgnoreCase(searchWrd)).ToList();
                                break;
                        }
                    }
                    tracksWithDupes.AddRange(sourceList);
                }
                source = new SortableBindingList<Track>(tracksWithDupes.Distinct().ToList());
                //source = new SortableBindingList<Track>(sourceList);
            }
            trackGrid.DataSource = source;
            //

            trackGrid.ClearSelection();
            HighlightPlayingTrack();
            RefreshTotalTimeLabel();
            //if(displayedItems!=ItemType.Cache) displayedItems = ItemType.Track;
            if (displayedItems != ItemType.Playlist) AddHeaderContextMenu();
        }

        void DisplayPlaylistsInGrid()
        {
            if (knownPlaylists.Count > 0)
            {
                trackGrid.Columns.Clear();
                trackGrid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = (int)Math.Floor(trackGrid.Width * 0.05),
                    DataPropertyName = "num",
                    HeaderText = "#"
                });
                trackGrid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = (int)Math.Floor(trackGrid.Width * 0.20),
                    DataPropertyName = "title",
                    HeaderText = "Playlist"
                });
                trackGrid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = (int)Math.Floor(trackGrid.Width * 0.745),
                    DataPropertyName = "filename",
                    HeaderText = "File"
                });
                playlist = new List<Track>();
                for (int i = 0; i < playlistSelectorCombo.Items.Count; i++)
                {
                    PlaylistComboItem pci = (PlaylistComboItem)playlistSelectorCombo.Items[i];
                    string t = pci.shownName;
                    //MessageBox.Show(knownPlaylists.Count+" must be greater or equal to "+(i-1));
                    if (t != "<No playlist>" && t != "<Cache>")
                    {
                        Track tr = new Track(knownPlaylists[i - 2], t);
                        tr.num = i - 1;
                        playlist.Add(tr);
                    }
                }
                SortableBindingList<Track> source = new SortableBindingList<Track>(playlist);
                trackGrid.DataSource = source;
                trackGrid.ClearSelection();
            }
            displayedItems = ItemType.Playlist;
        }

        DataGridViewRow FindTrackRow(Track t)
        {
            //return FindTrackRow(t.num);
            foreach (DataGridViewRow r in trackGrid.Rows)
            {
                Track tt = (Track)r.DataBoundItem;
                if (tt == t) return r;
            }
            return null;
        }

        DataGridViewRow FindTrackRow(int num)
        {
            if (playlist.Count < num) return null;
            foreach (DataGridViewRow r in trackGrid.Rows)
            {
                Track tt = (Track)r.DataBoundItem;
                if (tt.num == num) return r;
            }
            return null;
        }

        void MoveTrackInPlaylist(int oldIndex, int newIndex)
        {
            if (oldIndex > playlist.Count || newIndex > playlist.Count || oldIndex < 0 || newIndex < 0) return;
            Track item = playlist[oldIndex];
            playlist.RemoveAt(oldIndex);
            bool item1Selected = trackGrid.Rows[oldIndex].Selected;
            bool item2Selected = trackGrid.Rows[newIndex].Selected;
            playlist.Insert(newIndex, item);
            FixPlaylistNumbers(playlist);
            RefreshPlaylistGrid();
            trackGrid.Rows[oldIndex].Selected = item2Selected;
            trackGrid.Rows[newIndex].Selected = item1Selected;
            playlistChanged = true;
        }

        Track GetTrackByNum(int num)
        {
            return playlist.First(x => x.num == num);
        }

        public void DeleteTrack(List<int> nums)
        {
            if (!sortingAllowed) return;
            int lastInt = 0;
            if (displayedItems == ItemType.Track)
            {
                //if track
                foreach (int i in nums)
                {
                    Track toRemove = GetTrackByNum(i);
                    playlist.Remove(toRemove);
                    lastInt = i;
                }
                RefreshPlaylistGrid();
                playlistChanged = true;
            }
            else
            {
                //if playlist
                foreach (int i in nums)
                {
                    Track toRemove = GetTrackByNum(i);
                    playlist.Remove(toRemove);
                    RemoveKnownPlaylist(toRemove.filename);
                    lastInt = i;
                }
                DisplayPlaylistsInGrid();
            }
            try
            {
                if (trackGrid.Rows[lastInt] != null) trackGrid.Rows[lastInt].Selected = true;
            }
            catch (Exception) { }
            RefreshTotalTimeLabel();
        }

        public void LoadXmlColorScheme(string name)
        {
            XDocument xml;
            try
            {
                xml = XDocument.Load(configXmlFileLocation);
            }
            catch (Exception)
            {
                return;
            }
            if (xml.Root.Descendants().FirstOrDefault(x => x.Name == "colorSchemes") == null) return;
            XElement group = xml.Root.Element("colorSchemes");
            XElement match = group.Elements().FirstOrDefault(x => x.Name == name);
            if (match == null) return;
            XElement ele;
            ele = match.Element("background");
            if (ele.Value != null) backgroundColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("controlBack");
            if (ele.Value != null) controlBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("controlFore");
            if (ele.Value != null) controlForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("highlightBack");
            if (ele.Value != null) highlightBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("highlightFore");
            if (ele.Value != null) highlightForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("seekBarBack");
            if (ele.Value != null) seekBarBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("seekBarFore");
            if (ele.Value != null) seekBarForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("volBarBack");
            if (ele.Value != null) volBarBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("volBarFore");
            if (ele.Value != null) volBarForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("trackTitle");
            if (ele.Value != null) trackTitleColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("trackAlbum");
            if (ele.Value != null) trackAlbumColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("trackArtist");
            if (ele.Value != null) trackArtistColor = ExtensionMethods.ColorFromHex(ele.Value);
            ele = match.Element("spectrum");
            if (ele.Value != null) spectrumColor = ExtensionMethods.ColorFromHex(ele.Value);
            SetColors();
        }

        public void SaveXmlColorScheme(string name)
        {
            XDocument xml;
            try
            {
                xml = XDocument.Load(configXmlFileLocation);
            }
            catch (Exception)
            {
                SaveXmlConfig();
                SaveXmlColorScheme(name);
                return;
            }
            XElement group;
            XElement item;
            XElement scheme;
            //general
            group = xml.Root.Element("colorSchemes");
            if (group == null)
            {
                group = new XElement("colorSchemes");
                xml.Root.Add(group);
            }
            scheme = group.Element(name);
            if (scheme != null) scheme.Remove();
            scheme = new XElement(name);
            item = new XElement("background");
            item.Value = backgroundColor.ToHex();
            scheme.Add(item);
            item = new XElement("controlBack");
            item.Value = controlBackColor.ToHex();
            scheme.Add(item);
            item = new XElement("controlFore");
            item.Value = controlForeColor.ToHex();
            scheme.Add(item);
            item = new XElement("highlightBack");
            item.Value = highlightBackColor.ToHex();
            scheme.Add(item);
            item = new XElement("highlightFore");
            item.Value = highlightForeColor.ToHex();
            scheme.Add(item);
            item = new XElement("seekBarBack");
            item.Value = seekBarBackColor.ToHex();
            scheme.Add(item);
            item = new XElement("seekBarFore");
            item.Value = seekBarForeColor.ToHex();
            scheme.Add(item);
            item = new XElement("volBarBack");
            item.Value = volBarBackColor.ToHex();
            scheme.Add(item);
            item = new XElement("volBarFore");
            item.Value = volBarForeColor.ToHex();
            scheme.Add(item);
            item = new XElement("trackTitle");
            item.Value = trackTitleColor.ToHex();
            scheme.Add(item);
            item = new XElement("trackAlbum");
            item.Value = trackAlbumColor.ToHex();
            scheme.Add(item);
            item = new XElement("trackArtist");
            item.Value = trackArtistColor.ToHex();
            scheme.Add(item);
            item = new XElement("spectrum");
            item.Value = spectrumColor.ToHex();
            scheme.Add(item);
            group.Add(scheme);
            xml.Save(configXmlFileLocation);
        }

        void SaveXmlConfig()
        {
            XDocument xml;
            XElement colorSchemes = null;
            try
            {
                xml = XDocument.Load(configXmlFileLocation);
                if (xml.Root.Element("colorSchemes") != null)
                {
                    colorSchemes = xml.Root.Element("colorSchemes");
                }
            }
            catch (Exception) { }
            xml = new XDocument();
            xml.Add(new XElement("config"));
            //general
            XElement general = new XElement("general");
            XElement item = new XElement("volume");
            item.Value = ((int)(volume * 100)).ToString();
            general.Add(item);
            item = new XElement("hotkeyVolumeIncrement");
            item.Value = hotkeyVolumeIncrement.ToString();
            general.Add(item);
            item = new XElement("hotkeyTransposeIncrement");
            item.Value = hotkeyTransposeIncrement.ToString();
            general.Add(item);
            item = new XElement("hotkeysAllowed");
            item.Value = hotkeysAllowed.ToString();
            general.Add(item);
            item = new XElement("autoAdvance");
            item.Value = autoAdvance.ToString();
            general.Add(item);
            item = new XElement("autoShuffle");
            item.Value = autoShuffle.ToString();
            general.Add(item);
            item = new XElement("trackChangePopup");
            item.Value = trackChangePopup.ToString();
            general.Add(item);
            item = new XElement("saveTranspose");
            item.Value = saveTranspose.ToString();
            general.Add(item);
            item = new XElement("showWaveform");
            item.Value = showWaveform.ToString();
            general.Add(item);
            item = new XElement("repeat");
            item.Value = repeat.ToString();
            general.Add(item);
            item = new XElement("normalize");
            item.Value = normalize.ToString();
            general.Add(item);
            item = new XElement("visualisationStyle");
            item.Value = visualisationStyle.ToString();
            general.Add(item);
            item = new XElement("visualfps");
            item.Value = visualfps.ToString();
            general.Add(item);
            item = new XElement("midisf");
            item.Value = midiSfLocation;
            general.Add(item);
            item = new XElement("font1");
            item.Value = font1.FontFamily.Name;
            item.Add(new XAttribute("Style", (int)font1.Style));
            general.Add(item);
            item = new XElement("font1Size");
            item.Value = fontSizePercentage + "";
            general.Add(item);
            item = new XElement("font2");
            item.Value = font2.FontFamily.Name;
            item.Add(new XAttribute("Style", (int)font2.Style));
            general.Add(item);
            item = new XElement("suppressHotkeys");
            item.Value = suppressHotkeys.ToString();
            general.Add(item);
            item = new XElement("converterSelectedBitrateIndex");
            item.Value = converterSelectedBitrateIndex.ToString();
            general.Add(item);
            item = new XElement("reshuffleAfterListLoop");
            item.Value = reshuffleAfterListLoop.ToString();
            general.Add(item);
            xml.Root.Add(general);
            //colors
            XElement colors = new XElement("colors");
            item = new XElement("background");
            item.Value = backgroundColor.ToHex();
            colors.Add(item);
            item = new XElement("controlBack");
            item.Value = controlBackColor.ToHex();
            colors.Add(item);
            item = new XElement("controlFore");
            item.Value = controlForeColor.ToHex();
            colors.Add(item);
            item = new XElement("highlightBack");
            item.Value = highlightBackColor.ToHex();
            colors.Add(item);
            item = new XElement("highlightFore");
            item.Value = highlightForeColor.ToHex();
            colors.Add(item);
            item = new XElement("seekBarBack");
            item.Value = seekBarBackColor.ToHex();
            colors.Add(item);
            item = new XElement("seekBarFore");
            item.Value = seekBarForeColor.ToHex();
            colors.Add(item);
            item = new XElement("volBarBack");
            item.Value = volBarBackColor.ToHex();
            colors.Add(item);
            item = new XElement("volBarFore");
            item.Value = volBarForeColor.ToHex();
            colors.Add(item);
            item = new XElement("trackTitle");
            item.Value = trackTitleColor.ToHex();
            colors.Add(item);
            item = new XElement("trackAlbum");
            item.Value = trackAlbumColor.ToHex();
            colors.Add(item);
            item = new XElement("trackArtist");
            item.Value = trackArtistColor.ToHex();
            colors.Add(item);
            item = new XElement("spectrum");
            item.Value = spectrumColor.ToHex();
            colors.Add(item);
            xml.Root.Add(colors);
            if (colorSchemes != null) xml.Root.Add(colorSchemes);
            //hotkeys
            XElement hotkeyz = new XElement("hotkeys");
            foreach (HotkeyData hk in hotkeys)
            {
                item = new XElement(hk.hotkey.ToString());
                item.Add(new XAttribute("key", hk.key.ToString()));
                item.Add(new XAttribute("ctrl", hk.ctrl.ToString()));
                item.Add(new XAttribute("alt", hk.alt.ToString()));
                item.Add(new XAttribute("shift", hk.shift.ToString()));
                item.Add(new XAttribute("enabled", hk.enabled.ToString()));
                hotkeyz.Add(item);
            }
            xml.Root.Add(hotkeyz);
            XElement columns = new XElement("columns");
            //columns
            foreach (ColumnInfo ci in currentColumnInfo)
            {
                item = new XElement(COLUMN_PROPERTIES[ci.id]);
                item.Value = ci.width.ToString();
                columns.Add(item);
            }
            xml.Root.Add(columns);
            xml.Save(configXmlFileLocation);
        }

        void LoadXmlConfig()
        {
            XDocument xml;
            try
            {
                xml = XDocument.Load(configXmlFileLocation);
            }
            catch (Exception)
            {
                SetDefaults();
                SaveXmlConfig();
                return;
            }
            XElement group;
            XElement ele;
            //general
            group = xml.Root.Element("general");
            ele = group.Element("volume");
            int vol;
            if (ele != null && Int32.TryParse(ele.Value, out vol)) SetVolume(((float)vol / 100));
            else SetVolume(Program.volumeDefault);
            ele = group.Element("autoAdvance");
            if (ele != null && Boolean.TryParse(ele.Value, out autoAdvance)) { }
            else autoAdvance = Program.autoAdvanceDefault;
            ele = group.Element("autoShuffle");
            if (ele != null && Boolean.TryParse(ele.Value, out autoShuffle)) { }
            else autoShuffle = Program.autoShuffleDefault;
            ele = group.Element("hotkeyVolumeIncrement");
            if (ele != null && float.TryParse(ele.Value, out hotkeyVolumeIncrement)) { }
            else hotkeyVolumeIncrement = Program.hotkeyVolumeIncrementDefault;
            ele = group.Element("hotkeyTransposeIncrement");
            if (ele != null && float.TryParse(ele.Value, out hotkeyTransposeIncrement)) { }
            else hotkeyTransposeIncrement = Program.hotkeyTransposeIncrementDefault;
            ele = group.Element("hotkeysAllowed");
            if (ele != null && Boolean.TryParse(ele.Value, out hotkeysAllowed)) { }
            else hotkeysAllowed = Program.hotkeysAllowedDefault;
            ele = group.Element("trackChangePopup");
            if (ele != null && Boolean.TryParse(ele.Value, out trackChangePopup)) { }
            else trackChangePopup = Program.trackChangePopupDefault;
            ele = group.Element("saveTranspose");
            if (ele != null && Boolean.TryParse(ele.Value, out saveTranspose)) { }
            else saveTranspose = Program.saveTransposeDefault;
            ele = group.Element("showWaveform");
            if (ele != null && Boolean.TryParse(ele.Value, out showWaveform)) { }
            else showWaveform = Program.showWaveformDefault;
            ele = group.Element("repeat");
            if (ele != null && Int32.TryParse(ele.Value, out repeat)) { }
            else repeat = Program.repeatDefault;
            repeatBtn_MouseLeave(null, null);
            SetLooping();
            ele = group.Element("normalize");
            if (ele != null && Boolean.TryParse(ele.Value, out normalize)) { }
            else normalize = Program.normalizeDefault;
            ele = group.Element("visualisationStyle");
            if (ele != null && Int32.TryParse(ele.Value, out visualisationStyle)) { }
            else visualisationStyle = Program.visualisationStyleDefault;
            ele = group.Element("visualfps");
            if (ele != null && Int32.TryParse(ele.Value, out visualfps)) { }
            else visualfps = Program.visualfpsDefault;
            ele = group.Element("midisf");
            if (ele != null) { midiSfLocation = ele.Value; }
            else midiSfLocation = "";
            ele = group.Element("font1");
            if (ele != null) { font1 = new Font(ele.Value, 8, (FontStyle)Int32.Parse(ele.Attribute("Style").Value)); }
            else font1 = Program.font1Default.Copy();
            ele = group.Element("font1Size");
            if (ele != null && Int32.TryParse(ele.Value, out fontSizePercentage)) { }
            else fontSizePercentage = 100;
            ele = group.Element("font2");
            if (ele != null) { font2 = new Font(ele.Value, 12, (FontStyle)Int32.Parse(ele.Attribute("Style").Value)); }
            else font2 = Program.font2Default.Copy();
            SetFonts();
            ele = group.Element("suppressHotkeys");
            if (ele != null && Boolean.TryParse(ele.Value, out suppressHotkeys)) { }
            else suppressHotkeys = Program.suppressHotkeysDefault;
            ele = group.Element("converterSelectedBitrateIndex");
            if (ele != null && Int32.TryParse(ele.Value, out converterSelectedBitrateIndex)) { }
            else converterSelectedBitrateIndex = Program.converterSelectedBitrateIndexDefault;
            ele = group.Element("reshuffleAfterListLoop");
            if (ele != null && Boolean.TryParse(ele.Value, out reshuffleAfterListLoop)) { }
            else reshuffleAfterListLoop = Program.reshuffleAfterListLoopDefault;
            //colors
            group = xml.Root.Element("colors");
            ele = group.Element("background");
            if (ele != null) backgroundColor = ExtensionMethods.ColorFromHex(ele.Value);
            else backgroundColor = Program.backgroundColorDefault.Copy();
            ele = group.Element("controlBack");
            if (ele != null) controlBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            else controlBackColor = Program.controlBackColorDefault.Copy();
            ele = group.Element("controlFore");
            if (ele != null) controlForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            else controlForeColor = Program.controlForeColorDefault.Copy();
            ele = group.Element("highlightBack");
            if (ele != null) highlightBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            else highlightBackColor = Program.highlightBackColorDefault.Copy();
            ele = group.Element("highlightFore");
            if (ele != null) highlightForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            else highlightForeColor = Program.highlightForeColorDefault.Copy();
            ele = group.Element("seekBarBack");
            if (ele != null) seekBarBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            else seekBarBackColor = Program.seekBarBackColorDefault.Copy();
            ele = group.Element("seekBarFore");
            if (ele != null) seekBarForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            else seekBarForeColor = Program.seekBarForeColorDefault.Copy();
            ele = group.Element("volBarFore");
            if (ele != null) volBarForeColor = ExtensionMethods.ColorFromHex(ele.Value);
            else volBarForeColor = Program.volBarForeColorDefault.Copy();
            ele = group.Element("volBarBack");
            if (ele != null) volBarBackColor = ExtensionMethods.ColorFromHex(ele.Value);
            else volBarBackColor = Program.volBarBackColorDefault.Copy();
            ele = group.Element("trackTitle");
            if (ele != null) trackTitleColor = ExtensionMethods.ColorFromHex(ele.Value);
            else trackTitleColor = Program.trackTitleColorDefault.Copy();
            ele = group.Element("trackAlbum");
            if (ele != null) trackAlbumColor = ExtensionMethods.ColorFromHex(ele.Value);
            else trackAlbumColor = Program.trackAlbumColorDefault.Copy();
            ele = group.Element("trackArtist");
            if (ele != null) trackArtistColor = ExtensionMethods.ColorFromHex(ele.Value);
            else trackArtistColor = Program.trackArtistColorDefault.Copy();
            ele = group.Element("spectrum");
            if (ele != null) spectrumColor = ExtensionMethods.ColorFromHex(ele.Value);
            else spectrumColor = Program.spectrumColorDefault.Copy();
            //hotkeys
            group = xml.Root.Element("hotkeys");
            hotkeys = new List<HotkeyData>();
            foreach (XElement elem in group.Elements())
            {
                Hotkey hk; Keys k; bool ctrl; bool alt; bool shift; bool enabled;
                if (elem.Name != null && elem.Attribute("key") != null && elem.Attribute("ctrl") != null && elem.Attribute("shift") != null
                    && elem.Attribute("alt") != null && elem.Attribute("enabled") != null)
                    if (Enum.TryParse<Hotkey>(elem.Name.ToString(), out hk) &&
                       Enum.TryParse<Keys>(elem.Attribute("key").Value, out k) &&
                       Boolean.TryParse(elem.Attribute("ctrl").Value, out ctrl) &&
                       Boolean.TryParse(elem.Attribute("alt").Value, out alt) &&
                       Boolean.TryParse(elem.Attribute("shift").Value, out shift) &&
                       Boolean.TryParse(elem.Attribute("enabled").Value, out enabled))
                    {
                        hotkeys.Add(new HotkeyData(hk, k, ctrl, alt, shift, enabled));
                    }
            }
            SetHotkeySuppression();
            //columns
            group = xml.Root.Element("columns");
            currentColumnInfo = new List<ColumnInfo>();
            foreach (XElement elem in group.Elements())
            {
                int width;
                if (COLUMN_PROPERTIES.Contains(elem.Name.ToString()))
                {
                    if (Int32.TryParse(elem.Value, out width))
                    {
                        currentColumnInfo.Add(new ColumnInfo(COLUMN_PROPERTIES.ToList().IndexOf(elem.Name.ToString()), width));
                    }
                }
            }
        }

        public void SetHotkeySuppression()
        {
            kh.suppressedKeys.Clear();
            if (suppressHotkeys)
            {
                foreach (HotkeyData hk in hotkeys)
                {
                    kh.suppressedKeys.Add(AddKeyModifiers(hk.key, hk.ctrl, hk.alt, hk.shift));
                }
            }
        }

        private Keys AddKeyModifiers(Keys key, bool ctrl, bool alt, bool shift)
        {
            if (ctrl) key = key | Keys.Control;
            if (alt) key = key | Keys.Alt;
            if (shift) key = key | Keys.Shift;
            return key;
        }

        public void AdjustGlobalVolume(float increment)
        {
            if (Bass.BASS_GetDevice() == -1) InitSoundDevice();
            float getVol = Bass.BASS_GetVolume();
            if (getVol + increment > 1) getVol = 1;
            else if (getVol + increment < 0) getVol = 0;
            else getVol = getVol + increment;
            Bass.BASS_SetVolume(getVol);
            ShowVolumePopup();
        }

        public void SetVolume(float vol)
        {
            if (vol > 1) vol = 1;
            else if (vol < 0) vol = 0;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, vol);
            volume = vol;
            volumeBar.Value = (int)(volumeBar.Maximum * vol);
        }

        private void Shuffle()
        {
            if (!sortingAllowed) return;
            /*SortableBindingList<Track> froms = (SortableBindingList<Track>)dataGridView1.DataSource;
            ShuffleBindingList(froms);*/
            Random rng = new Random();
            playlist = playlist.OrderBy(x => rng.Next()).ToList();
            //dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            //playlist = playlist.OrderBy(x => rng.Next()).ToList();
            RefreshPlaylistGrid();
            HighlightPlayingTrack();
        }

        void FixPlaylistNumbers(List<Track> p)
        {
            var invalidNumbaTracks = p.Where(x => x.num < 1).ToList();
            if (invalidNumbaTracks.Count == 0) return;
            for (int i = 0; i < invalidNumbaTracks.Count; i++)
            {
                invalidNumbaTracks[i].num = i + 1;
                p.Remove(invalidNumbaTracks[i]);
            }
            foreach (Track t in p) t.num += invalidNumbaTracks.Count;
            p.InsertRange(0, invalidNumbaTracks);
        }

        private void RenumberPlaylist(List<Track> p)
        {
            for (int i = 0; i < p.Count; i++)
            {
                p[i].num = i + 1;
            }
        }

        private void CopyTrackToPlaylist(List<Track> t, string playlistFile)
        {
            List<Track> thePlaylist = new List<Track>();
            List<Track> playlistFromFile = ReadM3u(playlistFile);
            t.Reverse();
            for (int i = 0; i < t.Count; i++)
            {
                if (playlistFromFile.Where(x => x.filename == t[i].filename).Count() > 0)
                {
                    if (MessageBox.Show("Playlist '" + Path.GetFileName(playlistFile) + "' already has this track: " + Environment.NewLine +
                        t[i].filename + ". Add regardless?", "Dupe", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    {
                        continue;
                    }
                }
                Track newTrack = new Track(t[i].filename, t[i].title);
                newTrack.length = t[i].length;
                newTrack.artist = t[i].artist;
                thePlaylist.Add(newTrack);
            }
            thePlaylist.AddRange(playlistFromFile);
            FixPlaylistNumbers(thePlaylist);
            ExportM3u(playlistFile, thePlaylist);
        }

        private void CopyTrackToPlaylist(Track t, string playlistFile)
        {
            CopyTrackToPlaylist(new List<Track>() { t }, playlistFile);
        }

        private void LoadPlaylistDb()
        {
            try
            {
                knownPlaylists.Clear();
                StreamReader sr = new StreamReader(playlistDbLocation);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (File.Exists(line))
                    {
                        knownPlaylists.Add(line);
                    }
                }
                sr.Close();
                sr.Dispose();
            }
            catch (Exception) { }
        }

        private void SavePlaylistDb()
        {
            using (StreamWriter sw = new StreamWriter(playlistDbLocation))
            {
                foreach (string s in knownPlaylists)
                {
                    sw.WriteLine(s + Environment.NewLine);
                }
                sw.Close();
                sw.Dispose();
            }
        }

        private void RemoveKnownPlaylist(int index)
        {
            knownPlaylists.RemoveAt(index);
            SavePlaylistDb();
            PopulatePlaylistList();
        }

        private void RemoveKnownPlaylist(string filename)
        {
            if (knownPlaylists.Contains(filename))
            {
                RemoveKnownPlaylist(knownPlaylists.IndexOf(filename));
            }
        }

        private void AddKnownPlaylist(string filename)
        {
            if (!knownPlaylists.Contains(filename))
            {
                knownPlaylists.Add(filename);
                SavePlaylistDb();
                PopulatePlaylistList();
            }
        }

        class PlaylistComboItem
        {
            public string filepath { get; set; }
            public string shownName { get; set; }
            public override string ToString()
            {
                return shownName;
            }
            public PlaylistComboItem(string filepath, string shownName)
            {
                this.filepath = filepath;
                this.shownName = shownName;
            }
        }

        private void PopulatePlaylistList()
        {
            BindingList<PlaylistComboItem> playlistItems = new BindingList<PlaylistComboItem>();
            playlistItems.Add(new PlaylistComboItem(null, "<No playlist>"));
            playlistItems.Add(new PlaylistComboItem(null, "<Cache>"));
            LoadPlaylistDb();
            foreach (string s in knownPlaylists)
            {
                playlistItems.Add(new PlaylistComboItem(s, Path.GetFileNameWithoutExtension(s)));
            }

            playlistSelectorCombo.ValueMember = "filepath";
            playlistSelectorCombo.DisplayMember = "shownName";
            playlistSelectorCombo.DataSource = playlistItems;
        }

        public void ExportM3u(string filename, List<Track> plist)
        {
            List<string> filenames = new List<string>();
            List<string> artists = new List<string>();
            List<string> titles = new List<string>();
            List<int> lengths = new List<int>();
            try
            {
                foreach (Track t in plist)
                {
                    filenames.Add(t.filename);
                    if (!supportedModuleTypes.Contains(Path.GetExtension(t.filename).ToLower())) artists.Add(t.artist);
                    else artists.Add(null);
                    titles.Add(t.title);
                    if (t.length < 0) t.length = 0;
                    lengths.Add(t.length);
                }
                try
                {
                    SuperM3U.Writer.WriteFile(filename, filenames, titles, artists, lengths);
                    if (!knownPlaylists.Contains(filename))
                    {
                        AddKnownPlaylist(filename);
                    }
                }
                catch (Exception w)
                {
                    MessageBox.Show(w.ToString());
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString());
            }
        }

        private List<Track> ReadM3u(string filename)
        {
            List<Track> ret = new List<Track>();
            try
            {
                List<M3UTrack> tracks = SuperM3U.Reader.GetTracks(filename);
                for (int i = 0; i < tracks.Count; i++)
                {
                    Track t = new Track(tracks[i].filename, tracks[i].title);
                    t.num = i + 1;
                    t.length = tracks[i].length;
                    ret.Add(t);
                }
            }
            catch (Exception)
            {
                //error
            }
            return ret;
        }

        private void ImportM3u(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("File doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (tagsLoaderWorker.IsBusy)
            {
                tagsLoaderWorker.CancelAsync();
                ImportM3u(filename);
                return;
            }
            searchBox.Text = "";
            System.Diagnostics.Process.GetCurrentProcess().StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
            try
            {
                displayedItems = ItemType.Track;
                playlist = new List<Track>();
                currentPlaylist = filename;
                AddKnownPlaylist(filename);
                var plist = SuperM3U.Reader.GetTracks(filename);
                for (int i = 0; i < plist.Count; i++)
                {
                    Track t = new Track(plist[i].filename, plist[i].title);
                    t.length = plist[i].length;
                    t.num = i + 1;
                    playlist.Add(t);
                }
                foreach (object o in playlistSelectorCombo.Items)
                {
                    PlaylistComboItem pci = (PlaylistComboItem)o;
                    if (pci.filepath == currentPlaylist)
                    {
                        playlistSelectorCombo.SelectedItem = o;
                        break;
                    }
                }
                RefreshGrid();
                if (autoShuffle) Shuffle();
                RefreshPlaylistGrid();
                playlistChanged = false;
                AllowSorting(false);
                if (tagsLoaderWorker.IsBusy) tagsLoaderWorker.CancelAsync();
                else
                {
                    trackText.Text = "Loading...";
                    tagsLoaderWorker.RunWorkerAsync();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(filename + " is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Console.WriteLine(e.StackTrace);
            }
        }

        void ReloadTags()
        {
            if (tagsLoaderWorker.IsBusy) return;
            reloadTagsFlag = true;
            tagsLoaderWorker.RunWorkerAsync();
        }

        private void AdvancePlayer()
        {
            try
            {
                StopPlayer();
                DataGridViewRow currentRow = FindTrackRow(nowPlaying);
                if (currentRow.Index >= trackGrid.Rows.Count - 1 && repeat == 0) return;
                else
                {
                    int indexToPlay = currentRow.Index + 1;
                    if (currentRow.Index >= trackGrid.Rows.Count - 1)
                    {
                        //list loop
                        if (reshuffleAfterListLoop) Shuffle();
                        indexToPlay = 0;
                    }
                    Track t = (Track)trackGrid.Rows[indexToPlay].DataBoundItem;
                    //nowPlaying = t;
                    if (File.Exists(t.filename))
                    {
                        LoadAudio(t);
                    }
                    else
                    {
                        AdvancePlayer();
                        return;
                    }
                }
            }
            catch (Exception) { }
        }

        private void PlayerPrevious()
        {
            try
            {
                StopPlayer();
                DataGridViewRow currentRow = FindTrackRow(nowPlaying);
                if (currentRow.Index <= 0) return;
                else
                {
                    Track t = (Track)trackGrid.Rows[currentRow.Index - 1].DataBoundItem;
                    LoadAudio(t);
                }
            }
            catch (Exception) { }
        }

        void FlushTimeListened(Track t)
        {
            if (t == null) return;

            t.listened = (int)timeListenedTracker.Elapsed.TotalSeconds + t.listened;
            xmlCacher.AddOrUpdate(new List<Track> { t });

            RefreshTotalTimeLabel();
            if (timeListenedTracker.IsRunning) timeListenedTracker.Restart();
            else timeListenedTracker.Reset();
        }

        void playbackEnded(int handle, int channel, int data, IntPtr user)
        {
            timeListenedTracker.Stop();
            if (streamLoaded)
            {
                if (repeat < 2)
                    advanceFlag = true;
            }
            RefreshPlayIcon();
        }

        void playbackStalled(int handle, int channel, int data, IntPtr user)
        {
            if ((int)data == 1) timeListenedTracker.Start();
            else timeListenedTracker.Stop();
        }

        void LoadAudioFile(string filename)
        {
            var match = playlist.First(x => x.filename == filename);
            if (match == null) LoadFiles(new List<string> { filename });
            else LoadAudioFile(match);
        }

        public void UnloadAudioFile()
        {
            if (nowPlaying != null)
            {
                StopPlayer();
                Bass.BASS_Free();
            }
        }

        public void InitSoundDevice()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, this.Handle);
        }

        void PrepareToLoadFile(Track t)
        {
            if (DSPGain != null)
            {
                DSPGain.Stop();
                DSPGain.Dispose();
            }
            volumeBar.Refresh();
            if (streamLoaded) Bass.BASS_ChannelStop(stream);
            FlushTimeListened(nowPlaying);
            Bass.BASS_Free();

            if (playlist != null && playlist.Contains(t))
            {
                int numzor = t.num;
                int index = playlist.IndexOf(playlist.First(x => x.num == t.num));
                playlist[index].num = numzor;
            }
            t.GetTags();
            t.lastOpened = DateTime.Now;
            nowPlaying = t;
            if (!tagsLoaderWorker.IsBusy) try { xmlCacher.AddOrUpdate(new List<Track>() { t }); }
                catch (Exception) { }
            InitSoundDevice();
        }

        public void LoadAudioFile(Track t)
        {
            if (!LoadStream(t.filename, out stream, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_SOFTWARE))
            {
                streamLoaded = false;
                return;
            }
        }

        public void Normalize()
        {
            if (DSPGain != null && DSPGain.IsAssigned) DSPGain.Stop();
            if (wf == null || !wf.IsRendered) return;
            float peak = 0;
            normGain = wf.GetNormalizationGain(-1, -1, ref peak);
            try
            {
                if (Math.Abs(1 - (decimal)normGain) > 0.03m)
                {
                    DSPGain = new DSP_Gain();
                    DSPGain.ChannelHandle = stream;
                    DSPGain.Gain = normGain;
                    DSPGain.Start();
                    Console.WriteLine(" | Applied gain of " + (normGain));
                }
                else
                {
                    normGain = 0;
                    Console.WriteLine(" | Gain not applied");
                }
            }
            catch (Exception) { }
            volumeBar.Refresh();
        }

        public bool LoadStream(string filename, out int outStream, BASSFlag flags)
        {
            string ext = Path.GetExtension(filename.ToLower());
            if (supportedModuleTypes.Contains(ext))
            {
                outStream = Bass.BASS_MusicLoad(filename, 0, 0,
                    BASSFlag.BASS_MUSIC_SINCINTER |
                    BASSFlag.BASS_MUSIC_PRESCAN |
                    BASSFlag.BASS_MIXER_DOWNMIX |
                    BASSFlag.BASS_MUSIC_RAMPS |
                    BASSFlag.BASS_SAMPLE_FX |
                    BASSFlag.BASS_MUSIC_STOPBACK |
                    flags, 0);
                return true;
            }
            else if (supportedMidiTypes.Contains(ext))
            {
                if (!InitMidi())
                {
                    outStream = -1;
                    return false;
                }
                BassMidi.BASS_MIDI_StreamSetFonts(0, midiFonts, midiFonts.Length);
                outStream = BassMidi.BASS_MIDI_StreamCreateFile(filename, 0, 0, flags, 0);
                return true;
            }
            else if (ext == ".flac")
            {
                outStream = BassFlac.BASS_FLAC_StreamCreateFile(filename, 0, 0, flags);
                return true;
            }
            else if (ext == ".wma")
            {
                outStream = BassWma.BASS_WMA_StreamCreateFile(filename, 0, 0, flags);
                return true;
            }
            else if (supportedAudioTypes.Contains(ext))
            {
                outStream = Bass.BASS_StreamCreateFile(filename, 0, 0, flags);
                return true;
            }
            else
            {
                outStream = -1;
                return false;
            }
        }

        void SetLooping()
        {
            if (!streamLoaded) return;
            if (repeat > 1)
            {
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_MUSIC_LOOP, BASSFlag.BASS_MUSIC_LOOP);
            }
            else
            {
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_MUSIC_LOOP);
            }
            Console.WriteLine(Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT).ToString());
        }

        public bool InitMidi()
        {
            //if (midiFonts != null) return true;
            if (!File.Exists(midiSfLocation))
            {
                if (MessageBox.Show("To play Midi files, you need a soundfont. Do you wanna specify a soundfont file to use?", "Midi", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Midi soundfont";
                    dlg.Filter = "Soundfont (*.sf2)|*.sf2";
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        midiSfLocation = dlg.FileName.ToString();
                    }
                    else
                    {
                        return false;
                    }
                    dlg.Dispose();
                }
                else
                {
                    return false;
                }
            }
            var font = BassMidi.BASS_MIDI_FontInit(midiSfLocation);
            midiFonts = new BASS_MIDI_FONT[1];
            midiFonts[0] = new BASS_MIDI_FONT(font, -1, 0);
            return true;
        }

        void DisplayTrackInfo(Track t)
        {
            if (t == null) return;
            trackText.Text = "";
            trackText.AppendText(t.title, trackTitleColor);
            if (!String.IsNullOrWhiteSpace(t.artist))
            {
                trackText.AppendText(" - ", controlForeColor);
                trackText.AppendText(t.artist, trackArtistColor);
            }
            if (!String.IsNullOrWhiteSpace(t.album))
            {
                trackText.AppendText(" [" + t.album + "]", trackAlbumColor);
            }
            trackText.SelectionStart = 0;
            trackText.SelectionLength = 0;
        }

        private void ScrollGridTo(DataGridViewRow theRow)
        {
            if (theRow == null) return;
            if (!theRow.Displayed) trackGrid.FirstDisplayedScrollingRowIndex = theRow.Index;
            /*int halfWay = (dataGridView1.DisplayedRowCount(false) / 2);
            if (dataGridView1.FirstDisplayedScrollingRowIndex + halfWay > theRow.Index ||
                (dataGridView1.FirstDisplayedScrollingRowIndex + dataGridView1.DisplayedRowCount(false) - halfWay) <= theRow.Index)
            {
                int targetRow = theRow.Index;
                targetRow = Math.Max(targetRow - halfWay, 0);
                dataGridView1.FirstDisplayedScrollingRowIndex = targetRow;
            }*/
        }

        DataGridViewRow HighlightPlayingTrack()
        {
            if (nowPlaying == null) return null;
            if (highlightedRow != null)
            {
                if (highlightedRow.Index % 2 == 0)
                    highlightedRow.DefaultCellStyle = defaultCellStyle;
                else highlightedRow.DefaultCellStyle = alternatingCellStyle;
            }
            DataGridViewRow rw = FindTrackRow(nowPlaying);
            if (rw == null) return null;
            rw.DefaultCellStyle = highlightedCellStyle;
            highlightedRow = rw;
            return rw;
        }

        string CreateNewPlaylist()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Playlist";
            dlg.Filter = "M3U playlists (*.m3u)|*.m3u";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                if (!dlg.FileName.ToString().ToLower().EndsWith(".m3u")) dlg.FileName += ".m3u";
                ExportM3u(dlg.FileName.ToString(), new List<Track>());
                return dlg.FileName.ToString();
            }
            dlg.Dispose();
            return null;
        }

        private void ExportPlaylist()
        {
            if (displayedItems == ItemType.Playlist) return;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Playlist";
            dlg.Filter = "M3U playlists (*.m3u)|*.m3u";
            dlg.FileName = Path.GetFileName(currentPlaylist);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ExportM3u(dlg.FileName.ToString(), playlist);
                if (currentPlaylist != dlg.FileName.ToString())
                {
                    playlistSelectorCombo.SelectedValue = currentPlaylist = dlg.FileName.ToString();
                }
            }
            dlg.Dispose();
        }

        private void ChooseFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open File";
            dlg.Filter = "Supported files|";
            foreach (string s in supportedAudioTypes)
            {
                dlg.Filter += "*" + s + ";";
            }
            dlg.Filter += "*.m3u;";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                LoadFiles(new List<string> { dlg.FileName.ToString() });
            }
            dlg.Dispose();
        }

        private void RefreshPlayIcon()
        {
            if (Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                playBtn.Image = LoadImage("play1");
                showingPlayIcon = true;
            }
            else
            {
                playBtn.Image = LoadImage("pause1");
                showingPlayIcon = false;
            }
        }

        Dictionary<string, Image> btnImages;
        string[] btnNames = new string[] { "play","pause","forward","back","stop","rewind","import","export","shuffle","options","repeatNone","repeatList","repeatTrack" };

        void ReadImages()
        {
            btnImages = new Dictionary<string, Image>();
            foreach (string n in btnNames) {
                for (int i = 1; i <= 4; i++)
                {
                    string name = $"{n}{i}";
                    btnImages.Add(name, ReadImage(name));
                }
            }
            btnImages.Add("echoesLogoWhiteDim", ReadImage("echoesLogoWhiteDim"));
            btnImages.Add("echoesLogoWhite", ReadImage("echoesLogoWhite"));
            btnImages.Add("vol", ReadImage("vol"));
        }

        Image ReadImage(string name)
        {
            string path = Path.Combine("Resources", name + ".png");
            try
            {
                using (FileStream i_Stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (Bitmap i_Bmp = new Bitmap(i_Stream))
                    {
                        return new Bitmap(i_Bmp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        Image LoadImage(string name)
        {
            if (btnImages == null)
                ReadImages();

            return btnImages[name];
        }

        void PlayFirst()
        {
            Track t;
            if (trackGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                {
                    t = (Track)rw.DataBoundItem;
                    if (File.Exists(t.filename))
                    {
                        OpenFile(t);
                        break;
                    }
                }
            }
            else if (trackGrid.Rows[0] != null)
            {
                foreach (DataGridViewRow rw in trackGrid.Rows)
                {
                    t = (Track)rw.DataBoundItem;
                    if (File.Exists(t.filename))
                    {
                        OpenFile(t);
                        break;
                    }
                }
            }
        }

        public void Play()
        {
            Bass.BASS_ChannelStop(stream);
            if (!streamLoaded) PlayFirst();
            if (nowPlaying != null)
            {
                Bass.BASS_ChannelPlay(stream, false);
                timeListenedTracker.Start();
                RefreshPlayIcon();
            }
        }

        public void StopPlayer()
        {
            if (!streamLoaded) return;
            Bass.BASS_ChannelStop(stream);
            timeListenedTracker.Stop();
            SetPosition(0);
            RefreshPlayIcon();
        }

        private void PausePlayer()
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(stream);
                timeListenedTracker.Stop();
                RefreshPlayIcon();
            }
        }

        void SaveColumnInfos()
        {
            ColumnInfo[] infos = new ColumnInfo[currentColumnInfo.Count];
            currentColumnInfo.Clear();
            foreach (DataGridViewColumn clmn in trackGrid.Columns)
            {
                infos[clmn.DisplayIndex] = new ColumnInfo((int)clmn.Tag, clmn.Width);
            }
            currentColumnInfo = infos.ToList();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskToQuitWorker())
            {
                e.Cancel = true;
                return;
            }
            AskToSavePlaylistChanges();
            if (displayedItems != ItemType.Playlist) SaveColumnInfos();
            try { SaveXmlConfig(); }
            catch (Exception zx)
            {
                MessageBox.Show(zx.ToString());
            }
            FlushTimeListened(nowPlaying);
            Bass.BASS_Free();
        }

        private void button1_Click(object sender, MouseEventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = LoadImage("play3");
            else playBtn.Image = LoadImage("pause3");
        }

        private void button2_Click(object sender, MouseEventArgs e)
        {
            stopBtn.Image = LoadImage("stop3");
        }

        private void button3_Click(object sender, MouseEventArgs e)
        {
            PausePlayer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChooseFile();
        }
        Random rng = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (streamLoaded)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    //-- row flashing --

                    /*Color highlightedBackColor, highlightedForeColor;
                    if (highlightedRow.Index % 2 == 0)
                    {
                        highlightedBackColor = defaultCellStyle.BackColor;
                        highlightedForeColor = defaultCellStyle.ForeColor;
                    }
                    else 
                    { 
                        highlightedBackColor = alternatingCellStyle.BackColor;
                        highlightedForeColor = alternatingCellStyle.ForeColor;
                    }
                    float energy=0;
                    vs.DetectPeakFrequency(stream, out energy);
                    highlightedCellStyle.BackColor = highlightedBackColor.MixWith(highlightBackColor, Math.Abs(energy));
                    highlightedCellStyle.ForeColor = highlightedForeColor.MixWith(highlightForeColor, Math.Abs(energy));
                    highlightedCellStyle.SelectionBackColor = controlForeColor.Darken().Darken().MixWith(highlightBackColor, Math.Abs(energy));
                    highlightedCellStyle.SelectionForeColor = highlightedCellStyle.BackColor;
                    highlightedRow.DefaultCellStyle = highlightedCellStyle;*/

                    //-- row flashing end <<TOO MUCH CPU USAGE>> WTF setting control BackColor/ForeColor strains cpu

                    switch (visualisationStyle)
                    {
                        case 1:
                            visualsPicture.Image = vs.CreateSpectrumLine(stream, visualsPicture.Width, visualsPicture.Height, spectrumColor, spectrumColor, backgroundColor, 5, 1, false, true, true);
                            break;
                        case 2:
                            visualsPicture.Image = vs.CreateSpectrumWave(stream, visualsPicture.Width, visualsPicture.Height, spectrumColor, spectrumColor, backgroundColor, 1, false, true, true);
                            break;
                        case 3:
                            visualsPicture.Image = vs.CreateSpectrum(stream, visualsPicture.Width, visualsPicture.Height, spectrumColor, spectrumColor, backgroundColor, false, true, true);
                            break;
                        case 4:
                            visualsPicture.Image = vs.CreateSpectrumLinePeak(stream, visualsPicture.Width, visualsPicture.Height, spectrumColor, spectrumColor, spectrumColor, backgroundColor, 5, 5, 1, 5, false, true, true);
                            break;
                    }
                }
                else
                {
                    visualsPicture.Image = null;
                }
                if (advanceFlag || GetPosition() > GetLength())
                {
                    AdvancePlayer();
                    advanceFlag = false;
                }
                if (!draggingSeek)
                {
                    int val = (int)(((double)GetPosition() / (double)GetLength()) * seekBar.Maximum);
                    if (val > seekBar.Maximum) val = seekBar.Maximum;
                    if (val < seekBar.Minimum) val = seekBar.Minimum;
                    seekBar.Value = val;
                    seekBar.Refresh();
                }
            }
        }

        void ShowTrackChangedPopup()
        {
            if (nowPlaying == null) return;
            string notifyText = "";
            Track t;
            DataGridViewRow currRow = FindTrackRow(nowPlaying);
            notifyText += ">";

            notifyText += nowPlaying.title + " - ";
            if (!String.IsNullOrWhiteSpace(nowPlaying.artist)) notifyText += nowPlaying.artist + Environment.NewLine;

            if (currRow.Index < trackGrid.Rows.Count - 1)
            {
                notifyText += " ";
                t = (Track)trackGrid.Rows[FindTrackRow(nowPlaying).Index + 1].DataBoundItem;
                notifyText += t.title + " - ";
                if (!String.IsNullOrWhiteSpace(t.artist)) notifyText += t.artist + Environment.NewLine;

            }
            Font ft = new Font(font2.FontFamily, 20, FontStyle.Bold);
            Size sz = TextRenderer.MeasureText(notifyText, ft);
            Rectangle screenSize = Screen.PrimaryScreen.WorkingArea;
            Point pt = new Point(screenSize.Width - sz.Width - 10, screenSize.Height - sz.Height - 10);
            popupWindow.Show(pt, 255, trackTitleColor, ft, 5000, FloatingWindow.AnimateMode.Blend, 3500, notifyText);
        }

        void ShowVolumePopup()
        {
            var str = "Volume: " + (int)(Bass.BASS_GetVolume() * 100) + "%";
            Font ft = new Font(font2.FontFamily, 20, FontStyle.Bold);
            Size sz = TextRenderer.MeasureText(str, ft);
            Rectangle screenSize = Screen.PrimaryScreen.WorkingArea;
            Point pt = new Point(screenSize.Width - sz.Width - 10, screenSize.Height - sz.Height - 10);
            popupWindow.Show(pt, 255, trackTitleColor, ft, 5000, FloatingWindow.AnimateMode.Blend, 0, str);
        }

        private void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (streamLoaded)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelPause(stream);
                    timeListenedTracker.Stop();
                    wasPlaying = true;
                }
                draggingSeek = true;
                UpdateProgressToMouse(e.X);
            }
        }

        public long GetPosition()
        {
            return Bass.BASS_ChannelGetPosition(stream);
        }

        public void SetPosition(long pos)
        {
            Bass.BASS_ChannelSetPosition(stream, pos);
        }

        public long GetLength()
        {
            return Bass.BASS_ChannelGetLength(stream);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggingSeek)
            {
                SetPosition((GetLength() * seekBar.Value) / seekBar.Maximum);
                if (wasPlaying)
                {
                    Play();
                    wasPlaying = false;
                }
                draggingSeek = false;
            }
            if (draggingVolume)
            {
                SetVolume((float)volumeBar.Value / (float)volumeBar.Maximum);
                draggingVolume = false;
            }
            RefreshPlayIcon();
        }

        private void UpdateProgressToMouse(int x)
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x > seekBar.Width)
            {
                x = seekBar.Width;
            }
            seekBar.Value = (int)((float)x / (float)seekBar.Width * seekBar.Maximum);
            SetPosition((GetLength() * seekBar.Value) / seekBar.Maximum);
        }

        private void UpdateVolumeToMouse(int x)
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x > volumeBar.Width)
            {
                x = volumeBar.Width;
            }
            volumeBar.Value = (int)((float)x / (float)volumeBar.Width * volumeBar.Maximum);
            float pct = ((float)(volumeBar.Value) / (float)(volumeBar.Maximum));
            SetVolume(pct);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingSeek)
            {
                UpdateProgressToMouse(e.X);
            }
            if (draggingVolume)
            {
                UpdateVolumeToMouse(e.X);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            seekBar.Maximum = seekBar.Width;
            seekBar.Width = seekBar.Maximum = this.Width - 40;
            trackText.Width = this.Width - 40;
            volumeBar.Maximum = volumeBar.Width;
            volumeBar.Location = new Point(this.Width - 180, volumeBar.Location.Y);
            playlistSelectorCombo.Location = new Point(this.Width - 350, playlistSelectorCombo.Location.Y);
            //playlistInfoTxt.Location = new Point(playlistSelectorCombo.Location.X - playlistInfoTxt.Width - 5, playlistInfoTxt.Location.Y);
            trackGrid.Width = this.Width - 40;
            int spaceForOther = trackGrid.ColumnHeadersHeight + 2;
            if (trackGrid.Controls.OfType<HScrollBar>().First().Visible) spaceForOther += SystemInformation.HorizontalScrollBarHeight;
            int spaceForRows = Height - 210 - spaceForOther;
            spaceForRows -= spaceForRows % trackGrid.RowTemplate.Height;
            trackGrid.Height = spaceForOther + spaceForRows;
            echoesLogo.Location = new Point(this.Width - 85, echoesLogo.Location.Y);
            if (Width > transposeChangerNum.Width + transposeChangerNum.Location.X + echoesLogo.Width + 15)
            {
                transposeChangerNum.Visible = true;
                transposeTxt.Visible = true;
            }
            else
            {
                transposeChangerNum.Visible = false;
                transposeTxt.Visible = false;
            }
            if (Width > eqButton.Width + eqButton.Location.X + echoesLogo.Width + 15)
            {
                eqButton.Visible = true;
            }
            else
            {
                eqButton.Visible = false;
            }
            if (Width > saveButton.Width + saveButton.Location.X + echoesLogo.Width + 15)
            {
                saveButton.Visible = true;
            }
            else
            {
                saveButton.Visible = false;
            }
            if (Width > visualsPicture.Location.X + echoesLogo.Width + 265)
            {
                visualsPicture.Width = echoesLogo.Location.X - visualsPicture.Location.X - 5;
                visualsPicture.Visible = true;
            }
            else
            {
                visualsPicture.Visible = false;
            }
            //last column auto resize thingy
            /*int estimatedWidth = 0;
            foreach (DataColumnInfo dci in currentColumnInfo)
            {
                estimatedWidth += dci.width;
            }
            if (dataGridView1.Width > estimatedWidth) dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.None).Width = dataGridView1.Width - dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Displayed);
            else dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.None).Width = currentColumnInfo.Last().width;*/
            RepositionControls();
            Refresh();
        }

        private void progressBar2_MouseDown(object sender, MouseEventArgs e)
        {
            draggingVolume = true;
            UpdateVolumeToMouse(e.X);
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= trackGrid.Rows.Count || trackGrid.Rows[e.RowIndex] == null || trackGrid.Rows[e.RowIndex].DataBoundItem == null) return;
            Track t = (Track)trackGrid.Rows[e.RowIndex].DataBoundItem;
            if (streamLoaded) SetPosition(0);
            StopPlayer();
            if (!File.Exists(t.filename))
            {
                PointToMissingFile(t);
                return;
            }
            OpenFile(t);
            if (nowPlaying != null) Play();
        }

        private void button5_Click(object sender, MouseEventArgs e)
        {
            fwdBtn.Image = LoadImage("forward3");
        }

        private void button7_Click(object sender, MouseEventArgs e)
        {
            rewBtn.Image = LoadImage("rewind3");
        }

        private void button6_Click(object sender, MouseEventArgs e)
        {
            backBtn.Image = LoadImage("back3");
        }

        void LoadCustomPlaylist(List<Track> lst)
        {
            playlist = lst;
            RenumberPlaylist(playlist);
            displayedItems = ItemType.Track;
            RefreshPlaylistGrid();
            if (autoShuffle) Shuffle();
            RefreshTotalTimeLabel();
            currentPlaylist = "";
            playlistSelectorCombo.SelectedIndex = 0;
            playlistChanged = false;
        }

        public void LoadEverythingFromArtist(string artist)
        {
            LoadCustomPlaylist(xmlCacher.GetAllTracks().Where(x => x.artist == artist).ToList());
        }

        public void LoadEverythingFromAlbum(string album)
        {
            LoadCustomPlaylist(xmlCacher.GetAllTracks().Where(x => x.album == album).ToList());
        }

        void LoadCacheAsPlaylist()
        {
            LoadCustomPlaylist(xmlCacher.GetAllTracks());
            playlistSelectorCombo.SelectedIndex = 1;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            AskToSavePlaylistChanges();
            if (playlistSelectorCombo.SelectedIndex == 0)
            {
                playlist = new List<Track>();
                DisplayPlaylistsInGrid();
                currentPlaylist = (string)playlistSelectorCombo.SelectedValue;
            }
            else if (playlistSelectorCombo.SelectedIndex == 1)
            {
                LoadCacheAsPlaylist();
            }
            else if (currentPlaylist != (string)playlistSelectorCombo.SelectedValue)
            {
                ImportM3u(knownPlaylists[playlistSelectorCombo.SelectedIndex - 2]);
            }
            trackGrid.Focus();
        }

        void CopySelectedToClipboard()
        {

            StringCollection paths = new StringCollection();
            foreach (DataGridViewRow row in trackGrid.Rows)
            {
                if (!row.Selected) continue;
                Track t = (Track)row.DataBoundItem;
                paths.Add(t.filename);
            }
            MessageBox.Show(paths.Count + " files copied");
            Clipboard.SetFileDropList(paths);
        }

        private void Echoes_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (draggingSeek || searchBox.Focused || !String.IsNullOrWhiteSpace(gridSearchWord)) { e.SuppressKeyPress = false; return; }
                    if (!IsPlaying()) Play();
                    else PausePlayer();
                    break;
                case Keys.Right:
                    AdvancePlayer();
                    break;
                case Keys.Left:
                    PlayerPrevious();
                    break;
                case Keys.C:
                    if (e.Control && trackGrid.Focused) CopySelectedToClipboard(); else e.SuppressKeyPress = false;
                    break;
                case Keys.Delete:
                    DeleteSelected();
                    break;
                case Keys.Enter:
                    if (ActiveControl == searchBox) break;
                    try
                    {
                        Track theTrack = null;
                        if (trackGrid.SelectedRows.Count > 0)
                            theTrack = (Track)trackGrid.SelectedRows[0].DataBoundItem;
                        else if (trackGrid.Rows.Count > 0)
                            theTrack = (Track)trackGrid.Rows[0].DataBoundItem;
                        if (theTrack == null)
                        {
                            PlayFirst();
                        }
                        else if (nowPlaying == null)
                        {
                            OpenFile(theTrack);
                        }
                        else if (!nowPlaying.Equals(theTrack))
                        {
                            OpenFile(theTrack);
                        }
                    }
                    catch (Exception zz)
                    {
                        MessageBox.Show(zz.ToString());
                    }
                    break;
                default:
                    e.SuppressKeyPress = false;
                    break;
            }
        }

        List<Track> tracksToLoadForTags;

        private void tagsLoaderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            /*if (rebuildCache==false)
            {*/
            tracksToLoadForTags = new List<Track>();
            for (int z = 0; z < playlist.Count; z++)
            {
                if (e.Cancel) break;

                Track t = playlist[z];
                if (!xmlCacher.GetCacheInfo(t) || reloadTagsFlag)
                {
                    t.GetTags();
                    //xmlCacher.AddOrUpdate(new List<Track>(){t});
                    tracksToLoadForTags.Add(t);
                }
                trackGrid.InvalidateRow(z);
                tagsLoaderWorker.ReportProgress((int)(((float)z / (float)playlist.Count) * 100));
            }
            tagsLoaderWorker.ReportProgress(100);
            /*}
            else
            {
                for (int i = 0; i < trackGrid.Rows.Count; i++)
                {
                    Track t = (Track)trackGrid.Rows[i].DataBoundItem;
                    t.GetTags();
                    trackGrid.InvalidateRow(i);
                    backgroundWorker1.ReportProgress((int)(((float)i / (float)playlist.Count) * 100));
                    if (backgroundWorker1.CancellationPending) return;
                }
                xmlCacher.AddOrUpdate(playlist);
            }*/
        }

        private void tagsLoaderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            trackText.Text = e.ProgressPercentage + "% loaded";
        }
        private void tagsLoaderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            xmlCacher.AddOrUpdate(tracksToLoadForTags);
            reloadTagsFlag = false;
            RefreshTotalTimeLabel();
            AllowSorting(true);
            if (nowPlaying == null) trackText.Text = "100% Loaded";
            else DisplayTrackInfo(nowPlaying);
            trackGrid.Refresh();
            if (reinitTagLoader)
            {
                reinitTagLoader = false;
                tagsLoaderWorker.RunWorkerAsync();
            }
        }

        public TimeSpan TotalPlaylistListened(SortableBindingList<Track> list)
        {
            TimeSpan ret = new TimeSpan();
            int kount = 0;
            foreach (Track t in list)
            {
                kount++;
                ret += TimeSpan.FromSeconds(t.listened);
            }
            return ret;
        }

        public TimeSpan TotalPlaylistTime(SortableBindingList<Track> list)
        {
            TimeSpan ret = new TimeSpan();
            foreach (Track t in list)
            {
                //if(t.title==null || t.length==null) MessageBox.Show(t.title + t.length);
                ret += TimeSpan.FromSeconds(t.length);
            }
            return ret;
        }

        void AllowSorting(bool value)
        {
            searchBox.Enabled = value;
            sortingAllowed = value;
            foreach (DataGridViewColumn c in trackGrid.Columns)
            {
                if (value) c.SortMode = DataGridViewColumnSortMode.Automatic;
                else c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        long TotalFileSize(SortableBindingList<Track> tracksList)
        {
            long ret = 0;
            foreach (Track t in tracksList)
            {
                ret += t.size;
            }
            return ret;
        }

        void RefreshTotalTimeLabel()
        {
            SortableBindingList<Track> tehList = (SortableBindingList<Track>)(trackGrid.DataSource);
            if (displayedItems != ItemType.Playlist)
            {
                playlistInfoTxt.Text = tehList.Count + " tracks (" + TotalFileSize(tehList).BytesToString() + "), " + TotalPlaylistTime(tehList).ProperTimeFormat();
                playlistInfoTxt.Text += " [" + TotalPlaylistListened(tehList).ProperTimeFormat() + " listened]";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Shuffle();
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (trackGrid.Rows.Count > 0)
            {

                gridSearchWord += e.KeyChar;
                if (gridSearchWord == " ")
                {
                    gridSearchWord = "";
                    return;
                }

                DataGridViewRow currentRow;
                Track currentTrack;
                int currentIndex;
                if (trackGrid.SelectedRows.Count == 0)
                {
                    currentRow = trackGrid.Rows[0];
                }
                else
                {
                    currentRow = trackGrid.SelectedRows[0];
                }
                currentIndex = currentRow.Index;
                if (!gridSearchFirstStroke)
                {
                    currentRow = trackGrid.Rows[currentIndex];
                    currentTrack = (Track)currentRow.DataBoundItem;
                    int currentColumnIndex = 0;
                    if (highlightedColumn == null || highlightedColumn.ValueType != typeof(string))
                    {
                        foreach (DataGridViewColumn col in trackGrid.Columns)
                        {
                            if (col.HeaderText == "Title")
                            {
                                currentColumnIndex = col.Index;
                                break;
                            }
                        }
                    }
                    else
                    {
                        currentColumnIndex = highlightedColumn.Index;
                    }
                    string matchString = (string)currentRow.Cells[currentColumnIndex].Value;
                    if (matchString.StartsWith(gridSearchWord, true, null))
                    {
                        trackGrid.ClearSelection();
                        trackGrid.CurrentCell = currentRow.Cells[0];
                        currentRow.Selected = true;
                        return;
                    }
                }
                gridSearchFirstStroke = false;
                bool foundFlag = false;
                for (int i = 0; i < trackGrid.Rows.Count; i++)
                {
                    currentIndex++;
                    if (trackGrid.Rows.Count - 1 < currentIndex)
                    {
                        currentIndex = 0;
                    }
                    currentRow = trackGrid.Rows[currentIndex];
                    currentTrack = (Track)currentRow.DataBoundItem;
                    int currentColumnIndex = 0;
                    if (highlightedColumn == null || highlightedColumn.ValueType != typeof(string))
                    {
                        foreach (DataGridViewColumn col in trackGrid.Columns)
                        {
                            if (col.HeaderText == "Title")
                            {
                                currentColumnIndex = col.Index;
                                break;
                            }
                        }
                    }
                    else
                    {
                        currentColumnIndex = highlightedColumn.Index;
                    }
                    string matchString = (string)currentRow.Cells[currentColumnIndex].Value;
                    if (matchString != null && matchString.StartsWith(gridSearchWord, true, null))
                    {
                        trackGrid.ClearSelection();
                        trackGrid.Rows[currentIndex].Selected = true;
                        if (!trackGrid.Rows[currentIndex].Displayed)
                        {
                            trackGrid.FirstDisplayedScrollingRowIndex = currentIndex;
                        }
                        foundFlag = true;
                        break;
                    }
                }
                if (!foundFlag)
                {
                    gridSearchTimerRemaining = 0;
                    gridSearchWord = "";
                    System.Media.SystemSounds.Beep.Play();
                    gridSearchTimer.Stop();
                }
                else
                {
                    gridSearchTimerRemaining = gridSearchReset;
                    if (!gridSearchTimer.Enabled)
                    {
                        gridSearchTimer.Start();
                    }
                }
            }
        }

        private void gridSearchTimer_Tick(object sender, EventArgs e)
        {
            if (gridSearchTimerRemaining > 0)
            {
                gridSearchTimerRemaining--;
            }
            else
            {
                gridSearchWord = "";
                gridSearchFirstStroke = true;
                gridSearchTimer.Stop();
            }
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            HighlightPlayingTrack();
            trackGrid.ClearSelection();
            foreach (int i in savedSelectionForSort)
            {
                DataGridViewRow raw = FindTrackRow(i);
                if (raw != null) raw.Selected = true;
            }
            if (trackGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                {
                    if (rw.Visible && !rw.Displayed)
                    {
                        ScrollGridTo(rw);
                        break;
                    }
                }
            }
        }
        public void OpenInExplorer(List<string> filenames)
        {
            OpenInExplorer(filenames.ToArray());
        }

        public void OpenInExplorer(string[] filenames)
        {
            ShowSelectedInExplorer.FilesOrFolders(filenames);
        }

        void OpenDupeFinder()
        {
            df = new DupeFinder((trackGrid.DataSource as SortableBindingList<Track>).ToList());
            df.Show(this);
            df.Location = new Point(this.Location.X + this.Width / 2 - df.Width / 2, this.Location.Y + this.Height / 2 - df.Height / 2);
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (trackGrid.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.None)
            {
                trackGrid.ClearSelection();
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    DataGridViewRow r = trackGrid.Rows[trackGrid.HitTest(e.X, e.Y).RowIndex];
                    if (!r.Selected) trackGrid.ClearSelection();
                    r.Selected = true;
                    Track t = (Track)r.DataBoundItem;
                    List<Track> selectedTracks = new List<Track>();
                    foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                    {
                        selectedTracks.Add((Track)rw.DataBoundItem);
                    }
                    ContextMenu cm = new ContextMenu();
                    MenuItem miShowExplorer = new MenuItem("Show in Windows Explorer");
                    miShowExplorer.Click += (theSender, eventArgs) =>
                    {
                        string[] filenamesToShow = new string[selectedTracks.Count];
                        for (int i = 0; i < selectedTracks.Count; i++)
                        {
                            filenamesToShow[i] = selectedTracks[i].filename;
                        }
                        OpenInExplorer(filenamesToShow);
                    };
                    MenuItem miDelete = new MenuItem("Delete");
                    miDelete.Click += (theSender, eventArgs) =>
                    {
                        DeleteSelected();
                    };
                    MenuItem miSync = new MenuItem("Synchronize...");
                    miSync.Click += (theSender, eventArgs) =>
                    {
                        SynchronizePlaylist();
                    };
                    cm.MenuItems.Add(miShowExplorer);
                    cm.MenuItems.Add(miDelete);
                    cm.MenuItems.Add(miSync);
                    if (displayedItems != ItemType.Playlist)
                    {
                        MenuItem miAddToPlaylist = new MenuItem("Add to playlist");
                        MenuItem miRemove = new MenuItem("Remove");
                        MenuItem miRemoveEntriesContainedIn = new MenuItem("Entries contained in playlist");
                        miRemove.MenuItems.Add(miRemoveEntriesContainedIn);
                        for (int i = 0; i < knownPlaylists.Count; i++)
                        {
                            int z = i;
                            if (playlistSelectorCombo.SelectedIndex > 1 && knownPlaylists[i] == knownPlaylists[playlistSelectorCombo.SelectedIndex - 2])
                                continue;
                            string displayName = knownPlaylists[z];
                            if (knownPlaylists.Except(displayName).Where(x => Path.GetFileName(x) == Path.GetFileName(displayName)).Count() == 0)
                            {
                                displayName = Path.GetFileName(displayName);
                            }
                            MenuItem itm = new MenuItem(displayName);
                            itm.Click += (theSender, eventArgs) =>
                            {
                                CopyTrackToPlaylist(selectedTracks, knownPlaylists[z]);
                            };
                            miAddToPlaylist.MenuItems.Add(itm);
                            itm = new MenuItem(displayName);
                            itm.Click += (theSender, eventArgs) =>
                            {
                                List<string> thatPlaylist = SuperM3U.Reader.GetFiles(knownPlaylists[z]);
                                int before = playlist.Count;
                                playlist = playlist.Where(x => !thatPlaylist.Contains(x.filename)).ToList();
                                RefreshPlaylistGrid();
                                MessageBox.Show("Removed " + (before - playlist.Count) + " entries.", "Removed");
                            };
                            miRemoveEntriesContainedIn.MenuItems.Add(itm);
                        }
                        MenuItem npItem = new MenuItem("New playlist");
                        npItem.Click += (theSender, eventArgs) =>
                        {
                            string newList = CreateNewPlaylist();
                            CopyTrackToPlaylist(selectedTracks, newList);
                        };
                        miAddToPlaylist.MenuItems.Add(npItem);

                        MenuItem miEditTags = new MenuItem("Edit tags");
                        miEditTags.Click += (theSender, eventArgs) =>
                        {
                            LaunchTagEditor();
                        };
                        MenuItem miConvert = new MenuItem("Convert");
                        miConvert.Click += (theSender, eventArgs) =>
                        {
                            Convert();
                        };
                        //find column clicked
                        MenuItem miFindFrom = new MenuItem();
                        MenuItem miRemoveWith = miFindFrom;
                        MenuItem miRemoveWithout = miFindFrom;
                        bool includeFindFrom = false;
                        DataGridViewColumn columnClicked = trackGrid.Columns[trackGrid.HitTest(e.X, e.Y).ColumnIndex];
                        if (COLUMN_HEADERS_ALLOWED_CUSTOM.Contains<string>(columnClicked.HeaderText))
                        {
                            string columnProperty = COLUMN_PROPERTIES[COLUMN_HEADERS.IndexOf(columnClicked.HeaderText)];
                            miFindFrom = new MenuItem("Find everything with this " + columnClicked.HeaderText);
                            miFindFrom.Click += (theSender, eventArgs) =>
                            {
                                List<object> selectedTracksProperties = new List<object>();
                                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                                {
                                    Track rtr = (Track)rw.DataBoundItem;
                                    selectedTracksProperties.Add(typeof(Track).GetProperty(columnProperty).GetValue(rtr, null));
                                }
                                List<Track> customPlaylist = xmlCacher.GetAllTracks().Where(x => selectedTracksProperties.Contains(typeof(Track).GetProperty(columnProperty).GetValue(x, null))).ToList();
                                LoadCustomPlaylist(customPlaylist);
                            };
                            miRemoveWith = new MenuItem("Everything with this " + columnClicked.HeaderText);
                            miRemoveWith.Click += (theSender, eventArgs) =>
                            {
                                List<object> selectedTracksProperties = new List<object>();
                                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                                {
                                    Track rtr = (Track)rw.DataBoundItem;
                                    selectedTracksProperties.Add(typeof(Track).GetProperty(columnProperty).GetValue(rtr, null));
                                }
                                int before = playlist.Count;
                                playlist = playlist.Where(x => !selectedTracksProperties.Contains(typeof(Track).GetProperty(columnProperty).GetValue(x, null))).ToList();
                                RefreshPlaylistGrid();
                                MessageBox.Show("Removed " + (before - playlist.Count) + " entries.", "Removed");
                            };
                            miRemoveWithout = new MenuItem("Everything without this " + columnClicked.HeaderText);
                            miRemoveWithout.Click += (theSender, eventArgs) =>
                            {
                                List<object> selectedTracksProperties = new List<object>();
                                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                                {
                                    Track rtr = (Track)rw.DataBoundItem;
                                    selectedTracksProperties.Add(typeof(Track).GetProperty(columnProperty).GetValue(rtr, null));
                                }
                                int before = playlist.Count;
                                playlist = playlist.Where(x => selectedTracksProperties.Contains(typeof(Track).GetProperty(columnProperty).GetValue(x, null))).ToList();
                                RefreshPlaylistGrid();
                                MessageBox.Show("Removed " + (before - playlist.Count) + " entries.", "Removed");
                            };
                            includeFindFrom = true;
                        }
                        MenuItem miRenumber = new MenuItem("Save this order");
                        miRenumber.Click += (theSender, eventArgs) =>
                        {
                            RenumberPlaylist(playlist);
                        };
                        MenuItem miReloadTags = new MenuItem("Reload tag info");
                        miReloadTags.Click += (theSender, eventArgs) =>
                        {
                            ReloadTags();
                        };
                        MenuItem miDupes = new MenuItem("Find duplicates");
                        miDupes.Click += (theSender, eventArgs) =>
                        {
                            OpenDupeFinder();
                        };
                        MenuItem miRemoveMissing = new MenuItem("Missing file entries");
                        if (playlistSelectorCombo.SelectedIndex > 1)
                        {
                            miRemoveMissing.Click += (theSender, eventArgs) =>
                            {
                                RemoveUnexisting();
                            };
                        }
                        else
                        {
                            miRemoveMissing.Click += (theSender, eventArgs) =>
                            {
                                xmlCacher.CleanupCache();
                                playlist = xmlCacher.GetAllTracks();
                                FixPlaylistNumbers(playlist);
                                RefreshPlaylistGrid();
                            };
                        }
                        miRemove.MenuItems.Add(miRemoveMissing);
                        MenuItem miMoveUp = new MenuItem("Up");
                        miMoveUp.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedUp();
                        };
                        MenuItem miMoveDown = new MenuItem("Down");
                        miMoveDown.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedDown();
                        };
                        MenuItem miMoveToTop = new MenuItem("To top");
                        miMoveToTop.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedToTop();
                        };
                        MenuItem miMoveToBottom = new MenuItem("To bottom");
                        miMoveToBottom.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedToBottom();
                        };
                        MenuItem miMove = new MenuItem("Move");
                        miMove.MenuItems.Add(miMoveUp);
                        miMove.MenuItems.Add(miMoveDown);
                        miMove.MenuItems.Add(miMoveToTop);
                        miMove.MenuItems.Add(miMoveToBottom);



                        MenuItem miInvertSelection = new MenuItem("Invert selection");
                        miInvertSelection.Click += (theSender, eventArgs) =>
                        {
                            InvertSelection();
                        };
                        cm.MenuItems.Add(miAddToPlaylist);
                        cm.MenuItems.Add(miMove);
                        cm.MenuItems.Add(miRemove);
                        cm.MenuItems.Add(miEditTags);
                        cm.MenuItems.Add(miConvert);
                        if (includeFindFrom)
                        {
                            cm.MenuItems.Add(miFindFrom);
                            miRemove.MenuItems.Add(miRemoveWith);
                            miRemove.MenuItems.Add(miRemoveWithout);
                        }
                        cm.MenuItems.Add(miInvertSelection);
                        cm.MenuItems.Add(miRenumber);
                        cm.MenuItems.Add(miReloadTags);
                        cm.MenuItems.Add(miDupes);
                    }
                    cm.Show(trackGrid, e.Location);
                }
                catch (Exception)
                {
                }
            }
        }

        private void SynchronizePlaylist()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if (Exporter.inProgress) MessageBox.Show("Sync already in progress. Wait for it to finish.");
                    else
                    {
                        string playlistPath;
                        bool abort = false;
                        if (displayedItems != ItemType.Playlist)
                        {
                            if (!File.Exists(currentPlaylist))
                            {
                                EnterTextForm etf = new EnterTextForm("Enter playlist name", "");
                                if (etf.ShowDialog() != DialogResult.OK)
                                {
                                    abort = true;
                                }
                                else
                                {
                                    playlistPath = etf.enteredText.Text;
                                }
                            }
                            else playlistPath = currentPlaylist;
                        }
                        if (!abort)
                        {
                            List<List<Track>> listOfLists = new List<List<Track>>();
                            List<string> playlistNames = new List<string>();
                            if (displayedItems == ItemType.Playlist)
                            {
                                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                                {
                                    Track t = (Track)rw.DataBoundItem;
                                    List<Track> readPlist = ReadM3u(t.filename);
                                    listOfLists.Add(readPlist);
                                    playlistNames.Add(Path.GetFileName(t.filename));
                                }
                            }
                            else
                            {
                                listOfLists.Add(playlist);
                                playlistNames.Add(Path.GetFileName(currentPlaylist));
                            }
                            Exporter xporter = new Exporter(listOfLists, playlistNames, fbd.SelectedPath);
                            xporter.ShowDialog();
                        }
                    }
                }
            }
        }

        private void InvertSelection()
        {
            foreach (DataGridViewRow rw in trackGrid.Rows)
            {
                rw.Selected = !rw.Selected;
            }
        }

        void Convert()
        {
            if (theConverter != null) theConverter.Dispose();
            if (trackGrid.SelectedRows.Count == 0) return;
            List<Track> selectedToConvert = new List<Track>();
            foreach (DataGridViewRow rw in trackGrid.SelectedRows)
            {
                selectedToConvert.Add((Track)rw.DataBoundItem);
            }
            if (Bass.BASS_GetDevice() == -1) InitSoundDevice();
            if (!File.Exists(lameExeLocation))
            {
                if (MessageBox.Show("No lame.exe found. The converter needs the encoder executable placed in the same directory as the Echoes executable."
                    + Environment.NewLine + Environment.NewLine + "Would you like to visit a third party website to download LAME encoder?"
                    , "LAME encoder needed.", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(lameExeDownloadSite);
                }
                return;
            }
            theConverter = new Converter(selectedToConvert);
            theConverter.Show(this);

            /*if(trackGrid.SelectedRows.Count==0) return;
            Track toConvert=((Track)trackGrid.SelectedRows[0].DataBoundItem);
            if (!File.Exists(toConvert.filename)) return;

            int convertStream=20;

            if (!LoadStream(toConvert.filename, out convertStream, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_DECODE)) return;

            if (Bass.BASS_ErrorGetCode() == 0 && GetLength() > 0)
            {
                EncoderLAME l = new EncoderLAME(convertStream);
                l.InputFile = null;
                l.OutputFile = "converted.mp3";
                l.LAME_Bitrate = (int)EncoderLAME.BITRATE.kbps_192;
                l.LAME_Mode = EncoderLAME.LAMEMode.Default;
                l.LAME_Quality = EncoderLAME.LAMEQuality.Quality;
                //if(!Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(convertStream, l.TAGs)) Console.WriteLine("no tags");
                l.TAGs = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(toConvert.filename);
                l.Start(null, IntPtr.Zero, false);
                byte[] encBuffer = new byte[65536];
                while (Bass.BASS_ChannelIsActive(convertStream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // getting sample data will automatically feed the encoder
                    int len = Bass.BASS_ChannelGetData(convertStream, encBuffer, encBuffer.Length);
                }
                l.Stop();  // finish
                Bass.BASS_StreamFree(convertStream);
            }
            else
            {
                Console.WriteLine("Error converting: " + Bass.BASS_ErrorGetCode());
            }*/
        }

        void MoveSelectedDown()
        {
            List<int> selectedShit = new List<int>();
            List<Track> selectedTracks = new List<Track>();
            foreach (DataGridViewRow rw in trackGrid.SelectedRows)
            {
                selectedShit.Add(rw.Index);
                selectedTracks.Add((Track)rw.DataBoundItem);
            }
            selectedShit = selectedShit.OrderByDescending(x => x).ToList();
            foreach (int rw in selectedShit)
            {
                MoveTrackInPlaylist(rw, rw + 1);
            }
            foreach (Track t in selectedTracks)
            {
                FindTrackRow(t).Selected = true;
            }
        }

        void MoveSelectedToBottom()
        {
            List<int> selectedShit = new List<int>();
            List<Track> selectedTracks = new List<Track>();
            foreach (DataGridViewRow rw in trackGrid.SelectedRows)
            {
                selectedShit.Add(rw.Index);
                selectedTracks.Add((Track)rw.DataBoundItem);
            }
            selectedTracks = selectedTracks.OrderBy(x => FindTrackRow(x).Index).ToList();
            foreach (Track tr in selectedTracks)
            {
                MoveTrackInPlaylist(FindTrackRow(tr).Index, playlist.Count - 1);
            }
            foreach (Track t in selectedTracks)
            {
                FindTrackRow(t).Selected = true;
            }
        }

        void MoveSelectedToTop()
        {
            List<int> selectedShit = new List<int>();
            List<Track> selectedTracks = new List<Track>();
            foreach (DataGridViewRow rw in trackGrid.SelectedRows)
            {
                selectedShit.Add(rw.Index);
                selectedTracks.Add((Track)rw.DataBoundItem);
            }
            selectedTracks = selectedTracks.OrderByDescending(x => FindTrackRow(x).Index).ToList();
            foreach (Track tr in selectedTracks)
            {
                MoveTrackInPlaylist(FindTrackRow(tr).Index, 0);
            }
            foreach (Track t in selectedTracks)
            {
                FindTrackRow(t).Selected = true;
            }
        }

        void MoveSelectedUp()
        {
            List<int> selectedShit = new List<int>();
            List<Track> selectedTracks = new List<Track>();
            foreach (DataGridViewRow rw in trackGrid.SelectedRows)
            {
                selectedShit.Add(rw.Index);
                selectedTracks.Add((Track)rw.DataBoundItem);
            }
            selectedShit = selectedShit.OrderBy(x => x).ToList();
            foreach (int rw in selectedShit)
            {
                MoveTrackInPlaylist(rw, rw - 1);
            }
            foreach (Track t in selectedTracks)
            {
                FindTrackRow(t).Selected = true;
            }
        }

        void DeleteSelected()
        {
            List<int> lst = new List<int>();
            for (int i = trackGrid.SelectedRows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow rw = trackGrid.SelectedRows[i];
                if (rw.Visible)
                {
                    Track ss = (Track)rw.DataBoundItem;
                    lst.Add(ss.num);
                }
            }
            DeleteTrack(lst);
        }

        private void noSelectButton1_Click(object sender, EventArgs e)
        {
            ExportPlaylist();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
            /*else if (e.KeyCode == Keys.Space)
            {
                
                if (String.IsNullOrWhiteSpace(gridSearchWord))
                {
                    
                    e.SuppressKeyPress = true;
                }
            }*/
        }

        private void Echoes_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        List<string> GetFilesFromDrag(string[] input)
        {
            List<string> ret = new List<string>();
            foreach (string str in input)
            {
                FileAttributes attr = File.GetAttributes(str);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (string okType in supportedAudioTypes)
                    {
                        ret.AddRange(Directory.EnumerateFiles(str, "*" + okType, SearchOption.AllDirectories));
                    }
                }
                else
                {
                    if (supportedAudioTypes.Contains(Path.GetExtension(str)) || Path.GetExtension(str).ToLower() == ".m3u") ret.Add(str);
                }
            }
            return ret;
        }

        private void Echoes_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedInfo = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadFiles(GetFilesFromDrag(droppedInfo));
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            trackGrid.Focus();
        }

        public void RefreshGrid()
        {
            trackGrid.Refresh();
        }

        public void InvokeUI(Action a)
        {
            this.BeginInvoke(new MethodInvoker(a));
        }

        void SavePlaylist()
        {
            if (!File.Exists(currentPlaylist))
            {
                ExportPlaylist();
                return;
            }
            ExportM3u(currentPlaylist, playlist.OrderBy(x => x.num).ToList());
            System.Media.SystemSounds.Exclamation.Play();
            playlistChanged = false;
        }

        private void modifiedButton1_Click(object sender, EventArgs e)
        {
            SavePlaylist();
        }

        void RemoveUnexisting()
        {
            //List<int> nums = new List<int>();
            List<Track> tracksToRemove = new List<Track>();
            foreach (Track t in playlist)
            {
                if (!File.Exists(t.filename))
                {
                    tracksToRemove.Add(t);
                }
            }
            playlist = playlist.Where(x => !tracksToRemove.Contains(x)).ToList();
            MessageBox.Show(tracksToRemove.Count + " entries were removed.");
            RefreshPlaylistGrid();
            RefreshTotalTimeLabel();
        }

        void LaunchTagEditor()
        {
            if (te != null) { te.Close(); te.Dispose(); }
            if (trackGrid.SelectedRows.Count == 0) return;
            List<Track> tracksToEdit = new List<Track>();
            foreach (DataGridViewRow r in trackGrid.SelectedRows) tracksToEdit.Add((Track)r.DataBoundItem);
            te = new TagEditor(tracksToEdit, this);
            te.Show(this);
            te.Location = new Point(this.Location.X + this.Width / 2 - te.Width / 2, this.Location.Y + this.Height / 2 - te.Height / 2);
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle || displayedItems == ItemType.Playlist) return;
            trackGrid.ClearSelection();
            trackGrid.Rows[e.RowIndex].Selected = true;
            LaunchTagEditor();
        }

        void ShowOptions()
        {
            if (opts != null) opts.Hide();
            opts = new Options();
            opts.Show(this);
        }

        private void modifiedButton2_Click(object sender, EventArgs e)
        {
            ShowOptions();
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            currentColumnInfo.First(x => x.id == COLUMN_PROPERTIES.ToList().IndexOf(e.Column.DataPropertyName)).width = e.Column.Width;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                savedSelectionForSort = new List<int>();
                foreach (DataGridViewRow rw in trackGrid.SelectedRows)
                {
                    Track t = (Track)rw.DataBoundItem;
                    savedSelectionForSort.Add(t.num);
                }
            }
            else
            {
                foreach (DataGridViewColumn clmn in trackGrid.Columns)
                {
                    if (e.ColumnIndex == clmn.Index) clmn.HeaderCell.Style.BackColor = controlBackColor.Darken();
                    else clmn.HeaderCell.Style.BackColor = controlBackColor;
                }
                highlightedColumn = trackGrid.Columns[e.ColumnIndex];
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            RefreshPlaylistGrid();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SetFrequency();
        }

        void ShowStats()
        {
            if (stats != null) stats.Dispose();
            stats = new Statistics(xmlCacher.GetAllTracks());
            stats.Show(this);
        }

        void ShowEq()
        {
            if (eqWindow != null) eqWindow.Dispose();
            eqWindow = new Equalizer();
            eqWindow.Show(this);
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = LoadImage("play2");
            else playBtn.Image = LoadImage("pause2");
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = LoadImage("play1");
            else playBtn.Image = LoadImage("pause1");
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = LoadImage("play2");
            else playBtn.Image = LoadImage("pause2");
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                PausePlayer();
            }
            else
            {
                Play();
            }
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            stopBtn.Image = LoadImage("stop2");
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            stopBtn.Image = LoadImage("stop1");
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            stopBtn.Image = LoadImage("stop2");
            StopPlayer();
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            backBtn.Image = LoadImage("back2");
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            backBtn.Image = LoadImage("back1");
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            backBtn.Image = LoadImage("back2");
            PlayerPrevious();
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            rewBtn.Image = LoadImage("rewind2");
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            rewBtn.Image = LoadImage("rewind1");
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            rewBtn.Image = LoadImage("rewind2");
            SetPosition(0);
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            fwdBtn.Image = LoadImage("forward2");
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            fwdBtn.Image = LoadImage("forward1");
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            fwdBtn.Image = LoadImage("forward2");
            AdvancePlayer();
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            visualisationStyle++;
            if (visualisationStyle > 4) visualisationStyle = 1;
        }

        public static void TrackCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;
            DataGridView dgv = (DataGridView)sender;
            if (e.RowIndex >= 0)
            {
                if ((dgv.Columns[e.ColumnIndex].HeaderText == "Length" || dgv.Columns[e.ColumnIndex].HeaderText == "Listened"))
                {
                    int val = (int)e.Value;
                    e.Value = val.ToTime();
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Bitrate"))
                {
                    int val = (int)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = val + " kbps";
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Year"))
                {
                    int val = (int)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = val + "";
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Track #"))
                {
                    int val = (int)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = val + "";
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Size"))
                {
                    long val = (long)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = val.BytesToString();
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Last opened"))
                {
                    DateTime val = (DateTime)e.Value;
                    if (val == DateTime.MinValue) e.Value = "";
                    else e.Value = val.ToString(Program.lastOpenedDateFormat);
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "True Bitrate"))
                {
                    float val = (float)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = String.Format("{0:0.00}", val) + " kbps";
                    e.FormattingApplied = true;
                }
                else if ((dgv.Columns[e.ColumnIndex].HeaderText == "Times listened"))
                {
                    float val = (float)e.Value;
                    if (val == 0) e.Value = ""; else e.Value = String.Format("{0:0.0}", val);
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            TrackCellFormatting(sender, e);
        }

        private void openBtn_MouseDown(object sender, MouseEventArgs e)
        {
            openBtn.Image = LoadImage("import3");
        }

        private void openBtn_MouseEnter(object sender, EventArgs e)
        {
            openBtn.Image = LoadImage("import2");
        }

        private void openBtn_MouseLeave(object sender, EventArgs e)
        {
            openBtn.Image = LoadImage("import1");
        }

        private void openBtn_MouseUp(object sender, MouseEventArgs e)
        {
            openBtn.Image = LoadImage("import2");
            ChooseFile();
        }

        private void exportBtn_MouseDown(object sender, MouseEventArgs e)
        {
            exportBtn.Image = LoadImage("export3");
        }

        private void exportBtn_MouseEnter(object sender, EventArgs e)
        {
            exportBtn.Image = LoadImage("export2");
        }

        private void exportBtn_MouseLeave(object sender, EventArgs e)
        {
            exportBtn.Image = LoadImage("export1");
        }

        private void exportBtn_MouseUp(object sender, MouseEventArgs e)
        {
            exportBtn.Image = LoadImage("export2");
            ExportPlaylist();
        }

        private void shuffleBtn_MouseDown(object sender, MouseEventArgs e)
        {
            shuffleBtn.Image = LoadImage("shuffle3");
        }

        private void shuffleBtn_MouseEnter(object sender, EventArgs e)
        {
            shuffleBtn.Image = LoadImage("shuffle2");
        }

        private void shuffleBtn_MouseLeave(object sender, EventArgs e)
        {
            shuffleBtn.Image = LoadImage("shuffle1");
        }

        private void shuffleBtn_MouseUp(object sender, MouseEventArgs e)
        {
            shuffleBtn.Image = LoadImage("shuffle2");
            Shuffle();
        }

        private void optionsBtn_MouseDown(object sender, MouseEventArgs e)
        {
            optionsBtn.Image = LoadImage("options3");
        }

        private void optionsBtn_MouseEnter(object sender, EventArgs e)
        {
            optionsBtn.Image = LoadImage("options2");
        }

        private void optionsBtn_MouseLeave(object sender, EventArgs e)
        {
            optionsBtn.Image = LoadImage("options1");
        }

        private void optionsBtn_MouseUp(object sender, MouseEventArgs e)
        {
            optionsBtn.Image = LoadImage("options2");
            ShowOptions();
        }

        private void repeatBtn_MouseDown(object sender, MouseEventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = LoadImage("repeatNone3");
                    break;
                case 1:
                    repeatBtn.Image = LoadImage("repeatList3");
                    break;
                default:
                    repeatBtn.Image = LoadImage("repeatTrack3");
                    break;
            }
        }

        private void repeatBtn_MouseEnter(object sender, EventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = LoadImage("repeatNone2");
                    break;
                case 1:
                    repeatBtn.Image = LoadImage("repeatList2");
                    break;
                default:
                    repeatBtn.Image = LoadImage("repeatTrack2");
                    break;
            }
        }

        private void repeatBtn_MouseLeave(object sender, EventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = LoadImage("repeatNone1");
                    break;
                case 1:
                    repeatBtn.Image = LoadImage("repeatList1");
                    break;
                default:
                    repeatBtn.Image = LoadImage("repeatTrack1");
                    break;
            }
        }

        private void repeatBtn_MouseUp(object sender, MouseEventArgs e)
        {
            if (repeat == 0)
            {
                repeat = 1;
            }
            else if (repeat == 1)
            {
                repeat = 2;
            }
            else
            {
                repeat = 0;
            }
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = LoadImage("repeatNone2");
                    break;
                case 1:
                    repeatBtn.Image = LoadImage("repeatList2");
                    break;
                default:
                    repeatBtn.Image = LoadImage("repeatTrack2");
                    break;
            }
            SetLooping();
        }

        private void Echoes_Paint(object sender, PaintEventArgs e)
        {
            //draws a border with appropriate color on the searchbox properly
            Pen p = new Pen(controlForeColor);
            Graphics g = e.Graphics;
            int variance = 1;
            g.DrawRectangle(p, new Rectangle(searchBox.Location.X - variance, searchBox.Location.Y - variance, searchBox.Width + variance, searchBox.Height + variance));
        }

        private void trackText_FontChanged(object sender, EventArgs e)
        {
            ((RichTextBox)sender).Height = ((RichTextBox)sender).Font.Height;
            RepositionControls();
        }

        private void eqButton_Click(object sender, EventArgs e)
        {
            ShowEq();
        }

        private void normalizerWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            /*normalizationBenchmark = DateTime.Now;
            int strm;
            if (!LoadStream(toNormalize.filename, out strm, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_DECODE | BASSFlag.BASS_MUSIC_PRESCAN)) return;
            Bass.BASS_ChannelSetPosition(strm, 0d);
            long len = Bass.BASS_ChannelGetLength(strm);
            long totaltimeFrames = (long)((Bass.BASS_ChannelBytes2Seconds(strm, len)+1)*1000)/20;
            if ((Bass.BASS_ChannelFlags(strm, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_MUSIC_LOOP) == BASSFlag.BASS_MUSIC_LOOP)
            {
                Bass.BASS_ChannelFlags(strm, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_MUSIC_LOOP);
            }
            peak = 0;
            for (int i = 0; i < totaltimeFrames; i++)
            {
                if (System.Convert.ToBoolean(Bass.BASS_ChannelIsActive(strm)))
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                    {
                        e.Cancel = true;
                        Console.WriteLine("Normalization abandoned.");
                        return;
                    }
                    int level = Bass.BASS_ChannelGetLevel(strm);
                    int left = Utils.LowWord32(level); // the left level
                    int right = Utils.HighWord32(level); // the right level

                    if (peak < left) peak = left;
                    if (peak < right) peak = right;
                }
                else break;
            }
            float divideFrom = 32768f;
            if (peak > 32768) divideFrom = 65536f;
            normGain = (divideFrom/(float)peak);/*/
        }

        private void normalizerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DSPGain != null && DSPGain.IsAssigned) DSPGain.Stop();
            DateTime normBenchmarkEnd = DateTime.Now;
            //Console.WriteLine("----------------------------");
            Console.Write("Peak: " + peak);
            Console.Write(" | Gain: " + normGain);
            if (!e.Cancelled)
            {
                try
                {
                    if (Math.Abs(1 - (decimal)normGain) > 0.03m)
                    {
                        DSPGain = new DSP_Gain();
                        DSPGain.ChannelHandle = stream;
                        DSPGain.Gain = normGain;
                        DSPGain.Start();
                        Console.WriteLine(" | Applied gain of " + (normGain));
                    }
                    else
                    {
                        normGain = 0;
                        Console.WriteLine(" | Gain not applied");
                    }
                }
                catch (Exception) { }
            }
            else
            {
                Console.WriteLine("Normalizing of " + toNormalize.title + " cancelled.");
            }
            volumeBar.Refresh();
        }

        private void echoesLogo_MouseUp(object sender, MouseEventArgs e)
        {
            ShowStats();
        }

        private void echoesLogo_MouseLeave(object sender, EventArgs e)
        {
            echoesLogo.BackgroundImage = LoadImage("echoesLogoWhiteDim");
        }

        private void echoesLogo_MouseEnter(object sender, EventArgs e)
        {
            echoesLogo.BackgroundImage = LoadImage("echoesLogoWhite");
        }

        private void Echoes_Enter(object sender, EventArgs e)
        {
            kh.SetForegroundWindow();
        }

        private void loadTrackWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine(trackToLoad.filename);
            LoadAudioFile(trackToLoad);
        }

        private void loadTrackWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //after-load sequence
            if (showWaveform || normalize) CreateWaveform();

            DefineEQ();
            ApplyEQ();
            if (Bass.BASS_ErrorGetCode() == 0 && GetLength() > 0)
            {
                Bass.BASS_ChannelSetPosition(stream, 0d);
                SetVolume(volume);
                if (!saveTranspose) transposeChangerNum.Value = 0m;
                SetFrequency();
                streamLoaded = true;
                SetLooping();
                endSync = new SYNCPROC(playbackEnded);
                stallSync = new SYNCPROC(playbackStalled);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, endSync, IntPtr.Zero);
                DisplayTrackInfo(nowPlaying);
                ScrollGridTo(HighlightPlayingTrack());
                RefreshPlayIcon();
                if (trackChangePopup) ShowTrackChangedPopup();
            }
            else
            {
                AdvancePlayer();
            }
            //

            if (trackToReload != null)
            {
                trackToLoad = trackToReload;
                trackToReload = null;
                loadTrackWorker.RunWorkerAsync();
            }
            else
            {
                if (nowPlaying != null) Play();
            }
        }

        public void LoadAudio(Track t)
        {
            if (loadTrackWorker.IsBusy)
            {
                trackToReload = t;
            }
            else
            {
                trackText.Text = "Loading...";
                trackToReload = null;
                trackToLoad = t;

                PrepareToLoadFile(t);

                loadTrackWorker.RunWorkerAsync();
            }
        }
    }
}
