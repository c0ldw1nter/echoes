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
    public class ModifiedProgressBarLoading : ProgressBar
    {
        public bool HasBorder=true;
        public float BorderThickness = 1f;
        public ModifiedProgressBarLoading()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;
            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            if (Program.mainWindow == null) return;
            e.Graphics.FillRectangle(new SolidBrush(Program.mainWindow.volBarBackColor), 0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush(Program.mainWindow.volBarForeColor), 2, 2, rec.Width, rec.Height);
            if (HasBorder) e.Graphics.DrawRectangle(new Pen(Program.mainWindow.volBarForeColor, BorderThickness), 0, 0, Width - BorderThickness, Height - BorderThickness);
            string toWrite = (int)(((float)Value / (float)Maximum) * 100) + "%";
            if (rec.Height < 6) return;
            Font ft = new Font(Program.mainWindow.font2.FontFamily, rec.Height - 2, Program.mainWindow.font2.Style);
            SizeF sz = e.Graphics.MeasureString(toWrite, ft);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF((Width - sz.Width) / 2, (Height - sz.Height) / 2 + (int)(sz.Height * 0.15)));
        }
    }
}
