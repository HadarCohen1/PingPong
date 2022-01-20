﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Server
{
    public interface IServer<T>
    {
        public ISocket<T> Socket { get; set; }
        public void StartListening();
    }
}
