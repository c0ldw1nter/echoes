using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Echoes
{
    public partial class Exporter : Form
    {
        static Track currentTrackIteration;
        public static bool inProgress = false;
        public static int progress, total, playlistIndex;
        static List<ExporterMessage> messagesToLog = new List<ExporterMessage>();
        public static string playlistName;
        int updatedCount;
        List<List<Track>> listOfLists;
        List<string> playlistNames;
        string outPath;
        List<Track> newPlaylist = new List<Track>();
        List<PathAndSize> pathsNSizes = new List<PathAndSize>();
        public Exporter(List<List<Track>> listOfLists, List<string> playlistNames, string outPath)
        {
            this.playlistNames=playlistNames;
            this.listOfLists = listOfLists;
            this.outPath = outPath;
            InitializeComponent();
            this.SetColors();
            if (listOfLists.Count == 0 || !Directory.Exists(outPath))
            {
                infoLabel.Text = "Playlist empty or directory doesn't exist.";
                return;
            }
            List<string> outPathFiles=Directory.EnumerateFiles(outPath).ToList();
            foreach (string s in outPathFiles)
            {
                PathAndSize pns = new PathAndSize(s);
                pns.size = new FileInfo(s).Length;
                pathsNSizes.Add(pns);
            }
            
            exporterWorker.RunWorkerAsync();
        }
             
        string GetMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        string PathHasFile(Track t)
        {
            foreach (PathAndSize pns in pathsNSizes)
            {
                if (pns.size == t.size)
                {
                    if (Path.GetFileName(t.filename).Equals(Path.GetFileName(pns.path)))
                    {
                        return pns.path;
                    }
                    if(GetMD5(t.filename).Equals(GetMD5(pns.path))) {
                        return pns.path;
                    }
                }
            }
            return String.Empty;
        }

        string GetFreeFilename(string path, string file)
        {
            string newPath = Path.Combine(path, Path.GetFileName(file));
            if (File.Exists(newPath))
            {
                string newName=Path.GetFileNameWithoutExtension(file) + "_" + GetMD5(currentTrackIteration.filename) + Path.GetExtension(file);
                messagesToLog.Add(new ExporterMessage(progress + ". " + file + " name already exists. Renaming to "+newName, Color.Yellow));
                newPath = Path.Combine(path, newName);
            }
            return newPath;
        }

        private void exporterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            inProgress = true;
            updatedCount = 0;
            for (int i = 0; i < listOfLists.Count; i++)
            {
                playlistIndex = i + 1;
                exporterWorker.ReportProgress(0);
                List<Track> playlist = listOfLists[i];
                playlistName = playlistNames[i];
                foreach (Track t in playlist) t.GetSize();
                total = playlist.Count;
                foreach (Track t in playlist)
                {
                    currentTrackIteration = t;
                    progress = playlist.IndexOf(t) + 1;
                    string maybePath = PathHasFile(t);
                    if (maybePath.Equals(String.Empty))
                    {
                        //doesnt have file, copy
                        messagesToLog.Add(new ExporterMessage(progress + ". " + Path.GetFileName(t.filename) + " not found.", Color.Red));
                        maybePath = GetFreeFilename(outPath, Path.GetFileName(t.filename));
                        try
                        {
                            File.Copy(t.filename, maybePath);
                            updatedCount++;
                        }
                        catch (Exception)
                        {
                            maybePath = String.Empty;
                            messagesToLog.Add(new ExporterMessage(progress + ". " + Path.GetFileName(t.filename) + " error copying!!", Color.Magenta));
                            //MessageBox.Show(exc.StackTrace);
                        }
                    }
                    else
                    {
                        //already in
                        messagesToLog.Add(new ExporterMessage(progress + ". " + Path.GetFileName(t.filename) + " found.", Color.Lime));
                        if (!Path.GetFileName(t.filename).Equals(Path.GetFileName(maybePath)))
                        {
                            messagesToLog.Add(new ExporterMessage(progress + ". " + "Different filename: " + Path.GetFileName(maybePath), Color.Cyan));
                        }
                    }
                    //this down here is with double protection so you dont get an unexisting file in playlist (see catch above)
                    if (!maybePath.Equals(String.Empty)) newPlaylist.Add(new Track(maybePath, Path.GetFileName(maybePath)));
                    exporterWorker.ReportProgress(0);
                }
            }
        }

        void PrintMessage()
        {
            while (messagesToLog.Count > 0)
            {
                ExporterMessage dis = messagesToLog.First();
                log.AppendText(dis.text+Environment.NewLine, dis.color);
                messagesToLog.Remove(dis);
            }
        }

        void UpdateProgressLabel()
        {
            infoLabel.Text = "[Playlist "+playlistIndex+"/"+listOfLists.Count+"]"+"Processing file " + progress + "/" + total+" ...";
        }

        private void exporterWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressLabel();
            PrintMessage();
        }

        private void exporterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Program.mainWindow.ExportM3u(Path.Combine(outPath, playlistName), newPlaylist);
            PrintMessage();
            infoLabel.Text = "Done. Added "+updatedCount+" new files. Playlist total: "+newPlaylist.Count;
            inProgress = false;
        }
    }

    class PathAndSize
    {
        public PathAndSize(string path)
        {
            this.path = path;
        }
        public string path;
        public long size;
    }

    class ExporterMessage
    {
        public ExporterMessage(string text) { this.text = text; this.color = Program.mainWindow.controlForeColor; }
        public ExporterMessage(string text, Color color) { this.text = text; this.color = color; }
        public string text;
        public Color color;
    }
}
