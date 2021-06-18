using Outlet.AST;
using Outlet.Compiling;
using Outlet.Compiling.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Interpreting.ByteCode
{
    public partial class VirtualMachine
    {
        private readonly Stack<int> ValueStack = new();
        private readonly Dictionary<uint, int> Locals = new();

        public object? Interpret(Instruction[] byteCode)
        {
            int idx = 0;

            while (idx < byteCode.Length) {
                byteCode[idx++].Accept(this);
            }

            if(ValueStack.Count > 0)
            {
                var output = ValueStack.Peek();
                ValueStack.Clear();
                return output;
            }
            return null;
        }
    }
}
