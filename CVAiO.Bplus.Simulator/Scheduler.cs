using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Simulator
{
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class Scheduler : INotifyPropertyChanged
    {
        #region Fields
        [NonSerialized]
        protected bool threadFlag = false;
        [NonSerialized]
        protected Thread scheduleThread = null;
        private IFProcess iFCommunication;
        private bool mode;
        private int triggerNum = 1;
        private EState schedulerState;
        #endregion

        #region Properties
        [Browsable(false)]
        public IInterface ComIO { get => MainGUI.Instance.ComIO; set { if (MainGUI.Instance.ComIO == value) return; MainGUI.Instance.ComIO = value; } }

        [Browsable(false)]
        public bool ThreadFlag { get => threadFlag; set => threadFlag = value; }

        [Category("1. Scheduler"), PropertyOrder(1)]
        public bool Mode { get => mode; set => mode = value; }

        [Category("1. Scheduler"), PropertyOrder(2)]
        public int TriggerNum { get => triggerNum; set { if (triggerNum == value || value < 1 || value > 4) return; triggerNum = value; NotifyPropertyChanged(nameof(TriggerNum)); } }

        [Category("1. Scheduler"), PropertyOrder(3)]
        [TypeConverter(typeof(PropertySorter))]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFProcess IFCommunication { get { if (iFCommunication == null) iFCommunication = new IFProcess(); return iFCommunication; } set { iFCommunication = value; NotifyPropertyChanged(nameof(IFCommunication)); } }

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
        public Scheduler()
        {
            Mode = false;
        }
        public void ThreadStart()
        {
            Mode = false;
            if (threadFlag)
            {
                ThreadStop();
                Thread.Sleep(100);
            }
            threadFlag = true;
            schedulerState = EState.WaitReady;
            scheduleThread = new Thread(new ThreadStart(ThreadFunction));
            scheduleThread.Start();
        }
        public void ThreadStop()
        {
            ThreadFlag = false;
            if (scheduleThread != null)
            {
                Thread.Sleep(50);
            }
        }
        public void ThreadFunction()
        {
            schedulerState = EState.WaitReady;
            while (threadFlag)
            {
                UpdateIO();
                Thread.Sleep(10);
                if (!Mode) continue;
                if (!iFCommunication.ComIO.IsConnected) continue;
                switch (schedulerState)
                {
                    case EState.WaitReady:
                        if (iFCommunication.StateInReady)
                        {
                            Thread.Sleep(10);
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutComp, false);
                            Thread.Sleep(2000);
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutStart, true);
                            schedulerState = EState.SetTrigger1On;
                        }
                        break;

                    case EState.SetTrigger1On:
                        iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger1, true);
                        schedulerState = EState.WaitTrigger1Off;
                        break;
                    case EState.WaitTrigger1Off:
                        if (iFCommunication.StateInTrigger1Ack)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutStart, false);
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger1, false);
                            if (triggerNum == 1) schedulerState = EState.SetCalcOn;
                            else
                                schedulerState = EState.SetTrigger2On;
                            Thread.Sleep(10);
                        }
                        break;

                    case EState.SetTrigger2On:
                        iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger2, true);
                        schedulerState = EState.WaitTrigger2Off;
                        break;
                    case EState.WaitTrigger2Off:
                        if (iFCommunication.StateInTrigger2Ack)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger2, false);
                            if (triggerNum == 2) schedulerState = EState.SetCalcOn;
                            else
                                schedulerState = EState.SetTrigger3On;
                            Thread.Sleep(10);
                        }
                        break;

                    case EState.SetTrigger3On:
                        iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger3, true);
                        schedulerState = EState.WaitTrigger3Off;
                        break;
                    case EState.WaitTrigger3Off:
                        if (iFCommunication.StateInTrigger3Ack)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger3, false);
                            if (triggerNum == 3) schedulerState = EState.SetCalcOn;
                            else
                                schedulerState = EState.SetTrigger4On;
                            Thread.Sleep(10);
                        }
                        break;

                    case EState.SetTrigger4On:
                        iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger4, true);
                        schedulerState = EState.WaitTrigger4Off;
                        break;
                    case EState.WaitTrigger4Off:
                        if (iFCommunication.StateInTrigger4Ack)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutTrigger4, false);
                            schedulerState = EState.SetCalcOn;
                            Thread.Sleep(10);
                        }
                        break;

                    case EState.SetCalcOn:
                        iFCommunication.ComIO.SetOutValue(iFCommunication.OutCalc, true);
                        schedulerState = EState.WaitCalcOff;
                        break;
                    case EState.WaitCalcOff:
                        if (iFCommunication.StateInCalcAck)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutCalc, false);
                            schedulerState = EState.WaitOKNG;
                        }
                        break;
                    case EState.WaitOKNG:
                        if (iFCommunication.StateInOK || iFCommunication.StateInNG)
                        {
                            iFCommunication.ComIO.SetOutValue(iFCommunication.OutComp, true);
                            schedulerState = EState.WaitReady;
                        }
                        break;
                }
            }

            GC.Collect();
        }
        private void UpdateIO()
        {
            if (iFCommunication.ComIO != null)
            {
                iFCommunication.StateComIO = iFCommunication.ComIO.IsConnected;
                iFCommunication.StateInReady = iFCommunication.ComIO.GetInValue((int)iFCommunication.InReady);
                iFCommunication.StateInOK = iFCommunication.ComIO.GetInValue((int)iFCommunication.InOK);
                iFCommunication.StateInNG = iFCommunication.ComIO.GetInValue((int)iFCommunication.InNG);
                iFCommunication.StateInTrigger1Ack = iFCommunication.ComIO.GetInValue((int)iFCommunication.InTrigger1Ack);
                iFCommunication.StateInTrigger2Ack = iFCommunication.ComIO.GetInValue((int)iFCommunication.InTrigger2Ack);
                iFCommunication.StateInTrigger3Ack = iFCommunication.ComIO.GetInValue((int)iFCommunication.InTrigger3Ack);
                iFCommunication.StateInTrigger4Ack = iFCommunication.ComIO.GetInValue((int)iFCommunication.InTrigger4Ack);
                iFCommunication.StateInCalcAck = iFCommunication.ComIO.GetInValue((int)iFCommunication.InCalcAck);

                iFCommunication.StateOutStart = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutStart);
                iFCommunication.StateOutComp = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutComp);
                iFCommunication.StateOutTrigger1 = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutTrigger1);
                iFCommunication.StateOutTrigger2 = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutTrigger2);
                iFCommunication.StateOutTrigger3 = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutTrigger3);
                iFCommunication.StateOutTrigger4 = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutTrigger4);
                iFCommunication.StateOutCalc = iFCommunication.ComIO.GetOutValue((int)iFCommunication.OutCalc);


                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutStart, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutComp, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutTrigger1, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutTrigger2, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutTrigger3, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutTrigger4, true);
                //iFCommunication.ComIO.SetOutValue((int)iFCommunication.OutCalc, true);

            }
        }
    }
}
