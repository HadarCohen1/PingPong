using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Output.Abstract
{
    public interface IOutput<T>
    {
        public void Print(T toPrint);
    }
}
