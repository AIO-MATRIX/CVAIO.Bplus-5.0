//using System;
//using System.Collections;
//using System.Text;
//using System.IO;
//using System.Threading;
//using System.IO.Ports;
//using CVAiO.Bplus.Core;
//using System.ComponentModel;
//using System.Diagnostics;

//namespace CVAiO.Bplus.Interface.ByUser
//{
//    [Serializable]
//    public class ModbusRTU
//    {
//        private const byte fctReadCoil = 1;
//        private const byte fctReadDiscreteInputs = 2;
//        private const byte fctReadHoldingRegister = 3;
//        private const byte fctReadInputRegister = 4;
//        private const byte fctWriteSingleCoil = 5;
//        private const byte fctWriteSingleRegister = 6;
//        private const byte fctWriteMultipleCoils = 15;
//        private const byte fctWriteMultipleRegister = 16;
//        private const byte fctReadWriteMultipleRegister = 23;

//        public const byte excIllegalFunction = 1;
//        public const byte excIllegalDataAdr = 2;
//        public const byte excIllegalDataVal = 3;
//        public const byte excSlaveDeviceFailure = 4;
//        public const byte excAck = 5;
//        public const byte excSlaveIsBusy = 6;
//        public const byte excGatePathUnavailable = 10;
//        public const byte excExceptionNotConnected = 253;
//        public const byte excExceptionConnectionLost = 254;
//        public const byte excExceptionTimeout = 255;
//        private const byte excExceptionOffset = 128;
//        private const byte excSendFailt = 100;

//        private ushort timeout;
//        private bool connected;

//        [NonSerialized]
//        private bool isReceivedSuccess = false;
//        [NonSerialized]
//        private bool isReceivedFail = false;
//        private Stopwatch stopWatch = new Stopwatch();

//        [NonSerialized]
//        protected static object interfaceLock = new object();

//        [NonSerialized]
//        protected SerialPort serialPort;

//        [Browsable(false)]
//        public ushort Timeout
//        {
//            get { return timeout; }
//            set { timeout = value; }
//        }

//        public bool Connected
//        {
//            get { return connected; }
//        }

//        public ModbusRTU()
//        {
//            serialPort = new SerialPort();
//            timeout = 1000;
//            serialPort.ReadTimeout = timeout;
//            serialPort.WriteTimeout = timeout;
//        }

//        public override string ToString()
//        {
//            return "";
//        }

//        public void Dispose()
//        {
//            if (serialPort != null)
//                serialPort.Dispose();
//            serialPort = null;
//        }


//        public bool CheckConnected()
//        {
//            if (serialPort == null) return false;
//            return connected = serialPort.IsOpen;
//        }

//        public bool Reconnect(string portName, EBaudRate baudRate, Parity parity, int dataBits, StopBits stopBits)
//        {
//            try
//            {
//                if (serialPort == null) return false;
//                if (serialPort.IsOpen) serialPort.Close();
//                serialPort = new SerialPort(portName, (int)baudRate, parity, dataBits, stopBits);
//                serialPort.ReadTimeout = 1000;
//                serialPort.WriteTimeout = 1000;
//                serialPort.ReceivedBytesThreshold = 5;
//                serialPort.DataReceived += SerialPort_DataReceived;
//                serialPort.ErrorReceived += SerialPort_ErrorReceived;
//                if (serialPort.IsOpen)
//                {
//                    serialPort.Close();
//                    System.Threading.Thread.Sleep(100);
//                }
//                serialPort.Open();
//                if (serialPort.IsOpen) return true;
//                else
//                {
//                    serialPort.Close();
//                    throw new Exception("Serial Port open fail");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message.ToString());
//                return false;
//            }
//        }

