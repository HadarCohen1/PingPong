using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Client.SocketImplement.Abstract
{
    public interface IClientSocket
    {
        public void Connect();
        public object Receive();
        public void Send(object data);
        public void Close();
    }
}
