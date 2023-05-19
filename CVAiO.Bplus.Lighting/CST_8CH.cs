using System;
using System.ComponentModel;
using System.IO.Ports;
using CVAiO.Bplus.Core;

namespace CVAiO.Bplus.Lighting
{
    // https://www.cstmv.com.cn/product/constant_current_controller_cpl_series

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class CST_8CH : LightingBase
    {
        public static int ChannelCount = 8;
        public CST_8CH() : base()
        {
            ControllerName = "CST_8CH";
            MaxChannel = 8;
            MaxVolume = 255;
            PortName = "";
            BaudRate = EBaudRate.B19200;
            Parity = Parity.None;
            Databits = 8;
            Stopbits = StopBits.One;
            Header = "S";
            Footer = "#";
        }
        public override string ToString()
        {
            return "";
        }
        public override bool LightOn(EChannel channel, int value)
        {
            string SendData = "";
            SendData = Header + GetChanel(channel) + value.ToString("0000") + Footer;
            try
            {
                lock (LockObject)
                {
                    //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(SendData);
                    byte[] buffer = new byte[7] { 1, 2, 3, 4, 5, 6, 7 };
                    if (base.serialPort.IsOpen)
                        base.serialPort.Write(buffer, 0, 7);
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

            SendData = Header + GetChanel(channel) + "0000" + Footer;
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
        private string GetChanel(EChannel channel)
        {
            switch (channel)
            {
                case EChannel.CH1:
                    return "A";
                case EChannel.CH2:
                    return "B";
                case EChannel.CH3:
                    return "C";
                case EChannel.CH4:
                    return "D";
                case EChannel.CH5:
                    return "E";
                case EChannel.CH6:
                    return "F";
                case EChannel.CH7:
                    return "G";
                case EChannel.CH8:
                    return "H";
                default: return "";
            }
        }
    }
}
