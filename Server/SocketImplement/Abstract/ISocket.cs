﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server
{
    public interface ISocket
    {
        public void StartListening();
        public object Receive();
        public void Send(byte[] data);
        public void Close();
    }
}
