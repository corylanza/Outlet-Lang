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
        private readonly Instruction[] ByteCode;

        private int Accumulator { get; set; }
        private int idx = 0;

        public VirtualMachine(IASTNode program)
        {
            var compiler = new ByteCodeGenerator();
            ByteCode = compiler.GenerateByteCode(program).ToArray();
        }

        public object Interpret()
        {
            object? last = null;
            while(idx < ByteCode.Length) {
                last = ByteCode[idx++].Accept(this);
            }
            return last;
        }
    }
}
