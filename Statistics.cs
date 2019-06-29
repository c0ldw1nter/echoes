using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echoes
{
    public partial class Statistics : Form
    {
        class StatsItem
        {
            public int num { get; set; }
            public int listened { get; set; }
            public string name { get; set; }
            [Browsable(false)] public Track track { get; set; }
            public StatsItem(string name)
            {
                this.name = name;
            }
        }
        SortableBindingList<StatsItem> stats;
        List<Track> tracks;
        public Statistics(List<Track> tracks)
        {
            InitializeComponent();
            this.tracks = tracks;
            tabControl1_SelectedIndexChanged(null, null);
            this.SetColors();
        }

        void RenumberStats()
        {
            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].num = i+1;
            }
        }

        void CalcTracks()
        {
            stats = new SortableBindingList<StatsItem>();
            foreach (Track t in tracks)
            {
                StatsItem itm=new StatsItem(t.title);
                if(t.artist!=null) itm.name+= " - "+t.artist;
                itm.track = t;
                itm.listened = t.listened;
                stats.Add(itm);
            }
            stats = new SortableBindingList<StatsItem>(stats.OrderByDescending(x => x.listened).ToList());
            RenumberStats();
            topTracksGrid.DataSource = stats;
        }

        void CalcArtists()
        {
            stats = new SortableBindingList<StatsItem>();
            foreach (Track t in tracks)
            {
                if (String.IsNullOrWhiteSpace(t.artist)) continue;
                StatsItem itm = stats.FirstOrDefault(x => x.name.Equals(t.artist,StringComparison.InvariantCultureIgnoreCase));
                if (itm == null)
                {
                    itm = new StatsItem(t.artist);
                    itm.listened = t.listened;
                    stats.Add(itm);
                }
                else
                {
                    itm.listened = itm.listened + t.listened;
                }
            }
            stats = new SortableBindingList<StatsItem>(stats.OrderByDescending(x => x.listened).ToList());
            RenumberStats();
            topArtistsGrid.DataSource = stats;
        }

        void CalcAlbums()
        {
            stats = new SortableBindingList<StatsItem>();
            foreach (Track t in tracks)
            {
                if (String.IsNullOrWhiteSpace(t.album)) continue;
                StatsItem itm = stats.FirstOrDefault(x => x.name.Equals(t.album, StringComparison.InvariantCultureIgnoreCase));
                if (itm == null)
                {
                    itm = new StatsItem(t.album);
                    itm.listened = t.listened;
                    stats.Add(itm);
                }
                else
                {
                    itm.listened = itm.listened + t.listened;
                }
            }
            stats = new SortableBindingList<StatsItem>(stats.OrderByDescending(x => x.listened).ToList());
            RenumberStats();
            topAlbumsGrid.DataSource = stats;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchBoxAlbum.Text = "";
            searchBoxArtist.Text = "";
            searchBoxTrack.Text = "";
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
            {
                CalcTracks();
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
            {
                CalcArtists();
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
            {
                CalcAlbums();
            }
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

        private void topArtistsGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (e.RowIndex >= 0)
            {
                if ((dgv.Columns[e.ColumnIndex].HeaderText == "Listened"))
                {
                    int val = (int)e.Value;
                    e.Value = val.ToTime();
                    e.FormattingApplied = true;
                }
            }
        }

        private void topArtistsGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow rw = topArtistsGrid.Rows[e.RowIndex];
            Program.mainWindow.LoadEverythingFromArtist(((StatsItem)rw.DataBoundItem).name);
        }

        private void topAlbumsGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow rw = topAlbumsGrid.Rows[e.RowIndex];
            Program.mainWindow.LoadEverythingFromAlbum(((StatsItem)rw.DataBoundItem).name);
        }

        private void topTracksGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow rw = topTracksGrid.Rows[e.RowIndex];
            Program.mainWindow.StopPlayer();
            Program.mainWindow.LoadAudio(((StatsItem)rw.DataBoundItem).track);
        }

        void FilterList(DataGridView dgv, string word)
        {
            SortableBindingList<StatsItem> source;
            if (String.IsNullOrEmpty(word))
            {
                source = new SortableBindingList<StatsItem>(stats);
            }
            else
            {
                var sourceList = new List<StatsItem>(stats);
                var searchKeywords = word.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
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
                    if (invertSearch) sourceList = sourceList.Where(x => !x.name.ContainsIgnoreCase(searchWrd)).ToList();
                    else sourceList = sourceList.Where(x => x.name.ContainsIgnoreCase(searchWrd)).ToList();
                }
                source = new SortableBindingList<StatsItem>(sourceList);
            }
            dgv.DataSource = source;
        }

        private void searchBoxTextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender) == searchBoxTrack)
            {
                FilterList(topTracksGrid, ((TextBox)sender).Text);
            }
            else if (((TextBox)sender) == searchBoxArtist)
            {
                FilterList(topArtistsGrid, ((TextBox)sender).Text);
            }
            else
            {
                FilterList(topAlbumsGrid, ((TextBox)sender).Text);
            }
        }
    }
}
