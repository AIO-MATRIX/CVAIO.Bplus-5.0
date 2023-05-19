using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus.Simulator
{
    public class ButtonExtension : Button
    {
        public ButtonExtension()
        {
            this.TabStop = false;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var buttonArea = new RectangleF(new PointF(0, 0), this.Size);
            buttonArea.Inflate(-1.0f, -1.0f);
            GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 2.0f), buttonArea, 6);
        }
    }
}
