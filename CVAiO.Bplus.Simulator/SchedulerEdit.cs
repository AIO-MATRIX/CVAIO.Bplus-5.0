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
    public partial class SchedulerEdit : UserControl
    {
        private Scheduler subject;
        private int triggerCount;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scheduler Subject
        {
            get { return subject; }
            set
            { 
                subject = value;
                source.DataSource = Subject;
                source.ResetBindings(true);
            }
        }

        public int TriggerCount
        { 
            get => triggerCount;
            set
            {
                if (triggerCount == value || value < 1 || value > 4) return;
                triggerCount = value;
                if (triggerCount == 1)
                {
                    lbTrigger1.Visible = true; lbTrigger1Ack.Visible = true;
                    lbTrigger2.Visible = false; lbTrigger2Ack.Visible = false;
                    lbTrigger3.Visible = false; lbTrigger3Ack.Visible = false;
                    lbTrigger4.Visible = false; lbTrigger4Ack.Visible = false;
                }    
                else if (triggerCount == 2)
                {
                    lbTrigger1.Visible = true; lbTrigger1Ack.Visible = true;
                    lbTrigger2.Visible = true; lbTrigger2Ack.Visible = true;
                    lbTrigger3.Visible = false; lbTrigger3Ack.Visible = false;
                    lbTrigger4.Visible = false; lbTrigger4Ack.Visible = false;
                }
                else if (triggerCount == 3)
                {
                    lbTrigger1.Visible = true; lbTrigger1Ack.Visible = true;
                    lbTrigger2.Visible = true; lbTrigger2Ack.Visible = true;
                    lbTrigger3.Visible = true; lbTrigger3Ack.Visible = true;
                    lbTrigger4.Visible = false; lbTrigger4Ack.Visible = false;
                }
                else if (triggerCount == 4)
                {
                    lbTrigger1.Visible = true; lbTrigger1Ack.Visible = true;
                    lbTrigger2.Visible = true; lbTrigger2Ack.Visible = true;
                    lbTrigger3.Visible = true; lbTrigger3Ack.Visible = true;
                    lbTrigger4.Visible = true; lbTrigger4Ack.Visible = true;
                }
            }
        }

        private BindingSource source;

        public SchedulerEdit()
        {
            InitializeComponent();
            InitBinding();
            
        }
        void InitBinding()
        {
            source = new BindingSource(typeof(Scheduler), null);
            lbStart.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutStart", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger1.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutTrigger1", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger2.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutTrigger2", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger3.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutTrigger3", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger4.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutTrigger4", true, DataSourceUpdateMode.OnPropertyChanged);
            lbCalc.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutCalc", true, DataSourceUpdateMode.OnPropertyChanged);
            lbComp.DataBindings.Add("StateStatus", source, "IFCommunication.StateOutComp", true, DataSourceUpdateMode.OnPropertyChanged);

            lbReady.DataBindings.Add("StateStatus", source, "IFCommunication.StateInReady", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger1Ack.DataBindings.Add("StateStatus", source, "IFCommunication.StateInTrigger1Ack", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger2Ack.DataBindings.Add("StateStatus", source, "IFCommunication.StateInTrigger2Ack", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger3Ack.DataBindings.Add("StateStatus", source, "IFCommunication.StateInTrigger3Ack", true, DataSourceUpdateMode.OnPropertyChanged);
            lbTrigger4Ack.DataBindings.Add("StateStatus", source, "IFCommunication.StateInTrigger4Ack", true, DataSourceUpdateMode.OnPropertyChanged);
            lbCalcAck.DataBindings.Add("StateStatus", source, "IFCommunication.StateInCalcAck", true, DataSourceUpdateMode.OnPropertyChanged);
            lbOK.DataBindings.Add("StateStatus", source, "IFCommunication.StateInOK", true, DataSourceUpdateMode.OnPropertyChanged);
            lbNG.DataBindings.Add("StateStatus", source, "IFCommunication.StateInNG", true, DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("TriggerCount", source, "TriggerNum", true, DataSourceUpdateMode.OnPropertyChanged);
        }
        private void lbStart_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutStart, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutStart));
        }

        private void lbComp_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutComp, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutComp));
        }

        private void lbTrigger1_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutTrigger1, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutTrigger1));
        }

        private void lbTrigger2_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutTrigger2, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutTrigger2));
        }

        private void lbTrigger3_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutTrigger3, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutTrigger3));
        }

        private void lbTrigger4_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutTrigger4, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutTrigger4));
        }

        private void lbCalc_Click(object sender, EventArgs e)
        {
            if (Subject.Mode) return;
            Subject.ComIO.SetOutValue((int)Subject.IFCommunication.OutCalc, !Subject.ComIO.GetOutValue(Subject.IFCommunication.OutCalc));
        }

        private void lbMode_Click(object sender, EventArgs e)
        {
            Subject.Mode = !Subject.Mode;
            lbCurrentMode.Text = Subject.Mode ? "Auto" : "Manual";
        }

        private void pnMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
