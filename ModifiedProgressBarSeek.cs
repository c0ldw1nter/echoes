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
        ToolTip seekTimeTooltip = new ToolTip();
        Point lastTipLocation;
        int trig = 0;
        
        public ModifiedProgressBarSeek()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            //this.MouseMove += ModifiedProgressBarSeek_MouseMove;
            //this.MouseLeave += ModifiedProgressBarSeek_MouseLeave;
        }

        void ModifiedProgressBarSeek_MouseLeave(object sender, EventArgs e)
        {
            seekTimeTooltip.Hide(this);
        }

        void ModifiedProgressBarSeek_MouseMove(object sender, MouseEventArgs e)
        {
            if(lastTipLocation==e.Location) return;
            double time=(Program.mainWindow.GetLength() * (int)((float)e.X / (float)this.Width*this.Maximum)) / this.Maximum;
            seekTimeTooltip.Show(time.ToTime()+" ;; "+trig,this);
            trig++;
            lastTipLocation = e.Location;
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if ((this.Parent as Echoes.Echoes) == null) return;

            //bar
            Rectangle rec = e.ClipRectangle;
            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).seekBarBackColor), 0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush((Parent as Echoes.Echoes).seekBarForeColor), 2, 2, rec.Width, rec.Height);

            /*draw tracker
            /Image traker = Echoes.Properties.Resources.tracker;
            int trakerWidth = traker.Width * e.ClipRectangle.Height / traker.Height;
            e.Graphics.DrawImage(traker, rec.Width - trakerWidth / 2, 0, trakerWidth, e.ClipRectangle.Height);*/


            //waveform
            if (Program.mainWindow.showWaveform && Program.mainWindow.waveformImage != null)
            {
                GraphicsUnit unit=GraphicsUnit.Pixel;
                e.Graphics.DrawImage(Program.mainWindow.waveformImage, new Rectangle(-3,2,Width,rec.Height), Rectangle.Round(Program.mainWindow.waveformImage.GetBounds(ref unit)), GraphicsUnit.Pixel);
                /*try
                {
                    e.Graphics.DrawImage(CropImage(Program.mainWindow.waveformImage, new Rectangle(0, 0, rec.Width, Height)), 2, 2);
                }
                catch (Exception) {  }*/
            }
            if (HasBorder) e.Graphics.DrawRectangle(new Pen((Parent as Echoes.Echoes).seekBarForeColor, BorderThickness), 0, 0, Width - BorderThickness, Height - BorderThickness);

            //text

            if (!Program.mainWindow.streamLoaded) return;
            time1 = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(Program.mainWindow.stream, Program.mainWindow.GetPosition()));
            time2 = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(Program.mainWindow.stream, Program.mainWindow.GetLength()));
            time2 -= time1;

            string toWrite = time1.TotalSeconds.ToTime();
            Font ft = new Font(Program.mainWindow.font2.FontFamily, rec.Height - 2, Program.mainWindow.font2.Style);
            SizeF sz = e.Graphics.MeasureString(toWrite, ft);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle(2, 2, (int)sz.Width + 1, Height), sf);
            toWrite = time2.TotalSeconds.ToTime();
            sz = e.Graphics.MeasureString(toWrite, ft);
            e.Graphics.DrawString(toWrite, ft, Brushes.Black, new Rectangle((int)Width - (int)sz.Width - 2, 2, (int)sz.Width + 1, Height), sf);
        }
    }
}
