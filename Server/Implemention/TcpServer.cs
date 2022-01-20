using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server.Implemention
{
    public class TcpServer : IServer
    {
        public ISocket Socket { get; set; }

        public TcpServer(ISocket socket)
        {
            Socket = socket;
        }

        public void StartListening()
        { 
            Socket.StartListening();
        }
    }
}
