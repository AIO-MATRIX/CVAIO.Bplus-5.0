using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus.Simulator
{
    public class LabelStatus : Label
    {
        #region Fields
        private bool stateStatus;
        private int sizeStatus = 6;
        private bool blinkStatus = false;
        private bool visibleStatus = true;
        private int blinkTime = 1000;
        private Timer timer = new Timer();
        private bool blink = true;
        #endregion

        #region Properties
        public bool StateStatus { get => stateStatus; set { stateStatus = value; Invalidate(); } }
        public int SizeStatus { get => sizeStatus; set { if (value < 1 || sizeStatus == value) return; sizeStatus = value; Invalidate(); } }
        public bool BlinkStatus
        {
            get => blinkStatus;
            set
            {
                if (blinkStatus == value) return;
                blinkStatus = value;
                if (blinkStatus == true)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                        timer = null;
                    }
                    timer = new Timer();
                    timer.Interval = blinkTime;
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
            }
        }
        public int BlinkTime
        {
            get => blinkTime;
            set
            {
                if (value < 1 || blinkTime == value) return;
                blinkTime = value;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                timer = new Timer();
                timer.Interval = blinkTime;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        public bool VisibleStatus
        {
            get => visibleStatus;
            set
            {
                visibleStatus = value;
                blink = value;
                Invalidate();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            blink = !blink;
            Invalidate();
        }

        #endregion

        public LabelStatus()
        {
            this.TextAlign = ContentAlignment.MiddleCenter;
            timer = new Timer();
            timer.Interval = BlinkTime;
            timer.Tick += Timer_Tick;
            if (blinkStatus) timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var labelArea = new RectangleF(new PointF(0, 0), this.Size);
            labelArea.Inflate(-1.0f, -1.0f);
            var statusArea = new RectangleF(new PointF(8, this.Size.Height / 2 - sizeStatus), new Size(2 * sizeStatus, 2 * sizeStatus));
            if (blink)
            {
                if (StateStatus)
                {
                    GraphicsExtension.FillRoundedRectangle(e.Graphics, Brushes.GreenYellow, statusArea, 6);
                    GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 1.0f), statusArea, 6);
                }
                else
                {
                    GraphicsExtension.FillRoundedRectangle(e.Graphics, Brushes.OrangeRed, statusArea, 6);
                    GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 1.0f), statusArea, 6);
                }

            }
            GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 2.0f), labelArea, 6);

        }
    }
}
