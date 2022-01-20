using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingPong.Client.SocketImplement.Abstract;
using PingPong.Output.Abstract;
using PingPong.Input.Abstract;

namespace PingPong.Client.Implemention
{
    public class PingPongClient : IClient
    {
        public IClientSocket ClientSocket;
        public IInput<string> Reader;
        public IOutput<string> Printer;

        public PingPongClient(IClientSocket clientSocket, IInput<string> reader, IOutput<string> printer)
        {
            ClientSocket = clientSocket;
            Reader = reader;
            Printer = printer;
        }

        public void StartClient()
        {
            ClientSocket.Connect();
            while(true)
            {
                Console.WriteLine("enter");
                ClientSocket.Send(Reader.Read());
                Printer.Print(ClientSocket.Receive());
            }
        }
    }
}