//        public bool Connect(string portName, EBaudRate baudRate, Parity parity, int dataBits, StopBits stopBits)
//        {
//            try
//            {
//                connected = true;
//                lock (interfaceLock)
//                {
//                    if (serialPort != null) serialPort.Close();
//                    serialPort = new SerialPort(portName, (int)baudRate, parity, dataBits, stopBits);
//                    serialPort.ReadTimeout = 1000;
//                    serialPort.WriteTimeout = 1000;
//                    serialPort.DataReceived += SerialPort_DataReceived;
//                    serialPort.ErrorReceived += SerialPort_ErrorReceived;
//                    serialPort.ReceivedBytesThreshold = 5;
//                    if (serialPort.IsOpen)
//                    {
//                        serialPort.Close();
//                        System.Threading.Thread.Sleep(100);
//                    }
//                    serialPort.Open();
//                    if (serialPort.IsOpen) return true;
//                    else
//                    {
//                        serialPort.Close();
//                        throw new Exception("Serial Port open fail");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogWriter.Instance.LogError("ModbusRTU: " + ex.ToString());
//                return false;
//            }
//        }

//        public bool ReadCoils(ushort id, ushort startAddress, ushort numInputs, ref byte[] values)
//        {
//            lock (interfaceLock)
//            {
//                byte[] sendPacket = new byte[1024];
//                sendPacket[0] = (byte)id;
//                sendPacket[1] = fctReadCoil;
//                sendPacket[2] = (byte)(startAddress >> 8);
//                sendPacket[3] = (byte)(startAddress & 0xFF);
//                sendPacket[4] = (byte)(numInputs >> 8);
//                sendPacket[5] = (byte)(numInputs & 0xFF);
//                ushort crc = CalculateCRC(sendPacket, 6);
//                sendPacket[6] = (byte)(crc & 0xFF);
//                sendPacket[7] = (byte)(crc >> 8);

//                System.Diagnostics.Debug.WriteLine("ReadCoils");
//                if (serialPort == null || !serialPort.IsOpen) return false;
//                serialPort.Write(sendPacket, 0, 8);
//                try
//                {
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    byte[] receiveByte = new byte[0];
//                    while (true)
//                    {
//                        if (isReceivedSuccess)
//                        {
//                            isReceivedSuccess = false;
//                            int byte2Read = serialPort.BytesToRead;
//                            byte[] rcvPacket = new byte[byte2Read];
//                            int readByte = serialPort.Read(rcvPacket, 0, rcvPacket.Length);
//                            receiveByte = ConcatByte(rcvPacket, receiveByte);
//                            if (receiveByte.Length < 1) continue;
//                            if (rcvPacket[0] != id)
//                                throw new Exception("ReadCoils: ID Mismatched");
//                            if (receiveByte.Length < 2) continue;
//                            if (rcvPacket[1] != fctReadCoil)
//                                throw new Exception("ReadCoils: Function Mistmatched");
//                            if (receiveByte.Length < 3) continue;
//                            ushort byteCount = (ushort)rcvPacket[2];
//                            if (receiveByte.Length < byteCount + 3) continue;
//                            crc = CalculateCRC(rcvPacket, byteCount + 3);
//                            if (receiveByte.Length < byteCount + 5) continue;
//                            if (rcvPacket[byteCount + 3] != (byte)(crc & 0xFF) || rcvPacket[byteCount + 3 + 1] != (byte)(crc >> 8)) throw new Exception("ReadCoils: Check SUM Mistmatched");
//                            Array.Copy(rcvPacket, 3, values, 0, byteCount);
//                            serialPort.DiscardInBuffer();
//                            return true;
//                        }
//                        if (isReceivedFail)
//                        {
//                            isReceivedFail = false;
//                            return false;
//                        }
//                        if (stopWatch.ElapsedMilliseconds > 2000)
//                            throw new Exception("ReadCoils: Receive Time Out");
//                        Thread.Sleep(2);
//                    }

//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogError("ModbusRTU: fctReadCoils " + ex.ToString());
//                    serialPort.DiscardInBuffer();
//                    return false;
//                }
//            }
//        }

