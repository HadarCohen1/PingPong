using PingPong.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Implemention
{
    class ObjectServer<T> : IServer<T>
    {
        public ISocket<T> Socket { get; set; }
        public object ToCreate;

        public ObjectServer(ISocket<T> socket, object toCreate)
        {
            Socket = socket;
            ToCreate = toCreate;
        }

        public void StartListening()
        {
            Socket.StartListening();
        }
    }
}
