using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Echoes;

namespace ModifiedControls
{
    public class ModifiedMenuItem : MenuItem
    {
        public float fontSize = 10f;
        public ModifiedMenuItem(string text)
            : base(text)
        {
            Init();
        }
        public ModifiedMenuItem()
            : base()
        {
            Init();
        }

        void Init()
        {
            this.OwnerDraw = true;
            this.DrawItem += ModifiedMenuItem_DrawItem;
            this.MeasureItem += ModifiedMenuItem_MeasureItem;
        }

        void ModifiedMenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            Font font = new Font(Program.mainWindow.font1.FontFamily, fontSize);
            SizeF sze = e.Graphics.MeasureString(m.Text, font);
            e.ItemHeight = (int)sze.Height;
            e.ItemWidth = (int)sze.Width;
        }

        void ModifiedMenuItem_DrawItem(object sender, DrawItemEventArgs e)
        {
            MenuItem cmb = sender as MenuItem;
            e.DrawBackground();
            Color c1 = Program.mainWindow.controlBackColor;
            Color c2 = Program.mainWindow.controlForeColor;
            if (e.State != DrawItemState.NoAccelerator)
            {
                c1 = Program.mainWindow.controlForeColor;
                c2 = Program.mainWindow.controlBackColor;
            }
            e.Graphics.FillRectangle(new SolidBrush(c1), e.Bounds);
            e.Graphics.DrawString(Text, new Font(Program.mainWindow.font1.FontFamily, fontSize), new SolidBrush(c2), e.Bounds);
            Console.Write(e.State.ToString() + "|");
        }
    }
}
