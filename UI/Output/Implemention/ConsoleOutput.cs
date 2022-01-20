using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Output.Abstract;

namespace UI.Output.Implemention
{
    public class ConsoleOutput : IOutput<string>
    {
        public void Print(string toPrint)
        {
            Console.WriteLine(toPrint);
        }
    }
}
