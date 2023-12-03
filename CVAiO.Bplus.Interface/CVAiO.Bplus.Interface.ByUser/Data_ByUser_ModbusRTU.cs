//using CVAiO.Bplus.Core;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO.Ports;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Timers;

//namespace CVAiO.Bplus.Interface.ByUser
//{
//    [Serializable]
//    public class Data_ByUser : IComData, ISerializable
//    {
//        #region fields
//        protected string portName = "COM6";
//        protected EBaudRate baudRate = EBaudRate.B19200;
//        protected Parity parity = Parity.None;
//        protected int dataBits = 8;
//        protected StopBits stopBits = StopBits.One;
//        protected int threadSleep = 50;
//        protected ushort offsetCoils = 0;
//        protected ushort offsetRegisters = 0;
//        protected ushort iD = 1;
//        protected byte unit = 0;

//        [NonSerialized]
//        protected ModbusRTU modbusRTU;

//        [NonSerialized]
//        protected System.Timers.Timer heartBeatTimer;
//        [NonSerialized]
//        protected object interfaceLock = new object();
//        [NonSerialized]
//        protected bool isConnected = false;

//        protected const int timeout = 20000;
//        //protected uint threadSleep = 5;
//        #endregion

//        #region Properties
//        [Browsable(false)]
//        public bool IsConnected
//        {
//            get
//            {
//                if (modbusRTU == null) return false;
//                return isConnected;
//            }
//        }


//        [PropertyOrder(22), Description("PortName")]
//        [TypeConverter(typeof(ListCOMConverter))]
//        public string PortName { get => portName; set => portName = value; }

//        [PropertyOrder(23), Description("BaudRate")]
//        public EBaudRate BaudRate { get => baudRate; set { if (baudRate == value) return; baudRate = value; } }

//        [PropertyOrder(24), Description("Parity")]
//        public Parity Parity { get => parity; set => parity = value; }

//        [PropertyOrder(25), Description("DataBits")]
//        public int DataBits { get => dataBits; set { if (dataBits == value || value < 7 || value > 8) return; dataBits = value; } }

//        [PropertyOrder(26), Description("StopBits")]
//        public StopBits StopBits { get => stopBits; set => stopBits = value; }

//        [PropertyOrder(23), Description("Scanning Thread Sleep (default: 50 ms")]
//        public uint ThreadSleep { get => (uint)threadSleep; set => threadSleep = (int)value; }

//        [PropertyOrder(24), Description("Modbus ID")]
//        public ushort ID { get => iD; set => iD = value; }

//        [PropertyOrder(25), Description("Modbus Unit"), ReadOnly(true)]
//        protected byte Unit { get => unit; set => unit = value; }

//        [PropertyOrder(26), Description("Offset Coils - Schneider standard: 0")]
//        public ushort OffsetCoils { get => offsetCoils; set => offsetCoils = value; }

//        [PropertyOrder(27), Description("Offset Coils - Schneider standard: 402000")]
//        public ushort OffsetRegisters { get => offsetRegisters; set => offsetRegisters = value; }

//        #endregion

//        public Data_ByUser()
//        {
//            modbusRTU = new ModbusRTU();
//        }
//        public override string ToString()
//        {
//            return "By User";
//        }
//        protected Data_ByUser(SerializationInfo info, StreamingContext context)
//        {
//            Serializer.Deserializing(this, info, context);
//        }
//        public void GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            Serializer.Serializing(this, info, context);
//        }

//        public bool OpenDevice()
//        {
//            isConnected = true;
//            modbusRTU = new ModbusRTU();
//            if (!modbusRTU.Connected)
//            {
//                if (!modbusRTU.Connect(portName, baudRate, parity, dataBits, stopBits))
//                {
//                    isConnected = false;
//                }
//            }
//            if (heartBeatTimer != null)
//            {
//                heartBeatTimer.Stop();
//                heartBeatTimer.Dispose();
//                heartBeatTimer = null;
//            }
//            heartBeatTimer = new System.Timers.Timer(2000);
//            heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
//            heartBeatTimer.Start();
//            return isConnected;
//        }
//        private void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            isConnected = modbusRTU.CheckConnected();
//            byte[] byteData = new byte[2];
//            bool readCoil = false;
//            heartBeatTimer.Stop();
//            modbusRTU.ReadCoils(ID, 0, 1, ref byteData);

//            for (int i = 0; i < 3; i++)
//                if (readCoil = modbusRTU.ReadCoils(ID, 0, 1, ref byteData)) break;
//            isConnected &= readCoil;
//            if (!isConnected)
//            {
//                isConnected = modbusRTU.Reconnect(portName, baudRate, parity, dataBits, stopBits);
//                if (!isConnected)
//                    LogWriter.Instance.LogError("[Interface] By User Open Fail");
//                else
//                    LogWriter.Instance.LogError("[Interface] By User ReConnect Success");
//            }
//        }

