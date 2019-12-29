using Outlet.Operands;
using System.Collections.Generic;

namespace Outlet.Interpreting
{
    public class StackFrame
    {
        public Stack<Operand> LocalVariables = new Stack<Operand>();
        public string Call { get; private set; }

        public StackFrame(string call)
        {
            Call = call;
        }
    }
}
