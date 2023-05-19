using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.PMAlign.Controls;

namespace CVAiO.Bplus.CogByUser.Controls
{
    public partial class CogToolEdit : Form
    {
        public CogToolEditControlBaseV2 toolEditBase;
        private static bool firstRun = false;
        public CogToolEdit(CogToolEditControlBaseV2 toolEditBase)
        {
            InitializeComponent();
            this.Size = toolEditBase.Size;
            this.Text = "Cognex Vision Pro";
            this.toolEditBase = toolEditBase;
            this.toolEditBase.Dock = DockStyle.Fill;
            this.toolEditBase.Padding = new Padding(1);
            this.lbToolName.Text = "Cognex Vision Pro";
            if (firstRun)
            {
                timer.Tick += Timer_Tick;
                timer.Start();
                frmMessageBox.Show(EMessageIcon.Information, "Loading Cognex library. Please wait ...", 1500);
                firstRun = false;
            }
            else
            {
                pnMain.Controls.Add(this.toolEditBase);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pnMain.Controls.Add(this.toolEditBase);
        }

        private void pnHeader_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            var panelArea = new RectangleF(new PointF(panel.Location.X, panel.Location.Y), panel.Size);
            panelArea.Inflate(-1.0f, -1.0f);
            GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 2.0f), panelArea, 6);
            panelArea.Inflate(-2.0f, -2.0f);
            GraphicsExtension.FillRoundedRectangle(e.Graphics, Brushes.Aquamarine, panelArea, 6);
        }

        private void pnMain_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            var panelArea = new RectangleF(new PointF(panel.Location.X, panel.Location.Y), panel.Size);
            panelArea.Inflate(-1.0f, -1.0f);
            GraphicsExtension.DrawRoundedRectangle(e.Graphics, new Pen(Brushes.CornflowerBlue, 2.0f), panelArea, 6);
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
        }
        private void btnZoom_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                this.btnZoom.BackgroundImage = global::CVAiO.Bplus.CogByUser.Controls.Properties.Resources.Fill;

            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.btnZoom.BackgroundImage = global::CVAiO.Bplus.CogByUser.Controls.Properties.Resources.Fit;
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.toolEditBase.Dispose();
            this.Close();
        }
        private Point lastmpos;
        private bool mdown = false;
        private void pnHeader_MouseDown(object sender, MouseEventArgs e)
        {
            lastmpos = e.Location;
            mdown = true;
            Cursor = Cursors.SizeAll;
        }

        private void pnHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (mdown)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.btnZoom.BackgroundImage = global::CVAiO.Bplus.CogByUser.Controls.Properties.Resources.Fit;
                }
                this.Location = new Point(this.Location.X + e.Location.X - lastmpos.X, this.Location.Y + e.Location.Y - lastmpos.Y);
            }
        }

        private void pnHeader_MouseUp(object sender, MouseEventArgs e)
        {
            mdown = false;
            Cursor = Cursors.Default;
        }

        private void pnHeader_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.SizeAll;
        }

        private void pnHeader_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void pnHeader_DoubleClick(object sender, EventArgs e)
        {
        }

        private bool MouseIsInLeftEdge;
        private bool MouseIsInRightEdge;
        private bool MouseIsInTopEdge;
        private bool MouseIsInBottomEdge;
        private void pnMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) return;
            MouseIsInLeftEdge = Math.Abs(e.Location.X) <= 5;
            MouseIsInRightEdge = Math.Abs(e.Location.X - this.pnMain.Width) <= 5;
            MouseIsInTopEdge = Math.Abs(e.Location.Y) <= 5;
            MouseIsInBottomEdge = Math.Abs(e.Location.Y - this.pnMain.Height) <= 5;
            lastmpos = e.Location;
            mdown = true;
        }

        private void pnMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) return;
            if (Math.Abs(e.Location.X - this.pnMain.Width) <= 5)
            {
                Cursor = Cursors.SizeWE;
            }
            else if (Math.Abs(e.Location.Y - this.pnMain.Height) <= 5)
            {
                Cursor = Cursors.SizeNS;
            }
            else
                Cursor = Cursors.Default;
            if (!mdown) return;
            if (MouseIsInLeftEdge)
            {
                //if (this.ClientSize.Width < 20) return;
                //this.Location = new Point(this.Location.X + e.Location.X - lastmpos.X, this.Location.Y);
                //this.Size = new Size(this.Size.Width - e.Location.X + lastmpos.X, this.Size.Height);
            }
            else if (MouseIsInRightEdge)
            {
                if (this.ClientSize.Width < 20) return;
                this.Size = new Size(this.Size.Width + e.Location.X - lastmpos.X, this.Size.Height);
            }
            else if (MouseIsInTopEdge)
            {
                //if (this.ClientSize.Height < 20) return;
                //this.Location = new Point(this.Location.X, this.Location.Y + e.Location.Y - lastmpos.Y);
                //this.Size = new Size(this.Size.Width, this.Size.Height - e.Location.Y + lastmpos.Y);
            }
            else if (MouseIsInBottomEdge)
            {
                if (this.ClientSize.Height < 20) return;
                this.Size = new Size(this.Size.Width, this.Size.Height + e.Location.Y - lastmpos.Y);
            }
            lastmpos = e.Location;
        }

        private void pnMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) return;
            mdown = false;
            Cursor = Cursors.Default;
        }
    }
}
