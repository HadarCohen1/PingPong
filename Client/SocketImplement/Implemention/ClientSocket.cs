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

namespace Client.SocketImplement.Implemention
{
   
    public class ClientSyncSocket : IClientSocket
    {
        public IPEndPoint IPEndPoint;
        public Socket Sender;
        private byte[] _bytes;
        private ManualResetEvent _connectDone;
        private ManualResetEvent _sendDone;
        private ManualResetEvent _receiveDone;

        private static String response = String.Empty;

        public ClientSyncSocket(IPEndPoint iPEndPoint)
        {
            _connectDone = new ManualResetEvent(false);
            _sendDone = new ManualResetEvent(false);
            _receiveDone = new ManualResetEvent(false);
            _bytes = new byte[1024];
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
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                _connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public object Receive()
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
                    _receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(object data)
        {
            byte[] byteData = JsonSerializer.SerializeToUtf8Bytes(data);

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
                _sendDone.Set();
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
