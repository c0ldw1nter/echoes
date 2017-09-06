using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;
using System.IO;
using ModifiedControls;

namespace Echoes
{
    public partial class TagEditor : Form
    {
        public class TagItem
        {
            public string tag { get; set; }
            public string value { get; set; }
            public TagItem(string tag)
            {
                this.tag = tag;
            }
        }
        DataGridView dgv = new DataGridView();
        List<TagItem> tagItems = null;
        List<Track> tracks;
        Label modWarning;
        ModifiedButton btnSave, btnCancel;
        int bottomOfGrid;
        public TagEditor(List<Track> tracks, Echoes parent)
        {
            InitializeComponent();
            this.Owner = parent;
            dgv.KeyDown += TagEditor_KeyDown;
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.AutoGenerateColumns = true;
            dgv.Font = new Font(new FontFamily("Courier New"), 8f);
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.AllowUserToOrderColumns = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowTemplate.Height = 18;
            dgv.EnableHeadersVisualStyles = false;
            dgv.MouseUp+=dgv_MouseUp;
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Width = (int)(this.dgv.Width * 0.3),
                DataPropertyName = "tag",
                HeaderText = "Tag",
                ReadOnly = true
                
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Width = (int)(this.dgv.Width * 0.695),
                DataPropertyName = "value",
                HeaderText = "Value"
            });

            bottomOfGrid = 7 * dgv.RowTemplate.Height + dgv.ColumnHeadersHeight + (int)(Owner as Echoes).font1.Size + 5;
            this.Text = "Tag Editor: " + tracks.Count + " files loaded";
            this.tracks = tracks;
            ControlBox = false;

            btnSave = new ModifiedButton();
            btnSave.Font = (Owner as Echoes).font1;
            btnSave.Size = new Size(60, 30);
            btnSave.Location = new Point(5, bottomOfGrid);
            btnSave.Dock = DockStyle.None;
            btnSave.Text = "Save";
            btnSave.Click += (sender, eventArgs) =>
            {
                SaveTags();
                this.Close();
                this.Dispose();
            };

            btnCancel = new ModifiedButton();
            btnCancel.Font = (Owner as Echoes).font1;
            btnCancel.Size = new Size(60, 30);
            btnCancel.Location = new Point(350, bottomOfGrid);
            btnCancel.Dock = DockStyle.None;
            btnCancel.Text = "Cancel";
            btnCancel.Click += (sender, eventArgs) =>
            {
                this.Close();
                this.Dispose();
            };
            if (tracks.Where(x => Program.mainWindow.supportedModuleTypes.Contains(Path.GetExtension(x.filename))).ToList().Count > 0)
            {
                modWarning = new Label();
                modWarning.Location = new Point(btnSave.Location.X+btnSave.Width+5, bottomOfGrid+7);
                modWarning.BackColor = Program.mainWindow.backgroundColor;
                modWarning.ForeColor = Program.mainWindow.controlForeColor;
                modWarning.Font = (Owner as Echoes).font1;
                modWarning.Dock = DockStyle.None;
                modWarning.Text = "Module file tag writing not supported.";
                Controls.Add(modWarning);
            }
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            
            Controls.Add(dgv);
            this.SetColors();
            PopulateDgv();

            this.ClientSize = new Size(btnCancel.Location.X+btnCancel.Width+5,btnSave.Location.Y+btnSave.Height+5);
            if(modWarning!=null) modWarning.Width = ClientSize.Width - btnSave.Width * 2 - 20;

