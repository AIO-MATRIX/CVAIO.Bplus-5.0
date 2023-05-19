using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Timers;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CVAiO.Bplus.Simulator.ModbusTCP
{
    [Serializable]
    public class InterfaceModbusTCP : IInterface
    {
        [NonSerialized]
        protected ModbusTCPIP socket;
        [NonSerialized]
        private Thread mReadThread, mWriteThread;
        private bool mThreadFlag = false;
        private int threadSleep = 10;
        [NonSerialized]
        protected ushort ID = 1;
        [NonSerialized]
        protected byte Unit = 0;
        [NonSerialized]
        private System.Timers.Timer heartBeatTimer;
        object interfaceLock = new object();
        bool isOpen = false;

        [Browsable(false)]
        public bool IsOpen { get { return isOpen; } }

        private string ip = "127.0.0.1";
        private int port = 502;
        const int timeout = 20000;
        [Browsable(false)]
        public bool IsConnected { get { if (socket == null) return false; return socket.IsConnected; } }
        [Browsable(false)]
        public Dictionary<int, bool> InIO { get { return mInIO; } }
        Dictionary<int, bool> mInIO = new Dictionary<int, bool>();
        [Browsable(false)]
        public Dictionary<int, bool> OutIO { get { return mOutIO; } }

        public string Ip { get => ip; set => ip = value; }

        [ReadOnly(true)]
        public int Port { get => port; set => port = value; }

        Dictionary<int, bool> mOutIO = new Dictionary<int, bool>();
        public InterfaceModbusTCP()
        {
            socket = new ModbusTCPIP();
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
            ThreadStart();
            return true;
        }
        public bool CloseDevice()
        {
            ThreadStop();
            Dispose();
            return true;
        }

        public void Dispose()
        {
            if (heartBeatTimer != null)
            {
                heartBeatTimer.Stop();
                heartBeatTimer.Dispose();
                heartBeatTimer = null;
            }
            if (socket != null)
            {
                if (socket.IsConnected)
                {
                    socket.Dispose();
                }
            }
        }

        void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (interfaceLock)
            {
               // isOpen = socket.CheckConnected();
            }
            if (!isOpen)
            {
                socket.Reconnect(ip, port);
            }
        }

        public void ThreadStart()
        {
            if (mThreadFlag)
            {
                ThreadStop();
                Thread.Sleep(100);
            }
            mThreadFlag = true;
            mReadThread = new Thread(new ThreadStart(GetBlockData));
            mReadThread.Start();
            mWriteThread = new Thread(new ThreadStart(SetBlockData));
            mWriteThread.Start();
        }

        public void ThreadStop()
        {
            mThreadFlag = false;
        }
        private void GetBlockData()
        {
            while (mThreadFlag)
            {
                try
                {
                    Thread.Sleep((int)threadSleep);
                    if (socket == null) continue;
                    int[] keys = InIO.Keys.ToArray();
                    byte[] data = new byte[16];
                    lock (interfaceLock)
                        socket.ReadCoils(ID, Unit, 0, 127, ref data);
                    if (data == null) continue;
                    for (int index = 0; index < InIO.Count; index++)
                    {
                        var sourceByteIndex = (keys[index]) / 8;
                        var sourceBitIndex = (keys[index]) % 8;
                        var isSet = (data[sourceByteIndex] & (1 << sourceBitIndex)) > 0;
                        InIO[keys[index]] = (data[sourceByteIndex] & (1 << sourceBitIndex)) > 0 ? true : false;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private void SetBlockData()
        {
            while (mThreadFlag)
            {
                try
                {
                    Thread.Sleep((int)threadSleep);
                    if (socket == null) continue;
                    int[] keys = OutIO.Keys.ToArray();
                    byte[] data = new byte[16];
                    byte[] result = new byte[16];

                    for (int index = 0; index < OutIO.Count; index++)
                    {
                        var sourceByteIndex = (keys[index]) / 8;
                        var sourceBitIndex = (keys[index]) % 8;
                        if (OutIO[keys[index]])
                            data[sourceByteIndex] |= (byte)(1 << sourceBitIndex);
                        else
                            data[sourceByteIndex] &= (byte)(~(byte)(1 << sourceBitIndex));
                    }
                    lock (interfaceLock)
                        socket.WriteMultipleCoils(ID, Unit, 0, 127, data, ref result);
                }
                catch
                {
                    continue;
                }
            }
        }


        #region IOFunction
        public bool SetOutValue(int _index, bool _value, bool _select = true)
        {
            if (_index < 0) return false;
            lock (interfaceLock)
                OutIO[_index] = _value;
            return true;
        }

        public bool SetInValue(int _index, bool _value)
        {
            if (_index < 0) return false;
            lock (interfaceLock)
                InIO[_index] = _value;
            return true;
        }

        public bool GetInValue(int _index)
        {
            if (_index < 0) return false;
            return InIO[_index];
        }

        public bool GetOutValue(int _index)
        {
            if (_index < 0) return false;
            return OutIO[_index];
        }
        #endregion

        #region DataFunction
        public bool WriteValue(int index, int value)
        {
            lock (interfaceLock)
            {
                try
                {
                    // int = 4 bytes
                    byte[] result = new byte[4];
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, GetByteContatiner(IntToWord(value)[0], IntToWord(value)[1]), ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {

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
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, GetByteContatiner(value), ref result);
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
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, GetByteContatiner(FloatToTwoWord(value)[0], FloatToTwoWord(value)[1]), ref result);
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
                    tempBytes[4 * i] = GetByteContatiner(FloatToTwoWord(values[i])[0], FloatToTwoWord(values[i])[1])[0];
                    tempBytes[4 * i + 1] = GetByteContatiner(FloatToTwoWord(values[i])[0], FloatToTwoWord(values[i])[1])[1];
                    tempBytes[4 * i + 2] = GetByteContatiner(FloatToTwoWord(values[i])[0], FloatToTwoWord(values[i])[1])[2];
                    tempBytes[4 * i + 3] = GetByteContatiner(FloatToTwoWord(values[i])[0], FloatToTwoWord(values[i])[1])[3];
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
                    byte[] byteData = Encoding.ASCII.GetBytes(value);
                    byte[] result = new byte[byteData.Length];
                    socket.WriteMultipleRegister(ID, Unit, (ushort)index, byteData, ref result);
                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
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
                    //value = TwoWordToInt((short)(data[0] >> 8 + data[1]), (short)(data[2] >> 8 + data[3]));
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
                    value = TwoWordToFloat((short)(data[0] >> 8 + data[1]), (short)(data[2] >> 8 + data[3]));
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
                    value = Encoding.ASCII.GetString(data);
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
        #endregion

        private byte[] GetByteContatiner(short lowWord, short highWord)
        {
            return new byte[4]
            {
            (byte)((uint)lowWord & 0xFFu),
            (byte)(lowWord >> 8),
            (byte)((uint)highWord & 0xFFu),
            (byte)(highWord >> 8)
            };
        }
        private byte[] GetByteContatiner(int word)
        {
            return new byte[2]
            {
            (byte)((uint)word & 0xFFu),
            (byte)(word >> 8)
            };
        }
        private byte[] GetByteContatiner(short word)
        {
            return new byte[2]
            {
            (byte)((uint)word & 0xFFu),
            (byte)(word >> 8)
            };
        }
        private short[] FloatToTwoWord(float value)
        {
            short[] twoWord = new short[2];
            byte[] by = BitConverter.GetBytes(value);
            twoWord[0] = (short)((by[0] | (by[1] << 8)) & 0xFFFF);
            twoWord[1] = (short)((by[2] | (by[3] << 8)) & 0xFFFF);
            return twoWord;
        }
        private short[] IntToWord(int value)
        {
            short[] oneWord = new short[1];
            IntToWord(value, oneWord, 0);
            return oneWord;
        }
        private void IntToWord(int value, short[] oneWord, int startIndex)
        {
            byte[] by = BitConverter.GetBytes(value);
            oneWord[startIndex] = (short)((by[0] | (by[1] << 8)) & 0x0000FFFF);  // low 
        }
        private int TwoWordToInt(short lowWord, short highWord)
        {
            return BitConverter.ToInt32(GetByteContatiner(lowWord, highWord), 0);
        }
        private float TwoWordToFloat(short lowWord, short highWord)
        {
            return BitConverter.ToSingle(GetByteContatiner(lowWord, highWord), 0);
        }
    }
}
