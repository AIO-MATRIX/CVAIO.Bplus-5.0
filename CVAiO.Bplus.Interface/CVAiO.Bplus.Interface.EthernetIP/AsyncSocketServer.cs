using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Interface.EthernetIP
{
    public class StateObject
    {
        private const int BUFFER_SIZE = 2048;

        private Socket worker;
        private byte[] buffer;

        public StateObject(Socket worker)
        {
            this.worker = worker;
            this.buffer = new byte[BUFFER_SIZE];
        }

        public Socket Worker
        {
            get { return this.worker; }
            set { this.worker = value; }
        }

        public byte[] Buffer
        {
            get { return this.buffer; }
            set { this.buffer = value; }
        }

        public int BufferSize
        {
            get { return BUFFER_SIZE; }
        }
    }
    public class AsyncSocketErrorEventArgs : EventArgs
    {
        private readonly Exception exception;
        private readonly int id = 0;

        public AsyncSocketErrorEventArgs(int id, Exception exception)
        {
            this.id = id;
            this.exception = exception;
        }

        public Exception AsyncSocketException
        {
            get { return this.exception; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }
    public class AsyncSocketConnectionEventArgs : EventArgs
    {
        private readonly int id = 0;

        public AsyncSocketConnectionEventArgs(int id)
        {
            this.id = id;
        }

        public int ID
        {
            get { return this.id; }
        }
    }
    public class AsyncSocketSendEventArgs : EventArgs
    {
        private readonly int id = 0;
        private readonly int sendBytes;

        public AsyncSocketSendEventArgs(int id, int sendBytes)
        {
            this.id = id;
            this.sendBytes = sendBytes;
        }

        public int SendBytes
        {
            get { return this.sendBytes; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }
    public class AsyncSocketReceiveEventArgs : EventArgs
    {
        private readonly int id = 0;
        private readonly int receiveBytes;
        private readonly byte[] receiveData;

        public AsyncSocketReceiveEventArgs(int id, int receiveBytes, byte[] receiveData)
        {
            this.id = id;
            this.receiveBytes = receiveBytes;
            this.receiveData = receiveData;
        }

        public int ReceiveBytes
        {
            get { return this.receiveBytes; }
        }

        public byte[] ReceiveData
        {
            get { return this.receiveData; }
        }

        public int ID
        {
            get { return this.id; }
        }
    }
    public class AsyncSocketAcceptEventArgs : EventArgs
    {
        private readonly Socket client;

        public AsyncSocketAcceptEventArgs(Socket client)
        {
            this.client = client;
        }

        public Socket Worker
        {
            get { return this.client; }
        }
    }

    public delegate void AsyncSocketErrorEventHandler(object sender, AsyncSocketErrorEventArgs e);
    public delegate void AsyncSocketConnectEventHandler(object sender, AsyncSocketConnectionEventArgs e);
    public delegate void AsyncSocketCloseEventHandler(object sender, AsyncSocketConnectionEventArgs e);
    public delegate void AsyncSocketSendEventHandler(object sender, AsyncSocketSendEventArgs e);
    public delegate void AsyncSocketReceiveEventHandler(object sender, AsyncSocketReceiveEventArgs e);
    public delegate void AsyncSocketAcceptEventHandler(object sender, AsyncSocketAcceptEventArgs e);

    public class AsyncSocketClass
    {
        #region Fields
        protected int id;
        public byte[] ReadPacket = new byte[2048];
        public int ReadSize;
        #endregion

        #region Properties
        public int ID
        {
            get { return this.id; }
        }
        #endregion

        public event AsyncSocketErrorEventHandler OnError;
        public event AsyncSocketConnectEventHandler OnConnect;
        public event AsyncSocketCloseEventHandler OnClose;
        public event AsyncSocketSendEventHandler OnSend;
        public event AsyncSocketReceiveEventHandler OnReceive;
        public event AsyncSocketAcceptEventHandler OnAccept;

        public AsyncSocketClass()
        {
            this.id = -1;
        }

        public AsyncSocketClass(int id)
        {
            this.id = id;
        }

        protected virtual void ErrorOccured(AsyncSocketErrorEventArgs e)
        {
            AsyncSocketErrorEventHandler handler = OnError;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Connected(AsyncSocketConnectionEventArgs e)
        {
            AsyncSocketConnectEventHandler handler = OnConnect;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Closed(AsyncSocketConnectionEventArgs e)
        {
            AsyncSocketCloseEventHandler handler = OnClose;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Sent(AsyncSocketSendEventArgs e)
        {
            AsyncSocketSendEventHandler handler = OnSend;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Received(AsyncSocketReceiveEventArgs e)
        {
            AsyncSocketReceiveEventHandler handler = OnReceive;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void Accepted(AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketAcceptEventHandler handler = OnAccept;

            if (handler != null)
                handler(this, e);
        }

        public bool CheckConnected(Socket s, string ipAddress)
        {
            if (s == null || ipAddress == null) return false;

            bool ConnectionState = true;

            if (s.Connected)
            {
                try
                {
                    Ping pingTest = new Ping();
                    PingReply reply = pingTest.Send(ipAddress);
                    if (reply.Status != IPStatus.Success) ConnectionState = false;
                }
                catch (PingException) { ConnectionState = false; }

                if (s.Poll(5000, SelectMode.SelectRead) && (s.Available == 0))
                {
                    ConnectionState = false;
                }
            }
            else
            {
                if (s.IsBound) ConnectionState = true;
                else ConnectionState = false;
            }

            return ConnectionState;
        }
    }

    [Serializable]
    public class AsyncSocketClient : AsyncSocketClass
    {
        [NonSerialized]
        private Socket conn = null;
        const int timeout = 3000;
        string ipAddress = null;

        public AsyncSocketClient(int id)
        {
            this.id = id;
        }

        public AsyncSocketClient(int id, Socket conn)
        {
            this.id = id;
            this.conn = conn;
        }

        public Socket Connection
        {
            get { return this.conn; }
            set { this.conn = value; }
        }

        public bool IsConnected
        {
            get
            {
                return CheckConnected(this.conn, this.ipAddress); ;
            }
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.ipAddress = ipAddress;
                IPAddress[] address = Dns.GetHostAddresses(ipAddress);
                if (address.Length == 0)
                {
                    Console.WriteLine("IP Is Wrong");
                    return false;
                }
                EndPoint endPoint = new IPEndPoint(address[0], port);
                conn.BeginConnect(endPoint, new AsyncCallback(OnConnectCallback), conn);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);

                ErrorOccured(eev);

                return false;
            }
            return true;

        }

        public bool Reconnect(string ipAddress, int port)
        {
            try
            {
                if (!IsConnected)
                {
                    conn.Shutdown(SocketShutdown.Both);
                    conn.Disconnect(true);
                }
                conn.Close();
                conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                conn.ReceiveTimeout = timeout;
                conn.SendTimeout = timeout;

                this.ipAddress = ipAddress;
                IPAddress[] address = Dns.GetHostAddresses(ipAddress);

                if (address.Length == 0)
                {
                    Console.WriteLine("IP Is Wrong");
                    return false;
                }

                EndPoint endPoint = new IPEndPoint(address[0], port);
                conn.BeginConnect(endPoint, new AsyncCallback(OnConnectCallback), conn);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
                return false;
            }
            return true;
        }

        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                conn = (Socket)ar.AsyncState;
                conn.EndConnect(ar);
                Receive();
                AsyncSocketConnectionEventArgs cev = new AsyncSocketConnectionEventArgs(this.id);
                Connected(cev);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        public void Receive()
        {
            try
            {
                StateObject so = new StateObject(conn);
                so.Worker.BeginReceive(so.Buffer, 0, so.BufferSize, 0, new AsyncCallback(OnReceiveCallBack), so);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        private void OnReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                int bytesRead = so.Worker.EndReceive(ar);
                AsyncSocketReceiveEventArgs rev = new AsyncSocketReceiveEventArgs(this.id, bytesRead, so.Buffer);
                if (bytesRead > 0)
                {
                    Received(rev);
                    ReadSize = rev.ReceiveBytes;
                    ReadPacket = rev.ReceiveData;
                }
                Receive();
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }
        static object socketlock = new object();
        public bool Send(byte[] buffer, int length)
        {
            try
            {
                lock (socketlock)
                {
                    conn.BeginSend(buffer, 0, length, 0, new AsyncCallback(OnSendCallBack), conn);
                }

            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
                return false;
            }

            return true;
        }

        private void OnSendCallBack(IAsyncResult ar)
        {
            try
            {
                conn = (Socket)ar.AsyncState;
                int bytesWritten = conn.EndSend(ar);
                AsyncSocketSendEventArgs sev = new AsyncSocketSendEventArgs(this.id, bytesWritten);
                Sent(sev);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        public void Close()
        {
            try
            {
                conn.Shutdown(SocketShutdown.Both);
                conn.BeginDisconnect(false, new AsyncCallback(OnCloseCallBack), conn);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        private void OnCloseCallBack(IAsyncResult ar)
        {
            try
            {
                conn = (Socket)ar.AsyncState;

                conn.EndDisconnect(ar);
                conn.Close();
                AsyncSocketConnectionEventArgs cev = new AsyncSocketConnectionEventArgs(this.id);
                Closed(cev);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }
    }
    public class AsyncSocketServer : AsyncSocketClass
    {
        private int port;
        private Socket listener = null;
        const int timeout = 2000;
        private const int backLog = 100;
        string ipAddress = null;
        public int Port
        {
            get { return this.port; }
        }

        public bool IsConnected
        {
            get
            {
                return CheckConnected(listener, this.ipAddress);
            }
        }

        public AsyncSocketServer()
        {
        }


        public void Dispose()
        {
            if (listener != null)
            {
                if (IsConnected)
                {
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Disconnect(true);
                }
                listener.Close();
                listener.Dispose();
                listener = null;
            }
        }
        public bool Connect(string ipAddress, int port)
        {
            try
            {
                this.ipAddress = ipAddress;
                this.port = port;
                IPAddress[] address = Dns.GetHostAddresses(ipAddress);

                if (address.Length == 0)
                {
                    return false;
                }

                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint endPoint = new IPEndPoint(address[0], port);
                listener.Bind(endPoint);
                listener.Listen(backLog);
                StartAccept();
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
                return false;
            }
            return true;
        }

        public bool Reconnect(string ipAddress, int port)
        {
            try
            {
                if (!IsConnected)
                {
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Disconnect(true);
                }
                listener.Close();
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress[] address = Dns.GetHostAddresses(ipAddress);
                if (address.Length == 0)
                {
                    return false;
                }

                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.ReceiveTimeout = timeout;
                listener.SendTimeout = timeout;

                EndPoint endPoint = new IPEndPoint(address[0], port);
                listener.Bind(endPoint);
                listener.Listen(backLog);
                StartAccept();
            }
            catch (Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
                return false;
            }
            return true;
        }

        private void StartAccept()
        {
            try
            {
                listener.BeginAccept(new AsyncCallback(OnListenCallBack), listener);
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        private void OnListenCallBack(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket worker = listener.EndAccept(ar);
                AsyncSocketAcceptEventArgs aev = new AsyncSocketAcceptEventArgs(worker);
                Accepted(aev);
                //StartAccept();
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }

        public void Stop()
        {
            try
            {
                if (listener != null)
                {
                    if (listener.IsBound)
                        listener.Close(100);
                }
            }
            catch (System.Exception e)
            {
                AsyncSocketErrorEventArgs eev = new AsyncSocketErrorEventArgs(this.id, e);
                ErrorOccured(eev);
            }
        }
    }
}
