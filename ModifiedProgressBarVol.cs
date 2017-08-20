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
    public class ModifiedProgressBarVol : ProgressBar
    {
        public bool HasBorder = true;
        public float BorderThickness = 1f;
        public ModifiedProgressBarVol()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if ((this.Parent as Echoes.Echoes) == null) return;
            Rectangle rec = e.ClipRectangle;
            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).volBarBackColor), 0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).volBarForeColor), 2, 2, rec.Width, rec.Height);
            if (HasBorder) e.Graphics.DrawRectangle(new Pen((Parent as Echoes.Echoes).volBarForeColor, BorderThickness), 0, 0, Width - BorderThickness, Height - BorderThickness);
            e.Graphics.DrawImage(Echoes.Properties.Resources.vol,new Rectangle(2,2,rec.Height,rec.Height));
            string toWrite = (int)(((float)Value/(float)Maximum)*100) + "%";
            if (rec.Height < 6) return;
            Font ft = new Font(Program.mainWindow.secondaryFont.FontFamily, rec.Height-2, Program.mainWindow.secondaryFont.Style);
            SizeF sz=e.Graphics.MeasureString(toWrite,ft);
            /*Bitmap bmp = new Bitmap((int)sz.Width, (int)sz.Height);
            bmp.MakeTransparent();
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(toWrite, ft, Brushes.Black, new Rectangle(0,0,(int)sz.Width,(int)sz.Height));
            g.Flush();
            e.Graphics.DrawImage(bmp, new PointF((Width - sz.Width) / 2, (Height - sz.Height) / 2 + (int)(sz.Height * 0.15)));*/
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle(2,2,Width,Height),sf);
            //e.Graphics.DrawRectangle(Pens.Red, new Rectangle(0, 0, Width, Height));
            //e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF((Width - sz.Width) / 2, 0),sf);
        }
    }
}
