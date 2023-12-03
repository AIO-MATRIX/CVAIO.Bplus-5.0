//using CVAiO.Bplus.Core;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CVAiO.Bplus.Interface.ByUser
//{
//    [Serializable]
//    public class IO_ByUser : Data_ByUser, IComIO, ISerializable
//    {
//        #region Fields
//        [NonSerialized]
//        private bool threadFlag = false;
//        [NonSerialized]
//        private Thread readThread;
//        [NonSerialized]
//        private Thread writeThread;
//        [NonSerialized]
//        private Dictionary<int, bool> inIO = new Dictionary<int, bool>();
//        [NonSerialized]
//        private Dictionary<int, bool> outIO = new Dictionary<int, bool>();
//        #endregion

//        #region Properties
//        [Browsable(false)]
//        public Dictionary<int, bool> InIO { get { return inIO; } }
//        [Browsable(false)]
//        public Dictionary<int, bool> OutIO { get { return outIO; } }
//        #endregion

//        public IO_ByUser() : base()
//        {
//        }
//        protected IO_ByUser(SerializationInfo info, StreamingContext context)
//        {
//            Serializer.Deserializing(this, info, context);
//        }
//        public void ThreadStart()
//        {
//            if (threadFlag)
//            {
//                ThreadStop();
//                Thread.Sleep(100);
//            }
//            threadFlag = true;
//            readThread = new Thread(new ThreadStart(GetBlockData));
//            readThread.Start();
//        }

//        public void ThreadStop()
//        {
//            threadFlag = false;
//        }
//        private void GetBlockData()
//        {
//            byte[] coilBytes = new byte[8];
//            while (threadFlag)
//            {
//                try
//                {
//                    Thread.Sleep((int)base.threadSleep);
//                    if (base.modbusRTU == null) continue;
//                    if (!modbusRTU.ReadCoils(ID, offsetCoils, 64, ref coilBytes)) continue;
//                    int[] keys = inIO.Keys.ToArray();
//                    for (int index = 0; index < inIO.Count; index++)
//                    {
//                        inIO[keys[index]] = (coilBytes[keys[index] / 8] & (1 << keys[index] % 8)) > 0 ? true : false;
//                    }
//                }
//                catch
//                {
//                    LogWriter.Instance.LogError("By User IO Get I/F Error");
//                    continue;
//                }
//            }
//        }

//        public bool GetInValue(int index)
//        {
//            if (index < 0) return false;
//            return inIO[index];
//        }

//        public bool GetOutValue(int index)
//        {
//            if (index < 0) return false;
//            return outIO[index];
//        }

//        public bool SetInValue(int index, bool value)
//        {
//            if (index < 0) return false;
//            inIO[index] = value;
//            return true;
//        }

//        public bool SetOutValue(int index, bool value)
//        {
//            if (index < 0) return false;
//            if (base.modbusRTU == null || !base.modbusRTU.Connected) return false;
//            bool writeSuccess = false;
//            for (int i = 0; i < 3; i++)
//                if (modbusRTU.WriteCoil(ID, (ushort)(offsetCoils + index), value)) { writeSuccess = true; break; }
//            if (!writeSuccess) return false;
//            outIO[index] = value;
//            return true;
//        }

//        public new void Dispose()
//        {
//            ThreadStop();
//        }
//    }
//}
