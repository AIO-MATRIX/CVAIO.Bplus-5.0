using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CVAiO.Bplus.Interface.ByUser
{
    [Serializable]
    public class Data_ByUser : IComData, ISerializable
    {
        #region fields
        [NonSerialized]
        protected ModbusTCPIP socket;
        
        [NonSerialized]
        protected System.Timers.Timer heartBeatTimer;
        [NonSerialized]
        protected object interfaceLock = new object();
        [NonSerialized]
        protected bool isOpen = false;
        protected string ip = "127.0.0.1";
        protected int port = 502;
        protected ushort id = 1;
        protected byte unit = 0;
        protected const int timeout = 20000;
        protected uint threadSleep = 5;
        #endregion

        #region Properties
        [Browsable(false)]
        public bool IsOpen { get { return isOpen; } }
        [Browsable(false)]
        public bool IsConnected { get { if (socket == null) return false; return socket.IsConnected; } }

        [PropertyOrder(22), Description("PLC IP Address (default: 127.0.0.1)")]
        public string Ip { get => ip; set => ip = value; }

        [PropertyOrder(22), Description("PLC Port (default: 502)")]
        public int Port { get => port; set => port = value; }

        [PropertyOrder(22), Description("PLC Modbus ID")]
        public ushort ID { get => id; set => id = value; }

        [PropertyOrder(22), Description("PLC Modbus Unit")]
        public byte Unit { get => unit; set => unit = value; }

        [PropertyOrder(27), Description("Scanning Thread Sleep (default: 50 ms")]
        public uint ThreadSleep { get => threadSleep; set => threadSleep = value; }
        
        #endregion

        public Data_ByUser()
        {
            socket = new ModbusTCPIP();
        }
        public override string ToString()
        {
            return "By User";
        }
        protected Data_ByUser(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }

        public bool OpenDevice()
        {
            isOpen = true;
            socket = new ModbusTCPIP();
            if (!socket.IsConnected)
            {
                if (!socket.Connect(ip, port))
                {
                    isOpen = false;
                }
            }
            if (heartBeatTimer != null)
            {
                heartBeatTimer.Stop();
                heartBeatTimer.Dispose();
                heartBeatTimer = null;
            }
            heartBeatTimer = new System.Timers.Timer(2000);
            heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
            heartBeatTimer.Start();
            return isOpen;
        }
        private void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            isOpen = socket.CheckConnected();
            if (!isOpen)
            {
                isOpen = socket.Reconnect(ip, port);
                if (!isOpen)
                    LogWriter.Instance.LogError("[Interface] By User Open Fail");
                else
                    LogWriter.Instance.LogError("[Interface] By User ReConnect Success");
            }
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
                    socket.Dispose();
                socket = null;
            }
        }

        public bool WriteValue(int index, int value)
        {
            lock (interfaceLock)
            {
                try
                {
                    // int = 4 bytes
                    byte[] result = new byte[4];
                    byte[] intBytes = BitConverter.GetBytes(value);
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, new byte[]{ intBytes[1], intBytes[0], intBytes[3], intBytes[2]} , ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return false;
        }
        public bool WriteValue(int index, short value)
        {
            lock (interfaceLock)
            {
                try
                {
                    byte[] result = new byte[2];
                    byte[] shortBytes = BitConverter.GetBytes(value);
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, new byte[] { shortBytes[1], shortBytes[0] }, ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                }
            }
            return false;
        }
        public bool WriteValue(int index, float value)
        {
            lock (interfaceLock)
            {
                try
                {
                    byte[] result = new byte[4];
                    byte[] floatBytes = BitConverter.GetBytes(value);
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, new byte[] { floatBytes[1], floatBytes[0], floatBytes[3], floatBytes[2] }, ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                }
            }
            return false;
        }
        public virtual bool WriteValues(int index, List<float> values)
        {
            lock (interfaceLock)
            {
                byte[] tempBytes = new byte[values.Count * 4];
                byte[] result = new byte[values.Count * 4];

                for (int i = 0; i < values.Count; i++)
                {
                    byte[] intByte = BitConverter.GetBytes(values[i]);
                    tempBytes[4 * i] = intByte[1];
                    tempBytes[4 * i + 1] = intByte[0];
                    tempBytes[4 * i + 2] = intByte[3];
                    tempBytes[4 * i + 3] = intByte[2];
                }
                socket.WriteMultipleRegister(ID, Unit, (ushort)index, tempBytes, ref result);
                return true;
            }
        }

        public bool WriteValue(int index, string value)
        {
            lock (interfaceLock)
            {
                try
                {
                    if (value.Length % 2 == 1) value += " ";
                    byte[] byteData = Encoding.ASCII.GetBytes(value);
                    byte[] byteSwap = new byte[byteData.Length];
                    for (int i = 0; i < byteSwap.Length/2; i++)
                    {
                        byteSwap[2 * i] = byteData[2 * i + 1];
                        byteSwap[2 * i + 1] = byteData[2 * i];
                    }
                    byte[] result = new byte[byteData.Length];
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, byteSwap, ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                }
            }
            return false;
        }

        public bool ReadValue(int index, out short value)
        {
            value = 0;
            lock (interfaceLock)
            {
                try
                {
                    byte[] data = new byte[2];
                    socket.ReadHoldingRegister(ID, Unit, (ushort)index, 1, ref data);
                    value = (short)((data[0] << 8) + data[1]);
                    if (value == short.MinValue)
                        return false; // throw new Exception("Communication Timeout");

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = 0;
                }
            }
            return false;
        }

        public bool ReadValue(int index, out int value)
        {
            value = 0;
            lock (interfaceLock)
            {
                try
                {
                    byte[] data = new byte[4];
                    socket.ReadHoldingRegister(ID, Unit, (ushort)index, 2, ref data);
                    value = BitConverter.ToInt32(new byte[4] { data[1], data[0], data[3], data[2]}, 0);
                    if (value == int.MinValue)
                        return false; // throw new Exception("Communication Timeout");

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = 0;
                }
            }
            return false;
        }

        public bool ReadValue(int index, out float value)
        {
            value = 0;
            lock (interfaceLock)
            {
                try
                {
                    byte[] data = new byte[4];
                    socket.ReadHoldingRegister(ID, Unit, (ushort)index, 2, ref data);
                    value = BitConverter.ToSingle(new byte[4] { data[1], data[0], data[3], data[2] }, 0);
                    if (value == -1)
                        throw new Exception("Communication Timeout");

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = 0;
                }
            }
            return false;
        }

        public bool ReadValue(int index, out string value)
        {
            value = null;
            lock (interfaceLock)
            {
                try
                {
                    byte[] data = new byte[40];
                    socket.ReadHoldingRegister(ID, Unit, (ushort)index, 20, ref data);
                    byte[] byteSwap = new byte[data.Length];
                    for (int i = 0; i < byteSwap.Length / 2; i++)
                    {
                        byteSwap[2 * i] = data[2 * i + 1];
                        byteSwap[2 * i + 1] = data[2 * i];
                    }
                    value = Encoding.ASCII.GetString(byteSwap);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = null;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

    }
}
