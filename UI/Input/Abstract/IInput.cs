using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Input.Abstract
{
    public interface IInput<T>
    {
        public T Read();
    }
}
