using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
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
        #region Fields
        [NonSerialized]
        protected ByUser byUser;
        [NonSerialized]
        protected bool isOpen = false;
        [NonSerialized]
        protected Thread mThread = null;
        [NonSerialized]
        public bool mThreadFlag = false;
        [NonSerialized]
        protected static object interfaceLock = new object();
        protected int threadsleep = 50;
        [NonSerialized]
        private System.Timers.Timer heartBeatTimer;
        #endregion

        #region Properties
        public bool IsConnected { get { if (byUser == null) return false; return byUser.IsConnected; } }
        public uint Threadsleep { get => (uint)threadsleep; set => threadsleep = (int)value; }
        #endregion

        public Data_ByUser()
        {
            byUser = new ByUser();
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
            lock (interfaceLock)
            {
                if (!IsConnected)
                {
                    isOpen = byUser.Connect();
                    if (!isOpen)
                        LogWriter.Instance.LogError("[Interface] By User Open Fail");
                }
            }
            if (heartBeatTimer != null)
            {
                heartBeatTimer.Stop();
                heartBeatTimer.Dispose();
                heartBeatTimer = null;
            }
            heartBeatTimer = new System.Timers.Timer(3000);
            heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
            heartBeatTimer.Start();
            return isOpen;
        }
        private void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (byUser == null)
            {
                mThreadFlag = false;
                if (mThread != null)
                {
                    mThread.Abort();
                    mThread.Join();
                    Thread.Sleep(50);
                }
                return;
            }

            isOpen = byUser.CheckConnected();
            if (!isOpen)
            {
                isOpen = byUser.Reconnect();
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
                if (byUser != null)
                    byUser.Dispose();
                byUser = null;
            }
        }

        public bool WriteValue(int index, int value)
        {
            lock (interfaceLock)
            {
                try
                {
                    //value = index;
                    bool b = byUser.WriteValue(index, value);
                    return b;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
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
                    bool b = byUser.WriteValue(index, value);
                    return b;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
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
                    return byUser.WriteValue(index, value);
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
                }
            }
            return false;
        }

        public bool WriteValue(int index, string value)
        {
            lock (interfaceLock)
            {
                try
                {
                    return byUser.WriteValue(index, value);
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
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
                    value = byUser.ReadInt(index);

                    if (value == int.MinValue)
                        return false;

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = 0;
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
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
                    value = byUser.ReadFloat(index);

                    if (value == -1)
                        throw new Exception("Communication Timeout");

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = 0;
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
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
                    value = byUser.ReadString(index);

                    if (string.IsNullOrEmpty(value))
                        throw new Exception("Communication Timeout");

                    return true;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    value = null;
                    LogWriter.Instance.LogError("[Interface] " + ex.Message);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool WriteValues(int index, List<float> values)
        {
            lock (interfaceLock)
            {
                float[] tempTwoWord = new float[values.Count];

                for (int i = 0; i < values.Count; i++)
                {
                    tempTwoWord[i] = values[i];
                }
                return byUser.WriteValues(index, tempTwoWord);
            }
        }
    }
}
