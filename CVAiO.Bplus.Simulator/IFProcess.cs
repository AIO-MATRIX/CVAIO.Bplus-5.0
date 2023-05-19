using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Simulator
{
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class IFProcess : INotifyPropertyChanged
    {
        #region Fields

        private int inReady = 19;
        private int inOK = 25;
        private int inNG = 26;
        private int inTrigger1Ack = 20;
        private int inTrigger2Ack = 21;
        private int inTrigger3Ack = 22;
        private int inTrigger4Ack = 23;
        private int inCalcAck = 24;

        private int outStart = 1;
        private int outComp = 7;
        private int outTrigger1 = 2;
        private int outTrigger2 = 3;
        private int outTrigger3 = 4;
        private int outTrigger4 = 5;
        private int outCalc = 6;

        [NonSerialized ]
        private bool stateComIO = false;
        [NonSerialized]
        private bool stateInReady = false;
        [NonSerialized]
        private bool stateInOK = false;
        [NonSerialized]
        private bool stateInNG = false;
        [NonSerialized]
        private bool stateInTrigger1Ack = false;
        [NonSerialized]
        private bool stateInTrigger2Ack = false;
        [NonSerialized]
        private bool stateInTrigger3Ack = false;
        [NonSerialized]
        private bool stateInTrigger4Ack = false;
        [NonSerialized]
        private bool stateInCalcAck = false;

        [NonSerialized]
        private bool stateOutStart = false;
        [NonSerialized]
        private bool stateOutComp = false;
        [NonSerialized]
        private bool stateOutTrigger1 = false;
        [NonSerialized]
        private bool stateOutTrigger2 = false;
        [NonSerialized]
        private bool stateOutTrigger3 = false;
        [NonSerialized]
        private bool stateOutTrigger4 = false;
        [NonSerialized]
        private bool stateOutCalc = false;

        private int dataAddr = 100;

        #endregion

        #region Properties
        [Browsable(false)]
        public IInterface ComIO { get => MainGUI.Instance.ComIO; set { if (MainGUI.Instance.ComIO == value) return; MainGUI.Instance.ComIO = value; } }


        [Category("2. IO"), PropertyOrder(10)]
        public int OutStart { get => outStart; set => outStart = value; }

        [Category("2. IO"), PropertyOrder(11)]
        public int OutTrigger1 { get => outTrigger1; set => outTrigger1 = value; }

        [Category("2. IO"), PropertyOrder(12)]
        public int OutTrigger2 { get => outTrigger2; set => outTrigger2 = value; }

        [Category("2. IO"), PropertyOrder(13)]
        public int OutTrigger3 { get => outTrigger3; set => outTrigger3 = value; }

        [Category("2. IO"), PropertyOrder(14)]
        public int OutTrigger4 { get => outTrigger4; set => outTrigger4 = value; }

        [Category("2. IO"), PropertyOrder(15)]
        public int OutCalc { get => outCalc; set => outCalc = value; }

        [Category("2. IO"), PropertyOrder(16)]
        public int OutComp { get => outComp; set => outComp = value; }

        [Category("2. IO"), PropertyOrder(17)]
        public int DataAddr { get => dataAddr; set { if (dataAddr == value || value < 0) return; dataAddr = value; NotifyPropertyChanged("DataAddr"); } }


        [Category("2. IO"), PropertyOrder(18)]
        public int InReady { get => inReady; set => inReady = value; }
       
        [Category("2. IO"), PropertyOrder(19)]
        public int InTrigger1Ack { get => inTrigger1Ack; set => inTrigger1Ack = value; }

        [Category("2. IO"), PropertyOrder(20)]
        public int InTrigger2Ack { get => inTrigger2Ack; set => inTrigger2Ack = value; }

        [Category("2. IO"), PropertyOrder(21)]
        public int InTrigger3Ack { get => inTrigger3Ack; set => inTrigger3Ack = value; }

        [Category("2. IO"), PropertyOrder(22)]
        public int InTrigger4Ack { get => inTrigger4Ack; set => inTrigger4Ack = value; }

        [Category("2. IO"), PropertyOrder(23)]
        public int InCalcAck { get => inCalcAck; set => inCalcAck = value; }

        [Category("2. IO"), PropertyOrder(24)]
        public int InOK { get => inOK; set => inOK = value; }

        [Category("2. IO"), PropertyOrder(25)]
        public int InNG { get => inNG; set => inNG = value; }

        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateComIO { get => stateComIO; set { if (stateComIO == value) return; stateComIO = value; NotifyPropertyChanged("StateComIO"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInReady { get => stateInReady; set { if (stateInReady == value) return; stateInReady = value; NotifyPropertyChanged("StateInReady"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInOK { get => stateInOK; set { if (stateInOK == value) return; stateInOK = value; NotifyPropertyChanged("StateInOK"); } }
       
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInNG { get => stateInNG; set { if (stateInNG == value) return; stateInNG = value; NotifyPropertyChanged("StateInNG"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInTrigger1Ack { get => stateInTrigger1Ack; set { if (stateInTrigger1Ack == value) return; stateInTrigger1Ack = value; NotifyPropertyChanged("StateInTrigger1Ack"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInTrigger2Ack { get => stateInTrigger2Ack; set { if (stateInTrigger2Ack == value) return; stateInTrigger2Ack = value; NotifyPropertyChanged("StateInTrigger2Ack"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInTrigger3Ack { get => stateInTrigger3Ack; set { if (stateInTrigger3Ack == value) return; stateInTrigger3Ack = value; NotifyPropertyChanged("StateInTrigger3Ack"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInTrigger4Ack { get => stateInTrigger4Ack; set { if (stateInTrigger4Ack == value) return; stateInTrigger4Ack = value; NotifyPropertyChanged("StateInTrigger4Ack"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateInCalcAck { get => stateInCalcAck; set { if (stateInCalcAck == value) return; stateInCalcAck = value; NotifyPropertyChanged("StateInCalcAck"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutStart { get => stateOutStart; set { if (stateOutStart == value) return; stateOutStart = value; NotifyPropertyChanged("StateOutStart"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutComp { get => stateOutComp; set { if (stateOutComp == value) return; stateOutComp = value; NotifyPropertyChanged("StateOutComp"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutTrigger1 { get => stateOutTrigger1; set { if (stateOutTrigger1 == value) return; stateOutTrigger1 = value; NotifyPropertyChanged("StateOutTrigger1"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutTrigger2 { get => stateOutTrigger2; set { if (stateOutTrigger2 == value) return; stateOutTrigger2 = value; NotifyPropertyChanged("StateOutTrigger2"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutTrigger3 { get => stateOutTrigger3; set { if (stateOutTrigger3 == value) return; stateOutTrigger3 = value; NotifyPropertyChanged("StateOutTrigger3"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutTrigger4 { get => stateOutTrigger4; set { if (stateOutTrigger4 == value) return; stateOutTrigger4 = value; NotifyPropertyChanged("StateOutTrigger4"); } }
        
        [ReadOnly(true)]
        [Category("2. IO"), PropertyOrder(26)]
        public bool StateOutCalc { get => stateOutCalc; set { if (stateOutCalc == value) return; stateOutCalc = value; NotifyPropertyChanged("StateOutCalc"); } }

        
        #endregion

        public IFProcess()
        {
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
