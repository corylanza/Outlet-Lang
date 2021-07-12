using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.ForeignFunctions
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