//        public bool CloseDevice()
//        {
//            lock (interfaceLock)
//            {
//                Dispose();
//                return true;
//            }
//        }

//        public void Dispose()
//        {
//            lock (interfaceLock)
//            {
//                if (heartBeatTimer != null)
//                {
//                    heartBeatTimer.Stop();
//                    heartBeatTimer.Dispose();
//                    heartBeatTimer = null;
//                }
//                if (modbusRTU != null)
//                    modbusRTU.Dispose();
//                modbusRTU = null;
//            }
//        }

//        public bool WriteValue(int index, int value)
//        {
//            byte[] intBytes = BitConverter.GetBytes(value);
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteMultipleRegister(ID, (ushort)index, 2, new byte[] { intBytes[1], intBytes[0], intBytes[3], intBytes[2] })) return true;
//            return false;
//        }
//        public bool WriteValue(int index, short value)
//        {
//            byte[] shortBytes = BitConverter.GetBytes(value);
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteMultipleRegister(ID, (ushort)index, 1, new byte[] { shortBytes[1], shortBytes[0] })) return true;
//            return false;
//        }
//        public bool WriteValue(int index, float value)
//        {
//            byte[] floatBytes = BitConverter.GetBytes(value);
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteMultipleRegister(ID, (ushort)index, 2, new byte[] { floatBytes[1], floatBytes[0], floatBytes[3], floatBytes[2] })) return true;
//            return false;
//        }
//        public virtual bool WriteValues(int index, List<float> values)
//        {
//            byte[] tempBytes = new byte[values.Count * 4];
//            for (int i = 0; i < values.Count; i++)
//            {
//                byte[] intByte = BitConverter.GetBytes(values[i]);
//                tempBytes[4 * i] = intByte[1];
//                tempBytes[4 * i + 1] = intByte[0];
//                tempBytes[4 * i + 2] = intByte[3];
//                tempBytes[4 * i + 3] = intByte[2];
//            }
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteMultipleRegister(ID, (ushort)index, 2, tempBytes)) return true;
//            return false;
//        }

//        public bool WriteValue(int index, string value)
//        {
//            if (value.Length % 2 == 1) value += " ";
//            byte[] byteData = Encoding.ASCII.GetBytes(value);
//            byte[] byteSwap = new byte[byteData.Length];
//            for (int i = 0; i < byteSwap.Length / 2; i++)
//            {
//                byteSwap[2 * i] = byteData[2 * i + 1];
//                byteSwap[2 * i + 1] = byteData[2 * i];
//            }
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteMultipleRegister(ID, (ushort)index, (ushort)(value.Length / 2), byteSwap)) return true;
//            return false;
//        }

//        public bool ReadValue(int index, out short value)
//        {
//            value = 0;
//            byte[] data = new byte[2];
//            bool readSuccess = false;
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.ReadHoldingRegister(ID, (ushort)index, 1, ref data)) { readSuccess = true; break; }
//            if (!readSuccess) return false;
//            value = (short)((data[0] << 8) + data[1]);
//            return true;
//        }

//        public bool ReadValue(int index, out int value)
//        {
//            value = 0;
//            byte[] data = new byte[4];
//            bool readSuccess = false;
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.ReadHoldingRegister(ID, (ushort)index, 2, ref data)) { readSuccess = true; break; }
//            if (!readSuccess) return false;
//            value = BitConverter.ToInt32(new byte[4] { data[1], data[0], data[3], data[2] }, 0);
//            return true;
//        }

//        public bool ReadValue(int index, out float value)
//        {
//            value = 0;
//            byte[] data = new byte[4];
//            bool readSuccess = false;
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.ReadHoldingRegister(ID, (ushort)index, 2, ref data)) { readSuccess = true; break; }
//            if (!readSuccess) return false;
//            value = BitConverter.ToSingle(new byte[4] { data[1], data[0], data[3], data[2] }, 0);
//            return true;
//        }

//        public bool ReadValue(int index, out string value)
//        {
//            value = null;
//            byte[] data = new byte[40];
//            bool readSuccess = false;
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.ReadHoldingRegister(ID, (ushort)index, 20, ref data)) { readSuccess = true; break; }
//            if (!readSuccess) return false;
//            byte[] byteSwap = new byte[data.Length];
//            for (int i = 0; i < byteSwap.Length / 2; i++)
//            {
//                byteSwap[2 * i] = data[2 * i + 1];
//                byteSwap[2 * i + 1] = data[2 * i];
//            }
//            value = Encoding.ASCII.GetString(byteSwap);
//            return true;
//        }
//    }
//    public class ListCOMConverter : StringConverter
//    {
//        public static string[] ListString;

//        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
//        {
//            return true;
//        }

//        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
//        {
//            return true;
//        }

//        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
//        {
//            ListString = System.IO.Ports.SerialPort.GetPortNames();
//            return new StandardValuesCollection(ListString);
//        }
//    }
//}
