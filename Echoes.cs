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
        ADVANCEPLAYER, PREVIOUSPLAYER, PLAYPAUSE, VOLUMEUP, VOLUMEDOWN, TRANSPOSEUP, TRANSPOSEDOWN, DELETE, GLOBAL_VOLUMEUP, GLOBAL_VOLUMEDOWN
    }
    public enum ItemType
    {
        Playlist, Track, Cache
    }
    public enum Modifier
    {
        NONE, ALT, CTRL, SHIFT, WIN
    }
    #endregion

    public partial class Echoes : Form
    {
        //GlobalKeyboardHook ghk = new GlobalKeyboardHook();
        #region DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion
        public int[] modValues = { 0, 1, 2, 4, 8 };

        #region Settings values
        /*To add a new setting value:
         * 1. define here; 
         * 2. define a default in Program class; 
         * 3. set it to default in SetDefaults() and equivalent;
         * 4. load it in LoadXmlConfig();
         * 5. save it in SaveXmlConfig();
         * 6. add a control of it to Options form;
        */

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
        public float volume;
        public float hotkeyVolumeIncrement;
        public float hotkeyTransposeIncrement;
        public bool saveTranspose;
        public bool autoShuffle;
        public bool autoAdvance;
        public bool trackChangePopup;
        public bool showWaveform;
        public bool stereo;
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

        public XmlCacher xmlCacher;

        public int stream;
        public bool confirmSaveFlag = false;
        public bool streamLoaded = false;
        public SYNCPROC endSync, stallSync;
        public BASS_MIDI_FONT[] midiFonts;
        public Visuals vs = new Un4seen.Bass.Misc.Visuals();

        public Track nowPlaying = null;
        public List<Track> playlist = new List<Track>();
        public List<string> supportedModuleTypes = new List<string>() { ".mo3", ".it", ".xm", ".s3m", ".mtm", ".mod", ".umx" };
        public List<string> supportedAudioTypes = new List<string>() { ".mp3", ".mp2", ".mp1", ".wav", ".ogg", ".wma", ".m4a", ".flac" };
        public List<string> supportedWaveformTypes = new List<String>() { ".mp3", ".mp2", ".mp1", ".wav" };
        public List<string> supportedMidiTypes = new List<string>() { ".midi",".mid" };

        TagEditor te;
        DupeFinder df;
        Statistics stats;
        Options opts;
        FloatingOSDWindow floatingWindow = new FloatingOSDWindow();
        DataGridViewRow highlightedRow;
        DataGridViewColumn highlightedColumn;
        Stopwatch timeListenedTracker = new Stopwatch();
        WaveForm wf;
        public Bitmap waveformImage;
        ItemType displayedItems;
        
        List<string> knownPlaylists = new List<string>();
        List<string> playlistNames = new List<string>();
        List<int> savedSelectionForSort;

        string currentPlaylist=null;
        string gridSearchWord = "";
        string tempPlaylistFile;
        
        bool draggingSeek = false;
        bool draggingVolume = false;
        bool rebuildCache = false;
        bool wasPlaying = false;
        bool advanceFlag = false;
        bool gridSearchFirstStroke = true;
        bool sortingAllowed = true;
        bool showingPlayIcon = true;

        public IntPtr theHandle;

        int gridSearchTimerRemaining = 0;
        int gridSearchReset = 30;
        public int waveformCompleted=0;
        public short[] waveform;

        public DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
        public DataGridViewCellStyle alternatingCellStyle = new DataGridViewCellStyle();
        public DataGridViewCellStyle highlightedCellStyle = new DataGridViewCellStyle();

        //                                        0      1         2        3          4         5            6        7        8         9         10
        readonly string[] COLUMN_PROPERTIES = { "num", "title", "artist", "length", "album", "listened", "filename", "year", "genre", "comment", "bitrate" };
        readonly string[] COLUMN_HEADERS={"#","Title","Artist","Length","Album","Listened","File","Year","Genre","Comment","Bitrate"};

        #endregion

        public Echoes()
        {

            vs.MaxFrequencySpectrum = Utils.FFTFrequency2Index(19200, 2047, 44100);
            xmlCacher = new XmlCacher(tagsCacheLocation);
            /*List<Track> trx = xmlCacher.GetAllTracks();
            xmlCacher.AddOrUpdate(trx);*/

            InitializeComponent();

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
            //LoadConfig();
            //SetDefaults();
            RepositionControls();

            SetColors();
            LoadPlaylistDb();
            StartupLoadProcedure();
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
                if(supportedAudioTypes.Contains(Path.GetExtension(dlg.FileName).ToLower())) {
                    xmlCacher.ChangeFilename(t.filename, dlg.FileName);
                    t.filename = dlg.FileName;
                    //playlist[playlist.IndexOf(t)] = GetTrackInfo(t.filename);
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
            if(inclSubdirs) opt=SearchOption.AllDirectories;
            else opt=SearchOption.TopDirectoryOnly;
            var files=Directory.EnumerateFiles(path, "*", opt);
            int foundFiles = 0;
            foreach (Track t in missingTracks)
            {
                string match = files.FirstOrDefault(x => Path.GetFileName(x) == Path.GetFileName(t.filename));
                if (match != null)
                {
                    xmlCacher.ChangeFilename(t.filename, match);
                    t.filename = match;
                    t.GetTags();//playlist[playlist.IndexOf(t)] = GetTrackInfo(t.filename);
                    foundFiles++;
                }
            }
            MessageBox.Show(foundFiles+"/"+missingTracks.Count+" missing tracks found."+Environment.NewLine+"Save the playlist.");
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


        public void SetDefaults()
        {
            //colors
            SetDefaultColors();

            //general
            font1 = Program.font1Default.Copy();
            font2 = Program.font2Default.Copy();
            hotkeyVolumeIncrement = Program.hotkeyVolumeIncrementDefault;
            hotkeyTransposeIncrement = Program.hotkeyTransposeIncrementDefault;
            SetVolume(Program.volumeDefault);
            autoAdvance = Program.autoAdvanceDefault;
            autoShuffle = Program.autoShuffleDefault;
            trackChangePopup = Program.trackChangePopupDefault;
            currentColumnInfo = Program.defaultColumnInfo.Copy();
            saveTranspose = Program.saveTransposeDefault;
            repeat = Program.repeatDefault;
            stereo = Program.stereoDefault;
            visualisationStyle = Program.visualisationStyleDefault;
            visualfps = Program.visualfpsDefault;
            midiSfLocation = Program.midiSfLocationDefault;

            //hotkey settings
            hotkeysAllowed = Program.hotkeysAllowedDefault;
            hotkeys = Program.defaultHotkeys.Copy();
            SetHotkeys();
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
                        LoadAudioFile(t);
                        Play();
                    }
                }
            }
            else if (k == Hotkey.GLOBAL_VOLUMEUP) AdjustGlobalVolume(0.01f);
            else if (k == Hotkey.GLOBAL_VOLUMEDOWN) AdjustGlobalVolume(-0.01f);
        } 

        void ghk_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("Clicked" + e.KeyCode.ToString());
            foreach (HotkeyData hk in hotkeys)
            {
                if (hk.key == e.KeyCode)
                {
                    if (hk.mod.IsModRequired(Modifier.ALT) == e.Alt &&
                        hk.mod.IsModRequired(Modifier.SHIFT) == e.Shift &&
                        hk.mod.IsModRequired(Modifier.CTRL) == e.Control)
                    {
                        DoHotkeyEvent(hk.hotkey);
                    }
                }
            }
        }

        public void SetHotkeys()
        {
            //ghk.HookedKeys.Clear();
            foreach(HotkeyData hk in hotkeys) {
                //ghk.HookedKeys.Add(hk.key);
                if (hk.enabled && hotkeysAllowed && !hk.registered)
                {
                    RegisterHotKey(theHandle, (int)hk.hotkey, hk.mod, (int)hk.key);
                    hk.registered = true;
                }
                else if (hk.registered)
                {
                    UnregisterHotKey(theHandle, (int)hk.hotkey);
                    hk.registered = false;
                }
            }
            //ghk.KeyDown += new KeyEventHandler(ghk_KeyDown);
        }

        void RepositionControls()
        {
            trackText.Location = new Point(trackText.Location.X, playBtn.Location.Y + playBtn.Height + 5);
            seekBar.Location = new Point(seekBar.Location.X, trackText.Location.Y + trackText.Height + 5);
            searchBox.Location = new Point(searchBox.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            playlistInfoTxt.Location = new Point(playlistInfoTxt.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            playlistSelectorCombo.Location = new Point(playlistSelectorCombo.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            volumeBar.Location = new Point(volumeBar.Location.X, seekBar.Location.Y + seekBar.Height + 5);
            volumeBar.Height = playlistSelectorCombo.Height;
            trackGrid.Location = new Point(trackGrid.Location.X, searchBox.Location.Y + searchBox.Height + 5);
            Form1_Resize(null, null);
            Refresh();
        }

        public void SetFonts()
        {
            trackText.Font = new Font(font1.FontFamily, 12*(float)fontSizePercentage / 100, font1.Style);
            searchBox.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            playlistInfoTxt.Font=new Font(font1.FontFamily,8.25f*(float)fontSizePercentage / 100, font1.Style);
            playlistSelectorCombo.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            trackGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            trackGrid.Font = new Font(font1.FontFamily, 8.25f * (float)fontSizePercentage / 100, font1.Style);
            //trackGrid.RowTemplate.Height = (int)(trackGrid.Font.Height*1.5f);
            trackGrid.Refresh();
            trackGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
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
                if (ctrl is Label || ctrl is PictureBox || ctrl is TextBox && ctrl.Name!="searchBox")
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

        void WaveFormCallback(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished) {
            if (finished)
            {
                // if your playback stream uses a different resolution than the WF
                // use this to sync them
                wf.SyncPlayback(stream);
                waveformImage = wf.CreateBitmap(this.seekBar.Width, this.seekBar.Height, -1, -1, true);
                waveformImage=waveformImage.SetOpacity(0.5d);
            }
        }
        public void DrawWaveform()
        {
            if (nowPlaying == null || !supportedWaveformTypes.Contains(Path.GetExtension(nowPlaying.filename))) { waveformImage = null; return; }
            wf = new Un4seen.Bass.Misc.WaveForm(nowPlaying.filename, new WAVEFORMPROC(WaveFormCallback), this);
            //wf.CallbackFrequency = 1; // every 10 seconds rendered
            wf.ColorBackground = Color.Transparent;
            wf.DrawWaveForm = WaveForm.WAVEFORMDRAWTYPE.Mono;
            wf.ColorLeft = wf.ColorLeft2 = wf.ColorLeftEnvelope = Color.Black;
            /*wf.ColorLeft = seekBarForeColor.Darken(96);
            wf.ColorLeft2 = wf.ColorLeft;
            wf.ColorLeftEnvelope = wf.ColorLeft;*/

            // it is important to use the same resolution flags as for the playing stream
            // e.g. if an already playing stream is 32-bit float, so this should also be
            // or use 'SyncPlayback' as shown below
            wf.RenderStart(true, BASSFlag.BASS_DEFAULT);
            /*int channel = Bass.BASS_StreamCreateFile(nowPlaying.filename, 0, 0, BASSFlag.BASS_DEFAULT);
            Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, 0);
            int width = Screen.FromControl(this).Bounds.Width;
            Bitmap ret = new Bitmap(width, height);
            if (nowPlaying == null) return ret;
            Graphics g = Graphics.FromImage(ret);
            Visuals visuals = new Visuals();
            Bass.BASS_ChannelPlay(channel, false);
            for (int i = 0; i < width; i++)
            {
                Bass.BASS_ChannelSetPosition(channel, 1d * i / width);
                visuals.CreateSpectrum3DVoicePrint(channel, g, new Rectangle(0, 0, ret.Width, ret.Height), Color.Black, Color.White, i, true, false);
            }
            Bass.BASS_ChannelStop(channel);
            return ret;*/
        }

        public bool IsPlaying()
        {
            return Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312) {
                Hotkey k=(Hotkey)m.WParam.ToInt32();
                DoHotkeyEvent(k);
            }
            base.WndProc(ref m);
        }

        int PitchFreq(int cents, int freq)
        {
            return (int)(Math.Pow(2, (double)cents / 1200d) * freq);
        }

        public void SetFrequency()
        {
            if(streamLoaded) Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, (float)(PitchFreq((int)(transposeChangerNum.Value * 100), Bass.BASS_ChannelGetInfo(stream).freq)));
            seekBar.Refresh();
        }

        public void SetChannels(bool stereo)
        {
            if (!streamLoaded) return;
            if(stereo) Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_MONO);
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
            track.listened=time;
            xmlCacher.AddOrUpdate(new List<Track> { track });
        }

        void CreateTempPlaylist()
        {
            string tryName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Temporary Playlist.m3u");
            int increment=0;
            while (File.Exists(tryName))
            {
                increment++;
                tryName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Temporary Playlist"+increment+".m3u");
            }
            tempPlaylistFile = tryName;
            AddKnownPlaylist(tryName);
            //PopulatePlaylistList();
        }

        void LoadFiles(List<string> files)
        {
            string lastList = null;
            bool tempPlaylistCreated = false;
            foreach (string file in files)
            {
                if (supportedAudioTypes.Contains(Path.GetExtension(file).ToLower()))
                {
                    if ((string)playlistSelectorCombo.SelectedValue == null)
                    {
                        //if no current playlist, create a temp one
                        if (tempPlaylistFile == null)
                        {
                            CreateTempPlaylist();
                            playlist = new List<Track>();
                        }
                        Track t = new Track(file, Path.GetFileName(file));
                        t.length = 0;
                        t.artist = "";
                        playlist.Add(t);
                        tempPlaylistCreated = true;
                    }
                    else
                    {
                        /*Track t = GetTrackInfo(file);
                        if (!playlist.Contains(t))
                        {
                            playlist.Insert(0, t);
                            RenumberPlaylist(playlist);
                        }*/
                        Track t = new Track(file, "");
                        t.GetTags();
                        playlist.Insert(0,t);

                        //add to current playlist
                    }
                    displayedItems = ItemType.Track;
                }
                else if(Path.GetExtension(file).ToLower()==".m3u")
                {
                    AddKnownPlaylist(file);
                    lastList = file;
                }
            }
            if (tempPlaylistCreated)
            {
                ExportM3u(tempPlaylistFile, playlist);
                //rebuildCache = true;
                ImportM3u(tempPlaylistFile);
            }
            else
            {
                if (displayedItems == ItemType.Track) RefreshPlaylistGrid();
                else DisplayPlaylistsInGrid();
            }
            RefreshTotalTimeLabel();
        }

        void StartupLoadProcedure()
        {
            string lastList = null;
            foreach (string file in Program.filesToOpen)
            {
                if (supportedAudioTypes.Contains(Path.GetExtension(file).ToLower()))
                {
                    if (tempPlaylistFile == null)
                    {
                        CreateTempPlaylist();
                        playlist = new List<Track>();
                    }
                    Track t=new Track(file, Path.GetFileName(file));
                    t.length=0;
                    t.artist="";
                    playlist.Add(t);
                }
                else if (Path.GetExtension(file).ToLower() == ".m3u")
                {
                    AddKnownPlaylist(file);
                    lastList = file;
                }
            }
            if(tempPlaylistFile !=null) ExportM3u(tempPlaylistFile, playlist);
            PopulatePlaylistList();
            if (tempPlaylistFile != null) { ImportM3u(tempPlaylistFile); Play(); }
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
                LoadAudioFile(t);
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
                });
            }
            foreach (DataGridViewTextBoxColumn clmn in columns)
            {
                trackGrid.Columns.Add(clmn);
            }
            SortableBindingList<Track> source;
            if (String.IsNullOrEmpty(searchBox.Text)) source = new SortableBindingList<Track>(playlist);
            else source = new SortableBindingList<Track>(playlist.Where(x => x.title.ContainsIgnoreCase(searchBox.Text) || x.album.ContainsIgnoreCase(searchBox.Text) || x.artist.ContainsIgnoreCase(searchBox.Text)).ToList());
            trackGrid.DataSource = source;
            trackGrid.ClearSelection();
            HighlightPlayingTrack();
            if(displayedItems!=ItemType.Cache) displayedItems = ItemType.Track;
        }

        void DisplayPlaylistsInGrid()
        {
            if (knownPlaylists.Count > 0) {
                trackGrid.Columns.Clear();
                trackGrid.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = (int)Math.Floor(trackGrid.Width * 0.05),
                    DataPropertyName = "num",
                    HeaderText = "#"
                });
                trackGrid.Columns.Add(new DataGridViewTextBoxColumn() {
                    Width=(int)Math.Floor(trackGrid.Width*0.20),
                    DataPropertyName="title",
                    HeaderText="Playlist"
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
                    if (t != "<No playlist>" && t!="<Cache>")
                    {
                        Track tr = new Track(knownPlaylists[i - 2], t);
                        tr.num = i;
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
                if (tt==t) return r;
            }
            return null;
        }

        DataGridViewRow FindTrackRow(int num)
        {
            if (playlist.Count<num) return null;
            foreach (DataGridViewRow r in trackGrid.Rows)
            {
                Track tt = (Track)r.DataBoundItem;
                if (tt.num == num) return r;
            }
            return null;
        }

        void MoveTrackInPlaylist(int oldIndex, int newIndex)
        {
            if (oldIndex > playlist.Count || newIndex > playlist.Count || oldIndex<0 || newIndex<0) return;
            Track item = playlist[oldIndex];
            playlist.RemoveAt(oldIndex);
            bool item1Selected = trackGrid.Rows[oldIndex].Selected;
            bool item2Selected = trackGrid.Rows[newIndex].Selected;
            playlist.Insert(newIndex, item);
            RenumberPlaylist(playlist);
            RefreshPlaylistGrid();
            trackGrid.Rows[oldIndex].Selected = item2Selected;
            trackGrid.Rows[newIndex].Selected = item1Selected;
        }

        Track GetTrackByNum(int num)
        {
            return playlist.First(x => x.num == num);
        }

        public void DeleteTrack(List<int> nums)
        {
            if (!sortingAllowed) return;
            int lastInt=0;
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
            catch (Exception){ }
            xml = new XDocument();
            xml.Add(new XElement("config"));
            //general
            XElement general = new XElement("general");
            XElement item = new XElement("volume");
            item.Value = ((int)(volume*100)).ToString();
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
            item = new XElement("stereo");
            item.Value = stereo.ToString();
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
            item.Value = fontSizePercentage+"";
            general.Add(item);
            item = new XElement("font2");
            item.Value = font2.FontFamily.Name;
            item.Add(new XAttribute("Style", (int)font2.Style));
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
                item.Add(new XAttribute("mod", hk.mod.ToString()));
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
            ele = group.Element("stereo");
            if (ele != null && Boolean.TryParse(ele.Value, out stereo)) { }
            else stereo = Program.stereoDefault;
            SetChannels(stereo);
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
            //colors
            group = xml.Root.Element("colors");
            ele=group.Element("background");
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
                Hotkey hk; Keys k; int mod; bool enabled;
                if (elem.Name != null && elem.Attribute("key") != null && elem.Attribute("mod") != null && elem.Attribute("enabled") != null)
                if(Enum.TryParse<Hotkey>(elem.Name.ToString(),out hk) &&
                   Enum.TryParse<Keys>(elem.Attribute("key").Value, out k) &&
                   Int32.TryParse(elem.Attribute("mod").Value, out mod) &&
                   Boolean.TryParse(elem.Attribute("enabled").Value, out enabled)) {
                       hotkeys.Add(new HotkeyData(hk, k, mod, enabled));
                }
            }
            SetHotkeys();
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
                        currentColumnInfo.Add(new ColumnInfo(COLUMN_PROPERTIES.ToList().IndexOf(elem.Name.ToString()),width));
                    }
                }
            }
        }

        public void AdjustGlobalVolume(float increment)
        {
            float getVol=Bass.BASS_GetVolume();
            if(getVol+increment>1) getVol=1;
            else if(getVol+increment<0) getVol=0;
            else getVol=getVol+increment;
            Bass.BASS_SetVolume(getVol);
        }

        public void SetVolume(float vol)
        {
            if (vol > 1) vol = 1;
            else if (vol < 0) vol = 0;
            /*ROLLBACKPOINT
             * soundOutput.Volume = vol;*/
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
            playlist=playlist.OrderBy(x => rng.Next()).ToList();
            //dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            //playlist = playlist.OrderBy(x => rng.Next()).ToList();
            RefreshPlaylistGrid();
            HighlightPlayingTrack();
        }

        private void RenumberPlaylist(List<Track> p)
        {
            for (int i = 0; i < p.Count; i++)
            {
                p[i].num = i+1;
            }
        }

        private void CopyTrackToPlaylist(List<Track> t, string playlistFile)
        {
            List<Track> thePlaylist = new List<Track>();
            t.Reverse();
            for (int i = 0; i < t.Count; i++)
            {
                Track newTrack = new Track(t[i].filename, t[i].title);
                newTrack.length = t[i].length;
                newTrack.artist = t[i].artist;
                thePlaylist.Add(newTrack);
            }
            thePlaylist.AddRange(ReadM3u(playlistFile));
            RenumberPlaylist(thePlaylist);
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

        class PlaylistComboItem {
            public string filepath {get;set;}
            public string shownName {get;set;}
            public override string ToString()
            {
                return shownName;
            }
            public PlaylistComboItem(string filepath, string shownName) {
                this.filepath=filepath;
                this.shownName=shownName;
            }
        }

        private void PopulatePlaylistList()
        {
            BindingList<PlaylistComboItem> playlistItems=new BindingList<PlaylistComboItem>();
            playlistItems.Add(new PlaylistComboItem(null,"<No playlist>"));
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

        private void ExportM3u(string filename, List<Track> plist)
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
            if (SuperM3U.Reader.FileIsValid(filename))
            {
                List<M3UTrack> tracks = SuperM3U.Reader.GetTracks(filename);
                for (int i=0;i<tracks.Count;i++)
                {
                    Track t = new Track(tracks[i].filename,tracks[i].title);
                    t.num = i+1;
                    t.length = tracks[i].length;
                    ret.Add(t);
                }
            }
            return ret;
        }

        void RebuildCache()
        {
            if (backgroundWorker1.IsBusy) return;
            AllowSorting(false);
            //XmlCacher.CleanupCache();
            rebuildCache = true;
            trackText.Text = "Loading...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void ImportM3u(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("File doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                //ImportM3u(filename);
                return;
            }
            searchBox.Text = "";
            System.Diagnostics.Process.GetCurrentProcess().StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
            if (SuperM3U.Reader.FileIsValid(filename))
            {
                playlist = new List<Track>();
                currentPlaylist = filename;
                AddKnownPlaylist(filename);
                var plist = SuperM3U.Reader.GetTracks(filename);
                for (int i = 0; i < plist.Count; i++)
                {
                    Track t = new Track(plist[i].filename, plist[i].title);
                    t.length = plist[i].length;
                    t.num = i+1;
                    playlist.Add(t);
                }
                foreach(object o in playlistSelectorCombo.Items) {
                    PlaylistComboItem pci = (PlaylistComboItem) o;
                    if(pci.filepath==currentPlaylist) {
                        playlistSelectorCombo.SelectedItem=o;
                        break;
                    }
                }
                RefreshGrid();
                if (autoShuffle) Shuffle();
                RefreshPlaylistGrid();
                AllowSorting(false);
                if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
                else
                {
                    trackText.Text = "Loading...";
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show(filename + " is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        
        /*private Track GetTrackInfo(string filename) {
            string title=null;
            string artist=null;
            string album=null;
            //string year=null;
            //string comment=null;
            string length = null;
            string bitrate = null;
            try
            {
                if (supportedModuleTypes.Contains(Path.GetExtension(filename).ToLower()))
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    int track = Bass.BASS_MusicLoad(filename, 0, 0, BASSFlag.BASS_MUSIC_NOSAMPLE | BASSFlag.BASS_MUSIC_PRESCAN, 0);
                    title = Bass.BASS_ChannelGetMusicName(track);
                    artist = Bass.BASS_ChannelGetMusicMessage(track);
                    length = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(track, (int)Bass.BASS_ChannelGetLength(track))).ToString("mm\\:ss");
                    Bass.BASS_Free();
                }
                else if (supportedMidiTypes.Contains(Path.GetExtension(filename).ToLower()))
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    int track = BassMidi.BASS_MIDI_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT, 0);
                    title = Path.GetFileName(filename);
                    length = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(track, (int)Bass.BASS_ChannelGetLength(track))).ToString("mm\\:ss");
                    Bass.BASS_Free();
                }
                else
                {
                    TagLib.File tagFile = TagLib.File.Create(filename);
                    title = tagFile.Tag.Title;
                    if (tagFile.Tag.Performers.Length > 0)
                    {
                        artist = tagFile.Tag.Performers[0];
                    }
                    album = tagFile.Tag.Album;
                    //year = tagFile.Tag.Year+"";
                    //comment = tagFile.Tag.Comment;
                    length = tagFile.Properties.Duration.ToString("mm\\:ss");
                    bitrate = tagFile.Properties.AudioBitrate+" kbps";
                    tagFile.Dispose();
                }
            }
            catch (Exception asdf) {
                //MessageBox.Show(asdf.ToString());
                title = filename;
                length = "00:00";
            }
            if (String.IsNullOrWhiteSpace(title))
            {
                title = Path.GetFileName(filename);
            }
            Track t = new Track(filename, title);
            t.artist = artist;
            t.album = album;
            t.length = length;
            t.bitrate = bitrate;
            t.listened = XmlCacher.GetListened(filename); //TimeSpan.FromSeconds(GetListenedTime(filename)).ToString("hh\\:mm\\:ss");
            return t;
        }*/

        private void AdvancePlayer()
        {
            try
            {
                StopPlayer();
                DataGridViewRow currentRow = FindTrackRow(nowPlaying);
                if (currentRow.Index >= trackGrid.Rows.Count - 1 && repeat==0) return;
                else
                {
                    int indexToPlay = currentRow.Index + 1;
                    if (currentRow.Index >= trackGrid.Rows.Count - 1) indexToPlay = 0;
                    Track t = (Track)trackGrid.Rows[indexToPlay].DataBoundItem;
                    FlushTimeListened();
                    nowPlaying = t;
                    if (File.Exists(t.filename))
                    {
                        LoadAudioFile(t);
                        Play();
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
                if (currentRow.Index<=0) return;
                else
                {
                    Track t = (Track)trackGrid.Rows[currentRow.Index - 1].DataBoundItem;
                    LoadAudioFile(t);
                    Play();
                }
            }
            catch (Exception) { }
        }

        void FlushTimeListened()
        {
            if (nowPlaying == null || timeListenedTracker.Elapsed.TotalSeconds <= 1) return;
            int beforeFlush=nowPlaying.listened;
            SetListenedTime(nowPlaying, (int)timeListenedTracker.Elapsed.TotalSeconds + nowPlaying.listened);
            RefreshTotalTimeLabel();
            if (timeListenedTracker.IsRunning) timeListenedTracker.Restart();
            else timeListenedTracker.Reset();
        }

        void playbackEnded(int handle, int channel, int data, IntPtr user)
        {
            timeListenedTracker.Stop();
            if (streamLoaded)
            {
                if(repeat<2) advanceFlag = true;
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

        public void LoadAudioFile(Track t)
        {
            if (streamLoaded) Bass.BASS_ChannelStop(stream);
            FlushTimeListened();
            Bass.BASS_Free();
            if (!File.Exists(t.filename))
            {
                PointToMissingFile(t);
                return;
            }
            int numzor = t.num;
            int index = playlist.IndexOf(playlist.First(x => x.num == t.num));
            t.GetTags();
            playlist[index].num = numzor;
            nowPlaying = playlist[index];
            if (!backgroundWorker1.IsBusy) try { xmlCacher.AddOrUpdate(new List<Track>() { playlist[index] }); }
                catch (Exception) { }
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, this.Handle);
            string ext=Path.GetExtension(t.filename.ToLower());
            if (supportedModuleTypes.Contains(ext))
            {
                stream = Bass.BASS_MusicLoad(t.filename, 0, 0, 
                    BASSFlag.BASS_MUSIC_SINCINTER | 
                    BASSFlag.BASS_MUSIC_PRESCAN | 
                    BASSFlag.BASS_SAMPLE_SOFTWARE | 
                    BASSFlag.BASS_MIXER_DOWNMIX | 
                    BASSFlag.BASS_MUSIC_RAMPS, 0);
            }
            else if (supportedMidiTypes.Contains(ext))
            {
                if (!InitMidi()) { nowPlaying = null; return; }
                BassMidi.BASS_MIDI_StreamSetFonts(0, midiFonts, midiFonts.Length);
                stream = BassMidi.BASS_MIDI_StreamCreateFile(t.filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_SOFTWARE, 0);
            }else if (ext==".flac") {
                stream = BassFlac.BASS_FLAC_StreamCreateFile(t.filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_SOFTWARE);
            }
            else if (ext == ".wma")
            {
                stream = BassWma.BASS_WMA_StreamCreateFile(t.filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_SOFTWARE);
            }
            else if (supportedAudioTypes.Contains(ext))
            {
                stream = Bass.BASS_StreamCreateFile(t.filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_SOFTWARE);
            }
            else
            {
                streamLoaded = false;
                return;
            }
            /*float peak=0;
            float gainFactor=Utils.GetNormalizationGain(t.filename,0.2f,-1d,-1d,ref peak);*/
            if (showWaveform)
            {
                DrawWaveform();
            }
            if (Bass.BASS_ErrorGetCode() == 0 && GetLength()>0)
            {
                SetVolume(volume);
                if (!saveTranspose) transposeChangerNum.Value = 0m;
                SetFrequency();
                streamLoaded = true;
                SetLooping();
                SetChannels(stereo);
                endSync = new SYNCPROC(playbackEnded);
                stallSync = new SYNCPROC(playbackStalled);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, endSync, IntPtr.Zero);
                DisplayTrackInfo(nowPlaying);
                ScrollGridTo(HighlightPlayingTrack());
                RefreshPlayIcon();
                if (trackChangePopup) ShowTrackChangedFloat();
            }
            else
            {
                AdvancePlayer();
            }
        }

        void SetLooping()
        {
            if (!streamLoaded) return;
            if (repeat>1)
            {
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_MUSIC_LOOP, BASSFlag.BASS_MUSIC_LOOP);
            }
            else
            {
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_MUSIC_LOOP);
            }
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
            midiFonts[0]=new BASS_MIDI_FONT(font, -1, 0);
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
            if(!theRow.Displayed) trackGrid.FirstDisplayedScrollingRowIndex = theRow.Index;
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
                else highlightedRow.DefaultCellStyle=alternatingCellStyle;
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
                ExportM3u(dlg.FileName.ToString(), new List<Track>());
                return dlg.FileName.ToString();
            }
            dlg.Dispose();
            return null;
        }

        private void ExportPlaylist()
        {
            if ((string)playlistSelectorCombo.SelectedValue == null) return;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Playlist";
            dlg.Filter = "M3U playlists (*.m3u)|*.m3u";
            dlg.FileName = Path.GetFileName(currentPlaylist);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ExportM3u(dlg.FileName.ToString(), playlist);
                DeleteTempFile();
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
            dlg.Title="Open File";
            dlg.Filter = "Supported files|";
            foreach (string s in supportedAudioTypes)
            {
                dlg.Filter += "*" + s + ";";
            }
            dlg.Filter += "*.m3u;";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                LoadFiles(new List<string>{dlg.FileName.ToString()});
            }
            dlg.Dispose();
        }

        private void RefreshPlayIcon()
        {
            if (Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                playBtn.Image = global::Echoes.Properties.Resources.play1;
                showingPlayIcon = true;
            }
            else
            {
                playBtn.Image = global::Echoes.Properties.Resources.pause1;
                showingPlayIcon = false;
            }
        }

        void PlayFirst()
        {
            if (trackGrid.SelectedRows.Count > 0)
            {
                Track t = (Track)trackGrid.SelectedRows[0].DataBoundItem;
                OpenFile(t);
            }
            else if (trackGrid.Rows[0] != null)
            {
                Track t = (Track)trackGrid.Rows[0].DataBoundItem;
                OpenFile(t);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { SaveXmlConfig(); }
            catch (Exception zx)
            {
                MessageBox.Show(zx.ToString());
            }
            FlushTimeListened();
            Bass.BASS_Free();
            DeleteTempFile();
        }

        

        void DeleteTempFile()
        {
            if (knownPlaylists.Contains(tempPlaylistFile)) RemoveKnownPlaylist(knownPlaylists.IndexOf(tempPlaylistFile));
            if (File.Exists(tempPlaylistFile)) File.Delete(tempPlaylistFile);
            tempPlaylistFile = null;
        }

        private void button1_Click(object sender, MouseEventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = global::Echoes.Properties.Resources.play3;
            else playBtn.Image = global::Echoes.Properties.Resources.pause3;
        }

        private void button2_Click(object sender, MouseEventArgs e)
        {
            stopBtn.Image = global::Echoes.Properties.Resources.stop3;
        }

        private void button3_Click(object sender, MouseEventArgs e)
        {
            PausePlayer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChooseFile();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (streamLoaded)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING | Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
                {
                    switch (visualisationStyle) {
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

        void ShowTrackChangedFloat()
        {
            if (nowPlaying == null) return;
            string notifyText = "";
            Track t;
            DataGridViewRow currRow = FindTrackRow(nowPlaying);
            notifyText += ">";

            notifyText += nowPlaying.title+" - ";
            if (!String.IsNullOrWhiteSpace(nowPlaying.artist)) notifyText += nowPlaying.artist+Environment.NewLine;

            if (currRow.Index < trackGrid.Rows.Count - 1)
            {
                notifyText += " ";
                t = (Track)trackGrid.Rows[FindTrackRow(nowPlaying).Index + 1].DataBoundItem;
                notifyText += t.title + " - ";
                if (!String.IsNullOrWhiteSpace(t.artist)) notifyText += t.artist + Environment.NewLine;
                
            }
            Font ft=new Font(font2.FontFamily,20,FontStyle.Bold);
            Size sz=TextRenderer.MeasureText(notifyText,ft);
            Rectangle screenSize=Screen.PrimaryScreen.WorkingArea;
            Point pt=new Point(screenSize.Width-sz.Width-10,screenSize.Height-sz.Height-10);
            floatingWindow.Show(pt, 255, trackTitleColor, ft, 5000, FloatingWindow.AnimateMode.Blend, 3500, notifyText);
        }

        private void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (streamLoaded)
            {
                if (Bass.BASS_ChannelIsActive(stream)==BASSActive.BASS_ACTIVE_PLAYING)
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
            seekBar.Value = (int)((float)x / (float)seekBar.Width*seekBar.Maximum);
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
            volumeBar.Location = new Point(this.Width - 180,volumeBar.Location.Y);
            playlistSelectorCombo.Location = new Point(this.Width - 350, playlistSelectorCombo.Location.Y);
            playlistInfoTxt.Location = new Point(playlistSelectorCombo.Location.X - playlistInfoTxt.Width - 5, playlistInfoTxt.Location.Y);
            trackGrid.Width = this.Width - 40;
            int spaceForOther=trackGrid.ColumnHeadersHeight+2;
            if (trackGrid.Controls.OfType<HScrollBar>().First().Visible) spaceForOther += SystemInformation.HorizontalScrollBarHeight;
            int spaceForRows = Height-210-spaceForOther;
            spaceForRows -= spaceForRows % trackGrid.RowTemplate.Height;
            trackGrid.Height = spaceForOther + spaceForRows;
            echoesLogo.Location = new Point(this.Width - 85, echoesLogo.Location.Y);
            visualsPicture.Width = echoesLogo.Location.X - visualsPicture.Location.X - 5;
            //last column auto resize thingy
            /*int estimatedWidth = 0;
            foreach (DataColumnInfo dci in currentColumnInfo)
            {
                estimatedWidth += dci.width;
            }
            if (dataGridView1.Width > estimatedWidth) dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.None).Width = dataGridView1.Width - dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Displayed);
            else dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.None).Width = currentColumnInfo.Last().width;*/
            Refresh();
        }

        private void progressBar2_MouseDown(object sender, MouseEventArgs e)
        {
            draggingVolume = true;
            UpdateVolumeToMouse(e.X);
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex<0 || e.RowIndex>=trackGrid.Rows.Count || trackGrid.Rows[e.RowIndex] == null || trackGrid.Rows[e.RowIndex].DataBoundItem==null) return;
            Track t = (Track)trackGrid.Rows[e.RowIndex].DataBoundItem;
            if (streamLoaded) SetPosition(0);
            StopPlayer();
            OpenFile(t);
            if (nowPlaying != null) Play();
        }

        private void button5_Click(object sender, MouseEventArgs e)
        {
            fwdBtn.Image = global::Echoes.Properties.Resources.forward3;
        }

        private void button7_Click(object sender, MouseEventArgs e)
        {
            rewBtn.Image = global::Echoes.Properties.Resources.rewind3;
        }

        private void button6_Click(object sender, MouseEventArgs e)
        {
            backBtn.Image = global::Echoes.Properties.Resources.back3;
        }

        void LoadCustomPlaylist(List<Track> lst)
        {
            playlist = lst;
            RenumberPlaylist(playlist);
            displayedItems = ItemType.Cache;
            RefreshPlaylistGrid();
            if (autoShuffle) Shuffle();
            RefreshTotalTimeLabel();
            currentPlaylist = "";
            playlistSelectorCombo.SelectedIndex = 0;
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
            
            StringCollection paths=new StringCollection();
            foreach(DataGridViewRow row in trackGrid.Rows) {
                if (!row.Selected) continue;
                Track t=(Track)row.DataBoundItem;
                paths.Add(t.filename);
            }
            MessageBox.Show(paths.Count+" files copied");
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
                    if (e.Control) CopySelectedToClipboard(); else e.SuppressKeyPress = false;
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
                        if (nowPlaying != null)
                        {
                            Play();
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (rebuildCache==false)
            {
                for(int z=0;z<playlist.Count;z++){
                    Track t = playlist[z];
                    if (!xmlCacher.GetCacheInfo(t))
                    {
                        t.GetTags();
                        xmlCacher.AddOrUpdate(new List<Track>(){t});
                    }
                    trackGrid.InvalidateRow(z);
                    backgroundWorker1.ReportProgress((int)(((float)z / (float)playlist.Count) * 100));
                }
                /*try
                {
                    uncachedTracks = xmlCacher.LoadTagsToPlaylist(playlist);
                }
                catch (Exception)
                {
                    uncachedTracks = playlist;
                }
                //MessageBox.Show(uncachedTracks.Count + " uncached tracks");
                for (int i = 0; i < uncachedTracks.Count; i++)
                {
                    Track t = uncachedTracks[i];
                    //int numzor = t.num;
                    int index = playlist.IndexOf(t);
                    t.GetTags();//playlist[index] = GetTrackInfo(t.filename);
                    //playlist[index].num = numzor;
                    dataGridView1.InvalidateRow(index);
                    backgroundWorker1.ReportProgress((int)(((float)i / (float)uncachedTracks.Count) * 100));
                    if (backgroundWorker1.CancellationPending) return;
                }
                uncachedTracks.Clear();*/
            }
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
            }
            //if(useCache) xmlCacher.AddOrUpdate(playlist);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            trackText.Text = e.ProgressPercentage + "% loaded";
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            rebuildCache = false;
            RefreshTotalTimeLabel();
            AllowSorting(true);
            if (nowPlaying == null) trackText.Text = "100% Loaded";
            else DisplayTrackInfo(nowPlaying);
            trackGrid.Refresh();
        }

        public TimeSpan TotalPlaylistListened(List<Track> list)
        {
            TimeSpan ret = new TimeSpan();
            int kount=0;
            foreach (Track t in list)
            {
                kount++;
                ret += TimeSpan.FromSeconds(t.listened);
            }
            return ret;
        }

        public TimeSpan TotalPlaylistTime(List<Track> list)
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

        void RefreshTotalTimeLabel()
        {
            if (displayedItems != ItemType.Playlist)
            {
                playlistInfoTxt.Text = playlist.Count + " tracks, " + TotalPlaylistTime(playlist).ProperTimeFormat();
                playlistInfoTxt.Text += " [" + TotalPlaylistListened(playlist).ProperTimeFormat()+" listened]";
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
                    if (highlightedColumn == null)
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
                    if(matchString.StartsWith(gridSearchWord, true, null))
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
                    if (highlightedColumn == null)
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
                    if (matchString !=null && matchString.StartsWith(gridSearchWord, true, null))
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

        private void OpenInExplorer(string filename)
        {
            ShowSelectedInExplorer.FilesOrFolders();
        }
        void OpenDupeFinder()
        {
                df = new DupeFinder(playlist);
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
                    cm.MenuItems.Add(miShowExplorer);
                    cm.MenuItems.Add(miDelete);
                    if (displayedItems != ItemType.Playlist)
                    {
                        MenuItem miAddToPlaylist = new MenuItem("Add to playlist");
                        for (int i = 0; i < knownPlaylists.Count; i++)
                        {
                            int z = i;
                            if (knownPlaylists[i] == knownPlaylists[playlistSelectorCombo.SelectedIndex - 1])
                                continue;
                            MenuItem itm = new MenuItem(knownPlaylists[z]);
                            itm.Click += (theSender, eventArgs) =>
                            {
                                //MessageBox.Show(selectedTracks[0].filename);
                                CopyTrackToPlaylist(selectedTracks, knownPlaylists[z]);
                            };
                            miAddToPlaylist.MenuItems.Add(itm);
                        }
                        MenuItem npItem = new MenuItem("New playlist");
                        npItem.Click += (theSender, eventArgs) =>
                        {
                            string newList=CreateNewPlaylist();
                            CopyTrackToPlaylist(selectedTracks, newList);
                        };
                        miAddToPlaylist.MenuItems.Add(npItem);

                        MenuItem miEditTags = new MenuItem("Edit tags");
                        miEditTags.Click += (theSender, eventArgs) =>
                        {
                            LaunchTagEditor();
                        };
                        MenuItem miRenumber = new MenuItem("Save this order");
                        miRenumber.Click += (theSender, eventArgs) =>
                        {
                            RenumberPlaylist(playlist);
                        };
                        MenuItem miDupes = new MenuItem("Find duplicates");
                        miDupes.Click += (theSender, eventArgs) =>
                        {
                            OpenDupeFinder();
                        };
                        MenuItem miUnexisting = new MenuItem("Remove missing file entries");
                        if (displayedItems == ItemType.Track)
                        {
                            miUnexisting.Click += (theSender, eventArgs) =>
                            {
                                RemoveUnexisting();
                            };
                        }
                        else if (displayedItems == ItemType.Cache)
                        {
                            miUnexisting.Click += (theSender, eventArgs) =>
                            {
                                xmlCacher.CleanupCache();
                                playlist = xmlCacher.GetAllTracks();
                                RenumberPlaylist(playlist);
                                RefreshPlaylistGrid();
                            };
                        }
                        MenuItem miMoveUp = new MenuItem("Move up");
                        miMoveUp.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedUp();
                        };
                        MenuItem miMoveDown = new MenuItem("Move down");
                        miMoveDown.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedDown();
                        };
                        MenuItem miMoveToTop = new MenuItem("Move to top");
                        miMoveToTop.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedToTop();
                        };
                        MenuItem miMoveToBottom = new MenuItem("Move to bottom");
                        miMoveToBottom.Click += (theSender, eventArgs) =>
                        {
                            MoveSelectedToBottom();
                        };
                        cm.MenuItems.Add(miAddToPlaylist);
                        cm.MenuItems.Add(miEditTags);
                        cm.MenuItems.Add(miRenumber);
                        cm.MenuItems.Add(miDupes);
                        cm.MenuItems.Add(miUnexisting);
                        cm.MenuItems.Add(miMoveUp);
                        cm.MenuItems.Add(miMoveDown);
                        cm.MenuItems.Add(miMoveToTop);
                        cm.MenuItems.Add(miMoveToBottom);
                    }
                    cm.Show(trackGrid, e.Location);
                }
                catch (Exception) { }
            }
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
                MoveTrackInPlaylist(FindTrackRow(tr).Index, playlist.Count-1);
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
            selectedShit=selectedShit.OrderBy(x => x).ToList();
            foreach (int rw in selectedShit)
            {
                MoveTrackInPlaylist(rw, rw-1);
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
                        ret.AddRange(Directory.EnumerateFiles(str, "*"+okType, SearchOption.AllDirectories));
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

        private void modifiedButton1_Click(object sender, EventArgs e)
        {
            if (currentPlaylist != tempPlaylistFile)
            {
                ExportM3u(currentPlaylist, playlist.OrderBy(x => x.num).ToList());
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                ExportPlaylist();
            }
        }

        void RemoveUnexisting()
        {
            List<int> nums = new List<int>();
            foreach (Track t in playlist)
            {
                if (!File.Exists(t.filename))
                {
                    nums.Add(t.num);
                }
            }
            playlist = playlist.Where(x => !nums.Contains(x.num)).ToList();
            MessageBox.Show(nums.Count + " entries were removed.");
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

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowStats();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = global::Echoes.Properties.Resources.play2;
            else playBtn.Image = global::Echoes.Properties.Resources.pause2;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if(showingPlayIcon) playBtn.Image = global::Echoes.Properties.Resources.play1;
            else playBtn.Image = global::Echoes.Properties.Resources.pause1;
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (showingPlayIcon) playBtn.Image = global::Echoes.Properties.Resources.play2;
            else playBtn.Image = global::Echoes.Properties.Resources.pause2;
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
            stopBtn.Image = global::Echoes.Properties.Resources.stop2;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            stopBtn.Image = global::Echoes.Properties.Resources.stop1;
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            stopBtn.Image = global::Echoes.Properties.Resources.stop2;
            StopPlayer();
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            backBtn.Image = global::Echoes.Properties.Resources.back2;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            backBtn.Image = global::Echoes.Properties.Resources.back1;
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            backBtn.Image = global::Echoes.Properties.Resources.back2;
            PlayerPrevious();
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            rewBtn.Image = global::Echoes.Properties.Resources.rewind2;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            rewBtn.Image = global::Echoes.Properties.Resources.rewind1;
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            rewBtn.Image = global::Echoes.Properties.Resources.rewind2;
            SetPosition(0);
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            fwdBtn.Image = global::Echoes.Properties.Resources.forward2;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            fwdBtn.Image = global::Echoes.Properties.Resources.forward1;
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            fwdBtn.Image = global::Echoes.Properties.Resources.forward2;
            AdvancePlayer();
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            visualisationStyle++;
            if (visualisationStyle > 4) visualisationStyle = 1;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                }else if((dgv.Columns[e.ColumnIndex].HeaderText == "Bitrate")) {
                    int val = (int)e.Value;
                    if(val==0) e.Value=""; else e.Value = val+" kbps";
                    e.FormattingApplied = true;
                }
            }
        }

        private void openBtn_MouseDown(object sender, MouseEventArgs e)
        {
            openBtn.Image = global::Echoes.Properties.Resources.import3;
        }

        private void openBtn_MouseEnter(object sender, EventArgs e)
        {
            openBtn.Image = global::Echoes.Properties.Resources.import2;
        }

        private void openBtn_MouseLeave(object sender, EventArgs e)
        {
            openBtn.Image = global::Echoes.Properties.Resources.import1;
        }

        private void openBtn_MouseUp(object sender, MouseEventArgs e)
        {
            openBtn.Image = global::Echoes.Properties.Resources.import2;
            ChooseFile();
        }

        private void exportBtn_MouseDown(object sender, MouseEventArgs e)
        {
            exportBtn.Image = global::Echoes.Properties.Resources.export3;
        }

        private void exportBtn_MouseEnter(object sender, EventArgs e)
        {
            exportBtn.Image = global::Echoes.Properties.Resources.export2;
        }

        private void exportBtn_MouseLeave(object sender, EventArgs e)
        {
            exportBtn.Image = global::Echoes.Properties.Resources.export1;
        }

        private void exportBtn_MouseUp(object sender, MouseEventArgs e)
        {
            exportBtn.Image = global::Echoes.Properties.Resources.export2;
            ExportPlaylist();
        }

        private void shuffleBtn_MouseDown(object sender, MouseEventArgs e)
        {
            shuffleBtn.Image = global::Echoes.Properties.Resources.shuffle3;
        }

        private void shuffleBtn_MouseEnter(object sender, EventArgs e)
        {
            shuffleBtn.Image = global::Echoes.Properties.Resources.shuffle2;
        }

        private void shuffleBtn_MouseLeave(object sender, EventArgs e)
        {
            shuffleBtn.Image = global::Echoes.Properties.Resources.shuffle1;
        }

        private void shuffleBtn_MouseUp(object sender, MouseEventArgs e)
        {
            shuffleBtn.Image = global::Echoes.Properties.Resources.shuffle2;
            Shuffle();
        }

        private void optionsBtn_MouseDown(object sender, MouseEventArgs e)
        {
            optionsBtn.Image = global::Echoes.Properties.Resources.options3;
        }

        private void optionsBtn_MouseEnter(object sender, EventArgs e)
        {
            optionsBtn.Image = global::Echoes.Properties.Resources.options2;
        }

        private void optionsBtn_MouseLeave(object sender, EventArgs e)
        {
            optionsBtn.Image = global::Echoes.Properties.Resources.options1;
        }

        private void optionsBtn_MouseUp(object sender, MouseEventArgs e)
        {
            optionsBtn.Image = global::Echoes.Properties.Resources.options2;
            ShowOptions();
        }

        private void repeatBtn_MouseDown(object sender, MouseEventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatNone3;
                    break;
                case 1:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatList3;
                    break;
                default:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatTrack3;
                    break;
            }
        }

        private void repeatBtn_MouseEnter(object sender, EventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatNone2;
                    break;
                case 1:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatList2;
                    break;
                default:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatTrack2;
                    break;
            }
        }

        private void repeatBtn_MouseLeave(object sender, EventArgs e)
        {
            switch (repeat)
            {
                case 0:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatNone1;
                    break;
                case 1:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatList1;
                    break;
                default:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatTrack1;
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
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatNone2;
                    break;
                case 1:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatList2;
                    break;
                default:
                    repeatBtn.Image = global::Echoes.Properties.Resources.repeatTrack2;
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
    }
}
