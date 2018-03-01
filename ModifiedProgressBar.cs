using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Echoes;

namespace ModifiedControls
{
    public class ModifiedProgressBar : ProgressBar
    {
        public bool HasBorder = true;
        public float BorderThickness = 1f;
        public string Text = String.Empty;
        public ModifiedProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Program.mainWindow == null)
            {
                base.OnPaint(e);
                return;
            }
            Rectangle rec = e.ClipRectangle;
            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.FillRectangle(new SolidBrush(Program.mainWindow.volBarBackColor), 0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush(Program.mainWindow.volBarForeColor), 2, 2, rec.Width, rec.Height);
            if (HasBorder) e.Graphics.DrawRectangle(new Pen(Program.mainWindow.volBarForeColor, BorderThickness), 0, 0, Width - BorderThickness, Height - BorderThickness);

            if (String.IsNullOrWhiteSpace(Text)) return;
            Font ft = new Font(Program.mainWindow.font2.FontFamily, rec.Height - 2, Program.mainWindow.font2.Style);
            SizeF sz = e.Graphics.MeasureString(Text, ft);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(Text, ft, Brushes.Black, new Rectangle(2, 2, Width, Height), sf);
        }
    }
}
