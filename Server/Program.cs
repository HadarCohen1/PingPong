using PingPong.Server.Implemention;
using PingPong.Server.SocketImplement.Implemention;
using System;
using System.Net;
using UI.Output.Implemention;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = int.Parse(args[0]);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            var serverSocket = new ServerSocket(localEndPoint, new ConsoleOutput());
            var tcpServer = new TcpServer(serverSocket);

            tcpServer.StartListening();
        }
    }
}
