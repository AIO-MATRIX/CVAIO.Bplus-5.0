using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Interface.ByUser
{
    [Serializable]
    public class ByUser
    {
        public ByUser()
        {
        }
        public override string ToString()
        {
            return "";
        }
        public void Dispose()
        {
        }
        bool mIsConnected;
        public bool IsConnected { get { return mIsConnected; } }
        public bool CheckConnected() { return true; }
        public bool Reconnect()
        {
            try
            {
                // try to reconnect if disconnected
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Connect()
        {
            try
            {
                // Connect to the device
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool WriteValue(int index, int value)
        {
            return false;
        }
        public bool WriteValue(int index, float value)
        {
            return false;
        }
        public bool WriteValue(int index, string value)
        {
            return false;
        }
        public bool WriteValues(int index, float[] values)
        {
            return false;
        }
        public int ReadInt(int index)
        {
            int readInt = 0;
            return readInt;
        }
        public float ReadFloat(int index)
        {
            float readFloat = 0;

            return readFloat;
        }

        public string ReadString(int index)
        {
            string readString = "";
            return readString;
        }

    }
}
