using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Simulator
{
    public class IFCalibration : INotifyPropertyChanged
    {
        #region Fields
        private IInterface comIO = new ModbusTCP.InterfaceModbusTCP();
        private int inReady = 100;
        private int inStart = 101;
        private int inMove = 102;
        private int inDoneAck = 103;
        private int inEnd = 104;
        private int outReady = 50;
        private int outStartAck = 51;
        private int outMoveAck = 52;
        private int outDone = 53;
        private int outEndAck = 54;

        private bool stateComIO = false;
        private bool stateInReady = false;
        private bool stateInStart = false;
        private bool stateInMove = false;
        private bool stateInDoneAck = false;
        private bool stateInStop = false;
        private bool stateOutReady = false;
        private bool stateOutStartAck = false;
        private bool stateOutMoveAck = false;
        private bool stateOutDone = false;
        private bool stateOutEndAck = false;
        private int dataAddr = 200;
        // P Ready O -> V Ready O -> V Start O -> P Start Ack O -> V Start F -> V Start F
        // -> Write Data
        // -> V Move O -> P Move Ack O -> V Move F -> P Move Ack F
        // -> Servo Move
        // -> P Done O -> V Done Ack O -> P Done F -> Done Ack F -> Write Data
        // -> V Stop O -> P Stop Ack O -> V Stop F -> P Stop Ack F

        #endregion

        #region Properties
        [ReadOnly(true)]
        [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public IInterface ComIO { get => comIO; set { if (comIO == value) return; comIO = value; } }

        [ReadOnly(true)]
        public bool StateComIO { get => stateComIO; set { if (stateComIO == value) return; stateComIO = value; NotifyPropertyChanged("StateComIO"); } }


        public int InReady { get => inReady; set { if (inReady == value || value < 0) return; inReady = value; NotifyPropertyChanged("InReady"); } }
        [ReadOnly(true)]
        public bool StateInReady { get => stateInReady; set { if (stateInReady == value) return; stateInReady = value; NotifyPropertyChanged("StateInReady"); } }

        public int InStart { get => inStart; set { if (inStart == value || value < 0) return; inStart = value; NotifyPropertyChanged("InStart"); } }

        [ReadOnly(true)]
        public bool StateInStart { get => stateInStart; set { if (stateInStart == value) return; stateInStart = value; NotifyPropertyChanged("StateInStart"); } }

        public int InMove { get => inMove; set { if (inMove == value || value < 0) return; inMove = value; NotifyPropertyChanged("InMove"); } }

        [ReadOnly(true)]
        public bool StateInMove { get => stateInMove; set { if (stateInMove == value) return; stateInMove = value; NotifyPropertyChanged("StateInMove"); } }

        public int InDoneAck { get => inDoneAck; set { if (inDoneAck == value || value < 0) return; inDoneAck = value; NotifyPropertyChanged("InDoneAck"); } }
        [ReadOnly(true)]
        public bool StateInDoneAck { get => stateInDoneAck; set { if (stateInDoneAck == value) return; stateInDoneAck = value; NotifyPropertyChanged("StateInDoneAck"); } }


        public int InEnd { get => inEnd; set { if (inEnd == value || value < 0) return; inEnd = value; NotifyPropertyChanged("InEnd"); } }
        [ReadOnly(true)]
        public bool StateInStop { get => stateInStop; set { if (stateInStop == value) return; stateInStop = value; NotifyPropertyChanged("StateInStop"); } }

        public int OutReady { get => outReady; set { if (outReady == value || value < 0) return; outReady = value; NotifyPropertyChanged("OutReady"); } }

        [ReadOnly(true)]
        public bool StateOutReady { get => stateOutReady; set { if (stateOutReady == value) return; stateOutReady = value; NotifyPropertyChanged("StateOutReady"); } }

        public int OutStartAck { get => outStartAck; set { if (outStartAck == value || value < 0) return; outStartAck = value; NotifyPropertyChanged("OutStartAck"); } }

        [ReadOnly(true)]
        public bool StateOutStart { get => stateOutStartAck; set { if (stateOutStartAck == value) return; stateOutStartAck = value; NotifyPropertyChanged("StateOutStartAck"); } }

        public int OutMoveAck { get => outMoveAck; set { if (outMoveAck == value || value < 0) return; outMoveAck = value; NotifyPropertyChanged("OutMoveAck"); } }

        [ReadOnly(true)]
        public bool StateOutMoveAck { get => stateOutMoveAck; set { if (stateOutMoveAck == value) return; stateOutMoveAck = value; NotifyPropertyChanged("StateOutMoveAck"); } }

        public int OutDone { get => outDone; set { if (outDone == value || value < 0) return; outDone = value; NotifyPropertyChanged("OutDone"); } }

        [ReadOnly(true)]
        public bool StateOutDone { get => stateOutDone; set { if (stateOutDone == value) return; stateOutDone = value; NotifyPropertyChanged("StateOutDone"); } }

        public int OutEndAck { get => outEndAck; set { if (outEndAck == value || value < 0) return; outEndAck = value; NotifyPropertyChanged("OutEndAck"); } }

        [ReadOnly(true)]
        public bool StateOutEndAck { get => stateOutEndAck; set { if (stateOutEndAck == value) return; stateOutEndAck = value; NotifyPropertyChanged("StateOutEndAck"); } }

        public int DataAddr { get => dataAddr; set { if (dataAddr == value || value < 0) return; dataAddr = value; NotifyPropertyChanged("DataAddr"); } }
        #endregion

        public IFCalibration()
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
        public void CloseIO()
        {
            if (ComIO == null) return;
            ComIO.CloseDevice();
        }

    }
}
