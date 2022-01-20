using System;
using System.Net;
using Client;
using UI.Input.Implemention;
using UI.Output.Implemention;

namespace PingPong
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
