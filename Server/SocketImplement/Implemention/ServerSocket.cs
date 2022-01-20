using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;
using UI.Output.Abstract;
using System.Text.Json;

namespace PingPong.Server.SocketImplement.Implemention
{
    public class ServerSocket : ISocket
    {
        public IPEndPoint IPEndPoint;
        public string Data;
        public Socket Listener;
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private IOutput<string> _printer;
        
        public ServerSocket(IPEndPoint iPEndPoint, IOutput<string> printer)
        {
            _printer = printer;
            IPEndPoint = iPEndPoint;
            Data = null;
            Listener = new Socket(IPEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening()
        {
            try
            {
                Listener.Bind(IPEndPoint);
                Listener.Listen();
                while (true)
                { 
                    allDone.Reset();
                    _printer.Print("Waiting for a connection...");
                    Listener.BeginAccept(new AsyncCallback(AcceptCallback),Listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                _printer.Print(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {  
            allDone.Set();
  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            Receive(handler);
        }

        public void Receive(Socket handler)
        {
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty; 
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            { 
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
 
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    _printer.Print($"Read {content.Length} bytes from socket. \n Data : {content}");
                    Send(handler, content);
                }
                else
                {  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public void Send(Socket handler, object data)
        { 
            byte[] byteData = JsonSerializer.SerializeToUtf8Bytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                _printer.Print($"Sent {bytesSent} bytes to client.");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }   
        }
    }
}