//        public bool WriteCoil(ushort id, ushort startAddress, bool coilState)
//        {
//            lock (interfaceLock)
//            {
//                byte[] sendPacket = new byte[1024];
//                sendPacket[0] = (byte)id;
//                sendPacket[1] = fctWriteSingleCoil;
//                sendPacket[2] = (byte)(startAddress >> 8);
//                sendPacket[3] = (byte)(startAddress & 0xFF);
//                sendPacket[4] = (byte)(coilState ? 0xFF : 0x00);
//                sendPacket[5] = 0;
//                ushort crc = CalculateCRC(sendPacket, 6);
//                sendPacket[6] = (byte)(crc & 0xFF);
//                sendPacket[7] = (byte)(crc >> 8);

//                System.Diagnostics.Debug.WriteLine("WriteCoil");
//                if (serialPort == null || !serialPort.IsOpen) return false;
//                serialPort.Write(sendPacket, 0, 8);
//                try
//                {
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    byte[] receiveByte = new byte[0];
//                    while (true)
//                    {
//                        if (isReceivedSuccess)
//                        {
//                            isReceivedSuccess = false;
//                            int byte2Read = serialPort.BytesToRead;
//                            byte[] rcvPacket = new byte[byte2Read];
//                            int readByte = serialPort.Read(rcvPacket, 0, rcvPacket.Length);
//                            receiveByte = ConcatByte(rcvPacket, receiveByte);
//                            if (receiveByte.Length < 1) continue;
//                            if (rcvPacket[0] != id)
//                                throw new Exception("WriteCoil: ID Mismatched");
//                            if (receiveByte.Length < 2) continue;
//                            if (rcvPacket[1] != fctWriteSingleCoil)
//                                throw new Exception("WriteCoil: Function Mistmatched");
//                            if (receiveByte.Length < 6) continue;
//                            crc = CalculateCRC(rcvPacket, 6);
//                            if (receiveByte.Length < 8) continue;
//                            if (rcvPacket[6] != (byte)(crc & 0xFF) || rcvPacket[7] != (byte)(crc >> 8)) throw new Exception("ReadCoils: Check SUM Mistmatched");
//                            serialPort.DiscardInBuffer();
//                            return true;

//                        }
//                        if (isReceivedFail)
//                        {
//                            isReceivedFail = false;
//                            return false;
//                        }
//                        if (stopWatch.ElapsedMilliseconds > 2000) throw new Exception("ReadCoils: Receive Time Out");
//                        Thread.Sleep(2);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogError("ModbusRTU: fctWriteSingleCoil " + ex.ToString());
//                    serialPort.DiscardInBuffer();
//                    return false;
//                }
//            }
//        }

//        public bool WriteMultipleCoils(ushort id, ushort startAddress, ushort numcoils, byte[] values)
//        {
//            lock (interfaceLock)
//            {
//                byte[] sendPacket = new byte[1024];
//                sendPacket[0] = (byte)id;
//                sendPacket[1] = fctWriteMultipleCoils;
//                sendPacket[2] = (byte)(startAddress >> 8);
//                sendPacket[3] = (byte)(startAddress & 0xFF);
//                sendPacket[4] = (byte)(numcoils >> 8);
//                sendPacket[5] = (byte)(numcoils & 0xFF);
//                sendPacket[6] = (byte)(values.Length);
//                Array.Copy(values, 0, sendPacket, 7, values.Length);
//                ushort crc = CalculateCRC(sendPacket, 7 + values.Length);
//                sendPacket[7 + values.Length] = (byte)(crc & 0xFF);
//                sendPacket[7 + values.Length + 1] = (byte)(crc >> 8);

