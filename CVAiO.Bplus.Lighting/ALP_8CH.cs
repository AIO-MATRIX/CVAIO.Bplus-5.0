using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using CVAiO.Bplus.Core;

namespace CVAiO.Bplus.Lighting
{
    [Serializable]
    public class ALP_8CH : LightingBase
    {
        public static int ChannelCount = 8;

        public ALP_8CH()
            : base()
        {
            ControllerName = "ALP_8CH";
            MaxChannel = 8;
            MaxVolume = 255;
            PortName = "";
            BaudRate = EBaudRate.B9600;
            Parity = Parity.Even;
            Databits = 8;
            Stopbits = StopBits.None;
            Header = "[";
            Footer = "\r\n";
        }
        public override string ToString()
        {
            return "";
        }
        public override bool LightOn(EChannel channel, int value)
        {
            string SendData = "";
            SendData = Header + ((int)channel).ToString("00") + value.ToString("000") + "\r\n";
            try
            {
                lock (LockObject)
                {
                    if (base.serialPort.IsOpen)
                        base.serialPort.Write(SendData);
                }
            }
            catch (Exception ex)
            {
                base.IsOpened = false;
                return false;
            }
            return true;
        }

        public override bool LightOff(EChannel channel)
        {
            string SendData = "";

            SendData = Header + ((int)channel).ToString("00") + "000" + "\r\n";
            try
            {
                lock (LockObject)
                {
                    if (base.serialPort.IsOpen)
                        base.serialPort.Write(SendData);
                }
            }
            catch (Exception ex)
            {
                base.IsOpened = false;
                return false;
            }
            return true;
        }
    }
}
