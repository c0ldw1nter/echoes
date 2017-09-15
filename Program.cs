using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Echoes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 


        #region Default values for the program

        public readonly static List<ColumnInfo> defaultColumnInfo = new List<ColumnInfo>() {
            new ColumnInfo(0,35),
            new ColumnInfo(1,250),
            new ColumnInfo(2,140),
            new ColumnInfo(3,60),
            new ColumnInfo(4,155),
            new ColumnInfo(5,70),
            new ColumnInfo(6,100),
            new ColumnInfo(10,70)
        };

        //general
        public readonly static Font defaultFont = new Font("Courier New", 8, FontStyle.Regular);
        public readonly static int visualisationStyleDefault = 1;
        public readonly static int visualfpsDefault = 40;
        public readonly static int repeatDefault = 1;
        public readonly static float volumeDefault = 1f;
        public readonly static bool autoShuffleDefault = false;
        public readonly static bool autoAdvanceDefault = true;
        public readonly static bool trackChangePopupDefault = false;
        public readonly static bool saveTransposeDefault = true;
        public readonly static bool stereoDefault = true;
        public readonly static bool showWaveformDefault = false;
        public readonly static float hotkeyVolumeIncrementDefault = 0.03f;
        public readonly static float hotkeyTransposeIncrementDefault = 0.5f;
        public readonly static string midiSfLocationDefault = "";
        public readonly static Font font1Default = new Font("Courier New", 8, FontStyle.Regular);
        public readonly static Font font2Default = new Font("Consolas", 12, FontStyle.Regular);

        //hotkeys
        public readonly static bool hotkeysAllowedDefault = true;
        public readonly static List<HotkeyData> defaultHotkeys = new List<HotkeyData>() {
            new HotkeyData(Hotkey.ADVANCEPLAYER, Keys.Right, 2, true),
            new HotkeyData(Hotkey.PREVIOUSPLAYER, Keys.Left, 2, true),
            new HotkeyData(Hotkey.PLAYPAUSE, Keys.Down, 2, true),
            new HotkeyData(Hotkey.GLOBAL_VOLUMEUP, Keys.Add, 2, true),
            new HotkeyData(Hotkey.GLOBAL_VOLUMEDOWN, Keys.Subtract, 2, true),
            new HotkeyData(Hotkey.TRANSPOSEUP, Keys.PageUp, 2, true),
            new HotkeyData(Hotkey.TRANSPOSEDOWN, Keys.PageDown, 2, true),
            new HotkeyData(Hotkey.DELETE, Keys.Delete, 2, false),
            new HotkeyData(Hotkey.VOLUMEUP, Keys.Home, 2, true),
            new HotkeyData(Hotkey.VOLUMEDOWN, Keys.End, 2, true),
        };

        public readonly static Color backgroundColorDefault = Color.FromArgb(24, 24, 24);
        public readonly static Color controlBackColorDefault = Color.FromArgb(48, 48, 48);
        public readonly static Color controlForeColorDefault = Color.FromArgb(200, 200, 200);
        public readonly static Color highlightBackColorDefault = Color.FromArgb(180, 255, 153);
        public readonly static Color highlightForeColorDefault = Color.FromArgb(32, 32, 32);
        public readonly static Color seekBarBackColorDefault = Color.FromArgb(72, 72, 72);
        public readonly static Color seekBarForeColorDefault = Color.FromArgb(255, 255, 120);
        public readonly static Color volBarBackColorDefault = Color.FromArgb(72, 72, 72);
        public readonly static Color volBarForeColorDefault = Color.FromArgb(150, 230, 255);
        public readonly static Color trackTitleColorDefault = Color.FromArgb(150, 230, 255);
        public readonly static Color trackArtistColorDefault = Color.FromArgb(180, 255, 153);
        public readonly static Color trackAlbumColorDefault = Color.FromArgb(255, 181, 79);
        public readonly static Color spectrumColorDefault = Color.FromArgb(255, 100, 100);

        #endregion
        public static List<string> filesToOpen = new List<string>();
        public static Echoes mainWindow;

        [STAThread]
        static void Main(string[] args)
        {
            foreach (string s in args)
            {
                if (Directory.Exists(Path.GetDirectoryName(s)))
                {
                    filesToOpen.Add(s);
                }
                
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainWindow = new Echoes();
            Application.Run(mainWindow);
        }
    }
}
