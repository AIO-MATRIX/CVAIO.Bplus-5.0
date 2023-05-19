using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CVAiO.Bplus.Simulator.ModbusTCP;

namespace CVAiO.Bplus.Simulator
{
    public enum EState
    {
        WaitReady = 0,
        SetTrigger1On,
        WaitTrigger1Off,
        SetTrigger2On,
        WaitTrigger2Off,
        SetTrigger3On,
        WaitTrigger3Off,
        SetTrigger4On,
        WaitTrigger4Off,
        SetCalcOn,
        WaitCalcOff,
        WaitOKNG,
    }
    public enum EIOType
    {
        None, ModbusTCP
    }
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class MainGUI : INotifyPropertyChanged
    {
        #region Singleton
        private static MainGUI instance;
        public static MainGUI Instance
        {
            get
            {
                if (instance == null) instance = new MainGUI();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        #endregion

        #region Fields
        private int outReset = 0;
        private int inAlive = 64;
        private int inReady = 65;
        [NonSerialized]
        private bool comState = false;
        [NonSerialized]
        private bool outResetState = false;
        [NonSerialized]
        private bool inAliveState = false;
        [NonSerialized]
        private bool inReadyState = false;
        private int schedulerCount = 1;
        private EIOType iOType;
        private IInterface comIO;
        private Dictionary<string, Scheduler> dicScheduler;
        private bool isStart;
        #endregion

        #region Properties
        [Category("1. General"), PropertyOrder(1)]
        public int SchedulerCount
        {
            get => schedulerCount;
            set
            {
                if (schedulerCount == value || value < 1 || value > 4) return;
                schedulerCount = value;
                NotifyPropertyChanged("SchedulerCount");
            }
        }

        [Category("1. General"), PropertyOrder(2)]
        public EIOType IOType 
        { 
            get => iOType;
            set 
            {
                if (iOType == value) return;
                iOType = value;
                if (iOType == EIOType.ModbusTCP) comIO = new InterfaceModbusTCP();
                NotifyPropertyChanged(nameof(IOType));
            }
        }

        [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        [Category("1. General"), PropertyOrder(3)]
        public IInterface ComIO { get { return comIO; } set { if (comIO == value) return; comIO = value;  } }
        
        [Browsable(false)]
        public Dictionary<string, Scheduler> DicScheduler { get { if (dicScheduler == null) dicScheduler = new Dictionary<string, Scheduler>(); return dicScheduler; } set => dicScheduler = value; }
        
        [Browsable(false)]
        public bool IsStart { get => isStart; set => isStart = value; }

        [Category("1. General"), PropertyOrder(4)]
        public int OutReset { get => outReset; set { if (outReset == value || value < 0) return;  outReset = value; } }

        [Category("1. General"), PropertyOrder(5)]
        public int InAlive { get => inAlive; set { if (inAlive == value || value < 0) return; inAlive = value; } }

        [Category("1. General"), PropertyOrder(6)]
        public int InReady { get => inReady; set { if (inReady == value || value < 0) return; inReady = value; } }

        [ReadOnly(true)]
        public bool CommState { get => comState; set { if (comState == value) return; comState = value; NotifyPropertyChanged("CommState"); } }

        [ReadOnly(true)]
        public bool OutResetState { get => outResetState; set { if (outResetState == value) return; outResetState = value; NotifyPropertyChanged("OutResetState"); } }

        [ReadOnly(true)]
        public bool InAliveState { get => inAliveState; set { if (inAliveState == value) return; inAliveState = value; NotifyPropertyChanged("InAliveState"); } }

        [ReadOnly(true)]
        public bool InReadyState { get => inReadyState; set { if (inReadyState == value) return; inReadyState = value; NotifyPropertyChanged("InReadyState"); } }

        
        #endregion


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MainGUI()
        {
             //IOType = EIOType.ModbusTCP;
            dicScheduler = new Dictionary<string, Scheduler>();
            dicScheduler.Add("Scheduler1", new Scheduler());
            dicScheduler.Add("Scheduler2", new Scheduler());
            dicScheduler.Add("Scheduler3", new Scheduler());
            #region IO Default
            dicScheduler["Scheduler1"].IFCommunication.OutStart = 1;
            dicScheduler["Scheduler1"].IFCommunication.OutTrigger1 = 2;
            dicScheduler["Scheduler1"].IFCommunication.OutTrigger2 = 3;
            dicScheduler["Scheduler1"].IFCommunication.OutTrigger3 = 4;
            dicScheduler["Scheduler1"].IFCommunication.OutTrigger4 = 5;
            dicScheduler["Scheduler1"].IFCommunication.OutCalc = 6;
            dicScheduler["Scheduler1"].IFCommunication.OutComp = 7;
            dicScheduler["Scheduler1"].IFCommunication.InReady = 70;
            dicScheduler["Scheduler1"].IFCommunication.InTrigger1Ack = 71;
            dicScheduler["Scheduler1"].IFCommunication.InTrigger2Ack = 72;
            dicScheduler["Scheduler1"].IFCommunication.InTrigger3Ack = 73;
            dicScheduler["Scheduler1"].IFCommunication.InTrigger4Ack = 74;
            dicScheduler["Scheduler1"].IFCommunication.InCalcAck = 75;
            dicScheduler["Scheduler1"].IFCommunication.InOK = 76;
            dicScheduler["Scheduler1"].IFCommunication.InNG = 77;
            dicScheduler["Scheduler1"].IFCommunication.DataAddr = 200;

            dicScheduler["Scheduler2"].IFCommunication.OutStart = 11;
            dicScheduler["Scheduler2"].IFCommunication.OutTrigger1 = 12;
            dicScheduler["Scheduler2"].IFCommunication.OutTrigger2 = 13;
            dicScheduler["Scheduler2"].IFCommunication.OutTrigger3 = 14;
            dicScheduler["Scheduler2"].IFCommunication.OutTrigger4 = 15;
            dicScheduler["Scheduler2"].IFCommunication.OutCalc = 16;
            dicScheduler["Scheduler2"].IFCommunication.OutComp = 17;
            dicScheduler["Scheduler2"].IFCommunication.InReady = 80;
            dicScheduler["Scheduler2"].IFCommunication.InTrigger1Ack = 81;
            dicScheduler["Scheduler2"].IFCommunication.InTrigger2Ack = 82;
            dicScheduler["Scheduler2"].IFCommunication.InTrigger3Ack = 83;
            dicScheduler["Scheduler2"].IFCommunication.InTrigger4Ack = 84;
            dicScheduler["Scheduler2"].IFCommunication.InCalcAck = 85;
            dicScheduler["Scheduler2"].IFCommunication.InOK = 86;
            dicScheduler["Scheduler2"].IFCommunication.InNG = 87;
            dicScheduler["Scheduler2"].IFCommunication.DataAddr = 300;

            dicScheduler["Scheduler3"].IFCommunication.OutStart = 21;
            dicScheduler["Scheduler3"].IFCommunication.OutTrigger1 = 22;
            dicScheduler["Scheduler3"].IFCommunication.OutTrigger2 = 23;
            dicScheduler["Scheduler3"].IFCommunication.OutTrigger3 = 24;
            dicScheduler["Scheduler3"].IFCommunication.OutTrigger4 = 25;
            dicScheduler["Scheduler3"].IFCommunication.OutCalc = 26;
            dicScheduler["Scheduler3"].IFCommunication.OutComp = 27;
            dicScheduler["Scheduler3"].IFCommunication.InReady = 90;
            dicScheduler["Scheduler3"].IFCommunication.InTrigger1Ack = 91;
            dicScheduler["Scheduler3"].IFCommunication.InTrigger2Ack = 92;
            dicScheduler["Scheduler3"].IFCommunication.InTrigger3Ack = 93;
            dicScheduler["Scheduler3"].IFCommunication.InTrigger4Ack = 94;
            dicScheduler["Scheduler3"].IFCommunication.InCalcAck = 95;
            dicScheduler["Scheduler3"].IFCommunication.InOK = 96;
            dicScheduler["Scheduler3"].IFCommunication.InNG = 97;
            dicScheduler["Scheduler3"].IFCommunication.DataAddr = 400;
            #endregion
        }
        public void Start()
        {
            GC.Collect();
            if (ComIO == null)
            {
                return;
            }
            if (!IOInit())
            {
                return;
            }
            IsStart = true;
            InterfaceStart();
            InitAutoThread();
            AutoThreadStart();
            StartSystemResetThread();
        }
        public void Stop()
        {
            if (!IsStart) return;
            IsStart = false;
            AutoThreadStop();
            StopSystemResetThread();
            Thread.Sleep(500);
            InterfaceStop();
        }
        private void InterfaceInit()
        {
            CloseIO();
        }
        private bool IOInit()
        {
            ComIO.InIO.Clear();
            ComIO.OutIO.Clear();
            if (!ComIO.InIO.Keys.Contains(InReady))
                ComIO.InIO.Add(InReady, false);
            if (!ComIO.InIO.Keys.Contains(InAlive))
                ComIO.InIO.Add(InAlive, false);
            if (!ComIO.OutIO.Keys.Contains(OutReset))
                ComIO.OutIO.Add(OutReset, false);

            foreach (var thread in DicScheduler.Values)
            {
                if (thread == null) continue;
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InReady))
                    ComIO.InIO.Add((int)thread.IFCommunication.InReady, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InOK))
                    ComIO.InIO.Add((int)thread.IFCommunication.InOK, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InNG))
                    ComIO.InIO.Add((int)thread.IFCommunication.InNG, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InTrigger1Ack))
                    ComIO.InIO.Add((int)thread.IFCommunication.InTrigger1Ack, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InTrigger2Ack))
                    ComIO.InIO.Add((int)thread.IFCommunication.InTrigger2Ack, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InTrigger3Ack))
                    ComIO.InIO.Add((int)thread.IFCommunication.InTrigger3Ack, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InTrigger4Ack))
                    ComIO.InIO.Add((int)thread.IFCommunication.InTrigger4Ack, false);
                if (!ComIO.InIO.Keys.Contains((int)thread.IFCommunication.InCalcAck))
                    ComIO.InIO.Add((int)thread.IFCommunication.InCalcAck, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutStart))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutStart, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutComp))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutComp, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger1))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutTrigger1, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger2))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutTrigger2, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger3))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutTrigger3, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutTrigger4))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutTrigger4, false);
                if (!ComIO.OutIO.Keys.Contains((int)thread.IFCommunication.OutCalc))
                    ComIO.OutIO.Add((int)thread.IFCommunication.OutCalc, false);
            }
            return true;
        }
        private void InterfaceStart()
        {
            if (!ComIO.OpenDevice())
            {
                return;
            }
        }
        private void InterfaceStop()
        {
            CloseIO();
        }
        private void CloseIO()
        {
            if (ComIO == null) return;
            ComIO.CloseDevice();
        }
  
        public void InitAutoThread()
        {
            try
            {
                foreach (var thread in DicScheduler.Values)
                {
                    if (thread == null) continue;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void AutoThreadStart()
        {
            foreach (Scheduler thread in DicScheduler.Values)
            {
                thread.ThreadStart();
                thread.ThreadFlag = true;
            }

        }
        public void AutoThreadStop()
        {
            foreach (Scheduler thread in DicScheduler.Values)
            {
                thread.ThreadFlag = false;
                thread.ThreadStop();
            }
        }
        [NonSerialized]
        private Thread mSystemResetThread = null;
        [NonSerialized]
        private bool mThreadFlag = false;
        private void StartSystemResetThread()
        {
            mThreadFlag = true;
            mSystemResetThread = new Thread(new ThreadStart(SystemReset));
            mSystemResetThread.IsBackground = true;
            mSystemResetThread.Start();
        }
        private void StopSystemResetThread()
        {
            mThreadFlag = false;
            if (mSystemResetThread != null)
            {
            }
        }
        private void SystemReset()
        {
            while (mThreadFlag)
            {
                try
                {
                    if (!InReadyState)
                        ComIO.SetOutValue(OutReset, false);
                    Thread.Sleep(100);
                    if (ComIO != null)
                    {
                        CommState = ComIO.IsConnected;
                        InReadyState = ComIO.GetInValue(inReady);
                        OutResetState = ComIO.GetOutValue(outReset);
                        InAliveState = ComIO.GetInValue(inAlive);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
    }
}
