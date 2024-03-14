using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Interface.EthernetIP
{
    [Serializable]
    public class IO_EthernetIP : Data_EthernetIP, IComIO, ISerializable
    {
        #region Fields
        [NonSerialized]
        Dictionary<int, bool> inIO = new Dictionary<int, bool>();
        [NonSerialized]
        Dictionary<int, bool> outIO = new Dictionary<int, bool>();
        #endregion

        #region Properties
        [Browsable(false)]
        public Dictionary<int, bool> InIO { get { return inIO; } }
        [Browsable(false)]
        public Dictionary<int, bool> OutIO { get { return outIO; } }
        #endregion

        public IO_EthernetIP()
        {
        }
        protected IO_EthernetIP(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void ThreadStart()
        {
        }

        public void ThreadStop()
        {
        }
        public bool SetOutValue(int index, bool value)
        {
            // Output: 64~127 read only
            if (index < 64 || index > 127) return false;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            if (value)
                base.coils[byteIndex] |= (byte)(1 << bitIndex);
            else
                base.coils[byteIndex] &= (byte)(~(byte)(1 << bitIndex));
            OutIO[index] = value;
            return IsConnected;
        }

        public bool SetInValue(int index, bool value)
        {
            // Input: 0~63 read / write
            if (index < 0 || index > 64) return false;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            base.coils[byteIndex] |= (byte)((value ? 1 : 0) << bitIndex);
            InIO[index] = value;
            return IsConnected;
        }

        public bool GetOutValue(int index)
        {
            if (index < 64 || index > 127) return false;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            var bit = (base.coils[byteIndex] & (1 << bitIndex));
            OutIO[index] = (bit != 0) ? true : false;
            return OutIO[index];
        }

        public bool GetInValue(int index)
        {
            if (index < 0 || index > 64) return false;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            var bit = (base.coils[byteIndex] & (1 << bitIndex));
            InIO[index] = (bit != 0) ? true : false;
            return InIO[index];
        }

        public new void Dispose()
        {
            ThreadStop();
        }
    }
}
