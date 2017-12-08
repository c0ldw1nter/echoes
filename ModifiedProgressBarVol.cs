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
            
            //draw volume icon
            e.Graphics.DrawImage(Echoes.Properties.Resources.vol,new Rectangle(2,2,rec.Height,rec.Height));

            //draw strings
            string toWrite = (int)(((float)Value/(float)Maximum)*100) + "%";
            if (rec.Height < 6) return;
            Font ft = new Font(Program.mainWindow.font2.FontFamily, rec.Height-2, Program.mainWindow.font2.Style);
            SizeF sz=e.Graphics.MeasureString(toWrite,ft);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle(2,2,Width,Height),sf);

            //normalizer stuff
            toWrite = "";
            if (Program.mainWindow.wf == null) return;
            if (Program.mainWindow.wf.IsRenderingInProgress)
            {
                toWrite = "---";
            }
            else if (Program.mainWindow.DSPGain == null || !Program.mainWindow.DSPGain.IsAssigned || Program.mainWindow.DSPGain.Gain_dBV <= 0) return;
            else toWrite = "+" + String.Format("{0:0.0}", Program.mainWindow.DSPGain.Gain_dBV);
            if (String.IsNullOrEmpty(toWrite)) return;
            sf.Alignment = StringAlignment.Far;
            ft = new Font(Program.mainWindow.font2.FontFamily, ft.Size*0.8f, Program.mainWindow.font2.Style);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle(2, 2, Width-4, Height), sf);
        }
    }
}