//                serialPort.Write(sendPacket, 0, 7 + values.Length + 2);
//                if (serialPort == null || !serialPort.IsOpen) return false;
//                try
//                {
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    byte[] receiveByte = new byte[0];
//                    while (true)
//                    {
//                        if (isReceivedSuccess)
//                        {
//                            isReceivedSuccess = false;
//                            int byte2Read = serialPort.BytesToRead;
//                            byte[] rcvPacket = new byte[byte2Read];
//                            int readByte = serialPort.Read(rcvPacket, 0, rcvPacket.Length);
//                            receiveByte = ConcatByte(rcvPacket, receiveByte);
//                            if (receiveByte.Length < 1) continue;
//                            if (rcvPacket[0] != id)
//                                throw new Exception("WriteMultipleCoils: ID Mismatched");
//                            if (receiveByte.Length < 2) continue;
//                            if (rcvPacket[1] != fctWriteMultipleCoils)
//                                throw new Exception("WriteMultipleCoils: Function Mistmatched");
//                            if (receiveByte.Length < 6) continue;
//                            crc = CalculateCRC(rcvPacket, 6);
//                            if (receiveByte.Length < 8) continue;
//                            if (rcvPacket[6] != (byte)(crc & 0xFF) || rcvPacket[7] != (byte)(crc >> 8)) throw new Exception("ReadCoils: Check SUM Mistmatched");
//                            serialPort.DiscardInBuffer();
//                            return true;
//                        }
//                        if (isReceivedFail)
//                        {
//                            isReceivedFail = false;
//                            return false;
//                        }
//                        if (stopWatch.ElapsedMilliseconds > 2000) throw new Exception("ReadCoils: Receive Time Out");
//                        Thread.Sleep(2);
//                    }

//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogError("ModbusRTU: fctReadCoils " + ex.ToString());
//                    serialPort.DiscardInBuffer();
//                    return false;
//                }
//            }
//        }

//        public bool ReadHoldingRegister(ushort id, ushort startAddress, ushort numRegisters, ref byte[] values)
//        {
//            lock (interfaceLock)
//            {
//                byte[] sendPacket = new byte[1024];
//                sendPacket[0] = (byte)id;
//                sendPacket[1] = fctReadHoldingRegister;
//                sendPacket[2] = (byte)(startAddress >> 8);
//                sendPacket[3] = (byte)(startAddress & 0xFF);
//                sendPacket[4] = (byte)(numRegisters >> 8);
//                sendPacket[5] = (byte)(numRegisters & 0xFF);
//                ushort crc = CalculateCRC(sendPacket, 6);
//                sendPacket[6] = (byte)(crc & 0xFF);
//                sendPacket[7] = (byte)(crc >> 8);

//                System.Diagnostics.Debug.WriteLine("ReadHoldingRegister");
//                if (serialPort == null || !serialPort.IsOpen) return false;
//                serialPort.Write(sendPacket, 0, 8);
//                try
//                {
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    byte[] receiveByte = new byte[0];
//                    while (true)
//                    {
//                        if (isReceivedSuccess)
//                        {
//                            isReceivedSuccess = false;
//                            int byte2Read = serialPort.BytesToRead;
//                            byte[] rcvPacket = new byte[byte2Read];
//                            int readByte = serialPort.Read(rcvPacket, 0, rcvPacket.Length);
//                            receiveByte = ConcatByte(rcvPacket, receiveByte);
//                            if (receiveByte.Length < 1) continue;
//                            if (rcvPacket[0] != id)
//                                throw new Exception("ReadHoldingRegister: ID Mismatched");
//                            if (receiveByte.Length < 2) continue;
//                            if (rcvPacket[1] != fctReadHoldingRegister)
//                                throw new Exception("ReadHoldingRegister: Function Mistmatched");
//                            if (receiveByte.Length < 3) continue;
//                            ushort registerCount = (ushort)rcvPacket[2];
//                            if (receiveByte.Length < registerCount + 3) continue;
//                            crc = CalculateCRC(rcvPacket, registerCount + 3);
//                            if (receiveByte.Length < registerCount + 5) continue;
//                            if (rcvPacket[registerCount + 3] != (byte)(crc & 0xFF) || rcvPacket[registerCount + 3 + 1] != (byte)(crc >> 8)) throw new Exception("ReadHoldingRegister: Check SUM Mistmatched");
//                            Array.Copy(rcvPacket, 3, values, 0, registerCount);
//                            serialPort.DiscardInBuffer();
//                            return true;
//                        }
//                        if (isReceivedFail)
//                        {
//                            isReceivedFail = false;
//                            return false;
//                        }
//                        if (stopWatch.ElapsedMilliseconds > 2000) throw new Exception("ReadHoldingRegister: Receive Time Out");
//                        Thread.Sleep(2);
//                    }

