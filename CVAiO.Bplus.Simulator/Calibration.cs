using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus.Simulator
{
    public partial class Calibration : UserControl
    {
        IFCalibration interfaceCalibration = new IFCalibration();

        [Category("Interface Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFCalibration InterfaceCalibration { get => interfaceCalibration; set => interfaceCalibration = value; }
        private bool isStart = false;

        public Calibration()
        {
            InitializeComponent();
            IOInit();
            this.PropertyGrid.SelectedObject = InterfaceCalibration;
            this.PropertyGrid.ExpandAllGridItems();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            
        }
        private void Timer_Tick(System.Object sender, System.EventArgs e)
        {
            if (InterfaceCalibration.ComIO.IsConnected)
                btnOpen.BackColor = Color.LightGreen;
            else
                btnOpen.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetOutValue(InterfaceCalibration.OutReady))
                btnOutReady.BackColor = Color.LightGreen;
            else
                btnOutReady.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetOutValue(InterfaceCalibration.OutStartAck))
                btnOutStartAck.BackColor = Color.LightGreen;
            else
                btnOutStartAck.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetOutValue(InterfaceCalibration.OutMoveAck))
                btnOutMoveAck.BackColor = Color.LightGreen;
            else
                btnOutMoveAck.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetOutValue(InterfaceCalibration.OutDone))
                btnOutDone.BackColor = Color.LightGreen;
            else
                btnOutDone.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetOutValue(InterfaceCalibration.OutEndAck))
                btnOutEndAck.BackColor = Color.LightGreen;
            else
                btnOutEndAck.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetInValue(InterfaceCalibration.InReady))
                btnInReady.BackColor = Color.LightGreen;
            else
                btnInReady.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetInValue(InterfaceCalibration.InStart))
                btnInStart.BackColor = Color.LightGreen;
            else
                btnInStart.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetInValue(InterfaceCalibration.InMove))
                btnInMove.BackColor = Color.LightGreen;
            else
                btnInMove.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetInValue(InterfaceCalibration.InDoneAck))
                btnInDoneAck.BackColor = Color.LightGreen;
            else
                btnInDoneAck.BackColor = Color.Red;

            if (InterfaceCalibration.ComIO.GetInValue(InterfaceCalibration.InEnd))
                btnInEnd.BackColor = Color.LightGreen;
            else
                btnInEnd.BackColor = Color.Red;
            int temp = 0;
            InterfaceCalibration.ComIO.ReadValue(InterfaceCalibration.DataAddr, out temp);
            tbX.Text = temp.ToString();
            InterfaceCalibration.ComIO.ReadValue(InterfaceCalibration.DataAddr + 2, out temp);
            tbY.Text = temp.ToString();
            InterfaceCalibration.ComIO.ReadValue(InterfaceCalibration.DataAddr + 4, out temp);
            tbT.Text = temp.ToString();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            InterfaceStart();
            timer.Start();
            isStart = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            isStart = false;
            timer.Stop();
            InterfaceStop();
        }

        private bool IOInit()
        {
            InterfaceCalibration.ComIO.InIO.Clear();
            InterfaceCalibration.ComIO.OutIO.Clear();
            InterfaceCalibration.ComIO.InIO.Add((int)InterfaceCalibration.InReady, false);
            InterfaceCalibration.ComIO.InIO.Add((int)InterfaceCalibration.InStart, false);
            InterfaceCalibration.ComIO.InIO.Add((int)InterfaceCalibration.InMove, false);
            InterfaceCalibration.ComIO.InIO.Add((int)InterfaceCalibration.InDoneAck, false);
            InterfaceCalibration.ComIO.InIO.Add((int)InterfaceCalibration.InEnd, false);
            InterfaceCalibration.ComIO.OutIO.Add((int)InterfaceCalibration.OutReady, false);
            InterfaceCalibration.ComIO.OutIO.Add((int)InterfaceCalibration.OutStartAck, false);
            InterfaceCalibration.ComIO.OutIO.Add((int)InterfaceCalibration.OutMoveAck, false);
            InterfaceCalibration.ComIO.OutIO.Add((int)InterfaceCalibration.OutDone, false);
            InterfaceCalibration.ComIO.OutIO.Add((int)InterfaceCalibration.OutEndAck, false);

            return true;
        }
        private void InterfaceStart()
        {
            if (!InterfaceCalibration.ComIO.OpenDevice())
                MessageBox.Show(string.Format("Open IO Fail"));
        }
        private void InterfaceStop()
        {
            InterfaceCalibration.CloseIO();
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutReady, true);
        }

        private void btnStartAck_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutStartAck, true);
        }

        private void btnMoveAck_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutMoveAck, true);
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutDone, false);

        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutMoveAck, false);
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutDone, true);
        }

        private void btnEndAck_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutEndAck, true);
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutMoveAck, false);
            InterfaceCalibration.ComIO.SetOutValue((int)InterfaceCalibration.OutDone, false);
        }

        private void frmCalibrationSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            InterfaceStop();
        }

        private void btnInReady_Click(object sender, EventArgs e)
        {

        }
    }
}
