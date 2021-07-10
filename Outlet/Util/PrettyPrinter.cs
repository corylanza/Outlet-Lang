using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util
{
    public static class PrettyPrinter
    {
        public static void PrettyPrint(params (ConsoleColor color, object value)[] toPrint)
        {
            foreach(var (color, value) in toPrint)
            {
                Console.ForegroundColor = color;
                Console.Write(value);
            }
            Console.WriteLine();
        }
    }
}
