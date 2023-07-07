using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Interface.ByUser
{
    [Serializable]
    public class IO_ByUser : Data_ByUser, IComIO, ISerializable
    {
        #region Fields
        [NonSerialized]
        private bool threadFlag = false;
        [NonSerialized]
        private Thread mReadThread;
        [NonSerialized]
        private Dictionary<int, bool> inIO = new Dictionary<int, bool>();
        [NonSerialized]
        private Dictionary<int, bool> outIO = new Dictionary<int, bool>();
        private object writeLock = new object();
        #endregion

        #region Properties
        [Browsable(false)]
        public Dictionary<int, bool> InIO { get { return inIO; } }
        [Browsable(false)]
        public Dictionary<int, bool> OutIO { get { return outIO; } }
        #endregion

        public IO_ByUser() : base()
        {
        }
        protected IO_ByUser(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void ThreadStart()
        {
            if (threadFlag)
            {
                ThreadStop();
                Thread.Sleep(100);
            }
            threadFlag = true;
            mReadThread = new Thread(new ThreadStart(GetBlockData));
            mReadThread.Start();
        }

        public void ThreadStop()
        {
            threadFlag = false;
        }
        private void GetBlockData()
        {
            while (threadFlag)
            {
                try
                {
                    Thread.Sleep((int)base.threadSleep);
                    if (base.socket == null) continue;
                    int[] iii = inIO.Keys.ToArray();
                    for (int index = 0; index < inIO.Count; index++)
                    {
                        short value;
                        base.ReadValue(iii[index], out value);
                        inIO[iii[index]] = value > 0 ? true : false;
                    }
                }
                catch
                {
                    LogWriter.Instance.LogError("By User IO Get I/F Error");
                    continue;
                }
            }
        }

        public bool GetInValue(int index)
        {
            if (index < 0) return false;
            return inIO[index];
        }

        public bool GetOutValue(int index)
        {
            if (index < 0) return false;
            return outIO[index];
        }

        public bool SetInValue(int index, bool value)
        {
            if (index < 0) return false;
            inIO[index] = value;
            return IsConnected;
        }

        public bool SetOutValue(int index, bool value)
        {
            if (index < 0) return false;
            bool result = false;
            lock (writeLock)
            {
                outIO[index] = value;
                result = WriteBlockData(index, value);
            }
            return result;
        }
        private bool WriteBlockData(int index, bool value)
        {
            lock (writeLock)
            {
                try
                {
                    if (base.socket == null || !base.socket.IsConnected) return false;
                    lock (writeLock)
                    {
                        short valueint = (short)(value ? 1 : 0);
                        bool result = base.WriteValue(index, valueint);
                        return result;
                    }
                }
                catch
                {
                    LogWriter.Instance.LogError("By User IO Set I/F Error");
                    return false;
                }
            }
        }

        public new void Dispose()
        {
            ThreadStop();
        }
    }
}
