using System;
using System.Net;
using Client.Implemention;
using Client.SocketImplement.Implemention;
using UI.Input.Implemention;
using UI.Output.Implemention;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            var printer = new ConsoleOutput();
            ClientSocket clientSocket = new ClientSocket(localEndPoint,printer);
            PingPongClient pingPongClient = new PingPongClient(clientSocket, new ConsoleInput(), printer);

            pingPongClient.StartClient();
        }
    }
}
