using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server
{
    public interface ISocket<T>
    {
        public void StartListening();
        public void Receive(T handler);
        public void Send(T handler, object data);
    }
}
