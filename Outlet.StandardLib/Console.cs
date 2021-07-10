using Outlet.ForeignFunctions;
using System;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "console")]
    public static class ConsoleClass
    {
        /// <summary>
        /// When true program will not be checked or executed, the tokens from the lexical analysis stage will be output
        /// </summary>
        public static bool LexingMode { get; set; }

        [ForeignFunction(Name = "print")]
        public static void PrintThis(SystemInterface sys, object text) => sys.StdOut(text.ToString());

        [ForeignFunction(Name = "read")]
        public static string ReadThis(SystemInterface sys) => sys.StdIn();

        [ForeignFunction(Name = "toggleLexMode")]
        public static void ToggleLexMode(SystemInterface sys, bool value) => LexingMode = true; 
    }
}
