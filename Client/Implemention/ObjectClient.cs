using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Abstract;
using PingPong.Client.SocketImplement.Abstract;
using UI.Input.Abstract;
using UI.Output.Abstract;

namespace Client.Implemention
{
    class ObjectClient : IClient
    {
        public IClientSocket ClientSocket;
        public IInput<string> Reader;
        public IOutput<string> Printer;
        public Object ToCreate;
        public string[] ObjectParams;
        public ObjectClient(IClientSocket clientSocket, IInput<string> reader, IOutput<string> printer, Object toCreate, string[] objectParams)
        {
            ObjectParams = objectParams;
            ClientSocket = clientSocket;
            Reader = reader;
            Printer = printer;
            ToCreate = toCreate;
        }

        public void StartClient()
        {
            ClientSocket.Connect();
            for(int i = 0; i<ObjectParams.Length; i++)
            {
                Printer.Print(Output(ObjectParams[i]));
                ClientSocket.Send(Reader.Read());
            }
            Printer.Print($"Server sent: {ClientSocket.Receive().ToString()}");
        }

        private string Output(string parameter)
        {
            return $"Please enter {parameter}";
        }
    }
}