//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogError("ModbusRTU: fctReadHoldingRegister " + ex.ToString());
//                    serialPort.DiscardInBuffer();
//                    return false;
//                }
//            }
//        }

//        public bool WriteMultipleRegister(ushort id, ushort startAddress, ushort numRegisters, byte[] values)
//        {
//            lock (interfaceLock)
//            {
//                byte[] sendPacket = new byte[1024];
//                sendPacket[0] = (byte)id;
//                sendPacket[1] = fctWriteMultipleRegister;
//                sendPacket[2] = (byte)(startAddress >> 8);
//                sendPacket[3] = (byte)(startAddress & 0xFF);
//                sendPacket[4] = (byte)(numRegisters >> 8);
//                sendPacket[5] = (byte)(numRegisters & 0xFF);
//                sendPacket[6] = (byte)(values.Length);
//                Array.Copy(values, 0, sendPacket, 7, values.Length);
//                ushort crc = CalculateCRC(sendPacket, 7 + values.Length);
//                sendPacket[7 + values.Length] = (byte)(crc & 0xFF);
//                sendPacket[7 + values.Length + 1] = (byte)(crc >> 8);

//                serialPort.Write(sendPacket, 0, 7 + values.Length + 3);
//                if (serialPort == null || !serialPort.IsOpen) return false;
//                try
//                {
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    byte[] receiveByte = new byte[0];
//                    while (true)
//                    {
//                        if (isReceivedSuccess)
//                        {
//                            isReceivedSuccess = false;
//                            int byte2Read = serialPort.BytesToRead;
//                            byte[] rcvPacket = new byte[byte2Read];
//                            int readByte = serialPort.Read(rcvPacket, 0, rcvPacket.Length);
//                            receiveByte = ConcatByte(rcvPacket, receiveByte);
//                            if (receiveByte.Length < 1) continue;
//                            if (rcvPacket[0] != id)
//                                throw new Exception("WriteMultipleRegister: ID Mismatched");
//                            if (receiveByte.Length < 2) continue;
//                            if (rcvPacket[1] != fctWriteMultipleRegister)
//                                throw new Exception("WriteMultipleRegister: Function Mistmatched");
//                            if (receiveByte.Length < 6) continue;
//                            crc = CalculateCRC(rcvPacket, 6);
//                            if (receiveByte.Length < 8) continue;
//                            if (rcvPacket[6] != (byte)(crc & 0xFF) || rcvPacket[7] != (byte)(crc >> 8)) throw new Exception("WriteMultipleRegister: Check SUM Mistmatched");
//                            serialPort.DiscardInBuffer();
//                            return true;
//                        }
//                        if (isReceivedFail)
//                        {
//                            isReceivedFail = false;
//                            return false;
//                        }
//                        if (stopWatch.ElapsedMilliseconds > 2000) throw new Exception("WriteMultipleRegister: Receive Time Out");
//                        Thread.Sleep(2);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogError("ModbusRTU: fctWriteMultipleRegister " + ex.ToString());
//                    serialPort.DiscardInBuffer();
//                    return false;
//                }
//            }
//        }

//        private byte[] ConcatByte(byte[] a, byte[] b)
//        {
//            byte[] output = new byte[a.Length + b.Length];
//            for (int i = 0; i < a.Length; i++)
//                output[i] = a[i];
//            for (int j = 0; j < b.Length; j++)
//                output[a.Length + j] = b[j];
//            return output;
//        }

//        private ushort CalculateCRC(byte[] data, int len)
//        {
//            int crc = 0xFFFF, i = 0;
//            while (len-- != 0)
//            {
//                crc ^= data[i++];
//                for (int k = 0; k < 8; k++)
//                    crc = ((crc & 0x01) != 0) ? (crc >> 1) ^ 0xA001 : (crc >> 1);
//            }
//            return (ushort)(crc);
//        }

//        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
//        {
//            isReceivedSuccess = true;
//        }
//        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
//        {
//            isReceivedFail = true;
//        }
//    }
//}
