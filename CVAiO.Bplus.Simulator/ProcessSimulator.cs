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
    public partial class ProcessSimulator : UserControl
    {
        IFProcess fFProcess;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IInterface ComIO { get => MainGUI.Instance.ComIO; set { if (MainGUI.Instance.ComIO == value) return; MainGUI.Instance.ComIO = value; } }
        [Category("Interface Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFProcess IFProcess { get { if (fFProcess == null) fFProcess = new IFProcess(); return fFProcess; } set => fFProcess = value; }
        private bool isStart = false;

        private BindingSource source;
        public ProcessSimulator()
        {
            InitializeComponent();
            InitBinding();
            InitSystem();
            MainGUI.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SchedulerCount")
                numSchedulerCount.Value = MainGUI.Instance.SchedulerCount;
            else if (e.PropertyName == "IOType")
                PropertyGrid.Refresh();
        }

        void InitBinding()
        {
            source = new BindingSource(typeof(MainGUI), null);
            lbReset.DataBindings.Add("StateStatus", source, "OutResetState", true, DataSourceUpdateMode.OnPropertyChanged);
            lbAlive.DataBindings.Add("StateStatus", source, "InAliveState", true, DataSourceUpdateMode.OnPropertyChanged);
            lbReady.DataBindings.Add("StateStatus", source, "InReadyState", true, DataSourceUpdateMode.OnPropertyChanged);

        }
        private void InitSystem()
        {
            try
            {
                MainGUI.Instance = (MainGUI)new Serializer().Deserializing(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tbName.Text));
            }
            catch { }
            numSchedulerCount.Value = MainGUI.Instance.SchedulerCount;
            numSchedulerCount_ValueChanged(this, null);
            source.DataSource = MainGUI.Instance;
            source.ResetBindings(true);
            schedulerEdit1.Subject = MainGUI.Instance.DicScheduler["Scheduler1"];
            schedulerEdit2.Subject = MainGUI.Instance.DicScheduler["Scheduler2"];
            schedulerEdit3.Subject = MainGUI.Instance.DicScheduler["Scheduler3"];
        }

        private void btnProgReset_Click(object sender, EventArgs e)
        {
            if (!isStart) return;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (MainGUI.Instance.IsStart) return;
            MainGUI.Instance.Start();
            numSchedulerCount.Enabled = false;
            PropertyGrid.Enabled = false;
            tableLayoutPanel3.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!MainGUI.Instance.IsStart) return;
            MainGUI.Instance.Stop();
            numSchedulerCount.Enabled = true;
            PropertyGrid.Enabled = true;
            tableLayoutPanel3.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void btnMainGUI_Click(object sender, EventArgs e)
        {
            this.PropertyGrid.SelectedObject = MainGUI.Instance;
            this.PropertyGrid.ExpandAllGridItems();
        }
        private void btnScheduler1_Click(object sender, EventArgs e)
        {
            if (MainGUI.Instance.SchedulerCount < 1) return;
            this.PropertyGrid.SelectedObject = MainGUI.Instance.DicScheduler["Scheduler1"];
            this.PropertyGrid.ExpandAllGridItems();
        }

        private void btnScheduler2_Click(object sender, EventArgs e)
        {
            if (MainGUI.Instance.SchedulerCount < 2) return;
            this.PropertyGrid.SelectedObject = MainGUI.Instance.DicScheduler["Scheduler2"];
            this.PropertyGrid.ExpandAllGridItems();
        }

        private void btnScheduler3_Click(object sender, EventArgs e)
        {
            if (MainGUI.Instance.SchedulerCount < 3) return;
            this.PropertyGrid.SelectedObject = MainGUI.Instance.DicScheduler["Scheduler3"];
            this.PropertyGrid.ExpandAllGridItems();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.InitialDirectory = "";
                fd.Filter = "Process files (*.sml)|*.sml|All files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.Multiselect = false;
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                tbName.Text = fd.FileName;
                InitSystem();
                MessageBox.Show("Load Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Fail: " + ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                fd.FileName = "";
                fd.Filter = "Simulation files (*.sml)|*.sml|All files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.DefaultExt = "*.sml";
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                tbName.Text = fd.FileName;
                bool success = new Serializer().Serializing(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tbName.Text), MainGUI.Instance);
                if (success)
                    MessageBox.Show("Save Successful");
                else
                    MessageBox.Show("Save Fail");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Fail: " + ex.ToString());
            }
        }

        

        private void numSchedulerCount_ValueChanged(object sender, EventArgs e)
        {
            MainGUI.Instance.SchedulerCount = (int)numSchedulerCount.Value;
            PropertyGrid.Refresh();
            if (MainGUI.Instance.SchedulerCount == 1)
            {
                schedulerEdit1.Visible = true;
                schedulerEdit2.Visible = false;
                schedulerEdit3.Visible = false;
            }   
            else if (MainGUI.Instance.SchedulerCount == 2)
            {
                schedulerEdit1.Visible = true;
                schedulerEdit2.Visible = true;
                schedulerEdit3.Visible = false;
            }
            else if (MainGUI.Instance.SchedulerCount == 3)
            {
                schedulerEdit1.Visible = true;
                schedulerEdit2.Visible = true;
                schedulerEdit3.Visible = true;
            }
        }

        private void lbReset_Click(object sender, EventArgs e)
        {
            if (!MainGUI.Instance.IsStart) return;
            if (MainGUI.Instance.ComIO == null) return;
            MainGUI.Instance.ComIO.SetOutValue(MainGUI.Instance.OutReset, true);
            foreach (var thread in MainGUI.Instance.DicScheduler.Values)
            {
                if (thread == null) continue;
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutStart))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutStart, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutComp))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutComp, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger1))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutTrigger1, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger2))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutTrigger2, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger3))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutTrigger3, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger4))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutTrigger4, false);
                if (ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutCalc))
                    MainGUI.Instance.ComIO.SetOutValue((int)thread.IFCommunication.OutCalc, false);
            }
            System.Threading.Thread.Sleep(500);
            MainGUI.Instance.AutoThreadStop();
            System.Threading.Thread.Sleep(500);
            MainGUI.Instance.AutoThreadStart();
            MainGUI.Instance.ComIO.SetOutValue(MainGUI.Instance.OutReset, false);
        }

    }
}
