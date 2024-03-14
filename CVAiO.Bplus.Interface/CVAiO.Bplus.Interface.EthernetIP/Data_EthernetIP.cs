using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CVAiO.Bplus.Interface.EthernetIP
{
    [Serializable]
    public class Data_EthernetIP : IComData, ISerializable
    {
        #region Fields
        protected string ip = "127.0.0.1";
        protected int port = 5000;
        protected int threadSleep = 50;
        [NonSerialized]
        protected UInt16 maxCoilAddress = 128; // Tổng số IO: 128, tương ứng với 16 bytes
        [NonSerialized]
        protected UInt16 maxHoldingRegisterAddress = 64; // Tổng số thanh ghi: 64, tương ứng với 128 bytes
        [NonSerialized]
        protected byte[] coils = new byte[16]; // Vision Input (Start/Comp/Trigger1,2,3,4/Calc) 2 ~ 63, 1: Connection Alive (00001-09999)
        [NonSerialized]
        protected byte[] holdingRegisters = new byte[128];

        [NonSerialized]
        protected AsyncSocketServer socket;
        [NonSerialized]
        protected Thread mThread = null;
        [NonSerialized]
        public bool mThreadFlag = false;
        [NonSerialized]
        protected const int timeout = 200000;
        [NonSerialized]
        bool isOpen = false;
        [NonSerialized]
        protected static object interfaceLock = new object();
        [NonSerialized]
        private IPAddress ipCheck;
        [NonSerialized]
        private System.Timers.Timer heartBeatTimer;
        [NonSerialized]
        private List<AsyncSocketClient> clientList;
        [NonSerialized]
        private int id;
        [NonSerialized]
        private int aliveCount = 1;
        [NonSerialized]
        private int aliveWatch = 0;
        [NonSerialized]
        private bool isConnected;

        [NonSerialized]
        private string fctReadCoils = "RIO";
        [NonSerialized]
        private string fctWriteMultipleCoils = "WIO";
        [NonSerialized]
        private string fctReadHoldingRegisters = "RVP";
        #endregion


        #region Properties
        [Browsable(false)]
        public bool IsConnected
        {
            get
            {
                if (clientList == null || clientList.Count == 0) return false;
                return isConnected;
            }
        }

        [PropertyOrder(21), Description("IP Address")]
        public string Ip { get => ip; set { if (!IPAddress.TryParse(value, out ipCheck)) return; ip = value; } }

        [PropertyOrder(22), Description("Port Number")]
        public int Port { get => port; set { if (value < 1 || port == value) return; port = value; } }

        [PropertyOrder(23), Description("Scanning Thread Sleep (default: 50 ms")]
        public uint ThreadSleep { get => (uint)threadSleep; set => threadSleep = (int)value; }

        #endregion

        public Data_EthernetIP()
        {
            socket = new AsyncSocketServer();
            socket.OnAccept += new AsyncSocketAcceptEventHandler(OnAccept);
            socket.OnError += new AsyncSocketErrorEventHandler(OnError);
            clientList = new List<AsyncSocketClient>();
            heartBeatTimer = new System.Timers.Timer(2000);
            heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
        }

        public override string ToString()
        {
            return "EthernetIP";
        }

        protected Data_EthernetIP(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }

        private void OnAccept(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient worker = new AsyncSocketClient(id++, e.Worker);
            worker.Receive();
            worker.OnConnect += new AsyncSocketConnectEventHandler(OnConnect);
            worker.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            worker.OnError += new AsyncSocketErrorEventHandler(OnError);
            worker.OnSend += new AsyncSocketSendEventHandler(OnSend);
            worker.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            clientList.Add(worker);
            heartBeatTimer = new System.Timers.Timer(2000);
            heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
            heartBeatTimer.Start();
            isConnected = true;
            aliveCount = 1;
            aliveWatch = 0;
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
        }

        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
        }

        private void OnConnect(object sender, AsyncSocketConnectionEventArgs e)
        {
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].ID == e.ID)
                {
                    clientList.Remove(clientList[i]);
                    break;
                }
            }
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            string function = Encoding.UTF8.GetString(e.ReceiveData, 0, 3);
            aliveCount++;
            if (function == fctReadCoils) // RIO - Read Input Output
            {
                #region Read Coil
                for (int i = 0; i < 128; i++)
                {
                    var sourceByteIndex = i / 8;
                    var sourceBitIndex = i % 8;
                    var isSet = (coils[sourceByteIndex] & (1 << sourceBitIndex)) > 0;
                    e.ReceiveData[i + 3] = isSet ? (byte)49 : (byte)48;
                }
                e.ReceiveData[131] = 13;
                e.ReceiveData[132] = 10;
                clientList[0].Send(e.ReceiveData, 133); // 131 bytes bao gồm 3 bytes RIO và 128 bytes IO 
                #endregion
            }
            else if (function == fctWriteMultipleCoils) //WIO -> Write Input Output
            {
                #region Write Multiple Coils
                for (int i = 0; i < 128; i++)
                {
                    if (i > 63) continue; // Input: 0~63 -> not write 64~127
                    var sourceByteIndex = i / 8;
                    var sourceBitIndex = i % 8;
                    if ((e.ReceiveData[i + 3]) != 48)
                        coils[sourceByteIndex] |= (byte)(1 << sourceBitIndex);
                    else
                        coils[sourceByteIndex] &= (byte)(~(byte)(1 << sourceBitIndex));
                }
                e.ReceiveData[3] = 13;
                e.ReceiveData[4] = 10;
                clientList[0].Send(e.ReceiveData, 5);
                #endregion
            }
            else if (function == fctReadHoldingRegisters) //RVP -> Read Value Position
            {
                #region Read Holding Register
                string dataString = "";
                for (int i = 0; i < 32; i++)
                {
                    int value = BitConverter.ToInt32(holdingRegisters, 4 * i);
                    dataString += value.ToString() + ",";
                }
                dataString.Remove(dataString.Length - 1);
                byte[] dataBytes = Encoding.ASCII.GetBytes(dataString);
                Array.Copy(dataBytes, 0, e.ReceiveData, 3, dataBytes.Length);
                e.ReceiveData[2 + dataBytes.Length] = 13;
                e.ReceiveData[3 + dataBytes.Length] = 10;
                clientList[0].Send(e.ReceiveData, 2 + dataBytes.Length + 2);
                #endregion
            }
            else
            {
                e.ReceiveData[0] = 69;//E
                e.ReceiveData[1] = 82;//R
                e.ReceiveData[2] = 82;//R
                e.ReceiveData[4] = 13;
                e.ReceiveData[5] = 10;
                clientList[0].Send(e.ReceiveData, 5);
            }
        }

        internal static UInt16 SwapUInt16(UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                     ((inValue & 0x00ff) << 8));
        }

        public bool OpenDevice()
        {
            isOpen = true;
            lock (interfaceLock)
            {
                if (socket == null)
                {
                    socket = new AsyncSocketServer();
                    socket.OnAccept += new AsyncSocketAcceptEventHandler(OnAccept);
                    socket.OnError += new AsyncSocketErrorEventHandler(OnError);
                }
                if (!IsConnected)
                {
                    isOpen = socket.Connect(ip, port);
                    if (!isOpen)
                        LogWriter.Instance.LogError("EthernetIP TCP/IP Open Fail");
                }
            }
            return isOpen;
        }

        private void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (interfaceLock)
            {
                if (aliveWatch != aliveCount)
                {
                    isConnected = true;
                    aliveWatch = aliveCount;
                    if (aliveCount > 100000) aliveCount = 1;
                }
                else
                    isConnected = false;
                if (!isConnected)
                {
                    isOpen = socket.Reconnect(ip, port);
                    if (clientList != null && clientList.Count > 0)
                    {
                        clientList[0].Close();
                        clientList.Clear();
                    }
                    heartBeatTimer.Stop();
                    if (!isOpen)
                        LogWriter.Instance.LogError("EthernetIP TCP/IP Open Fail");
                    else
                        LogWriter.Instance.LogDebug("EthernetIP TCP/IP Open Success");
                }
            }
        }

        public bool WriteValue(int index, int value)
        {
            try
            {
                if (index < 0 || index > 64 - 2)
                    throw new Exception("WriteValue int index : 0 ~ 62");
                byte[] data = BitConverter.GetBytes(value);
                holdingRegisters[2 * index] = data[0];
                holdingRegisters[2 * index + 1] = data[1];
                holdingRegisters[2 * index + 2] = data[2];
                holdingRegisters[2 * index + 3] = data[3];
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP:  " + ex.Message);
            }
            return false;
        }

        public bool WriteValue(int index, short value)
        {
            try
            {
                if (index < 0 || index > 64 - 1)
                    throw new Exception("WriteValue short index : 0 ~ 63");
                byte[] data = BitConverter.GetBytes(value);
                holdingRegisters[2 * index] = data[0];
                holdingRegisters[2 * index + 1] = data[1];
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool WriteValue(int index, float value)
        {
            try
            {
                if (index < 0 || index > 64 - 2)
                    throw new Exception("WriteValue float index : 0 ~ 62");
                byte[] data = BitConverter.GetBytes(value);
                holdingRegisters[2 * index] = data[0];
                holdingRegisters[2 * index + 1] = data[1];
                holdingRegisters[2 * index + 2] = data[2];
                holdingRegisters[2 * index + 3] = data[3];
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool WriteValues(int index, List<float> values)
        {
            try
            {
                if (index < 0 || index > 64 - 4 * values.Count)
                    throw new Exception("WriteValue float index : 0 ~ " + (64 - 4 * values.Count).ToString());

                for (int i = 0; i < values.Count; i++)
                {
                    byte[] data = BitConverter.GetBytes(values[i]);
                    holdingRegisters[2 * index + 4 * i] = data[0];
                    holdingRegisters[2 * index + 1 + 4 * i] = data[1];
                    holdingRegisters[2 * index + 2 + 4 * i] = data[2];
                    holdingRegisters[2 * index + 3 + 4 * i] = data[3];
                }
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool WriteValue(int index, string value)
        {
            try
            {
                if (index < 0 || index > 128 - value.Length)
                    throw new Exception("WriteValue string index : 0 ~ " + (64 - value.Length).ToString());

                char[] charsArray = value.ToCharArray();
                for (int i = 0; i < charsArray.Length; i += 1)
                {
                    byte[] first = BitConverter.GetBytes(charsArray[i]);
                    holdingRegisters[2 * index + i] = first[0];
                }
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool ReadValue(int index, out int value)
        {
            value = 0;
            try
            {
                if (index < 0 || index > 64 - 2)
                    throw new Exception("ReadValue int index : 0 ~ 62");
                value = BitConverter.ToInt32(holdingRegisters, 2 * index);
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                value = 0;
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool ReadValue(int index, out float value)
        {
            value = 0;
            try
            {
                if (index < 0 || index > 64 - 2)
                    throw new Exception("ReadValue float index : 0 ~ 62");
                value = BitConverter.ToInt16(holdingRegisters, 2 * index);
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                value = 0;
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            return false;
        }

        public bool ReadValue(int index, out string value)
        {
            value = null;
            try
            {
                if (index < 0 || index > 64 - 20)
                    throw new Exception("ReadValue string index : 0 ~ 44");
                value = "";

                char[] chars = new char[40];
                int cnt = 0;
                for (int i = 0; i < 20; i++)
                {
                    char first = Convert.ToChar(holdingRegisters[2 * i + index * 2]);
                    char second = Convert.ToChar(holdingRegisters[2 * i + 1 + index * 2]);
                    if (first <= 0x20 || first > 0x7E)
                        break;
                    else
                        chars[cnt++] = first;

                    if (second <= 0x20 || second > 0x7E)
                        break;
                    else
                        chars[cnt++] = second;
                }
                if (cnt == 0) return false;
                value += new string(chars, 0, cnt);
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                value = null;
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogDebug("EthernetIP TCP/IP: " + ex.Message);
                return false;
            }
            return false;
        }

        public bool CloseDevice()
        {
            lock (interfaceLock)
            {
                Dispose();
                return true;
            }
        }

        public void Dispose()
        {
            lock (interfaceLock)
            {
                if (heartBeatTimer != null)
                {
                    heartBeatTimer.Stop();
                    heartBeatTimer.Dispose();
                    heartBeatTimer = null;
                }
                if (socket != null)
                    socket.Stop();
                socket = null;
            }
        }
    }
}
