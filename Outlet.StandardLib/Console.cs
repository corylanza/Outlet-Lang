using Outlet.ForeignFunctions;
using System;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "console")]
    public static class ConsoleClass
    {
        [ForeignFunction(Name = "print")]
        public static void PrintThis(SystemInterface sys, object text) => sys.StdOut(text.ToString());

        [ForeignFunction(Name = "read")]
        public static string ReadThis(SystemInterface sys) => sys.StdIn();
    }
}
