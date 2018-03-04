using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Linq;

namespace Echoes
{
    public class Settings
    {
        public int volume { get; set; }
        public int repeat { get; set; }
        public int visualisationStyle { get; set; }
        public int visualFps { get; set; }
        public int fontSizePercentage { get; set; }

        public float hotkeyVolumeIncrement { get; set; }
        public float hotkeyTransposeIncrement { get; set; }

        public bool hotkeysAllowed { get; set; }
        public bool autoAdvance { get; set; }
        public bool autoShuffle { get; set; }
        public bool trackChangePopup { get; set; }
        public bool saveTranspose { get; set; }
        public bool showWaveform { get; set; }
        public bool normalize { get; set; }
        public bool suppressHotkeys { get; set; }

        public string midiSfLocation { get; set; }

        public Font font1 { get; set; }
        public Font font2 { get; set; }

        ColorScheme currentColorScheme { get; set; }
        List<ColorScheme> savedColorSchemes { get; set; }

        List<HotkeyData> hotkeys { get; set; }

        List<ColumnInfo> currentColumns { get; set; }

        public void SaveToXML(string xmlPath)
        {

        }

        public void LoadFromXml(string xmlPath)
        {

        }
    }
}
