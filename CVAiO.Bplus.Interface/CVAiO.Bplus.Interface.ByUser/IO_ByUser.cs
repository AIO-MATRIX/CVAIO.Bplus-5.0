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
    public class IO_ByUser : Data_ByUser, IComIO, ISerializable
    {
        #region Fields
        [NonSerialized]
        private Thread mReadThread, mWriteThread;
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

            if (mReadThread != null)
            {
                mReadThread.Join();
            }
            if (mWriteThread != null)
            {
                mWriteThread.Join();
            }
        }
        private void GetBlockData()
        {
            while (mThreadFlag)
            {
                try
                {
                    Thread.Sleep(threadsleep);
                    if (base.byUser == null) continue;

                    int[] iii = inIO.Keys.ToArray();

                    for (int index = 0; index < inIO.Count; index++)
                    {
                        int value;
                        base.ReadValue(iii[index], out value);
                        inIO[iii[index]] = value > 0 ? true : false;
                    }
                }
                catch
                {
                    LogWriter.Instance.LogError("RS232 IO Get I/F Error");
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
                    Thread.Sleep(threadsleep);
                    if (base.byUser == null) continue;
                    lock (writeLock)
                    {
                        int[] iii = outIO.Keys.ToArray();

                        for (int index = 0; index < outIO.Count; index++)
                        {
                            int value = outIO[iii[index]] == true ? 1 : 0;
                            base.WriteValue(iii[index], value);
                        }
                    }
                }
                catch
                {
                    LogWriter.Instance.LogError("By User IO Set I/F Error");
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
            lock (writeLock)
            {
                outIO[index] = value;
            }
            return IsConnected;
        }
    }
}
