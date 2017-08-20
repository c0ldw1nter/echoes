using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Echoes;
using Un4seen.Bass;

namespace ModifiedControls
{
    public class ModifiedProgressBarSeek : ProgressBar
    {
        public bool HasBorder=true;
        public float BorderThickness = 1f;
        public TimeSpan time1, time2;
        public ModifiedProgressBarSeek()
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
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).seekBarBackColor), 0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).seekBarForeColor), 2, 2, rec.Width, rec.Height);
            if (HasBorder) e.Graphics.DrawRectangle(new Pen((Parent as Echoes.Echoes).seekBarForeColor, BorderThickness), 0, 0, Width - BorderThickness, Height - BorderThickness);
            if (!Program.mainWindow.streamLoaded) return;
            time1 = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(Program.mainWindow.stream, Program.mainWindow.GetPosition()));
            time2 = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(Program.mainWindow.stream, Program.mainWindow.GetLength()));
            time2 -= time1;
            string toWrite = time1.TotalSeconds.ToTime();
            Font ft = new Font(Program.mainWindow.secondaryFont.FontFamily, rec.Height-2, Program.mainWindow.secondaryFont.Style);
            SizeF sz = e.Graphics.MeasureString(toWrite, ft);
            //e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF(2, ((float)Height-sz.Height)/2));
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle(2,2,(int)sz.Width + 1, Height), sf);
            toWrite = time2.TotalSeconds.ToTime();
            sz = e.Graphics.MeasureString(toWrite, ft);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle((int)Width - (int)sz.Width - 2, 2, (int)sz.Width + 1, Height), sf);
            //sz = e.Graphics.MeasureString(toWrite, ft);
            //e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF(Width - sz.Width, ((float)Height-sz.Height)/2));
            /*ROLLBACKPOINT
            string toWrite = Program.mainWindow.mainStream.CurrentTime.ToString("mm\\:ss");
            Font ft = new Font("White Rabbit", (int)(rec.Height*0.8), FontStyle.Regular);
            SizeF sz = e.Graphics.MeasureString(toWrite, ft);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF(2, (Height - sz.Height) / 2 + (int)(sz.Height * 0.15)));
            toWrite = (Program.mainWindow.mainStream.TotalTime - Program.mainWindow.mainStream.CurrentTime).ToString("mm\\:ss");
            sz = e.Graphics.MeasureString(toWrite, ft);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new PointF(Width-sz.Width, (Height - sz.Height) / 2 + (int)(sz.Height * 0.15)));*/

            /*Point[] pts=new Point[Echoes.Echoes.waveformCompleted];
            for (int i = 0; i < Echoes.Echoes.waveformCompleted; i++)
            {
                pts[i] = new Point(i, (int)((float)Echoes.Echoes.waveform[i]/Int16.MaxValue*Height+Height/2));
            }
            e.Graphics.DrawLines(Pens.Crimson, pts);*/
        }
    }
}
