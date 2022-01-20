using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server
{
    public interface ISocket
    {
        public void StartListening();
        public void Receive(Socket handler);
        public void Send(Socket handler, object data);
    }
}
