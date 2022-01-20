using System;
using System.Net;
using Client.Implemention;
using Client.SocketImplement.Implemention;
using UI.Input.Implemention;
using UI.Output.Implemention;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstraper bootstraper = new Bootstraper();
            bootstraper.Start(new ConsoleOutput(), new ConsoleInput());
        }
    }
}
