using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingPong.Client.SocketImplement.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PingPong.Client.SocketImplement.Implemention
{
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class ClientSyncSocket : IClientSocket
    {
        public IPEndPoint IPEndPoint;
        public Socket Sender;
        private byte[] _bytes;
        private static ManualResetEvent connectDone =
       new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        private static String response = String.Empty;

        public ClientSyncSocket(IPEndPoint iPEndPoint)
        {
            _bytes = new byte[1024];
            IPEndPoint = iPEndPoint;
            Sender= new Socket(IPEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            Sender.BeginConnect(IPEndPoint,
                new AsyncCallback(ConnectCallback), Sender);
            connectDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string Receive()
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = Sender;

                // Begin receiving the data from the remote device.
                Sender.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return response;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            Sender.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Sender);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Close()
        {
            Sender.Close();
        }
    }
}
