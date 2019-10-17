using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.FFI;

namespace Outlet.StandardLib
{
    [ForeignClass("global")]
    public class Global
    {
        [ForeignFunction("print")]
        public void Print()
        {
            Console.WriteLine();
        }
    }
}
