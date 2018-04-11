using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Midi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Wma;

namespace Echoes
{
    public partial class Converter : Form
    {
        public List<Track> files;
        int currentBitrate;

        string OutputPath;
        string outputPath {
            get
            {
                return OutputPath;
            }
            set 
            {
                this.OutputPath = value;
                outputPathText.Text = value;
            }
        }

        void BindBitrateComboSource()
        {
            BindingList<LAMEBitrateComboItem> bl = new BindingList<LAMEBitrateComboItem>();
            foreach (var value in Enum.GetValues(typeof(EncoderLAME.BITRATE)).Cast<int>())
            {
                bl.Add(new LAMEBitrateComboItem(value, ((EncoderLAME.BITRATE)value).ToString().Substring(5) + " kbps"));
            }
            qualityCombo.ValueMember = "bitrate";
            qualityCombo.DisplayMember = "shownName";
            qualityCombo.DataSource = bl;
            qualityCombo.SelectedIndex = Program.mainWindow.converterSelectedBitrateIndex;
        }
        
        public Converter(List<Track> files)
        {
            InitializeComponent();
            Bass.BASS_PluginLoad("bassflac.dll");
            Bass.BASS_PluginLoad("basswma.dll");
            BindBitrateComboSource();
            this.SetColors();
            convertList.DisplayMember = "filename";
            this.files = files;
            outputPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "converted");
            convertList.Items.AddRange(files.ToArray());
        }

        void QueueConversion()
        {
            if (files.Count <= 0) return;
            currentBitrate = (int)Enum.Parse(typeof(EncoderLAME.BITRATE), qualityCombo.SelectedValue.ToString());
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            convertButton.Text = "Cancel";
            outputPathButton.Enabled = false;
            qualityCombo.Enabled = false;
            outputPathText.Enabled = false;
            convertWorker.RunWorkerAsync();
        }

        private void convertWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(files.Count>0)
            {
                Track t = files.First();
                int convertStream = 20;
                if (!Program.mainWindow.LoadStream(t.filename, out convertStream, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_DECODE)) continue;

                if (Bass.BASS_ErrorGetCode() == 0 && t.length > 0)
                {
                    EncoderLAME l = new EncoderLAME(convertStream);
                    l.InputFile = null;
                    string tryName=Path.GetFileNameWithoutExtension(t.filename)+".mp3";
                    int num = 0;
                    while (File.Exists(Path.Combine(outputPath, tryName)))
                    {
                        tryName = Path.GetFileNameWithoutExtension(t.filename)+"_"+num+".mp3";
                        num++;
                    }
                    l.OutputFile = Path.Combine(outputPath,tryName);
                    l.LAME_Bitrate = currentBitrate;
                    l.LAME_Mode = EncoderLAME.LAMEMode.Default;
                    l.LAME_Quality = EncoderLAME.LAMEQuality.Quality;
                    //if(!Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(convertStream, l.TAGs)) Console.WriteLine("no tags");
                    l.TAGs = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(t.filename);
                    if (Bass.BASS_ErrorGetCode() != 0) Console.WriteLine(Bass.BASS_ErrorGetCode());
                    l.Start(null, IntPtr.Zero, false);
                    byte[] encBuffer = new byte[65536];
                    while (Bass.BASS_ChannelIsActive(convertStream) == BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        // getting sample data will automatically feed the encoder
                        int len = Bass.BASS_ChannelGetData(convertStream, encBuffer, encBuffer.Length);
                        convertWorker.ReportProgress((int)((float)Bass.BASS_ChannelGetPosition(convertStream)/(float)Bass.BASS_ChannelGetLength(convertStream)*100));
                        if (convertWorker.CancellationPending) break;
                    }
                    l.Stop();  // finish
                    Bass.BASS_StreamFree(convertStream);
                    if (convertWorker.CancellationPending)
                    {
                        File.Delete(l.OutputFile);
                    }
                }
                else
                {
                    MessageBox.Show("Error converting: " + Bass.BASS_ErrorGetCode()+Environment.NewLine+t.filename);
                }
                if (convertWorker.CancellationPending) break;
                files.RemoveAt(0);
                convertList.Invoke((MethodInvoker)delegate
                {
                    convertList.Items.RemoveAt(0);
                });
                convertWorker.ReportProgress(0);
            }
        }

        private void convertWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            convertProgress.Value = e.ProgressPercentage;
        }

        private void convertWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            convertButton.Text = "Convert";
            outputPathButton.Enabled = true;
            outputPathText.Enabled = true;
            qualityCombo.Enabled = true;
            convertProgress.Value = 0;
        }

        private void outputPathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                outputPath = dlg.SelectedPath;
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (!convertWorker.IsBusy)
            {
                QueueConversion();
            }
            else
            {
                convertWorker.CancelAsync();
            }
        }
        class LAMEBitrateComboItem
        {
            public int bitrate { get; set; }
            public string shownName { get; set; }
            public override string ToString()
            {
                return shownName;
            }
            public LAMEBitrateComboItem(int bitrate, string shownName)
            {
                this.bitrate = bitrate;
                this.shownName = shownName;
            }
        }

        private void outputPathText_TextChanged(object sender, EventArgs e)
        {
            outputPath = outputPathText.Text;
        }

        private void qualityCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.mainWindow.converterSelectedBitrateIndex = qualityCombo.SelectedIndex;
        }
    }
}
