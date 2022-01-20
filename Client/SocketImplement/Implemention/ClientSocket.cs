using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingPong.Client.SocketImplement.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.Json;
using UI.Output.Abstract;

namespace Client.SocketImplement.Implemention
{
   
    public class ClientSocket : IClientSocket
    {
        public IPEndPoint IPEndPoint;
        public Socket Sender;
        private ManualResetEvent _connectDone;
        private ManualResetEvent _sendDone;
        private ManualResetEvent _receiveDone;
        private IOutput<string> _printer;
        private String _response;

        public ClientSocket(IPEndPoint iPEndPoint, IOutput<string> printer)
        {
            _printer = printer;
            _response = String.Empty;
            _connectDone = new ManualResetEvent(false);
            _sendDone = new ManualResetEvent(false);
            _receiveDone = new ManualResetEvent(false);
            IPEndPoint = iPEndPoint;
            Sender= new Socket(IPEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            Sender.BeginConnect(IPEndPoint,
                new AsyncCallback(ConnectCallback), Sender);
            _connectDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            { 
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                _printer.Print($"Socket connected to {client.RemoteEndPoint.ToString()}");
                _connectDone.Set();
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public object Receive()
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = Sender;
                Sender.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                return _response;
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
            return _response;
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                StateObject state = (StateObject)asyncResult.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = client.EndReceive(asyncResult);

                if (bytesRead > 0)
                {
                    _response=Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (_response.Length > 1)
                    {
                        _response = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    }
                    _receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public void Send(object data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data.ToString());
            Console.WriteLine(byteData.Length);
            Sender.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Sender);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState; 
                int bytesSent = client.EndSend(ar);
                _printer.Print($"Sent {bytesSent} bytes to server.");
                _sendDone.Set();
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public void Close()
        {
            Sender.Close();
        }
    }
}
