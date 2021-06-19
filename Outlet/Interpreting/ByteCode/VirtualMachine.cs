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
        private readonly Stack<int> CallStack = new();
        private readonly Stack<int> ValueStack = new();
        
        //private readonly Stack<int[]> StackFrames = new();
        //private int GetLocal(uint localId) => StackFrames.Peek()[localId];
        
        // Temp Implementation
        private readonly Dictionary<uint, int> Locals = new();

        private int idx = 0;

        public object? Interpret(Instruction[] byteCode)
        {
            idx = 0;

            while (idx < byteCode.Length)
            {
                //Console.WriteLine($"Executing {byteCode[idx]} at {idx}");
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
