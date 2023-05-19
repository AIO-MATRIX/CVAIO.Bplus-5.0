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
    public class ALP_4CH : LightingBase
    {
        public static int ChannelCount = 4;

        public ALP_4CH()
            : base()
        {
            ControllerName = "ALP-4CH";
            MaxChannel = 4;
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
                if (base.serialPort.IsOpen)
                    base.serialPort.Write(SendData);
            }
            catch (Exception ex)
            {
                base.IsOpened = false;
            }
            return true;
        }

        public override bool LightOff(EChannel channel)
        {
            string SendData = "";

            SendData = Header + ((int)channel).ToString("00") + "000" + "\r\n";
            try
            {
                if (base.serialPort.IsOpen)
                    base.serialPort.Write(SendData);
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
