using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server.Implemention
{
    public class TcpServer<T> : IServer<T>
    {
        public ISocket<T> Socket { get; set; }

        public TcpServer(ISocket<T> socket)
        {   
            Socket = socket;
        }

        public void StartListening()
        { 
            Socket.StartListening();
        }
    }
}
