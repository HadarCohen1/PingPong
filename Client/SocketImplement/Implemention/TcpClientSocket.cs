using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PingPong.Client.SocketImplement.Abstract;
using UI.Output.Abstract;

namespace Client.SocketImplement.Implemention
{
    class TcpClientSocket : IClientSocket
    {
        public IPEndPoint IPEndPoint;
        public TcpClient Client;
        private IOutput<string> _printer;
        private ManualResetEvent _connectDone;

        public TcpClientSocket(IPEndPoint iPEndPoint, IOutput<string> printer)
        {
            _connectDone = new ManualResetEvent(false);
            IPEndPoint = iPEndPoint;
            Client = new TcpClient();
            _printer = printer;
        }

        public void Connect()
        {
            Client.BeginConnect(IPEndPoint.Address, IPEndPoint.Port, new AsyncCallback(ConnectCallback), Client);

        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Client.EndConnect(ar);
                _printer.Print($"Client connected to {IPEndPoint.ToString()}");
                _connectDone.Set();
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }
        public void Send(object data)
        {
            try
            {
                Byte[] dataByte = System.Text.Encoding.ASCII.GetBytes(data.ToString());
                NetworkStream stream = Client.GetStream();
                stream.Write(dataByte, 0, dataByte.Length);
                _printer.Print($"Sent: {data.ToString()}");
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public object Receive()
        {
            NetworkStream stream = Client.GetStream();
            Byte[] data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            _printer.Print($"Received: {responseData}");
            return responseData;
        }


        public void Close()
        {
            Client.Close();
        }
    }
}