            LoadTags();
        }

        void TagEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveTags();
                this.Close();
                this.Dispose();
            }
        }

        void dgv_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hitTestInfo = dgv.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
                    dgv.BeginEdit(true);
                else
                    dgv.EndEdit();
            }
        }

        void LoadTags()
        {
            if (tracks.Count == 1 && !Program.mainWindow.supportedModuleTypes.Contains(Path.GetExtension(tracks[0].filename)))
            {
                using (TagLib.File tagFile = TagLib.File.Create(tracks[0].filename))
                {
                    dgv.Rows[0].Cells[1].Value = tagFile.Tag.Title;
                    if (tagFile.Tag.Performers.Length > 0) dgv.Rows[1].Cells[1].Value = tagFile.Tag.Performers[0];
                    else dgv.Rows[1].Cells[1].Value = "";
                    dgv.Rows[2].Cells[1].Value = tagFile.Tag.Album;
                    dgv.Rows[3].Cells[1].Value = tagFile.Tag.Year;
                    dgv.Rows[4].Cells[1].Value = tagFile.Tag.Comment;
                    if(tagFile.Tag.Genres.Length>0) dgv.Rows[5].Cells[1].Value = tagFile.Tag.Genres[0];
                    else dgv.Rows[5].Cells[1].Value = "";
                    tagFile.Dispose();
                }
            }
            else
            {
                dgv.Rows[0].Cells[1].Value =
                dgv.Rows[1].Cells[1].Value =
                dgv.Rows[2].Cells[1].Value =
                dgv.Rows[3].Cells[1].Value =
                dgv.Rows[4].Cells[1].Value =
                dgv.Rows[5].Cells[1].Value = "&unchanged";
            }
        }
        void SaveTags()
        {
            dgv.EndEdit();
            foreach (Track t in tracks)
            {
                if (Program.mainWindow.nowPlaying!=null && Program.mainWindow.nowPlaying.filename == t.filename) Program.mainWindow.UnloadAudioFile();
                if (!System.IO.File.Exists(t.filename)) continue;
                using (TagLib.File tagFile = TagLib.File.Create(t.filename))
                {
                    string title = (string)dgv.Rows[0].Cells[1].Value;
                    if (title == "&unchanged") title = tagFile.Tag.Title;
                    string artist = (string)dgv.Rows[1].Cells[1].Value;
                    if (artist == "&unchanged")
                    {
                        if (tagFile.Tag.Performers.Length>0)
                        {
                            artist = tagFile.Tag.Performers[0];
                        }
                        else
                        {
                            artist = "";
                        }
                    }
                    string album = (string)dgv.Rows[2].Cells[1].Value;
                    if (album == "&unchanged") album = tagFile.Tag.Album;
                    string comment = (string)dgv.Rows[4].Cells[1].Value;
                    if (comment == "&unchanged") comment = tagFile.Tag.Comment;
                    string genre = (string)dgv.Rows[5].Cells[1].Value;
                    if (genre == "&unchanged")
                    {
                        if (tagFile.Tag.Genres.Length>0)
                        {
                            genre = tagFile.Tag.Genres[0];
                        }
                        else
                        {
                            genre = "";
                        }
                    }
                    int year = (int)tagFile.Tag.Year;
                    if ((string)dgv.Rows[3].Cells[1].Value != "&unchanged")
                        try
                        {
                            year = Int32.Parse((string)dgv.Rows[3].Cells[1].Value);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Year field is invalid. Must be a 4-digit number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    tagFile.Tag.Title = title;
                    tagFile.Tag.Performers = null;
                    tagFile.Tag.Performers = new[] { artist };
                    tagFile.Tag.Album = album;
                    tagFile.Tag.Year = (uint)year;
                    tagFile.Tag.Comment = comment;
                    tagFile.Tag.Genres = null;
                    tagFile.Tag.Genres = new[] { genre };
                    tagFile.Save();
                    if(!String.IsNullOrEmpty(title)) t.title = title;
                    t.artist = artist;
                    t.album = album;
                    //t.year=(string)year;
                    //t.comment=comment;
                    tagFile.Dispose();
                }
            }
            Program.mainWindow.UpdateCache(tracks);
            (Owner as Echoes).RefreshGrid();
        }
        void PopulateDgv() {
            tagItems = new List<TagItem>()
            {
                new TagItem("Title"),
                new TagItem("Artist"),
                new TagItem("Album"),
                new TagItem("Year"),
                new TagItem("Comment"),
                new TagItem("Genre")
            };
            BindingList<TagItem> source = new BindingList<TagItem>(tagItems);
            dgv.DataSource = tagItems;
        }
    }
}
