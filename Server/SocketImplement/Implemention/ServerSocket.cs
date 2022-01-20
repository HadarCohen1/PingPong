using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;

namespace PingPong.Server.SocketImplement.Implemention
{
    public class ServerSocket : ISocket
    {
        public IPEndPoint IPEndPoint;
        public string Data;
        public Socket Listener;
        public Socket Handler;
        private byte[] _bytes;
        public ManualResetEvent allDone = new ManualResetEvent(false);
        
        public ServerSocket(IPEndPoint iPEndPoint)
        {
            _bytes = new Byte[1024];
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
                    Console.WriteLine("Waiting for a connection...");
                    Listener.BeginAccept(new AsyncCallback(AcceptCallback),Listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {  
            allDone.Set();
  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public object Receive()
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection..."); 
                Socket handler = Listener.Accept();
                Data = null;
                while (true)
                {
                    _bytes = new byte[1024];
                    int bytesRec = Handler.Receive(_bytes);
                    Data += Encoding.ASCII.GetString(_bytes, 0, bytesRec);
                    if (Data.IndexOf("<EOF>") > -1)
                    {
                        return Data;
                    }
                }
            }
        }

        public void Send(byte[] data)
        {
            Handler.Send(data);
        }

        public void Close()
        {
            Handler.Shutdown(SocketShutdown.Both);
            Handler.Close();
        }
    }
}
