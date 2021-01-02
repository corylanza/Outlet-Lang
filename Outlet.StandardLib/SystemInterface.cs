using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Outlet.StandardLib
{
    public delegate string StandardIn();
    public delegate void StandardOut(string output);
    public delegate void StandardError(Exception error);

    public class SystemInterface
    {
        public StandardIn StdIn { get; private set; }
        public StandardOut StdOut { get; private set; } 
        public StandardError StdErr { get; private set; }

        public SystemInterface(StandardIn stdin, StandardOut stdout, StandardError stderr)
        {
            (StdIn, StdOut, StdErr) = (stdin, stdout, stderr);
        }
    }
}
