using PingPong.Server.Implemention;
using PingPong.Server.SocketImplement.Implemention;
using System;
using System.Net;

namespace PingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            var serverSocket = new ServerSocket(localEndPoint);
            var tcpServer = new TcpServer(serverSocket);

            tcpServer.StartListening();
        }
    }
}
