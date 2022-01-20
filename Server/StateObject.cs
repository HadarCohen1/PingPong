using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class StateObject
    {
        public const int BufferSize = 1024;
  
        public byte[] buffer = new byte[BufferSize];

        public Socket workSocket = null;
    }
}
