using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace Echoes
{
    public partial class DupeFinder : Form
    {
        class Dupe
        {
            public List<Track> files;
            public string match;
            public Dupe(string match)
            {
                this.match = match;
                this.files = new List<Track>();
            }
        }
        List<Track> foundDupes;
        List<Track> tracks;
        List<Dupe> listOfDupes = new List<Dupe>();
        public DupeFinder(List<Track> tracks)
        {
            
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            modifiedProgressBarLoading1.Maximum = modifiedProgressBarLoading1.Width;
            this.tracks = tracks;
        }

        public byte[] GetMd5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        private Expression<Func<Dupe, bool>> HashPredicate(Track tr)
        {
            return i => new FileInfo(tr.filename).Length == new FileInfo(i.files.FirstOrDefault().filename).Length && GetMd5(tr.filename).SequenceEqual(GetMd5(i.files.FirstOrDefault().filename));
        }

        private Expression<Func<Dupe, bool>> TitlePredicate(Track tr)
        {
            return i => i.files.FirstOrDefault().title == tr.title;
        }

        private Expression<Func<Dupe, bool>> FilenamePredicate(Track tr)
        {
            return i => i.files.FirstOrDefault().filename == tr.filename;
        }

        private void dupeFinderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            /*Track t=null;
            Expression<Func<Dupe, bool>> exp;
            if(radioHash.Checked) exp=HashPredicate(t);
            else if(radioTitle.Checked) exp=TitlePredicate(t);
            else exp=FilenamePredicate(t);*/
            listOfDupes.Clear();
            for(int i=0;i<tracks.Count;i++)
            {
                Track t=tracks[i];
                if (!File.Exists(t.filename)) continue;
                Dupe where;
                string matchstring;
                if (radioHash.Checked)
                {
                    matchstring = GetMd5(t.filename).ToHex(false);
                    where = listOfDupes.FirstOrDefault(d => new FileInfo(t.filename).Length  == new FileInfo(d.files.FirstOrDefault().filename).Length && matchstring==GetMd5(d.files.FirstOrDefault().filename).ToHex(false));
                }
                else if (radioTitle.Checked)
                {
                    where = listOfDupes.FirstOrDefault(d => d.files.FirstOrDefault().title.ToLower() == t.title.ToLower());
                    matchstring=t.title;
                }
                else
                {
                    where = listOfDupes.FirstOrDefault(d => d.files.FirstOrDefault().filename == t.filename);
                    matchstring = t.filename;
                }
                if (where!=null)
                {
                    where.files.Add(t);
                }
                else
                {
                    Dupe dp = new Dupe(matchstring);
                    dp.files.Add(t);
                    listOfDupes.Add(dp);
                }
                dupeFinderWorker.ReportProgress((int)((float)i/(float)tracks.Count*100));
            }
            listOfDupes = listOfDupes.Where(x => x.files.Count > 1).ToList();
        }

        private void dupeFinderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            modifiedProgressBarLoading1.Value = (int)(modifiedProgressBarLoading1.Maximum * (float)e.ProgressPercentage/100f);
        }

        private void dupeFinderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            modifiedProgressBarLoading1.Value = modifiedProgressBarLoading1.Maximum;
            List<Color> colors=new List<Color>();
            List<bool> checkbx=new List<bool>();
            foundDupes = new List<Track>();
            Random rng=new Random();
            foreach (Dupe dp in listOfDupes)
            {
                Color clr = Color.FromArgb((byte)rng.Next(60, 255), (byte)rng.Next(60, 255), (byte)rng.Next(60, 255));
                for (int z = 0; z < dp.files.Count; z++)
                //foreach (Track tr in dp.files)
                {
                    foundDupes.Add(dp.files[z]);
                    colors.Add(clr);
                    if (z == 0) checkbx.Add(false);
                    else checkbx.Add(true);
                }
            }
            dataGridView1.DataSource = foundDupes;
            foreach (DataGridViewRow rw in dataGridView1.Rows)
            {
                rw.DefaultCellStyle.BackColor = colors[rw.Index];
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)rw.Cells[0];
                if (checkbx[rw.Index]) chk.Value = chk.TrueValue;
                else chk.Value = chk.FalseValue;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if(!dupeFinderWorker.IsBusy) dupeFinderWorker.RunWorkerAsync();
        }

        private void radioHash_CheckedChanged(object sender, EventArgs e)
        {
            if (radioHash.Checked)
            {
                radioTitle.Checked = false;
                radioFilepath.Checked = false;
            }
        }

        private void radioTitle_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTitle.Checked)
            {
                radioHash.Checked = false;
                radioFilepath.Checked = false;
            }
        }

        private void radioFilepath_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFilepath.Checked)
            {
                radioTitle.Checked = false;
                radioHash.Checked = false;
            }
        }

        void OpenCheckedInExplorer()
        {
            List<string> trackzors = new List<string>();
            foreach (DataGridViewRow rw in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)rw.Cells[0];
                if (chk.Value == chk.TrueValue)
                {
                    Track t = (Track)rw.DataBoundItem;
                    trackzors.Add(t.filename);
                }
            }
            Program.mainWindow.OpenInExplorer(trackzors);
        }

        void DeleteChecked()
        {
            List<int> nums = new List<int>();
            foreach (DataGridViewRow rw in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)rw.Cells[0];
                if (chk.Value == chk.TrueValue)
                {
                    Track t = (Track)rw.DataBoundItem;
                    nums.Add(t.num);
                }
            }
            Program.mainWindow.DeleteTrack(nums);
            foundDupes = foundDupes.Where(x => !nums.Contains(x.num)).ToList();
            dataGridView1.DataSource = foundDupes;
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest=dataGridView1.HitTest(e.X, e.Y);
                if (Program.mainWindow == null || hitTest.Type!=DataGridViewHitTestType.Cell) return;
                DataGridViewRow r = dataGridView1.Rows[hitTest.RowIndex];
                if (!r.Selected) dataGridView1.ClearSelection();
                r.Selected = true;
                Track t = (Track)r.DataBoundItem;
                List<Track> selectedTracks = new List<Track>();
                List<int> nums = new List<int>();
                foreach (DataGridViewRow rw in dataGridView1.SelectedRows)
                {
                    Track tr = (Track)rw.DataBoundItem;
                    selectedTracks.Add(tr);
                    nums.Add(tr.num);
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
                    Program.mainWindow.OpenInExplorer(filenamesToShow);
                };
                MenuItem miDelete = new MenuItem("Delete");
                miDelete.Click += (theSender, eventArgs) =>
                {
                    Program.mainWindow.DeleteTrack(nums);
                    foundDupes = foundDupes.Where(x => !nums.Contains(x.num)).ToList();
                };
                cm.MenuItems.Add(miShowExplorer);
                cm.MenuItems.Add(miDelete);
                cm.Show(dataGridView1, e.Location);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count || dataGridView1.Rows[e.RowIndex] == null || dataGridView1.Rows[e.RowIndex].DataBoundItem == null) return;
            Track t = (Track)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            //ROLLBACKPOINT if (mainStream != null) mainStream.SetPosition(0);
            if (Program.mainWindow.nowPlaying==t)
            {
                Program.mainWindow.SetPosition(0);
                Program.mainWindow.Play();
                return;
            }
            Program.mainWindow.StopPlayer();
            Program.mainWindow.OpenFile(t);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenCheckedInExplorer();
        }

        void UncheckAll()
        {
            foreach (DataGridViewRow rw in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)rw.Cells[0];
                chk.Value = chk.FalseValue;
            }
        }

        void DeleteCheckedFiles()
        {
            List<int> nums = new List<int>();
            List<string> files = new List<string>();
            foreach (DataGridViewRow rw in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)rw.Cells[0];
                if (chk.Value == chk.TrueValue)
                {
                    Track t = (Track)rw.DataBoundItem;
                    nums.Add(t.num);
                    files.Add(t.filename);
                }
            }
            if (MessageBox.Show(files.Count+" files will be deleted permanently. Proceed?", "Caution", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Program.mainWindow.DeleteTrack(nums);
                foundDupes = foundDupes.Where(x => !nums.Contains(x.num)).ToList();
                dataGridView1.DataSource = foundDupes;
                foreach (string s in files)
                {
                    if (Program.mainWindow.nowPlaying.filename == s) Program.mainWindow.UnloadAudioFile();
                    if (File.Exists(s))
                    {
                        File.Delete(s);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteCheckedFiles();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UncheckAll();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Echoes.TrackCellFormatting(sender, e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
