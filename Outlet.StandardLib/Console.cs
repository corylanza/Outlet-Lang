using Outlet.ForeignFunctions;
using System;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "console")]
    public static class ConsoleClass
    {
        [ForeignFunction(Name = "print")]
        public static void PrintThis(object text) => Console.WriteLine(text);

        [ForeignFunction(Name = "read")]
        public static string ReadThis() => Console.ReadLine();
    }
}
