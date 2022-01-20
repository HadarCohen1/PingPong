using Client.Implemention;
using Client.SocketImplement.Implemention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UI.Input.Abstract;
using UI.Output.Abstract;


namespace Client
{
    public class Bootstraper
    {
        public void Start(IOutput<string> output, IInput<string> input)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            output.Print("Enter IP Address:");
            string ipInput = input.Read();
            string port = ipInput.Substring(ipInput.IndexOf(':')+1);
            string ipAddr = ipInput.Substring(0, ipInput.Length - (port.Length + 1));
            byte[] ip = Encoding.ASCII.GetBytes(ipAddr);

            IPAddress ipAddress = IPAddress.Parse(ipAddr);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(port));

            ClientSocket clientSocket = new ClientSocket(localEndPoint, output);
            PingPongClient pingPongClient = new PingPongClient(clientSocket, input, output);

            pingPongClient.StartClient();

        }
    }
}
