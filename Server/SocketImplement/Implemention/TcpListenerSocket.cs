using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PingPong.Server;
using UI.Output.Abstract;

namespace Server.SocketImplement.Implemention
{
    class TcpListenerSocket : ISocket<TcpClient>
    {
        public IPEndPoint IPEndPoint;
        public TcpListener Listener;
        private IOutput<string> _printer;
        public ManualResetEvent AllDone;

        public TcpListenerSocket(IPEndPoint iPEndPoint, IOutput<string> printer)
        {
            AllDone = new ManualResetEvent(false);
            IPEndPoint = iPEndPoint;
            Listener = new TcpListener(iPEndPoint);
            _printer = printer;
        }

        public void StartListening()
        {
            try
            {
                Listener.Start();
                while (true)
                {
                    AllDone.Reset();
                    _printer.Print("Waiting for a connection...");
                    Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), Listener);
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            AllDone.Set();
            TcpClient client = Listener.EndAcceptTcpClient(asyncResult);
            Receive(client);
        }

        public void Receive(TcpClient handler)
        {
            Byte[] bytes = new Byte[256];
            String data = null;
            while (true)
            {
                data = null;
                NetworkStream stream = handler.GetStream();
                int i = 0;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    _printer.Print($"Received: {data}");
                    data = data.ToUpper();
                    Send(handler, data);
                }
            }
        }
       
        public void Send(TcpClient handler, object data)
        {
            NetworkStream stream = handler.GetStream();
            byte[] msg = Encoding.ASCII.GetBytes(data.ToString());
            stream.Write(msg, 0, msg.Length);
            _printer.Print($"Sent: {data}");
            Receive(handler);
        }

    }
}
